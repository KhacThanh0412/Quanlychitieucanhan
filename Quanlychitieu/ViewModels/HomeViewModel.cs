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
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IExpendituresRepository expenditureRepo;
        private readonly ISettingsServiceRepository settingsService;
        private readonly IUsersRepository userRepo;
        private readonly IIncomeRepository incomeRepo;
        private readonly IDebtRepository debtRepo;
        public HomeViewModel(ISettingsServiceRepository settingsServiceRepo,IExpendituresRepository expendituresRepository,
                        IUsersRepository usersRepository)
        {
            expenditureRepo = expendituresRepository;
            settingsService = settingsServiceRepo;
            userRepo = usersRepository;
            expenditureRepo.OfflineExpendituresListChanged += OnExpendituresChanged;
            UpdateIsSyncing();
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
        public double pocketMoney;

        [ObservableProperty]
        bool isSyncing;

        [ObservableProperty]
        private UsersModel activeUser = new();

        public bool _isInitialized;

        void UpdateIsSyncing()
        {
            IsSyncing = LatestExpenditures.Count < 1;
        }
        public async Task DisplayInfo()
        {
            // await SyncAndNotifyAsync();
        }
        public void GetUserData()
        {
            if (userRepo.OfflineUser is not null)
            {
                PocketMoney = userRepo.OfflineUser.PocketMoney;
                Username = userRepo.OfflineUser.Username;
                UserCurrency = userRepo.OfflineUser.UserCurrency;
            }
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
            PocketMoney = userRepo.OfflineUser.PocketMoney;
        }

        private void InitializeExpenditures()
        {
            //var ListOfExp = await expenditureRepo.GetAllExpendituresAsync();
            var ListOfExp = expenditureRepo.OfflineExpendituresList;

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
            var ListOfInc = incomeRepo.OfflineIncomesList;
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
                    await Shell.Current.ShowPopupAsync(new ErrorPopUpAlert("Error when syncing " + ex.Message));
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
            var newUpSertExpPopUp = new UpSertExpendituresPopUp(NewUpSertVM);
            try
            {

                if (ActiveUser is null)
                {
                    Debug.WriteLine("Không thể mở hộp thoại!");
                    await Shell.Current.DisplayAlert("Wait", "Không thể đi", "Ok");
                }
                else
                {
                    var UpSertResult = (PopUpCloseResult)await Shell.Current.ShowPopupAsync(newUpSertExpPopUp);

                    if (UpSertResult.Result == PopupResult.OK)
                    {
                        ExpendituresModel exp = (ExpendituresModel)UpSertResult.Data;
                        //add logic if this exp is the latest in terms of datetime

                        PocketMoney -= exp.AmountSpent;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PopUp Exception on {DeviceInfo.Platform} : {ex.Message}");
            }
        }



    }
}
