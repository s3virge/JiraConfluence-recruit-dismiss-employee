using ActiveDirectoryLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;

namespace SeleniumAutomationLibrary
{
    class Confluence {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private WebDriverWait _wait;
        private IWebDriver _driver;        

        public Confluence(IWebDriver driver) {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

        public void Dismiss(Employee employee) {
            log.Debug("Try to fire off an employee");
            //LoginInToConfluence();
            _driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/admin/users/editusergroups-start.action?username={employee.Login}");

            log.Debug("Wait when display 'admin-content' element");
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
            /*конфлюенс требует Administrator Access - логинимся*/
            log.Debug($"Try to login in confluence");

            StackFrame callStack = new StackFrame(1, true);
            //log.Info($"Log message here - {callStack.GetFileName()} {callStack.GetFileLineNumber()}");

            string user, password;
            Settings.ReadLoginPasswordFromRegestry(out user, out password);
            
            _driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/authenticate.action?destination=/admin/users/viewuser.action?username={user}");
                        
            log.Debug("Wait when will display 'login-container' element ");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("login-container")).Displayed);
            
            log.Debug("Find element 'password' and send keys");
            _driver.FindElement(By.Id("password")).SendKeys(password);
            
            log.Debug("Find element 'authenticateButton' and click");
            _driver.FindElement(By.Id("authenticateButton")).Click();

            log.Info("'authenticateButton' element was clicked");
        }

        public void Recruit(Employee emp) {
            //LoginInToConfluence();
            //log.Info("Login to confluence was successful");

            CreateNewEmployee(emp);

            if (emp.Subcontractor == false) {
                AddToGroup(emp);
            }
        }

        private void CreateNewEmployee(Employee emplo) {
            log.Debug("Try to create user account in confluence");

            _driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/admin/users/createuser.action");

            log.Debug("Wait when display 'create-pane' element");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("create-pane")).Displayed);
            _driver.FindElement(By.Id("username")).SendKeys(emplo.Login);
            _driver.FindElement(By.Id("fullname")).SendKeys(emplo.FullName);
            _driver.FindElement(By.Id("email")).SendKeys(emplo.Mail);
            const string pass = "1";
            _driver.FindElement(By.Id("password")).SendKeys(pass);
            _driver.FindElement(By.Id("confirm")).SendKeys(pass);
            _driver.FindElement(By.XPath("//*[@id='create-user-form']/form/fieldset/div[7]/div/input")).SendKeys(Keys.Enter);

            log.Info("Employee was successfuly created ");
        }

        private void AddToGroup(Employee epl) {
            _driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/admin/users/editusergroups-start.action?username={epl.Login}");

            IWebElement checkBoxEmployees = _driver.FindElement(By.Id("confluence-user"));
            if (checkBoxEmployees.Selected == false)
            {
                checkBoxEmployees.Click();
            }

            checkBoxEmployees = _driver.FindElement(By.Id("confluence-employees"));
            if (checkBoxEmployees.Selected == false) {
                checkBoxEmployees.Click();
            }

            //immediately add to the project group
            checkBoxEmployees = _driver.FindElement(By.Id("odt-0279 (gde planview – spatial data processing) developers"));
            if (checkBoxEmployees.Selected == false)
            {
                checkBoxEmployees.Click();
            }

            _driver.FindElement(By.Name("save")).Click();
        }
    }
}
