using ActiveDirectoryLibrary;
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

            try {
                _driver.FindElement(By.CssSelector("option[value='jira-users']")).Click();                
            }
            catch {
                //trap here if jira-users items does not exist
                Console.WriteLine("jira-users options does not exist.");
            }

            try {
                _driver.FindElement(By.CssSelector("option[value='jira-users-intetics']")).Click();
            }
            catch {
                Console.WriteLine("jira-users options does not exist.");
            }

            _driver.FindElement(By.Id("user-edit-groups-leave")).Click();
        }

        public void Recruit(Employee employee) {
            //open new tab. Does not work
            //driver.FindElement(By.TagName("body")).SendKeys(Keys.Control+"t");

            _driver.Navigate().GoToUrl("https://jira.intetics.com/secure/admin/user/AddUser!default.jspa");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("user-create")).Displayed);

            _driver.FindElement(By.Id("user-create-email")).SendKeys(employee.Mail);
            _driver.FindElement(By.Id("user-create-fullname")).SendKeys(employee.FullName);
            _driver.FindElement(By.Id("user-create-username")).SendKeys(employee.Login);
            if (employee.Country.Equals(Country.UA) == true) {
                //in dropdown list need to select Kharkiv
                _driver.FindElement(By.Id("user-create-directoryId")).SendKeys("Kharkiv");
            }
            _driver.FindElement(By.Id("user-create-submit")).Click();
            //driver.FindElement(By.Id("user-create-cancel")).Click();
            //if user not a subcontractor
            if (employee.Subcontractor == false) {
                AddToGroup(employee);
            }
        }

        private void AddToGroup(Employee empl) {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("user-filter-userSearchFilter")).Displayed);
            string editGroupHref = _driver.FindElement(By.Id($"editgroups_{empl.Login}")).GetAttribute("href");
            //go to Manage user groups
            _driver.Navigate().GoToUrl(editGroupHref);

            _driver.FindElement(By.Id("groupsToJoin-textarea")).SendKeys("jira-users-intetics");
            _wait.Until(webDriver => webDriver.FindElement(By.ClassName("ajs-layer")).Displayed);
            _driver.FindElement(By.Id("groupsToJoin-textarea")).SendKeys(Keys.Enter);
            _driver.FindElement(By.Id("user-edit-groups-join")).Click();

            _driver.Navigate().GoToUrl($"https://jira.intetics.com/secure/admin/user/UserBrowser.jspa?createdUser={empl.Login}");
        }
    }
}
