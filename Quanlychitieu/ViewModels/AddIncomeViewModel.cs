using Quanlychitieu.Helpers;

namespace Quanlychitieu.ViewModels
{
    public partial class AddIncomeViewModel : BaseViewModel
    {
        private readonly IIncomeRepository incomeService;
        private readonly IUsersRepository userService;
        public bool IsAdd { get; }
        private bool _isTypeIncome;
        double InitialUserPockerMoney;
        double InitialIncomeAmout;
        double _initialTotalIncAmount;


        [ObservableProperty]
        IncomeModel _addIncomeDetails;
        [ObservableProperty]
        UsersModel _activeUser;
        [ObservableProperty]
        private DateTime _dateReceived = DateTime.Now;
        [ObservableProperty]
        private string _titleIncome;

        public AddIncomeViewModel(IIncomeRepository incomeRepository, IUsersRepository usersRepository)
        { 
            incomeService = incomeRepository;
            userService = usersRepository;
        }

        [RelayCommand]
        async Task UpSertIncome()
        {
            if (AddIncomeDetails.AmountReceived <= 0)
            {
                await AlertHelper.ShowErrorAlertAsync("Số tiền không thể nhỏ hơn 0");
                return;
            }
            else
            {
                if (!await AlertHelper.ShowConfirmationAlertAsync("Bạn có muốn lưu lại thông tin không?"))
                    return;
                AddIncomeDetails.UserId = ActiveUser.Id;
                AddIncomeDetails.DateReceived = DateReceived;
                if (!_isTypeIncome)
                {
                    if (await incomeService.AddIncomeAsync(AddIncomeDetails))
                    {
                        await AlertHelper.ShowInformationAlertAsync("Thêm thu nhập mới thành công");
                        await NavigationService.PopPageAsync();
                    }
                    else
                    {
                        await AlertHelper.ShowErrorAlertAsync("Đã gặp lỗi trong quá trình tạo mới");
                    }
                }
                else
                {
                    if (await incomeService.UpdateIncomeAsync(AddIncomeDetails))
                    {
                        AddIncomeDetails.AmountReceived = AddIncomeDetails.AmountReceived;
                        AddIncomeDetails.Reason = AddIncomeDetails.Reason;
                        await AlertHelper.ShowInformationAlertAsync("Sửa đơn thành công");
                        await NavigationService.PopPageAsync();
                    }
                    else
                    {
                        await AlertHelper.ShowErrorAlertAsync("Đã gặp lỗi trong quá trình sửa đơn");
                    }
                }
            }
        }

        public override Task InitAsync(object initData)
        {
            if (initData is null)
            {
                AddIncomeDetails = new IncomeModel();
                TitleIncome = "Thêm mới phiếu thu";
            }
            else
            {
                AddIncomeDetails = initData as IncomeModel;
                _isTypeIncome = true;
                TitleIncome = "Sửa phiếu thu";
            }

            return base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            ActiveUser = await userService.GetUserAsync();
            base.LoadDataAsync();
        }
    }
}
