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
        public async Task GetAllDepartments_OkResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            // ACT - Get all Departments - HTTP GET: api/DepartmentsApi2/GetDepartments
            var actionResultGet = await controller.GetDepartments();

            // ASSERT - if the IActionResult is Ok (HTTP 200)
            Assert.IsType<OkObjectResult>(actionResultGet);

            // ASSERT - if the Status Code is (HTTP 200) "Ok"
            //          NOTE: The OkObjectResult may or may not contain an object.  
            //                Hence, the StatusCode would be a nullable int!
            // ---- (a) Check the Status Code, if received.
            var expectedStatusCode = (int)System.Net.HttpStatusCode.OK;
            Assert.Equal<int>(expectedStatusCode, (actionResultGet as OkObjectResult).StatusCode.Value);
        }

        [Fact]
        public async Task GetAllDepartments_CorrectResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            // ACT - Get all Departments
            // ---- (a) HTTP GET: api/DepartmentsApi2/GetDepartments
            var actionResultGet = await controller.GetDepartments();

            // ASSERT - if the IActionResult is Ok (HTTP 200)
            Assert.IsType<OkObjectResult>(actionResultGet);

            // ---- (b) Extract the result from the IActionResult object.
            var okResult = actionResultGet.Should()
                                          .BeOfType<OkObjectResult>()
                                          .Subject;

            // ASSERT - if the OkResult contains an Object of the correct type (IEnumerable<Department>)
            //          NOTE: Since this is an interface, you cannot use Assert.IsType.
            Assert.IsAssignableFrom<List<Department>>(okResult.Value);

            // ---- (c) Extract the department from the result of the action.
            var departments = okResult.Value
                                     .Should()
                                     .BeAssignableTo<List<Department>>()
                                     .Subject;

            // ASSERT - if the department object is NOT NULL.
            Assert.NotNull(departments);

            // ASSERT - if the number of depatment objects matches with the Seed Data
            Assert.Equal(departments.Count, DbContextMocker.SeedData_Departments.Length);

            // ASSERT - if the department object contains the expected data (SEED Data)
            int ndx = 0;
            foreach(Department dept in DbContextMocker.SeedData_Departments)
            {
                Assert.Equal<Guid>(dept.DepartmentID, departments[ndx].DepartmentID);
                Assert.Equal(dept.DepartmentName, departments[ndx].DepartmentName);

                _testOutputHelper.WriteLine(
                    $"{departments[ndx].DepartmentName} [ {departments[ndx].DepartmentID} ]");
                
                ndx++;
            }
        }

    }
}
