using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;
using TaifunKazanExpress.Configurations;
using TaifunKazanExpress.Logging;

namespace TaifunKazanExpress.WebDrivers
{
    internal class ChromeWorker
    {
        private const int ThreadTimeout = 3000;
        private const int MaxErrorValue = 10;
        private static int _currentErrorValue = 0;
        public void OpeningStartPage(IWebDriver chrome, string baseUrl)
        {

            try
            {
                chrome.Manage().Window.Maximize();
                chrome.Navigate().GoToUrl(baseUrl);
                LogWriter.LogWrite($"Корректное открытие страницы {baseUrl}");
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite($"Некорректное открытие страницы {baseUrl} {ex.Message}");
            }
        }

        public void LoginData(IWebDriver chrome)
        {

            try
            {
                LogWriter.LogWrite($"Старт процедуры логина LoginData");

                chrome.FindElement(By.Id("username")).SendKeys(UserConfiguration.User);
                Thread.Sleep(ThreadTimeout);
                chrome.FindElement(By.Id("password")).SendKeys(UserConfiguration.Password);
                Thread.Sleep(ThreadTimeout);
                var buttonLogin = chrome.FindElement(SelectorByAttributeValue("data-test-id", "button__next"));
                Thread.Sleep(2 * ThreadTimeout);
                buttonLogin.Click();
                Thread.Sleep(ThreadTimeout);
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite($"Не удалось выполнить процедуру логина LoginData {ex.Message}");
            }
        }

        public void RedirectInvoicesPage(IWebDriver chrome)
        {

            try
            {
                var invoicesLink = chrome.FindElement(By.CssSelector("[href*='/seller/2135/invoices/']"));
                Thread.Sleep(2 * ThreadTimeout);
                invoicesLink.Click();
                LogWriter.LogWrite("Корректное открытие страницы Накладные");
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite($"Не удалось выполнить переход на страницу Накладные {ex.Message}");
            }
        }

        public static int CountButtonChooseTimeslot(IWebDriver chrome)
        {
            try
            {
                var buttonsChooseTimeslot = chrome.FindElements(SelectorByAttributeValue("data-test-id", "button__choose-timeslot"));
                var countButtonsChooseTimeslot = buttonsChooseTimeslot.Count;
                LogWriter.LogWrite(
                    countButtonsChooseTimeslot > 0
                        ? $"Осуществлен подсчет количества ссылок выбора таймслота. Всего обнаружено: {countButtonsChooseTimeslot}"
                        : "Осуществлен подсчет количества ссылок выбора таймслота. Свободных таймслотов не обнаружено.");

                return countButtonsChooseTimeslot;
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite($"Не удалось выполнить переход на страницу Накладные {ex.Message}");
                return 0;
            }
        }

        public int MonitoringTimeSlot(IWebDriver chrome, int countButtonChooseTimeslot)
        {
            try
            {
                ClickChooseTimeslot(chrome);

                var spanNoFreeTimeslots = chrome.FindElement(By.XPath("//span[text() = 'Нет свободных таймслотов']"));

                if (spanNoFreeTimeslots != null)
                {
                    LogWriter.LogWrite("Обнаружил 'Нет свободных таймслотов'.");
                    ButtonChooseLaterClick(chrome);
                }

                return CountButtonChooseTimeslot(chrome);
            }
            catch (NoSuchElementException)
            {
                LogWriter.LogWrite("Обнаружил отсутствие надписи 'Нет свободных таймслотов'.");
                var isFindAndClickFreeTimeslot = IsFindAndClickFreeTimeslot(chrome);
                return CountButtonChooseTimeslot(chrome);
            }
            catch (Exception ex)
            {
                // кнопка перекрыта всплывающим сообщением
                if (ex.Message.Contains("Other element would receive the click"))
                {
                    LogWriter.LogWrite(
                        $"Не нажал на кнопку так как она чем то перекрыта. Пауза {DelayConfiguration.HugDelayMs / 1000} секунд.");
                    Thread.Sleep(DelayConfiguration.HugDelayMs);
                    ButtonChooseLaterClick(chrome);

                    return CountButtonChooseTimeslot(chrome);
                }

                _currentErrorValue++;
                if (_currentErrorValue > MaxErrorValue)
                {
                    LogWriter.LogWrite("Счетчик ошибок привысил максимально допустимое число. Останавливаю работу.");
                    return 0;
                }

                LogWriter.LogWrite($"Не удалось выполнить переход на страницу Накладные {ex.Message}. " +
                                   $"Попробую после паузы {DelayConfiguration.PageInvoiceNotAvailableDelayMs / 1000} секунд. " +
                                   "Значение счетчика ошибок увеличино на 1.");

                Thread.Sleep(DelayConfiguration.PageInvoiceNotAvailableDelayMs);
                ButtonChooseLaterClick(chrome);

                return CountButtonChooseTimeslot(chrome);
            }
            finally
            {
                chrome.Manage().Timeouts().ImplicitWait.Add(TimeSpan.FromSeconds(5));
            }
        }

        private static void ClickChooseTimeslot(IWebDriver chrome)
        {
            var buttonChooseTimeslot = chrome.FindElements(SelectorByAttributeValue("data-test-id", "button__choose-timeslot"))
                .FirstOrDefault();
            buttonChooseTimeslot?.Click();
            LogWriter.LogWrite("Нажал на кнопку выбрать таймслот");
        }

        private static bool IsFindAndClickFreeTimeslot(IWebDriver chrome)
        {
            try
            {
                LogWriter.LogWrite("Пытаюсь обнаружить '00:00 - 23:59' на кнопке.");
                var freeTimeslots = chrome.FindElements(By.XPath("//span[text() = '00:00 - 23:59']"));
                var freeTimeslot = freeTimeslots.LastOrDefault();
                if (freeTimeslot != null)
                {
                    LogWriter.LogWrite("Обнаружил span freeTimeslot.");
                    freeTimeslot.Click();
                    LogWriter.LogWrite("Нажал span freeTimeslot.");
                    var buttonSaveTimeslot = chrome
                        .FindElements(SelectorByAttributeValue("data-test-id", "button__save")).FirstOrDefault();
                    LogWriter.LogWrite("Обнаружил кнопку buttonSaveTimeslot.");
                    if (buttonSaveTimeslot != null)
                    {
                        buttonSaveTimeslot.Click();
                        LogWriter.LogWrite("Нажал buttonSaveTimeslot.");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite(ex.Message);
                return false;
            }
        }

        private static void ButtonChooseLaterClick(IWebDriver chrome)
        {
            var buttonChooseLater = chrome.FindElement(By.XPath("//button[.='Выбрать позже']"));

            if (buttonChooseLater != null)
            {
                Thread.Sleep(DelayConfiguration.ChooseTimeSlotDelayMs);
                buttonChooseLater.Click();
                LogWriter.LogWrite("Нажал на кнопку 'Выбрать позже'.");
            }
        }

        public static By SelectorByAttributeValue(string pStrAttributeName, string pStrAttributeValue)
        {
            return (By.XPath($"//*[@{pStrAttributeName} = '{pStrAttributeValue}']"));
        }

    }
}
