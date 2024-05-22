
namespace Quanlychitieu.DataAccess.Repositories;

public class UserRepository : IUsersRepository
{
    private readonly IMongoCollection<UsersModel> UserCollection;
    private readonly IDataAccessRepo dataAccessRepo;

    private const string userDataCollectionName = "Users";

    public event Action OfflineUserDataChanged;

    public UsersModel OfflineUser { get; set; }

    public UserRepository(IDataAccessRepo dataAccess)
    {
        dataAccessRepo = dataAccess;
        var db = dataAccessRepo.GetDb();
        UserCollection = db.GetCollection<UsersModel>(userDataCollectionName);
    }

    public async Task<bool> CheckIfAnyUserExists()
    {
        long numberOfUsers = await UserCollection.EstimatedDocumentCountAsync();
        return numberOfUsers >= 1;
    }

    public async Task<UsersModel> GetUserAsync(string userEmail, string userPassword)
    {
        OfflineUser = await UserCollection.Find(x => x.Email == userEmail && x.Password == userPassword).FirstOrDefaultAsync();
        OfflineUserDataChanged?.Invoke();
        return OfflineUser;
    }

    public async Task<UsersModel> GetUserAsync(string userId)
    {
        OfflineUser = await UserCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
        OfflineUserDataChanged?.Invoke();
        return OfflineUser;
    }

    public async Task<bool> AddUserAsync(UsersModel user)
    {
        if (await GetUserAsync(user.Email, user.Password) is null)
        {
            await UserCollection.InsertOneAsync(user);
            OfflineUser = user;
            return true;
        }
        return false; // User already exists
    }

    public async Task<bool> UpdateUserAsync(UsersModel user)
    {
        var result = await UserCollection.ReplaceOneAsync(x => x.Id == user.Id, user);
        if (result.IsAcknowledged && result.ModifiedCount > 0)
        {
            OfflineUser = user;
            OfflineUserDataChanged?.Invoke();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteUserAsync(UsersModel user)
    {
        var result = await UserCollection.DeleteOneAsync(x => x.Id == user.Id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task DropCollection()
    {
        await UserCollection.Database.DropCollectionAsync(userDataCollectionName);
    }
}
