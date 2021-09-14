using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Identity;
using ContosoUni.Areas.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using ContosoUni.Data;

namespace ContosoUni.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Administrator")]
    public class UserDetailsModel : PageModel
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public UserDetailsModel(
            UserManager<MyIdentityUser> userManager,
            SignInManager<MyIdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public MyUserModel UserModel { get; set; }
        public string StatusMessage { get; set; }
        public class MyUserModel
        {
            [Display(Name = "User ID")]
            public Guid UserID { get; set; }

            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "Display Name")]
            [MinLength(3, ErrorMessage = "Name must have more then 2 characters")]
            [MaxLength(60, ErrorMessage = "Name must not more then 60 characters")]
            public string DisplayName { get; set; }

            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Email Confirmed")]
            public bool EmailConfirmed { get; set; }

            [Display(Name = "Phone Number")]
            [MinLength(8, ErrorMessage = "Name must have more then 7 characters")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Phone Number Confirmed")]
            public bool PhoneNumberConfirmed { get; set; }

            [Display(Name = "Two-Factor Authentication")]
            public bool TwoFactorEnabled { get; set; }

            [Display(Name = "Lockout Enabled")]
            public bool LockoutEnabled { get; set; }

            [Display(Name = "Lockout End")]
            public DateTimeOffset? LockoutEnd { get; set; }

            [Display(Name = "Access Failed Count")]
            public int AccessFailedCount { get; set; }

            [Display(Name = "IsActive")]
            public bool IsActive { get; set; }

            [Display(Name = "Roles")]
            public IList<string> Roles { get; set; }

            [Display(Name = "External Login")]
            public string ExternalLoginProvider { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            await LoadAsync(id);
            return Page();
        }

        private async Task LoadAsync(Guid id)
        {
            MyIdentityUser user = await _userManager.FindByIdAsync(id.ToString().ToUpper());
            UserModel = new();
            UserModel.UserID = user.Id;
            UserModel.UserName = user.UserName;
            UserModel.DisplayName = user.DisplayName;
            UserModel.Email = user.Email;
            UserModel.EmailConfirmed = user.EmailConfirmed;
            UserModel.PhoneNumber = user.PhoneNumber;
            UserModel.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            UserModel.TwoFactorEnabled = user.TwoFactorEnabled;
            UserModel.LockoutEnabled = user.LockoutEnabled;
            UserModel.LockoutEnd = user.LockoutEnd;
            UserModel.AccessFailedCount = user.AccessFailedCount;
            UserModel.IsActive = user.IsActive;
            UserModel.Roles = await _userManager.GetRolesAsync(user);
            UserModel.ExternalLoginProvider = (from p in _context.UserLogins
                                                     where p.UserId == user.Id
                                                     select p.ProviderDisplayName).FirstOrDefault();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)  //validation valid
            {
                return Page();
            }
            MyIdentityUser user = await _userManager.FindByIdAsync(id.ToString().ToUpper());
            user.DisplayName = UserModel.DisplayName;
            user.EmailConfirmed = UserModel.EmailConfirmed;
            user.PhoneNumber = UserModel.PhoneNumber;
            user.PhoneNumberConfirmed = UserModel.PhoneNumberConfirmed;
            user.TwoFactorEnabled = UserModel.TwoFactorEnabled;
            user.LockoutEnabled = UserModel.LockoutEnabled;
            user.AccessFailedCount = UserModel.AccessFailedCount;
            user.IsActive = UserModel.IsActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = result.Errors.ToList().ToString();
                return Page();
            }
            IList<string> roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRolesAsync(user, UserModel.Roles);
            //await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profile has been updated";
            await LoadAsync(id);
            return Page();
        }
    }
}
