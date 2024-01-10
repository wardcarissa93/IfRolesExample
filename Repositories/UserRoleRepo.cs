using IfRolesExample.Data;
using IfRolesExample.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IfRolesExample.Repositories
{
    public class UserRoleRepo
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public UserRoleRepo(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<UserRoleResult> CreateUserRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var hasRole = await _userManager.IsInRoleAsync(user, roleName);
                if (hasRole)
                {
                    return new UserRoleResult { Success = false, ErrorMessage = $"User {email} already has the role {roleName}." };
                }

                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return new UserRoleResult { Success = true };
                }

                return new UserRoleResult { Success = false, ErrorMessage = "Failed to add role to user." };
            }

            return new UserRoleResult { Success = false, ErrorMessage = "User not found." };
        }

        public async Task<UserRoleResult> RemoveUserRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return new UserRoleResult { Success = true };
                }

                return new UserRoleResult { Success = false, ErrorMessage = "Failed to remove role from user." };
            }

            return new UserRoleResult { Success = false, ErrorMessage = "User not found." };
        }

        public async Task<IEnumerable<RoleVM>> GetUserRolesAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.Select(roleName => new RoleVM { RoleName = roleName });
            }

            return Enumerable.Empty<RoleVM>();
        }

        public async Task<string> GetUserFullNameAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var registeredUser = await _db.MyRegisteredUsers.SingleOrDefaultAsync(u => u.Email == user.Email);

                if (registeredUser != null)
                {
                    return $"{registeredUser.FirstName} {registeredUser.LastName}";
                }
            }

            return null;
        }

        public class UserRoleResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
