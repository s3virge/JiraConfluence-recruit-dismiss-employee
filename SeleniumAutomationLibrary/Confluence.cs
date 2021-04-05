using ActiveDirectoryLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumAutomationLibrary {
    class Confluence {
        private WebDriverWait _wait;
        private IWebDriver _driver;        

        public Confluence(IWebDriver driver) {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

        public void Dismiss(Employee employee) {
            LoginInToConfluence();
            _driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/admin/users/editusergroups-start.action?username={employee.Login}");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("admin-content")).Displayed);

            _driver.FindElement(By.Id("confluence-employees")).Click();
            _driver.FindElement(By.Id("confluence-users")).Click();
            _driver.FindElement(By.Id("save-btn1")).Click();
        }

        private void LoginInToConfluence() {
            const string password = "2XeytrEV)78";
            const string user = "v.kobzar@intetics.com";
            _driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/authenticate.action?destination=/admin/users/viewuser.action?username={user}");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("login-container")).Displayed);
            _driver.FindElement(By.Id("password")).SendKeys(password);
            _driver.FindElement(By.Id("authenticateButton")).Click();
        }
    }
}
