using FellesForumAPI.Data;
using FellesForumAPI.Helpers;

namespace FellesForumAPI.Services
{
    public class BaseService
    {
        private readonly ApplicationDbContext _db;

        public BaseService(ApplicationDbContext db)
        {
            _db = db;
        }

        protected async Task<Outcome> InjectDatabaseFunc(Func<ApplicationDbContext, Task<Outcome>> func)
        {
            try
            {
                return await func(_db);
            }
            catch (Exception ex)
            {
                return new Outcome(false, "Server error");
            }
        }

        protected async Task<Outcome<T>> InjectDatabaseFunc<T>(Func<ApplicationDbContext, Task<Outcome<T>>> func)
        {
            try
            {
                return await func(_db);
            }
            catch (Exception ex)
            {
                return new Outcome<T>(false, message:"Server error");
            }
        }

        protected async Task<Outcome<T>> InjectFunc<T>(Func<Task<Outcome<T>>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                return new Outcome<T>(false, message: "Server error");
            }
        }

        protected async Task<Outcome> InjectFunc(Func<Task<Outcome>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                return new Outcome(false, message: "Server error");
            }
        }
    }
}
