using Quanlychitieu.DataAccess.Repositories;
using Quanlychitieu.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.ViewModels
{
    public partial class AddSertDebtViewModel : BaseViewModel
    {
        readonly IDebtRepository debtRepo;
        readonly IUsersRepository userRepo;

        [ObservableProperty]
        private DebtModel _debtDetails;
        [ObservableProperty]
        UsersModel _activeUser;
        [ObservableProperty]
        private bool _isLent;
        [ObservableProperty]
        private bool _isBorrow;
        [ObservableProperty]
        private bool _hasDeadLine;

        public AddSertDebtViewModel(IDebtRepository debtRepository, IUsersRepository usersRepository) 
        {
            debtRepo = debtRepository;
            userRepo = usersRepository;
        }

        public override Task InitAsync(object initData)
        {
            if (initData is null)
            {
                DebtDetails = new DebtModel
                {
                    PersonOrOrganization = new PersonOrOrganizationModel(),
                };
            }
            return base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            ActiveUser = await userRepo.GetUserAsync();
            base.LoadDataAsync();
        }

        [RelayCommand]
        async Task UpSertDebt()
        {
            if (DebtDetails.AmountDebt <= 0)
            {
                await AlertHelper.ShowErrorAlertAsync("Số tiền nhập phải lớn hơn 0");
                return;
            }
            else
            {
                if (!await AlertHelper.ShowConfirmationAlertAsync("Lưu thông tin đã nhập?"))
                    return;

                DebtDetails.UserId = ActiveUser.Id;
                DebtDetails.DebtType = IsLent ? DebtType.Lent : DebtType.Borrowed;
                if (await debtRepo.AddDebtAsync(DebtDetails))
                {
                    await AlertHelper.ShowInformationAlertAsync("Thêm mới phiếu chi thành công!");
                    await NavigationService.PopPageAsync();
                }
                else
                {
                    await AlertHelper.ShowErrorAlertAsync("Có lỗi xảy ra hãy thử lại");
                }
            }
        }

        private void OnIsLentChanged()
        {
            if (IsLent)
                IsLent = !IsBorrow;
        }

        private void OnIsBorrowChanged()
        {
            if (IsBorrow)
                IsBorrow = !IsLent;
        }
    }
}
