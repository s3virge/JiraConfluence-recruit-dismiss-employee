using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeleniumAutomationLibrary {
    class Microsoft {
        private WebDriverWait _wait;
        private IWebDriver _driver;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Microsoft(IWebDriver driver) {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

        public void Login(string login = null, string password = null) {
                                   
            _driver.Navigate().GoToUrl("https://login.microsoftonline.com");
            _driver.Manage().Window.Maximize();

            Thread.Sleep(1000);

            _wait.Until(webDriver => webDriver.FindElement(By.Name("loginfmt")).Displayed);

            //Microsoft login form
            login = "v.kobzar@intetics.com";            
            IWebElement msFormLogin = _driver.FindElement(By.Name("loginfmt"));
            msFormLogin.SendKeys(login);
            _driver.FindElement(By.Id("idSIButton9")).Click();

            Thread.Sleep(1000);

            _wait.Until(webDriver => webDriver.FindElement(By.Name("passwd")).Displayed);
            password = "2XeytrEV_78";
            _driver.FindElement(By.Name("passwd")).SendKeys(password);
            _driver.FindElement(By.Id("idSIButton9")).Click();

            //enter code
            //_wait.Until(webDriver => webDriver.FindElement(By.Id("idSubmit_SAOTCC_Continue")).Displayed);

            log.Debug($"Try to login with microsoft account. Wait when display 'idDiv_SAOTCAS_Title' element");
            //глюч происходит на этапе Debug
            _wait.Until(webDriver => webDriver.FindElement(By.Id("idDiv_SAOTCAS_Title")).Displayed);

            log.Debug($"Try to login with microsoft account. Wait when display 'idBtn_Back' element");
            //Thread.Sleep(3000);
            _wait.Until(webDriver => webDriver.FindElement(By.Id("idBtn_Back")).Displayed);          
        }
    }
}
