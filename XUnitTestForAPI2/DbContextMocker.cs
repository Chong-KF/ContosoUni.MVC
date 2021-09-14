using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContosoUni.Data;
using ContosoUni.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTestForAPI2
{
    public static class DbContextMocker
    {
        /// <remarks>
        ///     Typically, EF creates a single IServiceProvider for all contexts of a given type
        ///     in an AppDomain - meaning all context instances would share the same InMemory database instance.
        ///     So, ensure that each test method gets its own locally scoped InMemory database.
        /// </remarks>

        public static ApplicationDbContext GetApplicationDbContext(string databaseName)
        {
            // Create a fresh service provider, therefore a fresh InMemory database instance.
            var serviceProvider = new ServiceCollection()
                                  .AddEntityFrameworkInMemoryDatabase()
                                  .BuildServiceProvider();

            // Create a new options instance,
            // telling the context to use InMemory database and the new service provider.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                         .UseInMemoryDatabase(databaseName)
                         .UseInternalServiceProvider(serviceProvider)
                         .Options;

            // Create the instance of the DbContext (would be an InMemory database)
            // NOTE: It will use the Scema as defined in the Data and Models folders
            var dbContext = new ApplicationDbContext(options);

            // Add entities to the inmemory database
            dbContext.SeedData();

            return dbContext;
        }

        internal static readonly Department[] SeedData_Departments 
            = {                
                new Department
                {
                    DepartmentID = new Guid("40204a97-831a-43f5-b096-c8711f620990"),
                    //DepartmentID = Guid.Parse("40204a97-831a-43f5-b096-c8711f620990"),//Guid.NewGuid(),
                    DepartmentName = "Language Department",
                    CreatedByUserId = Guid.NewGuid(),
                    LastUpdatedOn = DateTime.Now,
                    IsDeleted = false
                },
                new Department
                {
                    DepartmentID = new Guid("40d4d4f1-130b-4e6a-bfc6-dfd609fd3e26"),
                    DepartmentName = "Science Department",
                    CreatedByUserId = Guid.NewGuid(),
                    LastUpdatedOn = DateTime.Now,
                    IsDeleted = false
                },
                new Department
                {
                    DepartmentID = new Guid("ace4c376-7cb9-429a-9c59-4e765bbd5f0e"),
                    DepartmentName = "Maths Department",
                    CreatedByUserId = Guid.NewGuid(),
                    LastUpdatedOn = DateTime.Now,
                    IsDeleted = false
                }
            };

        private static void SeedData(this ApplicationDbContext context)
        {
            context.Departments.AddRange(SeedData_Departments);

            context.SaveChanges();
        }
    }

}
