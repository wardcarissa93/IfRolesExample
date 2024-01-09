using IfRolesExample.Data;
using IfRolesExample.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace IfRolesExample.Repositories
{
    public class RoleRepo
    {
        private readonly ApplicationDbContext _context;

        public RoleRepo(ApplicationDbContext context)
        {
            this._context = context;
            CreateInitialRole();
        }

        public List<RoleVM> GetAllRoles()
        {
            var roles = _context.Roles.Select(r => new RoleVM
            {
                Id = r.Id,
                RoleName = r.Name
            }).ToList();

            return roles;
        }

        public RoleVM GetRole(string roleName)
        {
            var role =
                _context.Roles.Where(r => r.Name == roleName)
                              .FirstOrDefault();

            if (role != null)
            {
                return new RoleVM()
                {
                    RoleName = role.Name
                                    ,
                    Id = role.Id
                };
            }
            return null;
        }

        public bool CreateRole(string roleName)
        {
            bool isSuccess = true;

            try
            {
                _context.Roles.Add(new IdentityRole
                {
                    Name = roleName,
                    Id = roleName,
                    NormalizedName = roleName.ToUpper()
                });
                _context.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public void CreateInitialRole()
        {
            const string ADMIN = "Admin";

            var role = GetRole(ADMIN);

            if (role == null)
            {
                CreateRole(ADMIN);
            }
        }

        public bool IsRoleAssigned(string roleId)
        {
            return _context.UserRoles.Any(ur => ur.RoleId == roleId);
        }


        public bool DeleteRole(string roleId)
        {
            bool isSuccess = true;

            try
            {
                var role = _context.Roles.Find(roleId);

                if (role != null)
                {
                    _context.Roles.Remove(role);
                    _context.SaveChanges();
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return isSuccess;
        }
    }
}
