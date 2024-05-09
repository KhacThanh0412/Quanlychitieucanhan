
namespace Quanlychitieu.DataAccess.Repositories;

public class PlannedExpendituresRepository : IPlannedExpendituresRepository
{
    public List<PlannedExpendituresModel> OfflinePlannedExpendituresList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Task<bool> AddPlannedExp(PlannedExpendituresModel plannedExpendituresModel)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePlannedExp(string id)
    {
        throw new NotImplementedException();
    }

    public Task DropPlannedExpCollection()
    {
        throw new NotImplementedException();
    }

    public Task<List<PlannedExpendituresModel>> GetAllPlannedExp()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SynchronizePlannedExpendituresAsync(string userEmail, string userPassword)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdatePlannedExp(PlannedExpendituresModel plannedExpendituresModel)
    {
        throw new NotImplementedException();
    }
}
