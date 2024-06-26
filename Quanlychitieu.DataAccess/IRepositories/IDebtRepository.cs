﻿namespace Quanlychitieu.DataAccess.IRepositories;
public interface IDebtRepository
{
    event Action DebtListChanged;
    Task<List<DebtModel>> GetAllDebtAsync();
    List<DebtModel> DebtList { get; set; }
    Task<bool> AddDebtAsync(DebtModel debt);
    Task<bool> UpdateDebtAsync(DebtModel debt);
    Task<bool> DeleteDebtAsync(DebtModel debt);
    Task SynchronizeDebtsAsync();

    Task LogOutUserAsync();
    Task DropDebtCollection();
}
