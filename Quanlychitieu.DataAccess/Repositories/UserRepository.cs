using Newtonsoft.Json;

namespace Quanlychitieu.DataAccess.Repositories
{
    public class UserRepository : IUsersRepository
    {
        private readonly IDataAccessRepo dataAccessRepo;

        public event Action UserDataChanged;
        public UsersModel User { get; set; }

        public UserRepository(IDataAccessRepo dataAccess)
        {
            dataAccessRepo = dataAccess;
        }

        public async Task<bool> CheckIfAnyUserExists()
        {
            var users = await dataAccessRepo.GetDataFromApiAsync<List<UsersModel>>("api/v1/auth/users");
            return users != null && users.Count > 0;
        }

        public async Task<UsersModel> LoginAsync(string userEmail, string userPassword)
        {
            try
            {
                var loginData = new
                {
                    email = userEmail,
                    password = userPassword
                };

                // Sử dụng phương thức PostDataToApiAsync từ dataAccessRepo để gửi yêu cầu POST
                var response = await dataAccessRepo.PostDataToApiAsync("api/v1/auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<UserLoginResponseModel>(responseContent);
                    var user = loginResponse?.User;
                    User = user;

                    var userJson = JsonConvert.SerializeObject(user);
                    await SecureStorage.SetAsync("user", userJson);

                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RegisterAsync(string userName, string userEmail, string userPassword)
        {
            try
            {
                var registerData = new {
                    username = userName,
                    email = userEmail,
                    password = userPassword
                };
                var jsonContent = JsonConvert.SerializeObject(registerData);
                var response = await dataAccessRepo.PostDataToApiAsync("api/v1/auth/register", registerData);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }   
                else
                {
                    return false;
                }    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false;
            }
        }

        public async Task<UsersModel> GetUserAsync(string userEmail, string userPassword)
        {
            try
            {
                var users = await dataAccessRepo.GetDataFromApiAsync<List<UsersModel>>($"api/v1/auth/login");
                if (users != null && users.Count > 0)
                {
                    // Tìm người dùng có email và password trùng khớp
                    var user = users.FirstOrDefault(u => u.Email == userEmail && u.Password == userPassword);
                    Console.WriteLine($"====> {user}");
                    return user;
                }
                else
                {
                    return null; // Không tìm thấy người dùng nào trùng khớp
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return null;
            }
        }

        public async Task<UsersModel> GetUserAsync(string userId)
        {
            User = await dataAccessRepo.GetDataFromApiAsync<UsersModel>($"api/users/{userId}");
            UserDataChanged?.Invoke();
            return User;
        }

        public async Task<bool> AddUserAsync(UsersModel user)
        {
            try
            {
                var response = await dataAccessRepo.PostDataToApiAsync("api/users", user);
                if (response.IsSuccessStatusCode)
                {
                    User = user;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Add user Exception Message: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(UsersModel user)
        {
            try
            {
                //var response = await dataAccessRepo.PutDataToApiAsync($"api/users/{user.Id}", user);
                //if (response.IsSuccessStatusCode)
                //{
                //    User = user;
                //    UserDataChanged?.Invoke();
                //    return true;
                //}
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update user Exception Message: {ex.Message}");
                return false;
            }
        }

        public async Task LogOutUserAsync()
        {
            User = null;
        }
    }

    public class UserLoginResponseModel
    {
        public string Message { get; set; }
        public UsersModel User { get; set; }
    }
}
