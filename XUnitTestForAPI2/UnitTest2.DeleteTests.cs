using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;
using ContosoUni.Models;
using ContosoUni.Controllers;
using FluentAssertions;
using System.Net;

namespace XUnitTestForAPI2
{
    public partial class UnitTest2
    {
        [Fact]
        public async Task DeleteDepartment_OkResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);
            Guid deptToDelete = Guid.Parse("40204a97-831a-43f5-b096-c8711f620990");

            // ACT - HTTP DELETE
            var actionResultDelete = await controller.DeleteDepartment(deptToDelete);

            // ASSERT
            Assert.IsType<OkResult>(actionResultDelete);
        }

        [Fact]
        public async Task DeleteDepartment_NotFoundResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);
            Guid deptToDelete = Guid.Parse("40d4d4f1-130b-4e6a-bfc6-000000000000");         // Invalid

            // ACT - HTTP DELETE
            var actionResultDelete = await controller.DeleteDepartment(deptToDelete);

            // ASSERT
            Assert.IsType<NotFoundResult>(actionResultDelete);
        }

        [Fact]
        public async Task DeleteDepartment_BadRequestResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);
            Guid? deptToDelete = null;       // Invalid

            // ACT - HTTP DELETE
            var actionResultDelete = await controller.DeleteDepartment(deptToDelete);

            // ASSERT
            Assert.IsType<BadRequestResult>(actionResultDelete);
        }
    }
}
