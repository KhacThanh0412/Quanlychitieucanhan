using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quanlychitieu.ViewModels.Expenditures;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.PopUpPages;
using Quanlychitieu.Utilities;
using Quanlychitieu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IExpendituresRepository expenditureRepo;
        private readonly ISettingsServiceRepository settingsService;
        private readonly IUsersRepository userRepo;
        private readonly IIncomeRepository incomeRepo;
        private readonly IDebtRepository debtRepo;
        public HomeViewModel(IUsersRepository usersRepository, IIncomeRepository incomeRepo)
        {
            this.userRepo = usersRepository;
            this.incomeRepo = incomeRepo;
        }

        [ObservableProperty]
        ObservableCollection<ExpendituresModel> latestExpenditures = new();

        [ObservableProperty]
        ObservableCollection<IncomeModel> latestIncomes = new();

        [ObservableProperty]
        public int totalExp;

        [ObservableProperty]
        public string username;
        [ObservableProperty]
        public string userCurrency;
        [ObservableProperty]
        public double _totalIncomeAmount;

        [ObservableProperty]
        bool isSyncing;

        [ObservableProperty]
        private UsersModel _activeUser;

        public bool _isInitialized;

        public override Task InitAsync(object initData)
        {
            if (initData != null)
            {
                ActiveUser = (UsersModel)initData;
            }
            return base.InitAsync(initData);
        }
        public override async Task LoadDataAsync()
        {
            await Task.Delay(0);
            await Task.WhenAll(incomeRepo.SynchronizeIncomesAsync());
            TotalIncomeAmount = incomeRepo.CalculateTotalIncome();
            base.LoadDataAsync();
        }

        void UpdateIsSyncing()
        {
            IsSyncing = LatestExpenditures.Count < 1;
        }
        public async Task DisplayInfo()
        {
            await Task.Delay(0);
            // await SyncAndNotifyAsync();
        }
        public void GetUserData()
        {
            //if (userRepo.User is not null)
            //{
            //    PocketMoney = userRepo.User.PocketMoney;
            //    Username = userRepo.User.Username;
            //}
        }
        private void OnExpendituresChanged()
        {
            InitializeExpenditures();
        }
        private void OnIncomesChanged()
        {
            InitializeIncomes();
        }
        private void OnUserDataChanged()
        {
        }

        private void InitializeExpenditures()
        {
            var ListOfExp = expenditureRepo.ExpendituresList;

            LatestExpenditures = ListOfExp.Count != 0
                ? ListOfExp
                .Where(x => !x.IsDeleted)
                .OrderByDescending(s => s.DateSpent)
                .Take(5)
                .ToObservableCollection()
                : new ObservableCollection<ExpendituresModel>();
            UpdateIsSyncing();
        }
        private void InitializeIncomes()
        {
            var ListOfInc = incomeRepo.IncomesList;
            LatestIncomes = ListOfInc.Count != 0
                ? ListOfInc
                .Where(predicate: x => !x.IsDeleted)
                .OrderByDescending(s => s.DateReceived)
                .Take(5)
                .ToObservableCollection()
                : new ObservableCollection<IncomeModel>();
        }

        private async Task SyncAndNotifyAsync()
        {
            try
            {
                await Task.WhenAll(expenditureRepo.SynchronizeExpendituresAsync(), debtRepo.SynchronizeDebtsAsync(), incomeRepo.SynchronizeIncomesAsync());

                CancellationTokenSource cts = new();
                const ToastDuration duration = ToastDuration.Short;
                const double fontSize = 14;
                string text = "Tất cả đã được đồng bộ hóa !";
                var toast = Toast.Make(text, duration, fontSize);
                await toast.Show(cts.Token);
            }
            catch (AggregateException aEx)
            {
                foreach (var ex in aEx.InnerExceptions)
                {
                    await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Lỗi khi đồng bộ " + ex.Message));
                }
            }
        }

        [ObservableProperty]
        string selectedCalendarItem;

        public bool isFromShortCut;
        [RelayCommand]
        public async Task GoToAddExpenditurePage()
        {
            var newExpenditure = new ExpendituresModel() { DateSpent = DateTime.Now };
            var NewUpSertVM = new UpSertExpenditureViewModel(expenditureRepo, userRepo);
            try
            {

                if (ActiveUser is null)
                {
                    Debug.WriteLine("Không thể mở hộp thoại!");
                    await Shell.Current.DisplayAlert("Wait", "Không thể đi", "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception : {ex.Message}");
            }
        }



    }
}
