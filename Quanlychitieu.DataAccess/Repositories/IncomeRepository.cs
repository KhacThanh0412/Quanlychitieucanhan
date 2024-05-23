using LiteDB;

namespace Quanlychitieu.DataAccess.Repositories;

public class IncomeRepository : IIncomeRepository
{
    LiteDatabase db;
    public List<IncomeModel> IncomesList { get; set; }

    ILiteCollection<IncomeModel> AllIncomes;

    readonly IDataAccessRepo dataAccessRepo;
    readonly IUsersRepository usersRepo;
    readonly IOnlineCredentialsRepository onlineDataAccessRepo;

    private const string incomesDataCollectionName = "IncomesCollection";

    bool IsBatchUpdate;
    public event Action IncomesListChanged;

    public IncomeRepository(IDataAccessRepo dataAccess, IUsersRepository userRepository)
    {
        dataAccessRepo = dataAccess;
        usersRepo = userRepository;
        IncomesList = new List<IncomeModel>();
    }

    void OpenDB()
    {
        db = dataAccessRepo.GetDb();
        AllIncomes = db.GetCollection<IncomeModel>(incomesDataCollectionName);
        AllIncomes.EnsureIndex(x => x.Id);
    }

    public async Task<List<IncomeModel>> GetAllIncomesAsync()
    {
        if (IncomesList is not null)
        {
            return IncomesList;
        }
        try
        {
            OpenDB();
            string userId = usersRepo.User?.Id;
            string userCurrency = usersRepo.User.UserCurrency;
            IncomesList = AllIncomes.Query()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UpdatedDateTime)
                .ToList();
            db.Dispose();
            IncomesList ??= new List<IncomeModel>();
            return IncomesList;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException?.Message);
            Debug.WriteLine("Get all INC function Exception: " + ex.Message);
            IncomesList ??= new List<IncomeModel>();
            return IncomesList;
        }
    }

    public async Task<bool> AddIncomeAsync(IncomeModel newIncome)
    {
        try
        {
            OpenDB();

            if (AllIncomes.Insert(newIncome) is not null)
            {
                IncomesList.Add(newIncome);
                IncomesListChanged?.Invoke();
                db.Dispose();
                return true;
            }
            else
            {
                Debug.WriteLine("Error while adding Income");
                return false;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to add local income: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateIncomeAsync(IncomeModel income)
    {
        try
        {
            OpenDB();
            if (AllIncomes.Update(income))
            {
                Debug.WriteLine("Income updated Locally");

                int index = IncomesList.FindIndex(x => x.Id == income.Id);
                IncomesList[index] = income;
                if (!IsBatchUpdate)
                {
                    IncomesListChanged?.Invoke();
                }
                db.Dispose();
                return true;
            }
            else
            {
                Debug.WriteLine("Error while updating Income");
                return false;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to handle local income: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteIncomeAsync(IncomeModel income)
    {
        income.IsDeleted = true;

        try
        {
            OpenDB();
            if (AllIncomes.Update(income))
            {
                IncomesList.Remove(income);
                Debug.WriteLine("Income deleted Locally");
                if (!IsBatchUpdate)
                {
                    IncomesListChanged?.Invoke();
                }

                db.Dispose();
                return true;
            }
            else
            {
                Debug.WriteLine("Error while deleting Income");
                return false;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to handle local income: " + ex.Message);
            return false;
        }
    }

    public async Task DropIncomesCollection()
    {
        OpenDB();
        db.DropCollection(incomesDataCollectionName);
        db.Dispose();
        Debug.WriteLine("Incomes Collection dropped!");
    }

    public async Task LogOutUserAsync()
    {
        IncomesList.Clear();
        IncomesListChanged?.Invoke();
        await DropIncomesCollection();

    }

    bool IsSyncing;
    public async Task SynchronizeIncomesAsync()
    {
        await GetAllIncomesAsync();
        IsBatchUpdate = false;
        IncomesListChanged?.Invoke();
    }
}
