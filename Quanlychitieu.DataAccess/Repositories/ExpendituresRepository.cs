using LiteDB;

namespace Quanlychitieu.DataAccess.Repositories;

public class ExpendituresRepository : IExpendituresRepository
{
    LiteDatabase db;
    public List<ExpendituresModel> ExpendituresList { get; set; } = new List<ExpendituresModel>();
    bool IsBatchUpdate;

    public event Action ExpendituresListChanged;
    ILiteCollection<ExpendituresModel> AllExpenditures;

    private readonly IDataAccessRepo dataAccessRepo;
    private readonly IUsersRepository usersRepo;
    private const string expendituresDataCollectionName = "Expenditures";

    public ExpendituresRepository(IDataAccessRepo dataAccess, IUsersRepository userRepo)
    {
        dataAccessRepo = dataAccess;
        usersRepo = userRepo;
    }

    void OpenDB()
    {
        db = dataAccessRepo.GetDb();
        AllExpenditures = db.GetCollection<ExpendituresModel>(expendituresDataCollectionName);
        AllExpenditures.EnsureIndex(x => x.Id);
    }

    public async Task<List<ExpendituresModel>> GetAllExpendituresAsync()
    {
        if (ExpendituresList is not null && ExpendituresList.Any())
        {
            return ExpendituresList;
        }
        try
        {
            OpenDB();
            string userId = usersRepo.User.Id;
            string userCurrency = usersRepo.User.UserCurrency;
            ExpendituresList = AllExpenditures.Query()
                .Where(x => x.UserId == userId && x.Currency == userCurrency).ToList();

            db.Dispose();
            ExpendituresList ??= new List<ExpendituresModel>();

            return ExpendituresList;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException?.Message);
            Debug.WriteLine("Get all EXP function Exception: " + ex.Message);
            return new List<ExpendituresModel>();
        }
    }

    public async Task<bool> AddExpenditureAsync(ExpendituresModel expenditure)
    {
        try
        {
            OpenDB();
            var result = AllExpenditures.Insert(expenditure);
            if (result != null)
            {
                if (ExpendituresList == null)
                {
                    ExpendituresList = new List<ExpendituresModel>();
                }

                ExpendituresList.Add(expenditure);
                ExpendituresListChanged?.Invoke();
                return true;
            }
            else
            {
                Debug.WriteLine("Error while inserting Expenditure");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Add ExpLocal Exception: " + ex.Message);
            return false;
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<bool> UpdateExpenditureAsync(ExpendituresModel expenditure)
    {
        try
        {
            OpenDB();
            if (AllExpenditures.Update(expenditure))
            {
                int index = ExpendituresList.FindIndex(x => x.Id == expenditure.Id);
                if (index >= 0)
                {
                    ExpendituresList[index] = expenditure;
                }
                else
                {
                    ExpendituresList.Add(expenditure);
                }
                ExpendituresListChanged?.Invoke();
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to update Expenditure");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task<bool> DeleteExpenditureAsync(ExpendituresModel expenditure)
    {
        expenditure.IsDeleted = true;
        try
        {
            OpenDB();
            if (AllExpenditures.Update(expenditure))
            {
                ExpendituresList.RemoveAll(x => x.Id == expenditure.Id);
                ExpendituresListChanged?.Invoke();
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to delete Expenditure");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            db.Dispose();
        }
    }

    public async Task SynchronizeExpendituresAsync()
    {
        await GetAllExpendituresAsync();
        IsBatchUpdate = false;
        ExpendituresListChanged?.Invoke();
    }

    public async Task DropExpendituresCollection()
    {
        OpenDB();
        db.DropCollection(expendituresDataCollectionName);
        db.Dispose();
    }

    public async Task LogOutUserAsync()
    {
        await DropExpendituresCollection();
        ExpendituresList.Clear();
        ExpendituresListChanged?.Invoke();
    }
}
