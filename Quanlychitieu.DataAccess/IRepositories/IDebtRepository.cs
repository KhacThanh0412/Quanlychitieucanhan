using System.Collections.ObjectModel;

namespace Quanlychitieu.DataAccess.IRepositories;
public interface IDebtRepository
{
    event Action DebtListChanged;
    Task<ObservableCollection<DebtModel>> GetAllDebtAsync();
    ObservableCollection<DebtModel> DebtList { get; set; }
    Task<bool> AddDebtAsync(DebtModel debt);
    Task<bool> UpdateDebtAsync(DebtModel debt);
    Task<bool> DeleteDebtAsync(DebtModel debt);
    Task SynchronizeDebtsAsync();
}
