using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Remote;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using testValamis.Elements;
using testValamis.ViewModel;

namespace testValamis.Model
{
    class MainModel 
    {

        private IReadOnlyCollection<IWebElement> elementsFilter;


        public void StartTest(string newUrl, MainVM mainVM)
        {
            mainVM.Status = DateTime.Now.ToString() + ": Тест запущен. Идёт работа с Web...";

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
                        {
                            checkbox.Click();
                            break;
                        }
                          
            foreach(var we in elementsFilter)
                if (we.FindElement(By.ClassName("vendors-filter__title")).Text == "Заказ")
                    foreach (var checkbox in we.FindElements(By.TagName("label")))
                        if (checkbox.FindElement(By.TagName("input")).GetAttribute("value") == "500")
                        {
                            checkbox.Click();
                            break;
                        }
                            
            foreach (var we in elementsFilter)
                if (we.FindElement(By.ClassName("vendors-filter__title")).Text == "Кухни")
                    foreach (var checkbox in we.FindElements(By.TagName("label")))
                        if (checkbox.FindElement(By.TagName("input")).GetAttribute("value") == "Пицца")
                        {
                            checkbox.Click();
                            break;
                        }

            if(ResultSearch(webDriver, mainVM))
            {
                WorkWithFeedback(webDriver, mainVM);
                WorkWithLogin(webDriver, mainVM);
                mainVM.Status = DateTime.Now.ToString() + ": Тест успешно отработал!";
            }

            webDriver.Quit();
        }

        public bool ResultSearch(RemoteWebDriver currentWebDriver, MainVM mainVM)
        {
            IWebElement webElement = currentWebDriver.FindElementByClassName("vendor-list__container");
            try
            {
                webElement.FindElement(By.TagName("ul"));
            }
            catch (NoSuchElementException ex)
            {
                mainVM.Status = DateTime.Now.ToString() + ": Нет результатов соответствующих критериям фильтра \nРестораны закрыты, переходим во вкладку 'Все'.";
                foreach (var sp in webElement.FindElements(By.ClassName("btn-radio__span")))
                    if (sp.Text == "Все")
                        sp.Click();
            }

            foreach (var searchResult in webElement.FindElements(By.ClassName("vendor-item")))
                if (searchResult.FindElement(By.ClassName("vendor-item__title-link")).Text == "YES PIZZA")
                {
                    IWebElement we = searchResult.FindElement(By.ClassName("vendor-item-info__min-order"));
                    String se = we.Text;
                    se = Regex.Replace(se, "[^0-9]", "");
                    int s;
                    int.TryParse(se, out s);
                    if (s < 500 && Regex.IsMatch(searchResult.FindElement(By.ClassName("vendor-item__cuisines")).Text, @"\s*(Пицца)\W"))
                    {
                        mainVM.Status = DateTime.Now.ToString() + ": Ресторан 'YES PIZZA' cоответствует критериям запроса.";
                        searchResult.Click();
                        try
                        {
                            currentWebDriver.FindElementByClassName("popup--def__btn").Click();
                        }
                        catch (Exception ex)
                        { }
                        break;
                    }
                    else
                    {
                        mainVM.Status = DateTime.Now.ToString() + ": Ресторан 'YES PIZZA' не cоответствует критериям запроса.";
                        return false;
                    }


                }
                else
                {
                    mainVM.Status = DateTime.Now.ToString() + ": Ресторан 'YES PIZZA' не найден.";
                    return false;
                }


            return true;


        }

        private void WorkWithFeedback(RemoteWebDriver currentWebDriver, MainVM mainVM)
        {
            OutputDataViewModel outputDataViewModel = new OutputDataViewModel();

            foreach(var tab in currentWebDriver.FindElementsByClassName("vendor-headline__tab"))
            {
                if (Regex.IsMatch(tab.Text, @"Отзывы"))
                {
                    tab.Click();
                    break;
                }
            }

            try
            {
                var firstFeedback = currentWebDriver.FindElementByClassName("feedback-list__item");
                outputDataViewModel.Author = firstFeedback.FindElement(By.ClassName("feedback-list__item-title")).Text;
                outputDataViewModel.Feedback = firstFeedback.FindElement(By.ClassName("feedback-list__item-text")).Text;
                outputDataViewModel.Date = firstFeedback.FindElement(By.ClassName("feedback-list__item-time")).Text;
                foreach (var rating in firstFeedback.FindElements(By.ClassName("feedback-list__item-star")))
                    if (!Regex.IsMatch(rating.GetAttribute("class"), "active"))
                        outputDataViewModel.Rate += 1;
                    else
                    {
                        outputDataViewModel.Rate += 1;
                        break;
                    }
            }
            catch (NoSuchElementException ex)
            {
                mainVM.Status = DateTime.Now.ToString() + ": Отзывов не найденно...";
                return;
            }

            mainVM.Author = outputDataViewModel.Author;
            mainVM.Rating = outputDataViewModel.Rate;
            mainVM.Feedback = outputDataViewModel.Feedback;
            mainVM.Date = outputDataViewModel.Date;
            mainVM.Status = DateTime.Now.ToString() + ": Полученны данные первого отзыва.";

            return;

        }

        private void WorkWithLogin(RemoteWebDriver currentWebDriver, MainVM mainVM)
        {
            currentWebDriver.FindElementByClassName("feedback-action__container-btn--login").Click();
            if (currentWebDriver.FindElementByClassName("authorization-popup").GetCssValue("display") == "none")
            {
                mainVM.Status = DateTime.Now.ToString() + ": Кнопка входа не привелла к вызову окна авторизации.";
                return;
            }


            try
            {
                while(currentWebDriver.FindElementByClassName("user-login__btn--submit").GetCssValue("pointer-events")=="none")
                {
                    currentWebDriver.FindElementByClassName("user-login__input").SendKeys("9");
                }
            }
            catch(Exception ex)
            {
                mainVM.Status = DateTime.Now.ToString() + ": Припопытке ввода номера телефона произошла ошибка.";
                return;
            }

            currentWebDriver.FindElementByClassName("user-login__btn--submit").Click();

            try
            {
                currentWebDriver.FindElementByClassName("phone-verify");
            }
            catch(NoSuchElementException ex)
            {
                mainVM.Status = DateTime.Now.ToString() + ": После ввода номера телефона нет поля подтверждения.";
                return;
            }
            mainVM.Status = DateTime.Now.ToString() + ": Кнопка входа введёт на авторизацию через телефон.";

            currentWebDriver.ExecuteScript("$(document.elementFromPoint("+
                (currentWebDriver.FindElementByClassName("popup--def__container-wrap").Location.X-10) +", "+
                currentWebDriver.FindElementByClassName("popup--def__container-wrap").Location.Y+
                ")).click();");

        }

    }
}
