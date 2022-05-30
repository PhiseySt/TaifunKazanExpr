using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TaifunKazanExpress.Configurations;
using TaifunKazanExpress.Logging;

namespace TaifunKazanExpress.WebDrivers;

public class ChromeWorkerActions
{
    private static IWebDriverFactory _factory;
    private static RemoteWebDriver _browserDriver;
    private static ChromeWorker _chromeWorker;
    private static readonly TimeSpan WaitTime = new(0, 0, 0, 0, 300);

    private const int DelayBetweenActions = 5000;
    private const string RuSellerSignin = "https://business.kazanexpress.ru/seller/signin";

    public ChromeWorkerActions()
    {
        InitBrowserDriver();
    }

    private static void InitBrowserDriver()
    {
        _factory = new ChromeFactory();
        _browserDriver = _factory.CreateWebDriver();
        _browserDriver.Manage().Timeouts().ImplicitWait = WaitTime;
        _chromeWorker = new ChromeWorker();
    }
    public void OpenLoginPage()
    {
        try
        {
            LogWriter.LogWrite("");
            LogWriter.LogWrite("Старт процесса. Запуск бота");
            _chromeWorker.OpeningStartPage(_browserDriver, RuSellerSignin);
            Task.Delay(DelayBetweenActions);
            _chromeWorker.LoginData(_browserDriver);
            Task.Delay(2 * DelayBetweenActions);
            _chromeWorker.RedirectInvoicesPage(_browserDriver);
            Task.Delay(2 * DelayBetweenActions);
            var countButtonChooseTimeslot = ChromeWorker.CountButtonChooseTimeslot(_browserDriver);
            var startValCount = countButtonChooseTimeslot;
            if (countButtonChooseTimeslot > 0)
            {
                LogWriter.LogWrite("Так как существуют возможности для попытки выбора таймслота запускаю периодическую процедуру сканирования");
            }
            else
            {
                LogWriter.LogWrite("Не считаю нужным что то мониторить. Завершаю работу.");
                return;
            }

            while (startValCount == countButtonChooseTimeslot)
            {
                countButtonChooseTimeslot = _chromeWorker.MonitoringTimeSlot(_browserDriver, countButtonChooseTimeslot);
                Thread.Sleep(DelayConfiguration.InvoiceDelayMs);
            }

        }
        catch (System.ComponentModel.Win32Exception)
        {
            LogWriter.LogWrite("Проблемы в процедуре OpenLoginPage");
            Process.Start("chrome.exe", RuSellerSignin);
        }
    }
}