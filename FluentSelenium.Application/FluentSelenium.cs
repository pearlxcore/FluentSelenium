using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace FluentSelenium.Application
{
    public class FluentSelenium
    {
        private ChromeDriverService _chromeDriverService;
        private ChromeOptions _chromeOptions;
        private int driverProc { get; set; }
        private List<Action> _methodList = new List<Action>();
        private readonly bool _disableImageExtension;
        private readonly bool _headless;

        public int timeOut { get; set; }
        public IWebDriver _driver { get; set; }
        private IWebElement element { get; set; }

        public FluentSelenium(bool disableImageExtension, bool headless)
        {
            _disableImageExtension=disableImageExtension;
            _headless=headless;
        }

        public FluentSelenium Initialize()
        {
            try
            {
                _driver = InitializeDriver();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return this;
        }

        #region FluentApi

        public FluentSelenium WaitForUrlToMatch(string url)
        {
            var webDriverWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeOut));
            webDriverWait.Until(driver => driver.Url == url);
            return this;
        }

        public FluentSelenium WaitUrlContainString(string text)
        {
            var webDriverWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeOut));
            webDriverWait.Until(driver => driver.Url.Contains(text));
            return this;
        }

        public FluentSelenium GoToUrl(string url)
        {
            _driver.Navigate().GoToUrl(url);
            return this;
        }

        public FluentSelenium RefreshPage()
        {
            _driver.Navigate().Refresh();
            return this;
        }

        public FluentSelenium ReturnPage(string url)
        {
            _driver.Navigate().Back();
            return this;
        }

        public FluentSelenium WaitElementExists(By locator)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(ExpectedConditions.ElementExists(locator));
            return this;
        }

        public FluentSelenium WaitBySeconds(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            return this;
        }

        public FluentSelenium WaitElementClickable(By locator)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            return this;
        }

        public FluentSelenium WaitElementVisible(By locator)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(ExpectedConditions.ElementIsVisible(locator));
            return this;
        }

        public FluentSelenium QuitDriver()
        {
            _driver.Quit();
            _driver = null;
            return this;
        }

        public FluentSelenium SelectElement(By locator)
        {
            element = _driver.FindElement(locator);
            return this;
        }

        public FluentSelenium ClickElement()
        {
            element.Click();
            // javascript
            //IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
            //executor.ExecuteScript("arguments[0].click();", element);
            return this;
        }

        public FluentSelenium SendKeys(string text)
        {
            element.SendKeys(text);
            return this;
        }

        public FluentSelenium PressEnter()
        {
            element.SendKeys(Keys.Enter);
            return this;
        }

        public FluentSelenium ClearKeys()
        {
            element.Clear();
            return this;
        }

        public FluentSelenium SnapWebpage()
        {
            ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile("Test.png", ScreenshotImageFormat.Png);
            return this;
        }

        public void Run()
        {
            try
            {
                foreach (var method in _methodList)
                {
                    method.Invoke();
                }
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException($"Timeout.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion FluentApi

        #region ElementConditionCheck

        public bool UrlContainString(string text)
        {
            try
            {
                if (_driver.Url.Contains(text))
                    return true;
                else
                    return false;
            }
            catch (WebDriverTimeoutException)
            {
                throw new WebDriverTimeoutException("Timeout.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool ElementClickable(By locator)
        {
            try
            {
                WebDriverWait webDriverWait = new WebDriverWait(_driver, new TimeSpan(0, 0, timeOut));
                webDriverWait.Until(ExpectedConditions.ElementToBeClickable(locator));
                return true;
            }
            catch (WebDriverTimeoutException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool ElementExists(By locator)
        {
            try
            {
                _driver.FindElement(locator);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion ElementConditionCheck

        public IWebElement GetElement(By locator)
        {
            element = _driver.FindElement(locator);
            return element;
        }

        public void ClickElement(IWebElement webElement)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)_driver;
            executor.ExecuteScript("arguments[0].click();", webElement);
        }

        private IWebDriver InitializeDriver()
        {
            IWebDriver _webDriver;
            _chromeDriverService = ChromeDriverService.CreateDefaultService();
            _chromeOptions = new ChromeOptions();
            _chromeDriverService.HideCommandPromptWindow = true;
            _chromeOptions = GetOptions(_chromeOptions);
            driverProc = _chromeDriverService.ProcessId;
            _webDriver = new ChromeDriver(_chromeDriverService, _chromeOptions, TimeSpan.FromMinutes(3.0));
            _webDriver.Manage().Window.Maximize();
            return _webDriver;
        }

        private ChromeOptions GetOptions(ChromeOptions chromeOptions)
        {
            if (_disableImageExtension)
            {
                chromeOptions.AddArgument("--disable-extensions");
                chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                chromeOptions.AddArgument("--blink-settings=imagesEnabled=false");
            }
            else
                chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 1);
            if (_headless)
                chromeOptions.AddArgument("headless");
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument($"--profile-directory=Default");
            chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager;
            chromeOptions.AddExcludedArgument("enable-automation");
            chromeOptions.AddArgument("no-sandbox");
            return chromeOptions;
        }
    }
}