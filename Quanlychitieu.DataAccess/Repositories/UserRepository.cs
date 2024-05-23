﻿using LiteDB;
using System;
using System.Threading.Tasks;

namespace Quanlychitieu.DataAccess.Repositories
{
    public class UserRepository : IUsersRepository
    {
        private LiteDatabase db;
        private ILiteCollection<UsersModel> userCollection;

        private readonly IDataAccessRepo dataAccessRepo;

        private const string userDataCollectionName = "Users";

        public event Action UserDataChanged;

        public UsersModel User { get; set; }

        public UserRepository(IDataAccessRepo dataAccess)
        {
            dataAccessRepo = dataAccess;
        }

        void OpenDB()
        {
            db = dataAccessRepo.GetDb();
            userCollection = db.GetCollection<UsersModel>(userDataCollectionName);
        }

        public async Task<bool> CheckIfAnyUserExists()
        {
            OpenDB();
            int numberOfUsers = userCollection.Count();
            db.Dispose();
            return numberOfUsers >= 1;
        }

        public async Task<UsersModel> GetUserAsync(string userEmail, string userPassword)
        {
            OpenDB();
            User = userCollection.FindOne(x => x.Email == userEmail && x.Password == userPassword);
            db.Dispose();
            return User;
        }

        public async Task<UsersModel> GetUserAsync(string userId)
        {
            OpenDB();
            User = userCollection.FindOne(x => x.Id == userId);
            db.Dispose();
            UserDataChanged?.Invoke();
            return User;
        }

        public async Task<bool> AddUserAsync(UsersModel user)
        {
            if (GetUserAsync(user.Email, user.Password).Result == null)
            {
                OpenDB();
                userCollection.Insert(user);
                userCollection.EnsureIndex(x => x.Id);
                db.Dispose();
                User = user;
                return true;
            }
            else
            {
                return false; // User already exists
            }
        }

        public async Task<bool> UpdateUserAsync(UsersModel user)
        {
            try
            {
                OpenDB();
                if (userCollection.Update(user))
                {
                    db.Dispose();
                    User = user;
                    UserDataChanged?.Invoke();
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed to Update User");
                    db.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update user Exception Message: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(UsersModel user)
        {
            try
            {
                OpenDB();
                if (userCollection.Delete(user.Id))
                {
                    db.Dispose();
                    return true;
                }
                else
                {
                    Debug.WriteLine("Failed to delete User");
                    db.Dispose();
                    throw new Exception("Failed to Delete User");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete user Exception Message: {ex.Message}");
                return false;
            }
        }

        public async Task DropCollection()
        {
            OpenDB();
            db.DropCollection(userDataCollectionName);
            db.Dispose();
        }

        public async Task LogOutUserAsync()
        {
            User = null;
            await DropCollection();
        }
    }
}
