using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace testValamis.Model
{
    class MainModel : BindableBase
    {

        private IReadOnlyCollection<IWebElement> elementsFilter;

        public void StartTest(string newUrl)
        {
            RemoteWebDriver webDriver = new ChromeDriver();
            webDriver.Navigate().GoToUrl(newUrl);
            IWebElement webElement = webDriver.FindElementById("user-addr__input");
            webElement.SendKeys("Петрозаводск, улица Германа Титова, 11");
            webElement = webDriver.FindElementById("user-addr__form");
            webElement.FindElement(By.ClassName("user-addr__submit")).Click();
            webDriver.Manage().Timeouts().ImplicitWait=TimeSpan.FromSeconds(10);
            webElement = webDriver.FindElementByClassName("vendors-filter__form");
            elementsFilter = webDriver.FindElementByClassName("vendors-filter__form").FindElements(By.TagName("section"));
            foreach(var we in elementsFilter)
                if(we.FindElement(By.ClassName("vendors-filter__title")).Text=="Кухни")
                    foreach (var checkbox in we.FindElements(By.TagName("label")))
                        if (checkbox.FindElement(By.TagName("input")).GetAttribute("value") == "Здоровая еда")
                            checkbox.Click();
            foreach(var we in elementsFilter)
                if (we.FindElement(By.ClassName("vendors-filter__title")).Text == "Заказ")
                    foreach (var checkbox in we.FindElements(By.TagName("label")))
                        if (checkbox.FindElement(By.TagName("input")).GetAttribute("value") == "500")
                            checkbox.Click();
            foreach (var we in elementsFilter)
                if (we.FindElement(By.ClassName("vendors-filter__title")).Text == "Кухни")
                    foreach (var checkbox in we.FindElements(By.TagName("label")))
                        if (checkbox.FindElement(By.TagName("input")).GetAttribute("value") == "Пицца")
                            checkbox.Click();
            ResultSearch(webDriver);
        }

        public void ResultSearch(RemoteWebDriver currentWebDriver)
        {
            IWebElement webElement = currentWebDriver.FindElementByClassName("vendor-list__container");
            try
            {
                webElement.FindElement(By.TagName("ul"));
            }
            catch (NoSuchElementException ex)
            {
                System.Console.WriteLine("Error: " + ex.Message);
                foreach (var sp in webElement.FindElements(By.ClassName("btn-radio__span")))
                    if (sp.Text == "Все")
                        sp.Click();
            }

            foreach (var searchResult in webElement.FindElements(By.ClassName("vendor-item")))
                if (searchResult.FindElement(By.ClassName("vendor-item__title-link")).Text == "YES PIZZA")
                {
                    IWebElement we = searchResult.FindElement(By.ClassName("vendor-item-info__min-order"));
                    String se = we.Text;
                    Regex regex = new Regex("\d*");
                    se=regex.IsMatch(se);
                }


        }
    }
}
