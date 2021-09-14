using System;
using System.Collections.Generic;

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
        /// <remarks>
        ///     Bad update data scenarios:
        ///     - Name is NULL
        ///     - Name is EMPTY STRING
        ///     - Name contains more characters than what is allowed
        ///     - NULL Department object
        ///     - ID not matching with the ID of the row identified to be updated.
        ///     - ID replaced with a negative value
        ///     - Replacing the object retrieved before the update, with a completely new object
        ///     - since the HTTP PUT receives a Nullable INT as first parameter, pass a NULL value
        /// </remarks>

        [Fact]
        public async Task EditDepartment_OkResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            Guid editDeptId = Guid.Parse("ace4c376-7cb9-429a-9c59-4e765bbd5f0e");
            Department originalDept, changedDept;
            changedDept = new Department
            {
                DepartmentID = editDeptId,
                DepartmentName = "Commerce Department"
            };

            // ACT - Get the Department to edit (to ensure that the row exists before editing it).
            // ---- (a) HTTP GET: api/DepartmentsApi2/3
            var actionResultGet = await controller.GetDepartment(editDeptId);
            // ---- (b) Check if HTTP 200 Ok returned
            var okResult = actionResultGet.Should()
                                          .BeOfType<OkObjectResult>()
                                          .Subject;
            // ---- (c) Extract the Department from the result of the action.
            originalDept = okResult.Value
                                   .Should()
                                   .BeAssignableTo<Department>()
                                   .Subject;
            // ---- (d) Check if the data to be edited was received from the API
            Assert.NotNull(originalDept);

            _testOutputHelper.WriteLine("Retrieved the Data from the API.");

            try
            {
                // ACT - Try to update the data, using a completely new object
                //       NOTE: This will throw the InvalidOperationException!
                var actionResultPutAttempt1 = await controller.PutDepartment(editDeptId, changedDept);

                // ASSERT - if the update was successfull.
                Assert.IsType<OkResult>(actionResultPutAttempt1);

                _testOutputHelper.WriteLine("Updated the changes back to the API - using a new object.");
            }
            catch(System.InvalidOperationException exp)
            {
                // The PUT operation should throw this exception,
                // because it is an object that does not carry change tracking information.

                _testOutputHelper.WriteLine("Failed to update the change back to the API - using a new object");
                _testOutputHelper.WriteLine($"-- Exception Type: {exp.GetType()}");
                _testOutputHelper.WriteLine($"-- Exception Message: {exp.Message}");
                _testOutputHelper.WriteLine($"-- Exception Source: {exp.Source}");
                _testOutputHelper.WriteLine($"-- Exception TargetSite: {exp.TargetSite}");
            }

            // SOLUTION: The following lines would work!
            //           Reason, we are modifying the data originally received.
            //           And then, calling the PUT operation.
            //           So, the API is able to find the ChangeTracking data associated to the object.

            // ACT - Try to update the data, using a completely new object
            // ----- (a) Change the data of the object that was received from the API.
            originalDept.DepartmentName = changedDept.DepartmentName;
            // ----- (b) Call the HTTP PUT API to update the changes (with the updated object)
            var actionResultPutAttempt2 = await controller.PutDepartment(editDeptId, originalDept);

            // ASSERT - if the update was successfull.
            Assert.IsType<OkResult>(actionResultPutAttempt2);

            _testOutputHelper.WriteLine("Updated the changes back to the API - using the object received");
        }


        [Fact]
        public async Task EditDepartment_NotOkResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            Guid editDeptId = Guid.Parse("ace4c376-7cb9-429a-9c59-4e765bbd5f0e");
            Department editDepartment;

            // ACT - Get the Department to edit (to ensure that the row exists before editing it).
            // ---- (a) HTTP GET: api/DepartmentsApi2/3
            var actionResultGet = await controller.GetDepartment(editDeptId);
            // ---- (b) Check if HTTP 200 Ok returned
            var okResult = actionResultGet.Should()
                                          .BeOfType<OkObjectResult>()
                                          .Subject;
            // ---- (c) Extract the Department from the result of the action.
            editDepartment = okResult.Value
                                     .Should()
                                     .BeAssignableTo<Department>()
                                     .Subject;
            // ---- (d) Check if the data to be edited was received from the API
            Assert.NotNull(editDepartment);

            _testOutputHelper.WriteLine("Retrieved the Data from the API.");


            // ACT - Update the data, using a completely new object
            // ----- (a) Change the data of the object that was received from the API with a new one!
            editDepartment = new Department
            {
                DepartmentID = Guid.Parse("40204a97-831a-43f5-b096-000000000000"),         // INVALID: Id does not match with the ID of row to be edited!
                DepartmentName = "Commerce Department"
            };
            // ----- (b) Call the HTTP PUT API to update the changes (with the updated object)
            var actionResultPut = await controller.PutDepartment(editDeptId, editDepartment);

            // ASSERT - Confirm that the update was unsuccessfull.
            Assert.IsType<BadRequestResult>(actionResultPut);

            // ASSERT - if the Status Code is HTTP 400
            var expectedStatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            Assert.Equal<int>(expectedStatusCode, (actionResultPut as BadRequestResult).StatusCode);

        }

    }
}
