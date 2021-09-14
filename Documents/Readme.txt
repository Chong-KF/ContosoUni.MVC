PROBLEM STATEMENT:

 

DAY 01-02 (1 Point)

Design an NET CORE 5.0 Web Application for the CONTOSO UNIVERSITY Portal (that you designed during the CAPSTONE 1 Project). Ensure that the following key features are implemented:
The home page should leverage the HTML5 Location API to compute the distance (using the Location API you used during your Capstone 1 (HTML5, CSS3 Project), and again in the Capstone 4 (Xamarin Project)
Using the Weather API, showcase the Local Weather information on the home page.
Implement screens to perform CRUD operations on the following information:
Departments
Subjects per Department
Use EF Core code-first approach to define Data Models to store the data in SQL DB
Ensure that the pages performing the CRUD Operations are secured leveraging upon the Local Account NET Identity middleware.
 

DAY 03 (2 Points)

Define ASP.NET Web API endpoints to expose the Department related data to perform CRUD operations. Ensure that:
The Web API controller is not secured.
Proper Swagger Documentation is generated for each of the API endpoints
Add a link to the Swagger Documentation in the home page of the application.
 

DAY 04-05 (3 Points)

Define an Azure Queue Container in the Azure Storage
Every time a new Department is added/edited or deleted using the API, add a new Message to the Azure Queue container providing the following information:
Username of the user who performed the CUD operation.
Date and Time of the activity.
ID of the Department affected.
Create a Serverless Queue Trigger Function to process the queued message, and send an email to the Contoso University Principal.


CAPSTONE PROJECT 6 (Software Testing)

DAY 01 (1 Point)

Define a Test Case Document capturing all the kinds of tests that can be conducted on the Departments Controller implemented in the ASP.NET CORE Project
 

DAY 02-03 (2 Points)

Create a MS Unit Test Project
Test the Controller Action Methods for the Departments Controller
Ensure that:
the Models/ViewModels are validated for the arguments passed
the correct views are returned
the correct error messages are returned from the Action Methods.
 

DAY 04 (1 Point)

Add Tests to now test each of the Departments API CRUD endpoints
 

DAY 05 (1 Point)

Create a Test Project to test the UI using Cypress for the LOGIN SCREEN of the ASP.NET CORE 5.0 Application
 

NOTE:

If time permits, you could define the Test Project using all the three types of Test Projects:

MS Unit Test Project
XUnit Test Project
NUnit Test Project



//[TestMethod]
        //public async Task CheckIfIndexReturnView()
        //{
        //    // 1. Arrange
        //    // create a mock for logger            
        //    var mockLogger = Mock.Of<ILogger<DepartmentsController>>();
        //    var mockLogger2 = Mock.Of<ILogger<AuthenticationController>>();
        //    var mockUserManager = Mock.Of<UserManagerMocker>();
        //    var mockSignInManager = Mock.Of<SignInManagerMocker>();
        //    var mockAzureQueue = Mock.Of<IAzureQueue>();
        //    using var mockDbContext = DbContextMocker.GetApplicationDBContext("Testdb"); 
        //    // create an instance of the controller
        //    var controller = new DepartmentsController(mockDbContext, mockUserManager, mockLogger, mockAzureQueue);
        //    var jwtcontroller = new AuthenticationController(mockSignInManager, mockLogger2, mockUserManager);

        //    //var adminUser = new MyIdentityUser
        //    //{
        //    //    UserName = "admin@gmail.com",
        //    //    DisplayName = "Admin",
        //    //    Email = "admin@gmail.com",
        //    //    EmailConfirmed = true,
        //    //    //PhoneNumberConfirmed = true,
        //    //    IsActive = true
        //    //};
        //    //if (mockUserManager.Users.All(u => u.Id != adminUser.Id))
        //    //{
        //    //    MyIdentityUser user = await mockUserManager.FindByEmailAsync(adminUser.Email);
        //    //    if (user == null)
        //    //    {
        //    //        await mockUserManager.CreateAsync(adminUser, password);
        //    //        await mockUserManager.AddToRoleAsync(adminUser, MyRoles.Administrator.ToString());
        //    //        await mockUserManager.AddToRoleAsync(adminUser, MyRoles.Staff.ToString());
        //    //    }
        //    //}

        //    var password = "W7TzX6ZJjUQ-X74";
        //    var email = "admin@gmail.com";
        //    MyIdentityUser usertest = await mockUserManager.FindByEmailAsync(email);

        //    var loginUser = new UserRequest
        //    {
        //        Email = email,
        //        Password = password
        //    };

        //    var action1 = await jwtcontroller.Post(loginUser);
        //    //var result = await mockSignInManager.PasswordSignInAsync(adminUser.Email, password, false, lockoutOnFailure: false);
        //    //if (result.Succeeded)
        //    //{ }

        //    // 2. Act
        //    var action2 = controller.Index(false);

        //    // 3. Assert
        //    // Check if the result of the action is a View
        //    Assert.IsInstanceOfType(action2.Result, typeof(ViewResult));
        //    Console.WriteLine("(a) Check the result of Index");

        //    // 2. Act
        //    action2 = controller.Details(Guid.Parse("40204a97-831a-43f5-b096-c8711f620990"));

        //    // 3. Assert
        //    // Check if the result of the action is a View
        //    Assert.IsInstanceOfType(action2.Result, typeof(ViewResult));
        //    Console.WriteLine("(b) Check the result of Details");
        //}