using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Quanlychitieu.DataAccess.IRepositories;
using Quanlychitieu.Models;
using Quanlychitieu.Utilities;
using Quanlychitieu.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quanlychitieu.Helpers;

namespace Quanlychitieu.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IExpendituresRepository expenditureRepo;
        private readonly ISettingsServiceRepository settingsService;
        private readonly IUsersRepository userRepo;
        private readonly IIncomeRepository incomeRepo;
        private readonly IDebtRepository debtRepo;
        public HomeViewModel(IUsersRepository usersRepository, IIncomeRepository incomeRepo, IExpendituresRepository expendituresRepository, IDebtRepository debtRepository)
        {
            this.userRepo = usersRepository;
            this.incomeRepo = incomeRepo;
            expenditureRepo = expendituresRepository;
            debtRepo = debtRepository;
        }

        [ObservableProperty]
        ObservableCollection<ExpendituresModel> _latestExpenditures;

        [ObservableProperty]
        ObservableCollection<IncomeModel> _latestIncomes;

        [ObservableProperty]
        public int totalExp;

        [ObservableProperty]
        public string username;
        [ObservableProperty]
        public string userCurrency;
        [ObservableProperty]
        public double _totalIncomeAmount;
        [ObservableProperty]
        public double pocketMoney;
        [ObservableProperty]
        public double _totalDebtRepaid;

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
            await Task.WhenAll(incomeRepo.SynchronizeIncomesAsync());
            await Task.WhenAll(expenditureRepo.SynchronizeExpendituresAsync());
            LatestExpenditures = expenditureRepo.ExpendituresList;
            LatestIncomes = incomeRepo.IncomesList;

            CalculateTotalDebtRepaid(); // Tính toán tổng số tiền đã trả nợ
            CalculateTotalIncomeAmount();
            base.LoadDataAsync();
        }

        void UpdateIsSyncing()
        {
            IsSyncing = LatestExpenditures.Count < 1;
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
            var ListOfExp = LatestExpenditures;

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

        private void CalculateTotalDebtRepaid()
        {
            // Tính tổng số tiền đã trả nợ
            TotalDebtRepaid = debtRepo.DebtList
                .Where(d => d.IsPaidCompletely)
                .Sum(d => d.AmountDebt);
        }

        private void CalculateTotalIncomeAmount()
        {
            // Tính tổng thu nhập và trừ tổng chi tiêu, cộng tổng số tiền đã trả nợ
            double totalIncome = incomeRepo.IncomesList.Sum(i => i.AmountReceived);
            double totalExpenditures = expenditureRepo.ExpendituresList.Sum(e => e.AmountSpent);

            TotalIncomeAmount = totalIncome + TotalDebtRepaid - totalExpenditures;
        }

        public bool isFromShortCut;
        [RelayCommand]
        public async Task GoToAddExpenditurePage()
        {
            if (await AlertHelper.ShowConfirmationAlertAsync("Bạn có muốn thêm mới hóa đơn không?"))
            {
                await NavigationService.PushToPageAsync<AddExpendituresPage>();
            }
        }
    }
}
