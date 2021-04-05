using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumAutomationLibrary {
    class Jira {
        private WebDriverWait _wait;
        private IWebDriver _driver;
        //private string _userLogin;

        public Jira(IWebDriver driver) {
            _driver = driver;           
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

        public void Dismiss(string userLogin) {
            FindUser(userLogin);
            RemoveFromDefaultGroups(userLogin);
        }

        private void FindUser(string userToFind) {
            _driver.Navigate().GoToUrl("https://jira.intetics.com/secure/admin/user/UserBrowser.jspa");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("user-filter-userSearchFilter")).Displayed);
            _driver.FindElement(By.Id("user-filter-userSearchFilter")).SendKeys(userToFind);
            _driver.FindElement(By.Id("user-filter-userSearchFilter")).SendKeys(Keys.Enter);            
        }

        private void RemoveFromDefaultGroups(string userLoginToRemove) {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("user-filter-userSearchFilter")).Displayed);
            string editGroupHref = _driver.FindElement(By.Id($"editgroups_{userLoginToRemove}")).GetAttribute("href");
            //go to Manage user groups
            _driver.Navigate().GoToUrl(editGroupHref);
            _wait.Until(webDriver => webDriver.FindElement(By.Id("user-edit-groups")).Displayed);

            var groupToLeave = _driver.FindElements(By.Id("groupsToLeave"));
            _driver.FindElement(By.CssSelector("option[value='jira-users-intetics']")).Click();
            _driver.FindElement(By.CssSelector("option[value='jira-users']")).Click();
            _driver.FindElement(By.Id("user-edit-groups-leave")).Click();
        }
    }
}
