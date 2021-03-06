using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContosoUni.Areas.Identity.Models;
using ContosoUni.Data;
using ContosoUni.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MSUnitTestForDepartmentViews
{
    public static class DbContextMocker
    {
        /// <remark>
        /// Typically, EF creates a single IServiceProvider for all contexts of the given type
        /// in an AppDomain = meaning all context instance 
        /// </remark>


        public static ApplicationDbContext GetApplicationDBContext(string databasename)
        {
            // Create a fresh service provider, therefore a fresh InMemory database instanve
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create the options for the DbContext instance,
            // telling the context to use InMemory database and the new service provider
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databasename)
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            // Create the instance of the DbContext (would be an InMemory database)
            // Note: It will use the schema as defined in the Data and Model folders
            var dbContext = new ApplicationDbContext(options);

            // Add entities to the inMemory database
            dbContext.SeedData();

            return dbContext;
        }

        // Extension method
        private static void SeedData(this ApplicationDbContext context)
        {
            Guid user = Guid.NewGuid();

            context.Departments.AddRange
            (
                new Department
                {
                    DepartmentID = new Guid("40204a97-831a-43f5-b096-c8711f620990"),
                    //DepartmentID = Guid.Parse("40204a97-831a-43f5-b096-c8711f620990"),//Guid.NewGuid(),
                    DepartmentName = "Language Department",
                    CreatedByUserId = user,
                    LastUpdatedOn = DateTime.Now,
                    IsDeleted = false
                },
                new Department
                {
                    DepartmentID = new Guid("40d4d4f1-130b-4e6a-bfc6-dfd609fd3e26"),
                    DepartmentName = "Science Department",
                    CreatedByUserId = user,
                    LastUpdatedOn = DateTime.Now,
                    IsDeleted = false
                },
                new Department
                {
                    DepartmentID = new Guid("ace4c376-7cb9-429a-9c59-4e765bbd5f0e"),
                    DepartmentName = "Maths Department",
                    CreatedByUserId = user,
                    LastUpdatedOn = DateTime.Now,
                    IsDeleted = false
                }
            ); 
            context.SaveChanges();

            //context.Users.AddRange
            //( 
            //    new MyIdentityUser
            //    {
            //        Id = new Guid("1C6C679B-71BA-44D0-F844-08D91BB6F7B2"),
            //        DisplayName = "Admin",
            //        UserName = "admin@gmail.com",
            //        Email = "admin@gmail.com",
            //        EmailConfirmed= true,
            //        PasswordHash = "AQAAAAEAACcQAAAAEIYbffGQjFvC48MtJyii/SJquuD2WBsRyRdJ7Lq9U6OXIQD3O+LZxYJDtIswoyLoUA==",
            //        SecurityStamp = "BKMKUWWHNTBTLIPRWDC4DWYNC4RA6JO7",
            //        ConcurrencyStamp = "592bfa40-c972-409d-a752-053fb997d856",
            //        IsActive = true
            //    }
            //);
            //context.SaveChanges();
        }
    }
}
