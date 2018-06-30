using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests
{
    [TestFixture]
    public class MoviesTests
    {
        [Test]
        public void MoviesTest_GetRequest_Returns200Response()
        {
            //Arrange
            var controller = new MoviesController();
            //Act
            var getResponse = controller.Get();
            //Assert
            getResponse.Should().BeOfType(typeof(OkObjectResult));
        }
    }
}
