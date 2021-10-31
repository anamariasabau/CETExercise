using Microsoft.VisualStudio.TestTools.UnitTesting;
using CETAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Text;  
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.IO;
using CETAPI.Controllers.Tests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace CETAPI.Controllers.Tests
{
    [TestClass()]
    public class OrderActionControllerTests
    {
        [TestMethod()]
        public  void Get_WhenCalled_ReturnsOk()
        {

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Scheme = "htttp";
            httpContext.Request.Host = new HostString("localhost:4437");
            httpContext.Request.PathBase = "";

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };


            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns("Data Source=LAPTOP-IEBJT9DV;Initial Catalog=Trading;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            OrderActionController orderActionController = new OrderActionController(mockConfiguration.Object);
            var result = orderActionController.Get() as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod()]
        public void Get_WhenCalled_ReturnsError()
        {

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Scheme = "htttp";
            httpContext.Request.Host = new HostString("localhost:4437");
            httpContext.Request.PathBase = "";

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };


            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns("");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            OrderActionController orderActionController = new OrderActionController(mockConfiguration.Object);
            var result = orderActionController.Get() as ObjectResult;
            Assert.AreEqual(500, result.StatusCode);
        }


    }
}