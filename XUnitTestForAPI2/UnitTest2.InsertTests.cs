﻿using System;
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
    /// <remarks>
    ///     Bad insertion data scenarios:
    ///     - Name is NULL
    ///     - Name is EMPTY STRING
    ///     - Name contains more characters than what is allowed
    ///     - NULL Department object
    /// </remarks>
 
    public partial class UnitTest2
    {

        [Fact]
        public async Task InsertDepartment_NotOkResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            var deptToAdd = new Department
            {
                DepartmentID = new Guid(),
                DepartmentName = null                   // invalid!  DepartmentName is REQUIRED
            };

            // ACT - HTTP POST
            var actionResultPost = await controller.PostDepartment(deptToAdd);

            // ASSERT - if the insert was NOT successfull.
            Assert.IsType<BadRequestObjectResult>(actionResultPost);

            // ASSERT - if the Status Code is HTTP 400
            var expectedStatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            Assert.Equal<int>(expectedStatusCode, (actionResultPost as BadRequestObjectResult).StatusCode.Value);

            // ---- (b) Extract the result from the IActionResult object.
            var postResult = actionResultPost.Should()
                                          .BeOfType<BadRequestObjectResult>()
                                          .Subject;


            // Check if the ModelState error contains the expected error message!
            // ---- (a) ARRANGE
            var expectedError = new SerializableError
            {
                { "Post", new[] {"DepartmentName in Department is Required!"}},
            };
            // ---- (b) Extract the ModelState error received from the API on HTTP 400!
            var returnedError = Assert.IsType<SerializableError>(postResult.Value);

            // ---- (c) ASSERT
            Assert.IsType<SerializableError>(returnedError);
            Assert.Equal(expectedError, returnedError);
        }


        [Fact]
        public async Task InsertDepartment_OkResult()
        {
            // ARRANGE
            var logger = Mock.Of<ILogger<DepartmentsApi2Controller>>();
            using var dbContext = DbContextMocker.GetApplicationDbContext("MyInMemoryUnitTestDB"); // Disposable!
            var controller = new DepartmentsApi2Controller(logger, dbContext);

            var deptToAdd = new Department
            {
                DepartmentID = new Guid(),
                DepartmentName = "Arts Department"
            };

            // ACT - HTTP POST
            var actionResultPost = await controller.PostDepartment(deptToAdd);

            // ASSERT - if the insert was successfull.
            Assert.IsType<OkObjectResult>(actionResultPost);

            // ---- (a) Check if HTTP 201 returned
            var okResult = actionResultPost.Should()
                                          .BeOfType<OkObjectResult>()
                                          .Subject;
            // ---- (b) Check if the HTTP Response Code is 201 Created
            Assert.Equal((int)HttpStatusCode.Created, (okResult.Value as CreatedAtActionResult).StatusCode.Value);
            // ---- (c) Extract the Department from the result of the action.
            var deptAdded = (okResult.Value as CreatedAtActionResult).Value
                                    .Should()
                                    .BeAssignableTo<Department>()
                                    .Subject;
            // ---- (d) Check if the data inserted, matches with the data received after update from the API
            //          NOTE: Can't compare ID, since ID would be generated by DB (if not InMemory DB).
            Assert.NotNull(deptAdded);
            Assert.Equal(deptToAdd.DepartmentName, deptAdded.DepartmentName);

            _testOutputHelper.WriteLine("ID of the newly inserted data: {0}",
                (okResult.Value as CreatedAtActionResult).RouteValues["id"]);
            // same as above:
            //_testOutputHelper.WriteLine("ID of the newly inserted data: {0}",
            //    deptAdded.DepartmentID);
        }
    }
}
