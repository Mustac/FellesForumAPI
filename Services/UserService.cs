using FellesForumAPI.Data;
using FellesForumAPI.Helpers;
using FellesForumAPI.Models;
using FellesForumAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace FellesForumAPI.Services
{
    public class UserService : BaseService
    {
        public UserService(ApplicationDbContext db) : base(db)
        {
        }

        /// <summary>
        /// Checks if a user with the specified ID exists in the database and retrieves the user if found.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>An <see cref="Outcome{User}"/> indicating whether the user was found, with the user data if successful.</returns>
        public async Task<Outcome<User>> GetUserAsync(int id)
            => await InjectDatabaseFunc(async (context) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return new Outcome<User>(isSuccessful: false, message: "User doesn't exist");

                return new Outcome<User>(isSuccessful: true, data: user, message: "User found successfully");
            });

        /// <summary>
        /// Checks if a user with the specified Phone exists in the database and retrieves the user if found.
        /// </summary>
        /// <param name="phone">The unique Phone of the user.</param>
        /// <returns>An <see cref="Outcome{User}"/> indicating whether the user was found, with the user data if successful.</returns>
        public async Task<Outcome<User>> GetUserAsync(string phone)
            => await InjectDatabaseFunc(async (context) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Phone == phone);

                if (user == null)
                    return new Outcome<User>(isSuccessful: false, message: "User doesn't exist");

                return new Outcome<User>(isSuccessful: true, data: user, message: "User found successfully");
            });


        public async Task<Outcome<string>> SendTokenAsync(User user)
            => await InjectDatabaseFunc(async (context) =>
            {
                user.LastToken = CreateToken();

                // send sms with the token

                // .....

                context.Update(user);

                bool saveSuccess = await context.SaveChangesAsync() > 0;

                if (saveSuccess)
                    return new Outcome<string>(isSuccessful: true, data: user.LastToken, message: "User token in db is updated and sms has been sent");

                return new Outcome<string>(isSuccessful: false, "Could not update the user");
            });

        public async Task<Outcome<User>> CreateUserAsync(RegisterUserDto registerUserDto)
          => await InjectDatabaseFunc(async (context) =>

                        {
                            
                            if (await context.Users.AnyAsync(x=>x.Phone == registerUserDto.Phone.ToString()))
                                return new Outcome<User>(isSuccessful: false, data: null, message: "User already exists");

                            var user = new User
                            {
                                Phone = registerUserDto.Phone.Value.ToString(),
                                Email = registerUserDto.Email,
                                Created = DateTime.UtcNow,
                                LastLogin = DateTime.UtcNow,
                                LastToken = CreateToken()
                            };

                            var response = context.AddAsync(user);

                            user = response.Result.Entity;

                            bool saveSuccess = await context.SaveChangesAsync() > 0;

                            if (saveSuccess)
                                return new Outcome<User>(isSuccessful: true, data: user, message: "User has been created");

                            return new Outcome<User>(isSuccessful: false, message: "Could not create a user");
                        });

        public async Task<Outcome<User>> UpdateUserLoginTimeAsync(User user)
                        => await InjectDatabaseFunc(async (context) =>

                        {
                            user.LastLogin = DateTime.UtcNow;
                            user.LastToken = CreateToken();
                            context.Update(user);

                            var saveSuccess = await context.SaveChangesAsync() > 0;

                            if (saveSuccess)
                                return new Outcome<User>(isSuccessful: true, data: user, message: "Login time has been updated");

                            return new Outcome<User>(isSuccessful: false, message: "Could not edit login time");
                        });


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

        public async Task<Outcome> VerifyUserAsync(LoginWithTokenDto loginToken)
        => await InjectDatabaseFunc(async (context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.Phone == loginToken.Phone.ToString());

            if (user == null)
                return new Outcome<User>(isSuccessful: false, message: "User doesn't exist");

            if(loginToken.Token != user.LastToken)
                return new Outcome<User>(isSuccessful: false, message: "Tokens do not match");

            user.LastToken = "";

            await context.SaveChangesAsync();

            return new Outcome<User>(isSuccessful: true, message: "Login Successfull");
        });

    }
}
