using LiteDB;

namespace Quanlychitieu.DataAccess.Repositories;

public class DebtRepository : IDebtRepository
{
    private const string DebtsCollectionName = "Debts";
    LiteDatabase db;
    private ILiteCollection<DebtModel> AllDebts;
    private readonly IDataAccessRepo dataAccess;
    private readonly IUsersRepository usersRepo;

    public List<DebtModel> OfflineDebtList { get; set; }
    public List<DebtModel> OnlineDebtList { get; set; }

    public event Action OfflineDebtListChanged;
    bool IsSyncing;
    private bool IsBatchUpdate;
    public DebtRepository(IDataAccessRepo dataAccess, IUsersRepository userRepo)
    {
        this.dataAccess = dataAccess;
        usersRepo = userRepo;
    }

    async Task<LiteDatabase> OpenDB()
    {
        await Task.Delay(1);
        db = dataAccess.GetDb();
        AllDebts = db.GetCollection<DebtModel>(DebtsCollectionName);
        AllDebts.EnsureIndex(x => x.Id);
        return db;
    }

    public Task<bool> AddDebtAsync(DebtModel debt)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteDebtAsync(DebtModel debt)
    {
        debt.UpdateDateTime = DateTime.UtcNow;
        debt.IsDeleted = true;
        try
        {
            await OpenDB();
            OfflineDebtList.Remove(debt);
            if (!IsBatchUpdate)
            {
                OfflineDebtListChanged?.Invoke();
            }
            if (AllDebts.Delete(debt.Id))
            {

                Debug.WriteLine("Debt deleted locally");

                db.Dispose();
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to delete local debt");
                db.Dispose();
                return false;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to delete local debt: " + ex.Message);
            return false;
        }
    }

    public Task<List<DebtModel>> GetAllDebtAsync()
    {
        throw new NotImplementedException();
    }

    public Task SynchronizeDebtsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateDebtAsync(DebtModel debt)
    {
        debt.UpdateDateTime = DateTime.UtcNow;

        try
        {
            await OpenDB();
            if (AllDebts.Update(debt))
            {
                Debug.WriteLine("Debt updated locally");

                int index = OfflineDebtList.FindIndex(x => x.Id == debt.Id);
                OfflineDebtList[index] = debt;
                if (!IsBatchUpdate)
                {
                    OfflineDebtListChanged?.Invoke();
                }
                
                db.Dispose();
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to update local debt");
                return false;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception when doing local dept update " + ex.InnerException.Message);
            return false;
        }
    }

    public async Task DropDebtCollection()
    {
        await OpenDB();
        db.DropCollection(DebtsCollectionName);
        db.Dispose();
        Debug.WriteLine("debts Collection dropped!");
    }

    public async Task LogOutUserAsync()
    {
        OnlineDebtList.Clear();
        OfflineDebtList.Clear();
        OfflineDebtListChanged?.Invoke();

        await DropDebtCollection();
    }
}