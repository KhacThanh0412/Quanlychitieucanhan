using LiteDB;

namespace Quanlychitieu.DataAccess.Repositories;

public class DebtRepository : IDebtRepository
{
    private const string DebtsCollectionName = "Debts";
    LiteDatabase db;
    private ILiteCollection<DebtModel> AllDebts;
    private readonly IDataAccessRepo dataAccess;
    private readonly IUsersRepository usersRepo;

    public List<DebtModel> DebtList { get; set; }

    public event Action DebtListChanged;
    bool IsSyncing;
    private bool IsBatchUpdate;
    public DebtRepository(IDataAccessRepo dataAccess, IUsersRepository userRepo)
    {
        this.dataAccess = dataAccess;
        usersRepo = userRepo;
    }

    void OpenDB()
    {
        db = dataAccess.GetDb();
        AllDebts = db.GetCollection<DebtModel>(DebtsCollectionName);
        AllDebts.EnsureIndex(x => x.Id);
    }

    public async Task<List<DebtModel>> GetAllDebtAsync()
    {
        try
        {
            OpenDB();

            string userId = usersRepo.User.Id;
            string userCurrency = usersRepo.User.UserCurrency;
            if (usersRepo.User.UserIDOnline != string.Empty)
            {
                userId = usersRepo.User.UserIDOnline;
            }
            // await AllDebts.DeleteAllAsync();

            //await db.DropCollectionAsync(DebtsCollectionName);
            DebtList = AllDebts.Query().ToList();
            var ss = DebtList
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UpdateDateTime);

            //OfflineDebtList ??= Enumerable.Empty<DebtModel>().ToList();
            return DebtList;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException.Message);
            Debug.WriteLine("Get all Debts fxn Exception: " + ex.Message);
            return Enumerable.Empty<DebtModel>().ToList();
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<bool> AddDebtAsync(DebtModel debt)
    {
        debt.UpdateDateTime = DateTime.UtcNow;
        try
        {
            OpenDB();

            if (AllDebts.Insert(debt) is not null)
            {
                DebtList.Add(debt);
                DebtListChanged?.Invoke();
                db.Dispose();
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to add local debt");
                return false;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to add local debt: " + ex.InnerException.Message);
            db.Dispose();
            return false;
        }
    }

    public async Task<bool> DeleteDebtAsync(DebtModel debt)
    {
        debt.UpdateDateTime = DateTime.UtcNow;
        debt.IsDeleted = true;
        try
        {
            OpenDB();
            DebtList.Remove(debt);
            if (!IsBatchUpdate)
            {
                DebtListChanged?.Invoke();
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

    public async Task SynchronizeDebtsAsync()
    {
        await GetAllDebtAsync();
        IsBatchUpdate = false;
        DebtListChanged?.Invoke();
    }

    public async Task<bool> UpdateDebtAsync(DebtModel debt)
    {
        debt.UpdateDateTime = DateTime.UtcNow;

        try
        {
            OpenDB();
            if (AllDebts.Update(debt))
            {
                Debug.WriteLine("Debt updated locally");

                int index = DebtList.FindIndex(x => x.Id == debt.Id);
                DebtList[index] = debt;
                if (!IsBatchUpdate)
                {
                    DebtListChanged?.Invoke();
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
        OpenDB();
        db.DropCollection(DebtsCollectionName);
        db.Dispose();
        Debug.WriteLine("debts Collection dropped!");
    }

    public async Task LogOutUserAsync()
    {
        DebtList.Clear();
        DebtListChanged?.Invoke();

        await DropDebtCollection();
    }
}