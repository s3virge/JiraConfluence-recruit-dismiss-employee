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

            var chekbox = _driver.FindElement(By.Id("confluence-employees"));
            if (chekbox.Selected) {
                chekbox.Click();
            }

            chekbox = _driver.FindElement(By.Id("confluence-users"));
            if (chekbox.Selected) {
                chekbox.Click();
            }

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

        public void Recruit(Employee emp) {
            LoginInToConfluence();
            ProcessUserCreatingConfluence(emp);
            if (emp.Subcontractor == false) {
                AddToGroup(emp);
            }
        }

        private void ProcessUserCreatingConfluence(Employee emplo) {
            _driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/admin/users/createuser.action");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("create-pane")).Displayed);
            _driver.FindElement(By.Id("username")).SendKeys(emplo.Login);
            _driver.FindElement(By.Id("fullname")).SendKeys(emplo.FullName);
            _driver.FindElement(By.Id("email")).SendKeys(emplo.Mail);
            const string pass = "1";
            _driver.FindElement(By.Id("password")).SendKeys(pass);
            _driver.FindElement(By.Id("confirm")).SendKeys(pass);
            _driver.FindElement(By.XPath("//*[@id='create-user-form']/form/fieldset/div[7]/div/input")).SendKeys(Keys.Enter);
        }

        private void AddToGroup(Employee epl) {
            _driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/admin/users/editusergroups-start.action?username={epl.Login}");
            IWebElement checkBoxEmployees = _driver.FindElement(By.Id("confluence-employees"));
            if (checkBoxEmployees.Selected == false) {
                checkBoxEmployees.Click();
            }
            _driver.FindElement(By.Name("save")).Click();
        }
    }
}
