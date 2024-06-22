using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Quanlychitieu.DataAccess.Repositories;

public class IncomeRepository : IIncomeRepository
{
    public ObservableCollection<IncomeModel> IncomesList { get; set; }

    readonly IDataAccessRepo dataAccessRepo;
    readonly IUsersRepository usersRepo;

    public event Action IncomesListChanged;

    public IncomeRepository(IDataAccessRepo dataAccess, IUsersRepository userRepository)
    {
        dataAccessRepo = dataAccess;
        usersRepo = userRepository;
        IncomesList = new ObservableCollection<IncomeModel>();
    }

    public async Task<ObservableCollection<IncomeModel>> GetAllIncomesAsync()
    {
        try
        {
            var userJson = await usersRepo.GetUserAsync();
            var userId = userJson.Id;
            if (userId == null)
            {
                Console.WriteLine("=====> User not logged in");
                return new ObservableCollection<IncomeModel>();
            }

            var incomes = await dataAccessRepo.GetDataFromApiAsync<ObservableCollection<IncomeModel>>($"api/v1/income/user/{userId}");
            if (incomes != null)
            {
                IncomesList = incomes;
                IncomesListChanged?.Invoke();
            }
            else
            {
                IncomesList = new ObservableCollection<IncomeModel>();
            }

            return IncomesList;
        }
        catch (Exception)
        {
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
            var postData = new
            {
                amountReceived = newIncome.AmountReceived,
                dateReceived = newIncome.DateReceived,
                reason = newIncome.Reason,
                userId = newIncome.UserId,
            };

            var response = await dataAccessRepo.PostDataToApiAsync("api/v1/add-income", postData);
            if (response.IsSuccessStatusCode)
            {
                // Gọi lại GetAllIncomesAsync để cập nhật danh sách
                await GetAllIncomesAsync();
                return true;
            }
            else
            {
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
            var postData = new
            {
                amountReceived = income.AmountReceived,
                dateReceived = income.DateReceived,
                reason = income.Reason,
                userId = income.UserId,
            };

            var response = await dataAccessRepo.PutDataToApiAsync($"api/v1/updateIncome/{income.Id}", postData);

            if (response.IsSuccessStatusCode)
            {
                await GetAllIncomesAsync();
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to update income: " + response.ReasonPhrase);
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
        try
        {
            var response = await dataAccessRepo.DeleteDataFromApiAsync($"api/v1/income/{income.Id}");

            if (response.IsSuccessStatusCode)
            {
                await GetAllIncomesAsync();
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to delete income: " + response.ReasonPhrase);
                return false;
            }
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
