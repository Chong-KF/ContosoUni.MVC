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


// Inspired by:
// https://www.c-sharpcorner.com/article/crud-operations-unit-testing-in-asp-net-core-web-api-with-xunit/

namespace XUnitTestForAPI2
{
    public partial class UnitTest2
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest2(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
    
    }
}
