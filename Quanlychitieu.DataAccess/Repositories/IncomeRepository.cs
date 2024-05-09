

namespace Quanlychitieu.DataAccess.Repositories;

public class IncomeRepository : IIncomeRepository
{
    public List<IncomeModel> OfflineIncomesList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event Action OfflineIncomesListChanged;

    public Task<bool> AddIncomeAsync(IncomeModel income)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteIncomeAsync(IncomeModel incomeId)
    {
        throw new NotImplementedException();
    }

    public Task DropIncomesCollection()
    {
        throw new NotImplementedException();
    }

    public Task<List<IncomeModel>> GetAllIncomesAsync()
    {
        throw new NotImplementedException();
    }

    public Task LogOutUserAsync()
    {
        throw new NotImplementedException();
    }

    public Task SynchronizeIncomesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateIncomeAsync(IncomeModel income)
    {
        throw new NotImplementedException();
    }
}
