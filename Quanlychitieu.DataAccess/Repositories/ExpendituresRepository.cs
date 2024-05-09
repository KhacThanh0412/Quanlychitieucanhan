

namespace Quanlychitieu.DataAccess.Repositories;

public class ExpendituresRepository : IExpendituresRepository
{
    public List<ExpendituresModel> OfflineExpendituresList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event Action OfflineExpendituresListChanged;

    public Task<bool> AddExpenditureAsync(ExpendituresModel expenditure)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteExpenditureAsync(ExpendituresModel expenditure)
    {
        throw new NotImplementedException();
    }

    public Task DropExpendituresCollection()
    {
        throw new NotImplementedException();
    }

    public Task<List<ExpendituresModel>> GetAllExpendituresAsync()
    {
        throw new NotImplementedException();
    }

    public Task LogOutUserAsync()
    {
        throw new NotImplementedException();
    }

    public Task SynchronizeExpendituresAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateExpenditureAsync(ExpendituresModel expenditure)
    {
        throw new NotImplementedException();
    }
}
