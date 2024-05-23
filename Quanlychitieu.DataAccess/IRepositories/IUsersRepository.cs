namespace Quanlychitieu.DataAccess.IRepositories;

public interface IUsersRepository
{
    event Action UserDataChanged;
    UsersModel User { get; set; }
    Task<UsersModel> GetUserAsync(string userEmail, string userPassword);
    Task<UsersModel> GetUserAsync(string userId);
    Task<bool> AddUserAsync(UsersModel user);
    Task<bool> UpdateUserAsync(UsersModel user);
    Task<bool> DeleteUserAsync(UsersModel user);
    Task<bool> CheckIfAnyUserExists();
    Task LogOutUserAsync();
    Task DropCollection();
}
