using BokuNoGame.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Services
{
    public class IdentityInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string>() 
            { 
                "Admin", 
                "User" 
            };
            foreach(var role in roles)
            {
                if(await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var adminLogin = "SuperAdmin";
            var adminPass = "1Qwertyu";
            var adminFIO = "Тихонов Егор Максимович";
            var adminBirthdate = new DateTime(1999, 8, 6);

            if(await userManager.FindByNameAsync(adminLogin) == null)
            {
                var admin = new User()
                {
                    UserName = adminLogin,
                    FullName = adminFIO,
                    BirthDate = adminBirthdate
                };
                var result = await userManager.CreateAsync(admin, adminPass);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    await userManager.AddToRoleAsync(admin, "User");
                }
            }
        }
    }
}
