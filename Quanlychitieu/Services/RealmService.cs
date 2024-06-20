using CommunityToolkit.Mvvm.Messaging;
using Quanlychitieu.Messages;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Services
{
    public partial class RealmService : IRealmService, IRecipient<RealmDataMessage>
    {
        private readonly object _syncLock = new object();
        private HashSet<TaskInfo> _prevAsyncTask;
        private Realm _realms;
        private const string RealmDbPrefix = "Quanlychitieu-";
        private bool _isProcessing;
        public event EventHandler<string> TaskCompleted;

        protected void OnTaskCompleted(string message)
        {
            TaskCompleted?.Invoke(this, message);
        }

        private Realm GetOrCreateRealm()
        {
            lock (_syncLock)
            {
                if (_realms is not null && _realms.IsClosed)
                {
                    // Dispose previous instance if it exists and is closed
                    _realms?.Dispose();
                    _realms = this.CreateRealmInstance();
                }
                else if (_realms is null)
                {
                    _realms = CreateRealmInstance();
                }

                return _realms;
            }
        }

        private RealmConfiguration GetRealmConfig()
        {
            var dbName = RealmDbPrefix + ".realm";
            var config = new RealmConfiguration(dbName)
            {
#if DEBUG
                ShouldDeleteIfMigrationNeeded = true,
#endif
                SchemaVersion = 1,
                MigrationCallback = (migration, oldSchemaVersion) =>
                {
                    // process migrate for rename, changed
                }
            };

            return config;
        }

        private Realm CreateRealmInstance()
        {
            var config = GetRealmConfig();
            return Realm.GetInstance(config);
        }

        public void ClearWriteRealmTask()
        {
            _prevAsyncTask?.Clear();
        }

        public void Init()
        {
            if (!WeakReferenceMessenger.Default.IsRegistered<RealmDataMessage>(this))
                WeakReferenceMessenger.Default.Register<RealmDataMessage>(this);
        }

        public Task<Realm> GetCurrentInstanceAsync() => Task.FromResult(GetOrCreateRealm());

        public Task<ObservableCollection<T>> GetAllAsync<T>(CancellationToken cancellationToken = default)
            where T : RealmObject
        {
            try
            {
                var realm = GetOrCreateRealm();
                return Task.FromResult(new ObservableCollection<T>(realm.All<T>()));
            }
            catch (OperationCanceledException)
            {
#if DEBUG
                Console.WriteLine("===> Cancel Task GetAllAsync");
#endif
            }

            return Task.FromResult(new ObservableCollection<T>());
        }

        public Task<T> GetByIdAsync<T>(string id, CancellationToken cancellationToken = default)
            where T : RealmObject
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var realm = GetOrCreateRealm();
                return Task.FromResult(realm.Find<T>(id));
            }
            catch (OperationCanceledException)
            {
#if DEBUG
                Console.WriteLine("===> Cancel Task GetByIdAsync");
#endif
            }

            return null;
        }

        public async Task AddAsync<T>(T item, CancellationToken cancellationToken = default)
            where T : RealmObject
        {
            var realm = GetOrCreateRealm();
            using (var transaction = await realm.BeginWriteAsync(cancellationToken))
            {
                try
                {
                    realm.Add(item);
                    if (transaction.State == TransactionState.Running)
                        await transaction.CommitAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    if (transaction.State != TransactionState.RolledBack &&
                        transaction.State != TransactionState.Committed)
                    {
                        transaction.Rollback();
#if DEBUG
                        Console.WriteLine($"===> transaction rollback");
#endif
                    }
                }
            }
        }

        public async Task UpdateAsync<T>(T item, CancellationToken cancellationToken = default)
            where T : RealmObject
        {
            var realm = GetOrCreateRealm();
            using (var transaction = await realm.BeginWriteAsync(cancellationToken))
            {
                try
                {
                    realm.Add(item, update: true);
                    if (transaction.State == TransactionState.Running)
                        await transaction.CommitAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    if (transaction.State != TransactionState.RolledBack &&
                        transaction.State != TransactionState.Committed)
                    {
                        transaction.Rollback();
#if DEBUG
                        Console.WriteLine($"===> transaction rollback");
#endif
                    }
                }
            }
        }

        public async Task DeleteAsync<T>(T item, CancellationToken cancellationToken = default)
            where T : RealmObject
        {
            var realm = GetOrCreateRealm();
            using (var transaction = await realm.BeginWriteAsync(cancellationToken))
            {
                try
                {
                    realm.Remove(item);
                    if (transaction.State == TransactionState.Running)
                        await transaction.CommitAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    if (transaction.State != TransactionState.RolledBack &&
                        transaction.State != TransactionState.Committed)
                    {
                        transaction.Rollback();
#if DEBUG
                        Console.WriteLine($"===> transaction rollback");
#endif
                    }
                }
            }
        }

        public async Task DeleteAllAsync<T>(CancellationToken cancellationToken = default)
            where T : RealmObject
        {
            var realm = GetOrCreateRealm();
            using (var transaction = await realm.BeginWriteAsync(cancellationToken))
            {
                try
                {
                    realm.RemoveAll<T>();
                    if (transaction.State == TransactionState.Running)
                        await transaction.CommitAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    if (transaction.State != TransactionState.RolledBack &&
                        transaction.State != TransactionState.Committed)
                    {
                        transaction.Rollback();
#if DEBUG
                        Console.WriteLine($"===> transaction rollback");
#endif
                    }
                }
            }
        }

        private async Task AwaitLastTaskAsync()
        {
            _isProcessing = true;
            if (_prevAsyncTask != null)
            {
                while (_prevAsyncTask.Any())
                {
                    await Task.WhenAny(_prevAsyncTask.Select(x => x?.Task));
                }
            }

            _isProcessing = false;
        }

        private async Task WriteDataToRealmAsync(TaskInfo info, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var item in info.CurrentData)
                {
                    if (!item.IsManaged)
                        await UpdateAsync(item, cancellationToken: info.CancellationTokenSource.Token);
                }

                foreach (var item in info.ApiData)
                {
                    await UpdateAsync(item, cancellationToken: info.CancellationTokenSource.Token);
                }

                if (info.CurrentData != null)
                {
                    foreach (var item in info.CurrentData)
                    {
                        if (!info.ApiData.Contains(item))
                        {
                            await DeleteAsync(item, cancellationToken: info.CancellationTokenSource.Token);
                        }
                    }
                }

                await UpdateAsync(new DataSyncModel { DataName = info.DataName, Expired = false }, cancellationToken: info.CancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
#if DEBUG
                Console.WriteLine($"===> Cancel Task Write {info.DataName}");
#endif
            }
            finally
            {
                lock (_syncLock)
                {
                    // Remove the task from the list when it completed or canceled
                    _prevAsyncTask.Remove(info);
                    OnTaskCompleted(info.DataName);
                }
            }
        }

        private async Task UpdateRealmTableAsync<T>(ObservableCollection<T> apiData, ObservableCollection<T> currentData, string dataName, bool dataChanged = false, Action<bool> isCachedAction = null, CancellationToken cancellationToken = default)
        where T : RealmObject
        {
            if (apiData == null)
                return;
            lock (_syncLock)
            {
                if (_prevAsyncTask == null)
                    _prevAsyncTask = new HashSet<TaskInfo>();
                CancelTask(dataName, apiData);
                AddTask(currentData, apiData, dataName);
            }

            if (!_isProcessing)
                await this.AwaitLastTaskAsync();
        }

        public async void Receive(RealmDataMessage message)
        {
            await UpdateRealmTableAsync(apiData: message.Value.Data, currentData: message.Value.CurrentData, dataName: message.Value.DataName, cancellationToken: message.Value.CancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            GetOrCreateRealm()?.Dispose();
        }

        private void AddTask<T>(ObservableCollection<T> currentData, ObservableCollection<T> apiData, string dataName)
            where T : RealmObject
        {
            lock (_syncLock)
            {
                // Start the task
                var taskInfo = new TaskInfo(new ObservableCollection<RealmObject>(currentData), new ObservableCollection<RealmObject>(apiData), dataName);
                taskInfo.Task = WriteDataToRealmAsync(taskInfo, taskInfo.CancellationTokenSource.Token);
                _prevAsyncTask.Add(taskInfo);
            }
        }

        private void CancelTask<T>(string dataName, ObservableCollection<T> apiData)
            where T : RealmObject
        {
            lock (_syncLock)
            {
                var taskInfo = _prevAsyncTask?.Where(info => info.DataName == dataName);
                if (taskInfo != null && taskInfo.Count() > 0)
                {
                    foreach (var item in taskInfo)
                    {
                        if (!item.Task.IsCanceled && item.ApiData.Count() == apiData.Count())
                            item?.CancellationTokenSource?.Cancel();
                    }
                }
            }
        }
    }

    class TaskInfo
    {
        public ObservableCollection<RealmObject> CurrentData { get; }
        public ObservableCollection<RealmObject> ApiData { get; set; }
        public string DataName { get; set; }
        public Task Task { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
        public TaskInfo(ObservableCollection<RealmObject> currentData, ObservableCollection<RealmObject> apiData, string dataName)
        {
            CurrentData = currentData;
            ApiData = apiData;
            DataName = dataName;
        }
    }
}
