namespace Quanlychitieu.DataAccess.IRepositories;

public interface IIncomeRepository
{
    event Action IncomesListChanged;
    Task<List<IncomeModel>> GetAllIncomesAsync();
    List<IncomeModel> IncomesList { get; set; }
    Task<bool> AddIncomeAsync(IncomeModel income);
    Task<bool> DeleteIncomeAsync(IncomeModel incomeId);
    Task<bool> UpdateIncomeAsync(IncomeModel income);
    Task SynchronizeIncomesAsync();
    double CalculateTotalIncome();
}
