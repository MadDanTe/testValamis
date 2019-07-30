using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace testValamis.Model
{
    class MainModel : BindableBase
    {
        private IWebElement webElement;

        public void StartTest(string newUrl)
        {
            RemoteWebDriver webDriver = new ChromeDriver();
            webDriver.Navigate().GoToUrl(newUrl);
            webElement=webDriver.FindElementById("user-addr__input");
            webElement.SendKeys("Петрозаводск, улица Германа Титова, 11");
            webElement = webDriver.FindElementById("user-addr__form");
        }
    }
}
