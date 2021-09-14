using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Identity;
using ContosoUni.Areas.Identity.Models;
using ContosoUni.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContosoUni.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Administrator")]
    public class UserListModel : PageModel
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UserListModel(
            UserManager<MyIdentityUser> userManager,
            SignInManager<MyIdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public ICollection<UserModel> userModels { get; set; }

        public class UserModel
        {
            [Display(Name = "User ID")]
            public Guid UserID { get; set; }

            [Display(Name = "Name")]
            public string DisplayName { get; set; }

            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Confirmed")]
            public bool EmailConfirmed { get; set; }

            [Display(Name = "IsActive")]
            public bool IsActive { get; set; }

            [Display(Name = "2FA")]
            public bool TwoFactorEnabled { get; set; }

            [Display(Name = "Roles")]
            public IList<string> Roles { get; set; }

            [Display(Name = "External Login")]
            public string ExternalLoginProvider { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAsync();
            return Page();
        }

        private async Task LoadAsync()
        {
            userModels = new List<UserModel>();
            var users = _userManager.Users;
            foreach (var user in users)
            {
                UserModel userModel = new();
                userModel.UserID = user.Id;
                userModel.DisplayName = user.DisplayName;
                userModel.Email = user.Email;
                userModel.EmailConfirmed = user.EmailConfirmed;
                userModel.TwoFactorEnabled = user.TwoFactorEnabled;
                userModel.IsActive = user.IsActive;
                userModel.Roles = await _userManager.GetRolesAsync(user);
                userModel.ExternalLoginProvider = await (from p in _context.UserLogins
                                                   where p.UserId == user.Id
                                                   select p.ProviderDisplayName).FirstOrDefaultAsync();
                userModels.Add(userModel);
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            var role = await _context.UserRoles.FirstOrDefaultAsync(m => m.UserId == id);
               // (from r in _context.UserRoles where r.UserId.Equals(userId) select r).FirstOrDefault();
            _context.UserRoles.Remove(role);
            var external = await _context.UserLogins.FirstOrDefaultAsync(m => m.UserId == id);
            _context.UserLogins.Remove(external);
            await _context.SaveChangesAsync();
            var user = (from u in _context.Users where u.Id.Equals(id) select u).FirstOrDefault();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await LoadAsync();
            return Page();
        }
    }
}


//// old code
//userModels = await (from user in _context.Users
//    select new UserModel
//    {
//        UserID = user.Id,
//        DisplayName = user.DisplayName,
//        Email = user.Email,
//        EmailConfirmed = user.EmailConfirmed,
//        IsActive = user.IsActive,
//        Role = (from r in _context.Roles
//                where r.Id.Equals
//                ((from i in _context.UserRoles where i.UserId.Equals(user.Id) select i.RoleId).FirstOrDefault())
//                select r.Name).FirstOrDefault(),
//        //Roles = (from r in _context.Roles
//        //        where r.Id.Equals
//        //        ((from i in _context.UserRoles where i.UserId.Equals(user.Id) select i.RoleId).ToList())
//        //        select r).ToList()
//        Roles = (from userRole in _context.UserRoles//[AspNetUserRoles]
//                    join role in _context.Roles //[AspNetRoles]//
//                    on userRole.UserId
//                    equals user.Id
//                    select role.Name).ToList()
//    }).ToListAsync();