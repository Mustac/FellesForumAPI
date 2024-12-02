using FellesForumAPI.Data;
using FellesForumAPI.Helpers;
using FellesForumAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FellesForumAPI.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Checks if a user with the specified ID exists in the database and retrieves the user if found.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>An <see cref="Outcome{User}"/> indicating whether the user was found, with the user data if successful.</returns>
        public async Task<Outcome<User>> GetUserAsync(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return new Outcome<User>(isSuccessful: false, message: "User doesn't exist");

            return new Outcome<User>(isSuccessful: true, data: user, message: "User found successfully");
        }

        /// <summary>
        /// Checks if a user with the specified Phone exists in the database and retrieves the user if found.
        /// </summary>
        /// <param name="phone">The unique Phone of the user.</param>
        /// <returns>An <see cref="Outcome{User}"/> indicating whether the user was found, with the user data if successful.</returns>
        public async Task<Outcome<User>> GetUserAsync(string phone)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Phone == phone);

            if (user == null)
                return new Outcome<User>(isSuccessful: false, message: "User doesn't exist");

            return new Outcome<User>(isSuccessful: true, data: user, message: "User found successfully");
        }

    
        public async Task<Outcome<string>> SendTokenAsync(User user)
        {
            user.LastToken = CreateToken();

            // send sms with the token

            // .....

            _db.Update(user);

            bool saveSuccess = await _db.SaveChangesAsync() > 0;

            if (saveSuccess)
                return new Outcome<string>(isSuccessful: true, data: user.LastToken, message:"User token in db is updated and sms has been sent");

            return new Outcome<string>(isSuccessful: false, "Could not update the user");
        }

        public async Task<Outcome<User>> CreateUserAsync(string phone)
        {
            var user = new User
            {
                Phone = phone,
                Created = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                LastToken = CreateToken()
            };

            var response = _db.AddAsync(user);

            user = response.Result.Entity;

            bool saveSuccess = await _db.SaveChangesAsync() > 0;

            if (saveSuccess)
                return new Outcome<User>(isSuccessful: true, data: user, message: "User has been created");

            return new Outcome<User>(isSuccessful: false, message: "Could not create a user");
        }

        public async Task<Outcome<User>> UpdateUserLoginTimeAsync(User user)
        {
            user.LastLogin = DateTime.UtcNow;
            user.LastToken = CreateToken();
            _db.Update(user);

            var saveSuccess = await _db.SaveChangesAsync() > 0;

            if (saveSuccess)
                return new Outcome<User>(isSuccessful: true, data: user, message: "Login time has been updated");

            return new Outcome<User>(isSuccessful: false, message: "Could not edit login time");
        }


        private string CreateToken()
        {
            char[] token = new char[6];
            Random rand = new Random();

            for (int i = 0; i < token.Length; i++)
            {
                int number = rand.Next(0, 10); // Generate a number from 0 to 9
                token[i] = number.ToString()[0]; // Convert the number to a character
            }

            return new string(token); // Convert char array to string
        }

    }
}
