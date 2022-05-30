using System;
using TaifunKazanExpress.Configurations;

namespace TaifunKazanExpress;

public partial class App
{
    private void InitCredentials()
    {
        UserConfiguration.User = Configuration.GetSection("Credentials:User").Value;
        UserConfiguration.Password = Configuration.GetSection("Credentials:Password").Value;
    }

    private void InitWebDriver()
    {
        WebDriverConfiguration.PathDirectoryWebDriver = Configuration.GetSection("WebDriver:Directory").Value;
    }

    private void InitDelays()
    {
        DelayConfiguration.InvoiceDelayMs = Convert.ToInt32(Configuration.GetSection("Delays:InvoiceDelayMs").Value);
        DelayConfiguration.ChooseTimeSlotDelayMs = Convert.ToInt32(Configuration.GetSection("Delays:ChooseTimeSlotDelayMs").Value);
        DelayConfiguration.HugDelayMs = Convert.ToInt32(Configuration.GetSection("Delays:HugDelayMs").Value);
        DelayConfiguration.PageInvoiceNotAvailableDelayMs = Convert.ToInt32(Configuration.GetSection("Delays:PageInvoiceNotAvailableDelayMs").Value);
    }
}