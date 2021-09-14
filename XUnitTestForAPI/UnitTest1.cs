using ContosoUni.Controllers;
using ContosoUni.Data;
using ContosoUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTestForAPI
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;
       
        public UnitTest1(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task TestDepartmentGetAll()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsApiController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsApiController(mockLogger, mockDbContext); ;
            
            int totalrecords = 3;
            // 2. Act
            var actionResuit = await mockController.GetDepartments();
            var departments = actionResuit.Value;

            // 3. Assert - check if API action method returned any data
            _testOutputHelper.WriteLine($"(a) Check departments not null");
            Assert.NotNull(departments);
            
            // 3. Assert - check if number of departments is as per the seeded data
            int numberofDepartments = (departments as IList<Department>).Count;
            _testOutputHelper.WriteLine($"(b) Check number of records");
            Assert.Equal(totalrecords, numberofDepartments);
            _testOutputHelper.WriteLine($"    expect = {totalrecords}, actual = {numberofDepartments}");
            foreach (Department dept in departments)
            {
                _testOutputHelper.WriteLine($"    {dept.DepartmentID}, {dept.DepartmentName}");
            }
            _testOutputHelper.WriteLine("TestDepartmentGetAll Pass");
        }

        [Fact]
        public async Task TestDepartmentGetSingle()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsApiController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsApiController(mockLogger, mockDbContext); ;

            // 2. Act
            var id = Guid.Parse("40d4d4f1-130b-4e6a-bfc6-dfd609fd3e26");
            var actionResult = await mockController.GetDepartment(id);
            Department department = actionResult.Value;
            var check = "Science Department";

            // 3. Assert - check if API action method returned any data
            _testOutputHelper.WriteLine($"(a) Check departments not null");
            Assert.NotNull(department);

            // 3. Assert - check if data is the same as what was seeded in the inmerory database
            _testOutputHelper.WriteLine($"(b) Check DepartmentID");
            _testOutputHelper.WriteLine($"    Requested id: {id}");
            _testOutputHelper.WriteLine($"    DepartmentID: {department.DepartmentID}");
            Assert.Equal(id, department.DepartmentID);
            _testOutputHelper.WriteLine($"(c) Check DepartmentName");
            _testOutputHelper.WriteLine($"    Check name: {check}");
            _testOutputHelper.WriteLine($"    DepartmentName: {department.DepartmentName}");
            Assert.Equal(check, department.DepartmentName);
           
            
            _testOutputHelper.WriteLine("TestDepartmentGetSingle Pass");
        }

        [Fact]
        public async Task TestDepartmentGetSingleNotFound()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsApiController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsApiController(mockLogger, mockDbContext); ;

            // 2. Act
            var id = Guid.Parse("40d4d4f1-130b-4e6a-bfc6-000000000000");
            var action = await mockController.GetDepartment(id);

            // 3. Assert - check if API action method returned not found
            Assert.IsType<NotFoundResult>(action.Result);

            // 3. Assert - check if data is the same as what was seeded in the inmerory database
            _testOutputHelper.WriteLine($"Requested id: {id}");
            _testOutputHelper.WriteLine("TestDepartmentGetSingleNotFound Pass");
        }

        [Fact]
        public async Task TestDepartmentPut()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsApiController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockcontroller = new DepartmentsApiController(mockLogger, mockDbContext); ;


            // 2. Act
            var id = Guid.Parse("ace4c376-7cb9-429a-9c59-4e765bbd5f0e");
            var action = await mockcontroller.GetDepartment(id);
            Department department = action.Value;

            _testOutputHelper.WriteLine($"Check if OkResult is obtained");
            _testOutputHelper.WriteLine($"Requested id: {id}");
            _testOutputHelper.WriteLine($"DepartmentID: {department.DepartmentID}");
            _testOutputHelper.WriteLine($"DepartmentName before PUT: {department.DepartmentName}");

            string temp = "Test Department";
            department.DepartmentName = temp;
            var actionResult = await mockcontroller.PutDepartment(id, department);

            // 3. Assert - check if OkResult is obtained.            
            Assert.IsType<OkResult>(actionResult);

            // 3. Assert - check if the object edit matches with the object intended to be added
            action = await mockcontroller.GetDepartment(id);
            department = action.Value;
            _testOutputHelper.WriteLine($"Modify name: {temp}");
            _testOutputHelper.WriteLine($"DepartmentName after PUT: {department.DepartmentName}");
            Assert.Equal(temp, department.DepartmentName);
            _testOutputHelper.WriteLine("TestDepartmentPut Pass");
        }

        [Fact]
        public async Task TestDepartmentPutBadRequest()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsApiController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockcontroller = new DepartmentsApiController(mockLogger, mockDbContext); ;

            // 2. Act
            var id = Guid.Parse("ace4c376-7cb9-429a-9c59-4e765bbd5f0e");
            var requestId = Guid.Parse("40d4d4f1-130b-4e6a-bfc6-000000000000");
            var action = await mockcontroller.GetDepartment(id);
            Department department = action.Value;
            department.DepartmentName = "Test Department";
            var actionResult = await mockcontroller.PutDepartment(requestId, department);

            // 3. Assert
            Assert.IsType<BadRequestResult>(actionResult);
            _testOutputHelper.WriteLine($"Requested id: {requestId}");
            _testOutputHelper.WriteLine($"DepartmentID: {department.DepartmentID}");
            _testOutputHelper.WriteLine("TestDepartmentPutBadRequest Pass");
        }

        [Fact]
        public async Task TestDepartmentPost()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsApiController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockcontroller = new DepartmentsApiController(mockLogger, mockDbContext); ;

            Guid id = Guid.NewGuid();
            Guid user = Guid.NewGuid();
            Department deptToAdd = new()
            {
                DepartmentID = id,
                DepartmentName = "Arts Department",
                CreatedByUserId = user,
                LastUpdatedOn = DateTime.Now,
                IsDeleted = false
            };

            // 2. Act
            var actionResuit = await mockcontroller.GetDepartments();
            var departments = actionResuit.Value;
            var count1 = (departments as IList<Department>).Count;

            var action = await mockcontroller.PostDepartment(deptToAdd);
            var actionResult = action.Result as CreatedAtActionResult;

            // 3. Assert - check if OkObjectResult is obtained.
            _testOutputHelper.WriteLine($"(a) Check if CreatedAtActionResult is obtained");
            //Assert.NotNull(actionResult);
            Assert.IsType<CreatedAtActionResult>(actionResult);

            // 3. Assert - check if the Department was added successfully to the inmemory datacontext
            Department deptAdded = actionResult.Value as Department;
            _testOutputHelper.WriteLine($"(b) Check if the Department was added successfully");
            Assert.NotNull(deptAdded);

            // 3. Assert - check if the object added matches with the object intended to be added
            _testOutputHelper.WriteLine($"(c) Check if the object added matches with the object intended to be added");
            Assert.Equal(deptToAdd.DepartmentName, deptAdded.DepartmentName);
            // this would fail in actual DB, cos id is auto increment
            Assert.Equal(deptToAdd.DepartmentID, deptAdded.DepartmentID);

            actionResuit = await mockcontroller.GetDepartments();
            departments = actionResuit.Value;
            var count2 = (departments as IList<Department>).Count;
            _testOutputHelper.WriteLine($"(d) Check total records");
            _testOutputHelper.WriteLine($"    Departments before POST: {count1}");
            _testOutputHelper.WriteLine($"    Departments after  POST: {count2}");
            Assert.Equal(count1+1, count2);
            _testOutputHelper.WriteLine("TestDepartmentPost Pass");
        }

        [Fact]
        public async Task TestDepartmentDelete()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsApiController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockcontroller = new DepartmentsApiController(mockLogger, mockDbContext); ;

            // 2. Act
            var tempResuit = await mockcontroller.GetDepartments();
            var departments = tempResuit.Value;
            var count1 = (departments as IList<Department>).Count;
            var id = Guid.Parse("ace4c376-7cb9-429a-9c59-4e765bbd5f0e");
            // 3. Assert - check if NoContent is obtained.
            var actionResult = await mockcontroller.DeleteDepartment(id);
            _testOutputHelper.WriteLine($"(a) Check DELETE response {actionResult}");
            Assert.IsType<OkResult>(actionResult);

            // 3. Assert - check if record still in database
            var action = await mockcontroller.GetDepartment(id);
            _testOutputHelper.WriteLine($"(b) Check GET response {action.Result}");
            Assert.IsType<NotFoundResult>(action.Result);

            tempResuit = await mockcontroller.GetDepartments();
            departments = tempResuit.Value;
            var count2 = (departments as IList<Department>).Count;
            _testOutputHelper.WriteLine($"(c) Check record count");
            _testOutputHelper.WriteLine($"    Departments before DELETE: {count1}");
            _testOutputHelper.WriteLine($"    Departments after  DELETE: {count2}");
            Assert.Equal(count1-1, count2);
            _testOutputHelper.WriteLine("TestDepartmentDelete Pass");
        }

        [Fact]
        public async Task TestDepartmentDeleteNotFound()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsApiController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockcontroller = new DepartmentsApiController(mockLogger, mockDbContext); ;

            // 2. Act
            var id = Guid.Parse("ace4c376-7cb9-429a-9c59-000000000000");

            // 3. Assert - check if NoFound is obtained.
            var actionResult = await mockcontroller.DeleteDepartment(id);
            _testOutputHelper.WriteLine($"Check DELETE response {actionResult}");
            Assert.IsType<NotFoundResult>(actionResult);

            _testOutputHelper.WriteLine("TestDepartmentDeleteNotFound Pass");
        }
    }
}
