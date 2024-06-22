using Newtonsoft.Json;

namespace Quanlychitieu.DataAccess.Repositories;

public class IncomeRepository : IIncomeRepository
{
    public List<IncomeModel> IncomesList { get; set; }

    readonly IDataAccessRepo dataAccessRepo;
    readonly IUsersRepository usersRepo;

    public event Action IncomesListChanged;

    public IncomeRepository(IDataAccessRepo dataAccess, IUsersRepository userRepository)
    {
        dataAccessRepo = dataAccess;
        usersRepo = userRepository;
        IncomesList = new List<IncomeModel>();
    }

    public async Task<List<IncomeModel>> GetAllIncomesAsync()
    {
        try
        {
            var userJson = await SecureStorage.GetAsync("user");

            var userConvert = JsonConvert.DeserializeObject<UsersModel>(userJson);
            var userId = userConvert.Id;
            if (userId == null)
            {
                Console.WriteLine("=====> User not logged in");
                return new List<IncomeModel>();
            }

            var incomes = await dataAccessRepo.GetDataFromApiAsync<List<IncomeModel>>($"api/v1/income/user/{userId}");
            if (incomes != null)
            {
                IncomesList = incomes;
                IncomesListChanged?.Invoke();
            }
            else
            {
                IncomesList = new List<IncomeModel>();
            }

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

    public double CalculateTotalIncome()
    {
        return IncomesList
            .Where(income => !income.IsDeleted)
            .Sum(income => income.AmountReceived);
    }

    public async Task<bool> AddIncomeAsync(IncomeModel newIncome)
    {
        try
        {
            return false;
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
            return false;
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
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to handle local income: " + ex.Message);
            return false;
        }
    }

    bool IsSyncing;
    public async Task SynchronizeIncomesAsync()
    {
        await GetAllIncomesAsync();
        IncomesListChanged?.Invoke();
    }
}
