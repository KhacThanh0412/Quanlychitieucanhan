using System.Collections.ObjectModel;

namespace Quanlychitieu.DataAccess.IRepositories;

public interface IExpendituresRepository
{
    event Action ExpendituresListChanged;
    Task<ObservableCollection<ExpendituresModel>> GetAllExpendituresAsync();

    ObservableCollection<ExpendituresModel> ExpendituresList { get; set; }
    Task<bool> AddExpenditureAsync(ExpendituresModel expenditure);
    Task<bool> UpdateExpenditureAsync(ExpendituresModel expenditure);
    Task<bool> DeleteExpenditureAsync(ExpendituresModel expenditure);

    Task SynchronizeExpendituresAsync();
    double CalculateTotalExpends();

}
