using Quanlychitieu.Helpers;

namespace Quanlychitieu.ViewModels
{
    public partial class DebtLendingViewModel : BaseViewModel
    {
        private readonly IDebtRepository debtRepo;

        [ObservableProperty]
        ObservableCollection<DebtModel> _debtsList;
        [ObservableProperty]
        DebtModel _debtModelActive;
        [ObservableProperty]
        int totalPendingBorrowCount;
        [ObservableProperty]
        int totalPendingLentCount;
        [ObservableProperty]
        int totalCompletedBorrowCount;
        [ObservableProperty]
        int totalCompletedLentCount;
        [ObservableProperty]
        int totalDebts;
        [ObservableProperty]
        int totalLentCount;
        [ObservableProperty]
        double totalLentCompletedAmount;
        [ObservableProperty]
        double totalLentPendingAmount;
        [ObservableProperty]
        int totalBorrowedCount;
        [ObservableProperty]
        double totalBorrowedCompletedAmount;
        [ObservableProperty]
        double totalBorrowedPendingAmount;
        [ObservableProperty]
        ObservableCollection<DebtModel> _lentPendingList;
        [ObservableProperty]
        ObservableCollection<DebtModel> _lentCompletedList;
        public DebtLendingViewModel(IDebtRepository debtRepository)
        {
            debtRepo = debtRepository;
        }

        public override Task InitAsync(object initData)
        {
            if (initData is ObservableCollection<DebtModel> debts)
            {
                DebtsList = new ObservableCollection<DebtModel>(debts);
                RedoCountsAndAmountsCalculation(DebtsList);
            }

            return base.InitAsync(initData);
        }

        [RelayCommand]
        void SearchBar(string query)
        {
            try
            {
                var ListOfDebts = DebtsList
                .Where(d => d.PersonOrOrganization?.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                .Where(d => !d.IsDeleted)
                .ToList();

                DebtsList.Clear();
                DebtsList = ListOfDebts.ToObservableCollection();
                RedoCountsAndAmountsCalculation(ListOfDebts);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception : {ex.Message}");
            }
        }

        [RelayCommand]
        async Task ToggleDebtCompletionStatus(object s)
        {
            DebtModelActive = (DebtModel)s;
            try
            {
                string message = DebtModelActive.IsPaidCompletely ? "Đánh dấu là đang chờ xử lý" : "Đánh dấu đã hoàn thành";
                if (!await AlertHelper.ShowConfirmationAlertAsync(message))
                    return;

                DebtModelActive.IsPaidCompletely = !DebtModelActive.IsPaidCompletely;
                DebtModelActive.DatePaidCompletely = DebtModelActive.IsPaidCompletely ? DateTime.Now : (DateTime?)null;

                if (DebtModelActive.Deadline.HasValue)
                {
                    if (DebtModelActive.IsPaidCompletely)
                    {
                        DebtModelActive.DisplayText = FormatPaidText(DebtModelActive.DatePaidCompletely.Value, DebtModelActive.Deadline.Value);
                    }
                    else
                    {
                        DebtModelActive.DisplayText = FormatPendingText(DebtModelActive.Deadline.Value);
                    }
                }
                else
                {
                    DebtModelActive.DisplayText = DebtModelActive.IsPaidCompletely ? "Được thanh toán hôm nay" : "Đang chờ xử lý";
                }

                if (await debtRepo.UpdateDebtAsync(DebtModelActive))
                    ApplyChanges();
                else
                    await AlertHelper.ShowErrorAlertAsync("Chưa cập nhật thành công");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
        }

        private string FormatPendingText(DateTime deadline)
        {
            var diff = DateTime.Now.Date - deadline.Date;
            if (diff.TotalDays == 0)
            {
                return "Hạn hôm nay";
            }
            else if (diff.TotalDays > 0)
            {
                return $"Quá hạn {diff.TotalDays} ngày!";
            }
            else
            {
                return $"Đến hạn {-diff.TotalDays} ngày";
            }
        }

        private string FormatPaidText(DateTime datePaid, DateTime? deadline)
        {
            var DatePaidDiff = DateTime.Now.Date - datePaid.Date;
            if (DatePaidDiff.TotalDays == 0)
            {
                return "Đã thanh toán hôm nay";
            }
            else if (DatePaidDiff.TotalDays == 1)
            {
                return "Đã thanh toán 1 ngày trước";
            }
            else
            {
                return $"Trả {DatePaidDiff.TotalDays} ngày trước";
            }
        }

        [ObservableProperty]
        IEnumerable<string> listOfPeopleNames;
        public void ApplyChanges(string filterOption = null)
        {
            IEnumerable<DebtModel> filteredDebts = [];
            if (filterOption == "Completed")
            {
                filteredDebts = DebtsList
                    .Where(x => !x.IsDeleted && x.IsPaidCompletely);
            }
            else if (filterOption == "Pending")
            {
                filteredDebts = DebtsList
                    .Where(x => !x.IsDeleted && !x.IsPaidCompletely);
            }
            else
            {
                filteredDebts = DebtsList
                    .Where(x => !x.IsDeleted);
            }

            var sortedDebts = filteredDebts
                .OrderByDescending(x => x.IsPaidCompletely)
                .ThenBy(x => x.UpdateDateTime)
                .ToHashSet()
                .ToList();

            DebtsList = sortedDebts.ToObservableCollection();
            RedoCountsAndAmountsCalculation(filteredDebts);
        }

        private void RedoCountsAndAmountsCalculation(IEnumerable<DebtModel> filteredDebts)
        {
            if (filteredDebts == null || !filteredDebts.Any())
            {
                // Xử lý trường hợp danh sách rỗng hoặc null nếu cần
                return;
            }

            // Tạo một danh sách tạm để chứa tất cả các nợ đã được sắp xếp và lọc
            var sortedFilteredDebts = filteredDebts
                .OrderBy(x => x.DebtType)
                .ThenBy(x => x.IsPaidCompletely)
                .ThenBy(x => x.AddedDateTime)
                .ToList();

            // Cập nhật lại DebtsList với danh sách đã lọc và sắp xếp
            DebtsList = new ObservableCollection<DebtModel>(sortedFilteredDebts);

            // Tính toán số lượng và số tiền theo các tiêu chí khác nhau
            var borrowedCompleted = sortedFilteredDebts
                .Where(x => x.DebtType == DebtType.Borrowed && x.IsPaidCompletely);

            var lentCompleted = sortedFilteredDebts
                .Where(x => x.DebtType == DebtType.Lent && x.IsPaidCompletely);

            var borrowedPending = sortedFilteredDebts
                .Where(x => x.DebtType == DebtType.Borrowed && !x.IsPaidCompletely);

            var lentPending = sortedFilteredDebts
                .Where(x => x.DebtType == DebtType.Lent && !x.IsPaidCompletely);
            LentPendingList = new ObservableCollection<DebtModel>(lentPending);
            LentCompletedList = new ObservableCollection<DebtModel>(lentCompleted);
            TotalPendingBorrowCount = borrowedPending.Count();
            TotalCompletedBorrowCount = borrowedCompleted.Count();
            TotalPendingLentCount = lentPending.Count();
            TotalCompletedLentCount = lentCompleted.Count();

            TotalBorrowedCompletedAmount = borrowedCompleted.Sum(x => x.AmountDebt);
            TotalBorrowedPendingAmount = borrowedPending.Sum(x => x.AmountDebt);
            TotalLentCompletedAmount = lentCompleted.Sum(x => x.AmountDebt);
            TotalLentPendingAmount = lentPending.Sum(x => x.AmountDebt);

            TotalBorrowedCount = TotalPendingBorrowCount + TotalCompletedBorrowCount;
            TotalLentCount = TotalPendingLentCount + TotalCompletedLentCount;
        }
    }
}
