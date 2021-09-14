using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ContosoUni.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace ContosoUni.Data
{
    public class InitialSeed
    {
        public static async Task SeedAsync(UserManager<MyIdentityUser> userManager, RoleManager<MyIdentityRole> roleManager)
        {
            //Seed Roles
            foreach (MyRoles role in Enum.GetValues(typeof(MyRoles)))
            {
                MyIdentityRole roleObj = new ()
                {
                    Name = role.ToString(),
                    Description = $"The {role} for the Application"
                };
                await roleManager.CreateAsync(roleObj);
            }

            //Seed Default User
            var defaultUser = new MyIdentityUser
            {
                UserName = "admin@gmail.com",
                DisplayName = "Admin",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                //PhoneNumberConfirmed = true,
                IsActive=true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "W7TzX6ZJjUQ-X74");
                    await userManager.AddToRoleAsync(defaultUser, MyRoles.Administrator.ToString());
                    await userManager.AddToRoleAsync(defaultUser, MyRoles.Staff.ToString());
                }
                //else
                //{
                //   await userManager.AddToRoleAsync(user, MyRoles.Staff.ToString());
                //}
            }

            //set all user to active
            //foreach (var user in userManager.Users.ToList())
            //{
                //user.IsActive = true;
                //user.PhoneNumberConfirmed = false;
                //userManager.UpdateAsync(user).Wait();
            //}
        }
    }
}
