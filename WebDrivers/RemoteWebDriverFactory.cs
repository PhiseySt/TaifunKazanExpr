using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using TaifunKazanExpress.Configurations;

namespace TaifunKazanExpress.WebDrivers
{
    public interface IWebDriverFactory
    {
        RemoteWebDriver CreateWebDriver();
    }

    public class ChromeFactory : IWebDriverFactory
    {
        public RemoteWebDriver CreateWebDriver()
        {
            return new ChromeDriver(WebDriverConfiguration.PathDirectoryWebDriver);
        }
    }
}
