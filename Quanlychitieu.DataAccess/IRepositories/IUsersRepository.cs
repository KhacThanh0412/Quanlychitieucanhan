namespace Quanlychitieu.DataAccess.IRepositories;

public interface IUsersRepository
{
    event Action UserDataChanged;
    UsersModel User { get; set; }
    Task<UsersModel> GetUserAsync(string userEmail, string userPassword);
    Task<UsersModel> LoginAsync(string userEmail, string userPassword);
    Task<bool> RegisterAsync(string userName, string userEmail, string userPassword);
    Task<UsersModel> GetUserAsync();
    Task<bool> AddUserAsync(UsersModel user);
    Task<bool> UpdateUserAsync(UsersModel user);
    Task<bool> CheckIfAnyUserExists();
    Task LogOutUserAsync();
}