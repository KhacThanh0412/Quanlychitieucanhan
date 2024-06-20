using Realms;

namespace Quanlychitieu.Services
{
    public interface IRealmService
    {
        event EventHandler<string> TaskCompleted;
        void Init();
        Task<Realm> GetCurrentInstanceAsync();
        Task<ObservableCollection<T>> GetAllAsync<T>(CancellationToken cancellationToken = default)
            where T : RealmObject;
        Task<T> GetByIdAsync<T>(string id, CancellationToken cancellationToken = default)
            where T : RealmObject;
        Task AddAsync<T>(T item, CancellationToken cancellationToken = default)
            where T : RealmObject;
        Task UpdateAsync<T>(T item, CancellationToken cancellationToken = default)
            where T : RealmObject;
        Task DeleteAsync<T>(T item, CancellationToken cancellationToken = default)
            where T : RealmObject;
        Task DeleteAllAsync<T>(CancellationToken cancellationToken = default)
            where T : RealmObject;
        void ClearWriteRealmTask();
    }
}
