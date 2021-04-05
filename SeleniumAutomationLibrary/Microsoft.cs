using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumAutomationLibrary {
    class Microsoft {
        private WebDriverWait _wait;
        private IWebDriver _driver;

        public Microsoft(IWebDriver driver) {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

        public void Login(string login, string password) {
            _driver.Navigate().GoToUrl("https://login.microsoftonline.com");
            _driver.Manage().Window.Maximize();

            _wait.Until(webDriver => webDriver.FindElement(By.Name("loginfmt")).Displayed);

            //Microsoft login form
            IWebElement msFormLogin = _driver.FindElement(By.Name("loginfmt"));
            msFormLogin.SendKeys(login);
            _driver.FindElement(By.Id("idSIButton9")).Click();

            _wait.Until(webDriver => webDriver.FindElement(By.Name("passwd")).Displayed);
            _driver.FindElement(By.Name("passwd")).SendKeys(password);
            _driver.FindElement(By.Id("idSIButton9")).Click();

            _wait.Until(webDriver => webDriver.FindElement(By.Id("idSubmit_SAOTCC_Continue")).Displayed);
            _wait.Until(webDriver => webDriver.FindElement(By.Id("idBtn_Back")).Displayed);
        }
    }
}
