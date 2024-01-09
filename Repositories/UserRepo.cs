using IfRolesExample.Data;
using IfRolesExample.ViewModels;

namespace IfRolesExample.Repositories
{
    public class UserRepo
    {
        private readonly ApplicationDbContext _context;

        public UserRepo(ApplicationDbContext context)
        {
            this._context = context;
        }

        public List<UserVM> GetAllUsers()
        {
            var userEmails = _context.Users.Select(u => new UserVM
            {
                Email = u.Email
            }).ToList();

            return userEmails;
        }
    }
}
