using IfRolesExample.Data;
using IfRolesExample.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

public class RoleRepo
{
    private readonly ApplicationDbContext _db;

    public RoleRepo(ApplicationDbContext context)
    {
        this._db = context;
        CreateInitialRole();
    }

    public IEnumerable<RoleVM> GetAllRoles()
    {
        return _db.Roles.Select(r => new RoleVM { RoleName = r.Name });
    }

    public RoleVM GetRole(string roleName)
    {
        var role = _db.Roles.FirstOrDefault(r => r.Name == roleName);

        return role != null ? new RoleVM { RoleName = role.Name } : null;
    }

    public bool CreateRole(string roleName)
    {
        var normalizedRoleName = roleName.ToUpper();

        if (_db.Roles.Any(r => r.NormalizedName == normalizedRoleName))
        {
            return false; // Role already exists
        }

        _db.Roles.Add(new IdentityRole
        {
            Id = normalizedRoleName,
            Name = roleName,
            NormalizedName = normalizedRoleName
        });

        _db.SaveChanges();

        return true;
    }

    public SelectList GetRoleSelectList()
    {
        return new SelectList(GetAllRoles().Select(r => new SelectListItem
        {
            Value = r.RoleName,
            Text = r.RoleName
        }), "Value", "Text");
    }

    public void CreateInitialRole()
    {
        const string ADMIN = "Admin";

        if (GetRole(ADMIN) == null)
        {
            CreateRole(ADMIN);
        }
    }

    public bool DeleteRole(string roleName)
    {
        var role = _db.Roles.FirstOrDefault(r => r.Name == roleName);

        if (role == null)
        {
            return false; // Role not found
        }

        _db.Roles.Remove(role);
        _db.SaveChanges();

        return true;
    }

    public bool IsRoleAssigned(string roleName)
    {
        return _db.UserRoles.Any(ur => ur.RoleId == roleName);
    }
}

