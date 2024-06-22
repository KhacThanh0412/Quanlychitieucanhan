using Quanlychitieu.DataAccess.Repositories;
using Quanlychitieu.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.ViewModels
{
    public partial class AddExpendituresViewModel : BaseViewModel
    {
        readonly IExpendituresRepository expenditureRepo;
        readonly IUsersRepository userRepo;
        private bool IsEditExpenditures;

        [ObservableProperty]
        UsersModel _activeUser;
        [ObservableProperty]
        ExpendituresModel _expendituresDetails;
        [ObservableProperty]
        DateTime _dateSpent = DateTime.Now;
        [ObservableProperty]
        string _titlePage;
        [ObservableProperty]
        double totalAmountSpent;

        [ObservableProperty]
        double resultingBalance;

        [ObservableProperty]
        bool isBottomSheetOpened;

        public AddExpendituresViewModel(IExpendituresRepository expendituresRepository, IUsersRepository usersRepository)
        {
            expenditureRepo = expendituresRepository;
            userRepo = usersRepository;
        }

        public override Task InitAsync(object initData)
        {
            if (initData is null)
            {
                ExpendituresDetails = new ExpendituresModel();
                TitlePage = "Thêm mới hóa đơn";
            }
            else
            {
                ExpendituresDetails = initData as ExpendituresModel;
                IsEditExpenditures = true;
                TitlePage = "Sửa phiếu chi";
            }


            return base.InitAsync(initData);
        }

        public override async Task LoadDataAsync()
        {
            ActiveUser = await userRepo.GetUserAsync();
            base.LoadDataAsync();
        }

        [RelayCommand]
        async Task UpSertSpent()
        {
            if (ExpendituresDetails.AmountSpent <= 0)
            {
                await AlertHelper.ShowErrorAlertAsync("Số tiền nhập phải lớn hơn 0");
                return;
            }
            else
            {
                if (!await AlertHelper.ShowConfirmationAlertAsync("Lưu thông tin phiếu chi?"))
                    return;

                ExpendituresDetails.UserId = ActiveUser.Id;
                ExpendituresDetails.DateSpent = DateSpent;
                if (!IsEditExpenditures)
                {
                    if (await expenditureRepo.AddExpenditureAsync(ExpendituresDetails))
                    {
                        await AlertHelper.ShowInformationAlertAsync("Thêm mới phiếu chi thành công!");
                        await NavigationService.PopPageAsync();
                    }
                    else
                    {
                        await AlertHelper.ShowErrorAlertAsync("Có lỗi xảy ra hãy thử lại");
                    }
                }
                else
                {
                    if (await expenditureRepo.UpdateExpenditureAsync(ExpendituresDetails))
                    {
                        ExpendituresDetails.AmountSpent = ExpendituresDetails.AmountSpent;
                        ExpendituresDetails.Reason = ExpendituresDetails.Reason;
                        await AlertHelper.ShowInformationAlertAsync("Sửa đơn thành công");
                        await NavigationService.PopPageAsync();
                    }
                    else
                    {
                        await AlertHelper.ShowErrorAlertAsync("Có lỗi xảy ra hãy thử lại");
                    }
                }
            }
        }
    }
}
