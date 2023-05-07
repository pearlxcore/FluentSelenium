# FluentSelenium
Selenium wrapper using fluent API in C# simplifies the usage of Selenium WebDriver

```
Application.FluentSelenium fluentSelenium = new Application.FluentSelenium(false, false);
            fluentSelenium.Initialize()
                .GoToUrl("https://www.selenium.dev/")
                .WaitBySeconds(2)
                .SelectElement(By.XPath("//*[@id=\"docsearch\"]/button/span[1]/span"))
                .ClickElement()
                .WaitBySeconds(2)
                .SelectElement(By.XPath("//*[@id=\"docsearch-input\"]"))
                .SendKeys("Documentation")
                .WaitBySeconds(3)
                .PressEnter()
                .SnapWebpage()
                .QuitDriver();
```
