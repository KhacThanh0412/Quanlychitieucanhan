using LiteDB;
using Quanlychitieu.DataAccess.IRepositories;
using System.Collections.ObjectModel;

namespace Quanlychitieu.DataAccess.Repositories;

public class DebtRepository : IDebtRepository
{
    private readonly IDataAccessRepo dataAccess;
    private readonly IUsersRepository usersRepo;

    public ObservableCollection<DebtModel> DebtList { get; set; }

    public event Action DebtListChanged;
    bool IsSyncing;
    private bool IsBatchUpdate;
    public DebtRepository(IDataAccessRepo dataAccess, IUsersRepository userRepo)
    {
        this.dataAccess = dataAccess;
        usersRepo = userRepo;
        DebtList = new ObservableCollection<DebtModel>();
    }

    public async Task<ObservableCollection<DebtModel>> GetAllDebtAsync()
    {
        try
        {
            var userJson = await usersRepo.GetUserAsync();
            var userId = userJson.Id;
            if (userId == null)
            {
                Console.WriteLine("=====> User not logged in");
                return new ObservableCollection<DebtModel>();
            }

            var debts = await dataAccess.GetDataFromApiAsync<ObservableCollection<DebtModel>>($"api/v1/debt/user/{userId}");
            if (debts != null)
            {
                DebtList = debts;
                DebtListChanged?.Invoke();
            }
            else
            {
                debts = new ObservableCollection<DebtModel>();
            }

            return DebtList;

        }
        catch (Exception ex)
        {
            return DebtList;
        }
    }

    public async Task<bool> AddDebtAsync(DebtModel debt)
    {
        
        try
        {
            var response = await dataAccess.PostDataToApiAsync("api/v1/add-debt", debt);
            if (response.IsSuccessStatusCode)
            {
                await GetAllDebtAsync();
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to add local debt: " + ex.InnerException.Message);
            return false;
        }
    }

    public async Task<bool> DeleteDebtAsync(DebtModel debt)
    {
        try
        {
            var response = await dataAccess.DeleteDataFromApiAsync($"api/v1/delete-debt/{debt.Id}");
            if (response.IsSuccessStatusCode)
            {
                await GetAllDebtAsync();
                return true;
            }
            else
            {
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

        try
        {

            var response = await dataAccess.PutDataToApiAsync("api/v1/update-debt", debt);
            if (response.IsSuccessStatusCode)
            {
                await GetAllDebtAsync();
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception when doing local dept update " + ex.InnerException.Message);
            return false;
        }
    }
}