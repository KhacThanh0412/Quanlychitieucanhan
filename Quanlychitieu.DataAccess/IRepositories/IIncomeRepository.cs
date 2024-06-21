using System.Collections.ObjectModel;

namespace Quanlychitieu.DataAccess.IRepositories;

public interface IIncomeRepository
{
    event Action IncomesListChanged;
    Task<ObservableCollection<IncomeModel>> GetAllIncomesAsync();
    ObservableCollection<IncomeModel> IncomesList { get; set; }
    Task<bool> AddIncomeAsync(IncomeModel income);
    Task<bool> DeleteIncomeAsync(IncomeModel incomeId);
    Task<bool> UpdateIncomeAsync(IncomeModel income);
    Task SynchronizeIncomesAsync();
    double CalculateTotalIncome();
}
