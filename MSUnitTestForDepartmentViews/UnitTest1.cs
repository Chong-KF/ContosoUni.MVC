using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ContosoUni.Areas.Identity.Models;
using ContosoUni.Controllers;
using ContosoUni.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Net;
using System.Linq;
using ContosoUni.Data;
using Microsoft.AspNetCore.Http;
using ContosoUni.Areas.JWT.Controllers;
using ContosoUni.Areas.JWT.Models;
using ContosoUni.ViewModels;
using ContosoUni.Models;
using System.Collections.Generic;

namespace MSUnitTestForDepartmentViews
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void Check_Index_GET()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsTestController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsTestController(mockDbContext, mockLogger);

            // 2. Act
            var action = mockController.Index(false);
            var viewResult = action.Result as ViewResult;

            // 3. Assert
            // Check if the result of the action is a View
            Console.WriteLine($"(a) Check return view type");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(ViewResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(ViewResult));
            Console.WriteLine($"    PASS");

            // (b) Check if the Returned View is name as "Index"
            Console.WriteLine($"(b) Check return view name");
            Console.WriteLine($"    Actual  : {nameof(mockController.Index)}");
            Console.WriteLine($"    Expected: {viewResult.ViewName}");
            if (!string.IsNullOrEmpty(viewResult.ViewName))
            {
                //--- if defined explicitly, check if the name is index
                //Assert.AreEqual("Index", result.ViewName);
                Assert.AreEqual(nameof(mockController.Index), viewResult.ViewName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine("    The name of the view returned is not defined explicitly");
            }
        }

        [TestMethod]
        public void Check_Details_GET()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsTestController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsTestController(mockDbContext, mockLogger);

            // 2. Act
            var action = mockController.Details(new Guid("40204a97-831a-43f5-b096-c8711f620990"));
            var viewResult = action.Result as ViewResult;

            // 3. Assert
            // Check if the result of the action is a View with valid GUID
            Console.WriteLine($"(a) Check return view type");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(ViewResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(ViewResult));
            Console.WriteLine($"    PASS");

            // Check if the Returned View is name as "Create"
            // ---check if the name of the view returned is defined explicitly
            Console.WriteLine($"(b) Check return view name");
            Console.WriteLine($"    Actual  : {nameof(mockController.Details)}");
            Console.WriteLine($"    Expected: {viewResult.ViewName}");
            if (!string.IsNullOrEmpty(viewResult.ViewName))
            {
                //--- if defined explicitly, check if the name is index
                Assert.AreEqual(nameof(mockController.Details), viewResult.ViewName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine($"    The name of the view returned is not defined explicitly");
            }

            // 2. Act
            action = mockController.Details(new Guid("40204a97-831a-43f5-b096-000000000000"));

            // 3. Assert
            // Check if the result of the action is a NotFound with Invalid GUID
            Console.WriteLine("(c) Check return view type - NotFound ");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(NotFoundResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
            Console.WriteLine($"    PASS");
        }

        [TestMethod]
        public void Check_Create_GET()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsTestController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsTestController(mockDbContext, mockLogger);

            // 2. Act
            var actionresult = mockController.Create();
            var viewResult = actionresult as ViewResult;

            // 3. Assert
            // Check the return view of Create (GET) 
            Console.WriteLine($"(a) Check return view type");
            Console.WriteLine($"    Actual  : {actionresult}");
            Console.WriteLine($"    Expected: {typeof(ViewResult)}");
            Assert.IsInstanceOfType(actionresult, typeof(ViewResult));
            Console.WriteLine($"    PASS");

            // (b) Check if the Returned View is name as "Create"
            Console.WriteLine($"(b) Check return view name");
            Console.WriteLine($"    Actual  : {nameof(mockController.Create)}");
            Console.WriteLine($"    Expected: {viewResult.ViewName}");
            if (!string.IsNullOrEmpty(viewResult.ViewName))
            {
                //--- if defined explicitly, check if the name is index
                //Assert.AreEqual("Index", result.ViewName);
                Assert.AreEqual(nameof(mockController.Create), viewResult.ViewName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine($"    The name of the view returned is not defined explicitly");
            }

            // (c) Check if the model returned is name an object of the correct viewmodel type
            //Console.WriteLine("Check the name an object of the correct viewmodel type");
            Console.WriteLine($"(c) Check return view model");
            Console.WriteLine($"    Actual  : {viewResult.Model}");
            Console.WriteLine($"    Expected: {typeof(DepartmentViewModel)}");
            Assert.IsInstanceOfType(viewResult.Model, typeof(DepartmentViewModel));
            Console.WriteLine($"    PASS");
        }

        [TestMethod]
        public void Check_Create_POST()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsTestController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsTestController(mockDbContext, mockLogger);

            // 2. Act
            DepartmentViewModel modelToAdd = new()
            {
                DepartmentName = "Arts Department",
                CreatedByUserId = Guid.NewGuid(),
                LastUpdatedOn = DateTime.Now,
                IsDeleted = false
            };
            var action = mockController.Index(false);
            var viewResult = action.Result as ViewResult;
            var count1 = (viewResult.Model as IList<DepartmentViewModel>).Count;

            action = mockController.Create(modelToAdd);
            var result2 = action.Result as RedirectToActionResult;
            // 3. Assert
            // Check if the result of the action is a NotFound with valid GUID
            Console.WriteLine($"(a) Check return view type");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(RedirectToActionResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(RedirectToActionResult));
            Console.WriteLine($"    PASS");

            // 3. Assert
            Console.WriteLine("(b) Check return view name");
            Console.WriteLine($"    Actual  : {nameof(mockController.Index)}");
            Console.WriteLine($"    Expected: {result2.ActionName}");
            if (!string.IsNullOrEmpty(result2.ActionName))
            {
                //--- if defined explicitly, check if the name is index
                //Assert.AreEqual("Index", result.ViewName);
                Assert.AreEqual(nameof(mockController.Index), result2.ActionName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine($"    The name of the view returned is not defined explicitly");
            }

            action = mockController.Index(false);
            viewResult = action.Result as ViewResult;
            var count2 = (viewResult.Model as IList<DepartmentViewModel>).Count;
            // 3. Assert
            //Check if the total record is greater by 1 after create new
            Console.WriteLine($"(c) Check create new record");
            Console.WriteLine($"    Number of records before add {count1}");
            Console.WriteLine($"    Number of records after  add {count2}");
            Assert.AreEqual(count1 + 1, count2);
            Console.WriteLine($"    PASS");

            // 2. Act
            mockController.ModelState.AddModelError(string.Empty, "test error");
            action = mockController.Create(modelToAdd);
            viewResult = action.Result as ViewResult;

            // 3. Assert
            Console.WriteLine($"(d) Check return view type - invalid modelstate");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(ViewResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(ViewResult));
            Console.WriteLine($"    PASS");

            // 3. Assert
            Console.WriteLine($"(e) Check return view name - invalid modelstate ");
            Console.WriteLine($"    Actual  : {nameof(mockController.Create)}");
            Console.WriteLine($"    Expected: {viewResult.ViewName}");
            if (!string.IsNullOrEmpty(viewResult.ViewName))
            {
                //--- if defined explicitly, check if the name is index
                Assert.AreEqual(nameof(mockController.Create), viewResult.ViewName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine($"    The name of the view returned is not defined explicitly");
            }
        }

        [TestMethod]
        public void Check_Edit_GET()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsTestController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsTestController(mockDbContext, mockLogger);

            // 2. Act
            var action = mockController.Edit(new Guid("40204a97-831a-43f5-b096-c8711f620990"));
            var viewResult = action.Result as ViewResult;
            // 3. Assert
            Console.WriteLine($"(a) Check return view type");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(ViewResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(ViewResult));
            Console.WriteLine($"    PASS");

            Console.WriteLine($"(b) Check return view name");
            Console.WriteLine($"    Actual  : {nameof(mockController.Edit)}");
            Console.WriteLine($"    Expected: {viewResult.ViewName}");
            if (!string.IsNullOrEmpty(viewResult.ViewName))
            {
                //--- if defined explicitly, check if the name is index
                Assert.AreEqual(nameof(mockController.Edit), viewResult.ViewName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine($"    The name of the view returned is not defined explicitly");
            }

            Console.WriteLine($"(c) Check return view model");
            Console.WriteLine($"    Actual  : {viewResult.Model}");
            Console.WriteLine($"    Expected: {typeof(DepartmentViewModel)}");
            Assert.IsInstanceOfType(viewResult.Model, typeof(DepartmentViewModel));
            Console.WriteLine($"    PASS");

            // 2. Act
            action = mockController.Edit(new Guid("40204a97-831a-43f5-b096-000000000000"));

            // 3. Assert
            Console.WriteLine($"(d) Check return view type - NotFound");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(NotFoundResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
            Console.WriteLine($"    PASS");
        }

        [TestMethod]
        public void Check_Edit_POST()
        {
            // 1.Arrange
            var mockLogger = Mock.Of<ILogger<DepartmentsTestController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); //Disposable
            var mockController = new DepartmentsTestController(mockDbContext, mockLogger);

            // 2. Act
            DepartmentViewModel viewModel = new()
            {
                DepartmentID = new Guid("40204a97-831a-43f5-b096-c8711f620990"),
                DepartmentName = "Test Department",
                CreatedByUserId = Guid.NewGuid(),
                LastUpdatedOn = DateTime.Now,
                IsDeleted = false
            };
            var action = mockController.Edit(new Guid("40204a97-831a-43f5-b096-c8711f620990"), viewModel);
            var result2 = action.Result as RedirectToActionResult;
            // 3. Assert
            // Check if the result of the action is a NotFound with valid GUID
            Console.WriteLine($"(a) Check return view type");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(RedirectToActionResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(RedirectToActionResult));
            Console.WriteLine($"    PASS");

            // 3. Assert
            Console.WriteLine("(b) Check return view name");
            Console.WriteLine($"    Actual  : {nameof(mockController.Index)}");
            Console.WriteLine($"    Expected: {result2.ActionName}");
            if (!string.IsNullOrEmpty(result2.ActionName))
            {
                //--- if defined explicitly, check if the name is index
                //Assert.AreEqual("Index", result.ViewName);
                Assert.AreEqual(nameof(mockController.Index), result2.ActionName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine($"    The name of the view returned is not defined explicitly");
            }

            //2. Act
            action = mockController.Details(new Guid("40204a97-831a-43f5-b096-c8711f620990"));
            var viewResult = action.Result as ViewResult;
            DepartmentViewModel dept = viewResult.Model as DepartmentViewModel;
            // 3. Assert
            Console.WriteLine($"(c) Check result of Edit (POST)");
            Console.WriteLine($"    ToAdd: {viewModel.DepartmentName}");
            Console.WriteLine($"    Added: {dept.DepartmentName}");
            Assert.AreEqual(viewModel.DepartmentName, dept.DepartmentName);
            Console.WriteLine($"    PASS");

            // 2. Act
            action = mockController.Details(new Guid("40204a97-831a-43f5-b096-000000000000"));

            // 3. Assert
            Console.WriteLine("(d) Check return view type - NotFound");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(NotFoundResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
            Console.WriteLine($"    PASS");

            // 2. Act
            mockController.ModelState.AddModelError(string.Empty, "test error");
            action = mockController.Edit(new Guid("40204a97-831a-43f5-b096-c8711f620990"), viewModel);
            viewResult = action.Result as ViewResult;

            // 3. Assert
            Console.WriteLine($"(e) Check return view type - invalid modelstate");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(ViewResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(ViewResult));
            Console.WriteLine($"    PASS");

            // 3. Assert
            Console.WriteLine($"(f) Check return view name - invalid modelstate");
            Console.WriteLine($"    Actual  : {nameof(mockController.Edit)}");
            Console.WriteLine($"    Expected: {viewResult.ViewName}");
            if (!string.IsNullOrEmpty(viewResult.ViewName))
            {
                //--- if defined explicitly, check if the name is index
                Assert.AreEqual(nameof(mockController.Edit), viewResult.ViewName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine($"    The name of the view returned is not defined explicitly");
            }
        }

        [TestMethod]
        public void Check_Delete_POST()
        {
            var mockLogger = Mock.Of<ILogger<DepartmentsTestController>>();
            using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb");
            // create an instance of the controller
            var mockController = new DepartmentsTestController(mockDbContext, mockLogger);

            // 2. Act
            var action = mockController.DeleteConfirmed(new Guid("40204a97-831a-43f5-b096-c8711f620990"));
            var result2 = action.Result as RedirectToActionResult;
            // 3. Assert
            // Check if the result of the action is a NotFound with valid GUID
            Console.WriteLine($"(a) Check return view type");
            Console.WriteLine($"    Actual  : {action.Result}");
            Console.WriteLine($"    Expected: {typeof(RedirectToActionResult)}");
            Assert.IsInstanceOfType(action.Result, typeof(RedirectToActionResult));
            Console.WriteLine($"    PASS");

            // 3. Assert
            Console.WriteLine("(b) Check return view name");
            Console.WriteLine($"    Actual  : {nameof(mockController.Index)}");
            Console.WriteLine($"    Expected: {result2.ActionName}");
            if (!string.IsNullOrEmpty(result2.ActionName))
            {
                //--- if defined explicitly, check if the name is index
                //Assert.AreEqual("Index", result.ViewName);
                Assert.AreEqual(nameof(mockController.Index), result2.ActionName);
                Console.WriteLine($"    PASS");
            }
            else
            {
                Console.WriteLine($"    The name of the view returned is not defined explicitly");
            }

            // 2. Act
            action = mockController.Details(new Guid("40204a97-831a-43f5-b096-c8711f620990"));
            var viewResult = action.Result as ViewResult;
            DepartmentViewModel dept = viewResult.Model as DepartmentViewModel;
            // 3. Assert
            // Check if the result of the action is a NotFound with Invalid GUID
            Console.WriteLine("(c) Check if the record is deleted");
            Console.WriteLine($"    IsDeleted: {dept.IsDeleted}");
            Console.WriteLine($"    Expected : {true}");
            Assert.AreEqual(dept.IsDeleted, true);
            Console.WriteLine($"    PASS");
        }
    }
}
