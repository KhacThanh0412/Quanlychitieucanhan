using System.Collections.ObjectModel;

namespace Quanlychitieu.DataAccess.Repositories;

public class ExpendituresRepository : IExpendituresRepository
{
    public ObservableCollection<ExpendituresModel> ExpendituresList { get; set; } = new ObservableCollection<ExpendituresModel>();
    bool IsBatchUpdate;

    public event Action ExpendituresListChanged;

    private readonly IDataAccessRepo dataAccessRepo;
    private readonly IUsersRepository usersRepo;

    public ExpendituresRepository(IDataAccessRepo dataAccess, IUsersRepository userRepo)
    {
        dataAccessRepo = dataAccess;
        usersRepo = userRepo;
        ExpendituresList = new ObservableCollection<ExpendituresModel>();
    }

    public async Task<ObservableCollection<ExpendituresModel>> GetAllExpendituresAsync()
    {
        try
        {
            var getUser = await usersRepo.GetUserAsync();
            var userId = getUser.Id;
            if (userId is null)
            {
                return new ObservableCollection<ExpendituresModel>();
            }

            var expends = await dataAccessRepo.GetDataFromApiAsync<ObservableCollection<ExpendituresModel>>($"api/v1/expenses/user/{userId}");
            if (expends is not null)
            {
                ExpendituresList = expends;
                ExpendituresListChanged?.Invoke();
            }
            else
            {
                ExpendituresList = new ObservableCollection<ExpendituresModel>();
            }

            return ExpendituresList;
        }
        catch (Exception)
        {
            return ExpendituresList;
        }
    }

    public double CalculateTotalExpends()
    {
        return ExpendituresList.Where(expends => !expends.IsDeleted).Sum(expend => expend.AmountSpent);
    }

    public async Task<bool> AddExpenditureAsync(ExpendituresModel expenditure)
    {
        try
        {
            var postData = new
            {
                dateSpent = expenditure.DateSpent,
                amountSpent = expenditure.AmountSpent,
                reason = expenditure.Reason,
                description = expenditure.Description,
                category = expenditure.Category,
                userId = expenditure.UserId,
            };

            var response = await dataAccessRepo.PostDataToApiAsync("api/v1/add-expenses", postData);
            if (response.IsSuccessStatusCode)
            {
                await GetAllExpendituresAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Add ExpLocal Exception: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateExpenditureAsync(ExpendituresModel expenditure)
    {
        try
        {
            var updateData = new
            {
                dateSpent = expenditure.DateSpent,
                amountSpent = expenditure.AmountSpent,
                reason = expenditure.Reason,
                description = expenditure.Description,
                category = expenditure.Category,
                userId = expenditure.UserId,
            };

            var response = await dataAccessRepo.PutDataToApiAsync($"api/v1/updateExpenses/{expenditure.Id}", updateData);
            if (response.IsSuccessStatusCode)
            {
                await GetAllExpendituresAsync();
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
        finally
        {
        }
    }

    public async Task<bool> DeleteExpenditureAsync(ExpendituresModel expenditure)
    {
        try
        {
            var response = await dataAccessRepo.DeleteDataFromApiAsync($"api/v1/deleteExpenses/{expenditure.Id}");
            if (response.IsSuccessStatusCode)
            {
                await GetAllExpendituresAsync();
                return true;
            }    

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task SynchronizeExpendituresAsync()
    {
        await GetAllExpendituresAsync();
        IsBatchUpdate = false;
        ExpendituresListChanged?.Invoke();
    }
}
