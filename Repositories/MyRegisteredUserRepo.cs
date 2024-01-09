using IfRolesExample.Data;
using IfRolesExample.Models;

namespace IfRolesExample.Repositories
{
    public class MyRegisteredUserRepo
    {
        private readonly ApplicationDbContext _db;

        public MyRegisteredUserRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public void AddMyRegisteredUser(MyRegisteredUser user)
        {
            _db.MyRegisteredUsers.Add(user);
            _db.SaveChanges();
        }

        public string GetUserFullNameByEmail(string email)
        {
            var user = _db.MyRegisteredUsers
                          .Where(u => u.Email == email)
                          .FirstOrDefault();

            return user?.FirstName + " " + user?.LastName;
        }
    }
}
