namespace Quanlychitieu.DataAccess.IRepositories;

public interface IExpendituresRepository
{
    event Action ExpendituresListChanged;
    Task<List<ExpendituresModel>> GetAllExpendituresAsync();

    List<ExpendituresModel> ExpendituresList { get; set; }
    Task<bool> AddExpenditureAsync(ExpendituresModel expenditure);
    Task<bool> UpdateExpenditureAsync(ExpendituresModel expenditure);
    Task<bool> DeleteExpenditureAsync(ExpendituresModel expenditure);

    Task SynchronizeExpendituresAsync();

    Task LogOutUserAsync();
    Task DropExpendituresCollection();
}
