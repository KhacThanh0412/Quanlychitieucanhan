using LiteDB;

namespace Quanlychitieu.DataAccess.Repositories;

public class ExpendituresRepository : IExpendituresRepository
{
    LiteDatabase db;
    public List<ExpendituresModel> OfflineExpendituresList { get; set; } = new List<ExpendituresModel>();
    bool IsBatchUpdate;

    public event Action OfflineExpendituresListChanged;
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
        if (OfflineExpendituresList is not null && OfflineExpendituresList.Any())
        {
            return OfflineExpendituresList;
        }
        try
        {
            OpenDB();
            string userId = usersRepo.OfflineUser.Id;
            string userCurrency = usersRepo.OfflineUser.UserCurrency;
            if (usersRepo.OfflineUser.UserIDOnline != string.Empty)
            {
                userId = usersRepo.OfflineUser.UserIDOnline;
            }
            OfflineExpendituresList = AllExpenditures.Query()
                .Where(x => x.UserId == userId && x.Currency == userCurrency).ToList();

            db.Dispose();
            OfflineExpendituresList ??= new List<ExpendituresModel>();

            return OfflineExpendituresList;
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
                if (OfflineExpendituresList == null)
                {
                    OfflineExpendituresList = new List<ExpendituresModel>();
                }

                OfflineExpendituresList.Add(expenditure);
                OfflineExpendituresListChanged?.Invoke();
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
                int index = OfflineExpendituresList.FindIndex(x => x.Id == expenditure.Id);
                if (index >= 0)
                {
                    OfflineExpendituresList[index] = expenditure;
                }
                else
                {
                    OfflineExpendituresList.Add(expenditure);
                }
                OfflineExpendituresListChanged?.Invoke();
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
                OfflineExpendituresList.RemoveAll(x => x.Id == expenditure.Id);
                OfflineExpendituresListChanged?.Invoke();
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
        OfflineExpendituresListChanged?.Invoke();
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
        OfflineExpendituresList.Clear();
        OfflineExpendituresListChanged?.Invoke();
    }
}
