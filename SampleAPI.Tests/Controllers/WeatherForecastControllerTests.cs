using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SampleAPI.Controllers;
using System.Collections.Generic;

namespace SampleAPI.Tests.Controllers
{
    [TestClass]
    public class WeatherForecastControllerTests
    {
        [TestMethod]
        public void Get_Returns_ListOfWeatherForecast()
        {
            var controller = new WeatherForecastController(new Mock<ILogger<WeatherForecastController>>().Object);
            var result = controller.GetWeatherForecastList();
            Assert.IsInstanceOfType(result, typeof(IEnumerable<WeatherForecast>));
        }

        [TestMethod]
        public void Get_Returns_WeatherForecastCount()
        {
            var controller = new WeatherForecastController(new Mock<ILogger<WeatherForecastController>>().Object);
            var result = controller.GetWeatherForecastCount();
            Assert.IsInstanceOfType(result, typeof(int));
        }
    }
}
