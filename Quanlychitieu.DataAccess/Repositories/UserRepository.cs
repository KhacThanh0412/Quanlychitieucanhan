
namespace Quanlychitieu.DataAccess.Repositories;

public class UserRepository : IUsersRepository
{
    public UsersModel OfflineUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public UsersModel OnlineUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event Action OfflineUserDataChanged;

    public Task<bool> AddUserAsync(UsersModel user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddUserOnlineAsync(UsersModel user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfAnyUserExists()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(UsersModel user)
    {
        throw new NotImplementedException();
    }

    public Task DropCollection()
    {
        throw new NotImplementedException();
    }

    public Task<UsersModel> GetUserAsync(string UserEmail, string UserPassword)
    {
        throw new NotImplementedException();
    }

    public Task<UsersModel> GetUserAsync(string UserId)
    {
        throw new NotImplementedException();
    }

    public Task<UsersModel> GetUserOnlineAsync(UsersModel user)
    {
        throw new NotImplementedException();
    }

    public Task LogOutUserAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserAsync(UsersModel user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserOnlineAsync(UsersModel user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserOnlineGetSetLatestValues(UsersModel user)
    {
        throw new NotImplementedException();
    }
}
