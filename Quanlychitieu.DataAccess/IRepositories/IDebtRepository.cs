namespace Quanlychitieu.DataAccess.IRepositories;
public interface IDebtRepository
{
    Task<List<DebtModel>> GetAllDebtAsync();
    Task<bool> AddDebtAsync(DebtModel debt);
    Task<bool> UpdateDebtAsync(DebtModel debt);
    Task<bool> DeleteDebtAsync(DebtModel debt);
    Task SynchronizeDebtsAsync();

    Task LogOutUserAsync();
    Task DropDebtCollection();
}
