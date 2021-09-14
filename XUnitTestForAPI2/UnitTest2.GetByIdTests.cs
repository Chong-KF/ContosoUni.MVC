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


namespace XUnitTestForAPI2
{
    public partial class UnitTest2
    {

        [Fact]
        public async Task GetDepartmentById_NotFoundResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            Guid findDeptID = Guid.Parse("40204a97-831a-43f5-b096-000000000000");

            // ACT - Get the Department - HTTP GET: api/DepartmentsApi2/{id}
            var actionResultGet = await controller.GetDepartment(findDeptID);

            // ASSERT - if the IActionResult is NotOk (HTTP 404)
            Assert.IsType<NotFoundResult>(actionResultGet);

            // ASSERT - if the Status Code is (HTTP 404) "NotFound"
            var expectedStatusCode = (int)System.Net.HttpStatusCode.NotFound;
            Assert.Equal<int>(expectedStatusCode, (actionResultGet as NotFoundResult).StatusCode);
        }

        [Fact]
        public async Task GetDepartmentById_BadRequestResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            Guid? findDeptID = null;

            // ACT - Get the Department - HTTP GET: api/DepartmentsApi2/{id}
            var actionResultGet = await controller.GetDepartment(findDeptID);

            // ASSERT - if the IActionResult is NotOk (HTTP 400)
            Assert.IsType<BadRequestResult>(actionResultGet);

            // ASSERT - if the Status Code is HTTP 400
            var expectedStatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            Assert.Equal<int>(expectedStatusCode, (actionResultGet as BadRequestResult).StatusCode);
        }

        [Fact]
        public async Task GetDepartmentById_OkResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            Guid findDeptID = Guid.Parse("40d4d4f1-130b-4e6a-bfc6-dfd609fd3e26");

            // ACT - Get the Department - HTTP GET: api/DepartmentsApi2/{id}
            var actionResultGet = await controller.GetDepartment(findDeptID);

            // ASSERT - if the IActionResult is Ok (HTTP 200)
            Assert.IsType<OkObjectResult>(actionResultGet);

            // ASSERT - if the Status Code is (HTTP 200) "Ok"
            //          NOTE: The OkObjectResult may or may not contain an object.  
            //                Hence, the StatusCode would be a nullable int!
            var expectedStatusCode = (int)System.Net.HttpStatusCode.OK;
            Assert.Equal<int>(expectedStatusCode, (actionResultGet as OkObjectResult).StatusCode.Value);
        }

        [Fact]
        public async Task GetDepartmentById_CorrectResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            Guid findDeptID = Guid.Parse("40d4d4f1-130b-4e6a-bfc6-dfd609fd3e26");

            // ACT - Get the Department
            // ---- (a) HTTP GET: api/DepartmentsApi2/{id}
            var actionResultGet = await controller.GetDepartment(findDeptID);

            // ASSERT - if the IActionResult is Ok (HTTP 200)
            Assert.IsType<OkObjectResult>(actionResultGet);

            // ---- (b) Extract the result from the IActionResult object.
            var okResult = actionResultGet.Should()
                                          .BeOfType<OkObjectResult>()
                                          .Subject;

            // ASSERT - if the OkResult contains an Object of the correct type (Department)
            Assert.IsType<Department>(okResult.Value);

            // ---- (c) Extract the department from the result of the action.
            var department = okResult.Value
                                     .Should()
                                     .BeAssignableTo<Department>()
                                     .Subject;

            // ASSERT - if the department object is NOT NULL.
            Assert.NotNull(department);

            // ASSERT - if the department object contains the expected data.
            Assert.Equal<Guid>(findDeptID, department.DepartmentID);

            _testOutputHelper.WriteLine($"Found: {department.DepartmentName} [ {department.DepartmentID} ]");
        }
    }
}
