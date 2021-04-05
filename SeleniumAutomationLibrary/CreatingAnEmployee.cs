using ActiveDirectoryLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumAutomationLibrary {
    public class CreatingAnEmployee {
        private Employee _employee;
        private WebDriverWait _wait;
        private const string _password = "2XeytrEV)78";

        // declaring an event using built-in EventHandler
        public event EventHandler ProcessCompleted;

        protected virtual void OnProcessCompleted(EventArgs e) {
            ProcessCompleted?.Invoke(this, e);
        }

        public void Launch(Employee user) {
            _employee = user;

            //IWebDriver driver = new ChromeDriver(); 
            /*OpenQA.Selenium.DriverServiceNotFoundException: 'The chromedriver.exe file does not exist in the current 
            * directory or in a directory on the PATH environment variable. The driver can be downloaded 
            * at http://chromedriver.storage.googleapis.com/ */

            var driverService = ChromeDriverService.CreateDefaultService();
            //var driverService = ChromeDriverService.CreateDefaultService(@"D:\VSProjects\SeleniumAutomation\SeleniumAutomation");
            driverService.HideCommandPromptWindow = true;

            using (IWebDriver driver = new ChromeDriver(driverService, new ChromeOptions())) {
                try {
                    _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                    ProcessMicrosoftLogin(driver);
                    JiraUserCreating(driver);
                    ConfluenceUserCreating(driver);

                    OnProcessCompleted(EventArgs.Empty);
                }
                catch (Exception ex) {
                    throw new Exception(ex.Message);
                }
            }
        }

        private void JiraUserCreating(IWebDriver driver) {
            ProcessUserCreatingInJira(driver);

            //if user not a subcontractor
            if (_employee.Subcontractor == false) {
                AddToJiraGroup(driver);
            }
        }

        private void ConfluenceUserCreating(IWebDriver driver) {
            /*
             driver.findElement(By.cssSelector("body")).sendKeys(Keys.CONTROL + "t");
            By using JavaScript:

            WebDriver driver = new FirefoxDriver(); // Firefox or any other Driver
            JavascriptExecutor jse = (JavascriptExecutor)driver;
            jse.executeScript("window.open()");
            After opening a new tab it needs to switch to that tab:

            ArrayList<String> tabs = new ArrayList<String>(driver.getWindowHandles());
            driver.switchTo().window(tabs.get(1));
             */

            /*
             ((JavascriptExecutor)driver).executeScript("alert('Test')");
             driver.switchTo().alert().accept();
             driver.findElement(By.cssSelector("body")).sendKeys(Keys.CONTROL + "t");
             */

            // Opens a new tab and switches to new tab.
            // selenium 4 only
            driver.SwitchTo().NewWindow(WindowType.Tab);

            LoginToConfluence(driver);
            ProcessUserCreatingConfluence(driver);
            if (_employee.Subcontractor == false) {
                AddToConfluenceGroup(driver);
            }
        }

        private void ProcessUserCreatingConfluence(IWebDriver driver) {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("create-pane")).Displayed);
            driver.FindElement(By.Id("username")).SendKeys(_employee.Login);
            driver.FindElement(By.Id("fullname")).SendKeys(_employee.FullName);
            driver.FindElement(By.Id("email")).SendKeys(_employee.Mail);
            const string pass = "1";
            driver.FindElement(By.Id("password")).SendKeys(pass);
            driver.FindElement(By.Id("confirm")).SendKeys(pass);
            driver.FindElement(By.XPath("//*[@id='create-user-form']/form/fieldset/div[7]/div/input")).SendKeys(Keys.Enter);
        }

        private void AddToConfluenceGroup(IWebDriver driver) {
            driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/admin/users/editusergroups-start.action?username={_employee.Login}");
            IWebElement checkBoxEmployees = driver.FindElement(By.Id("confluence-employees"));
            if (checkBoxEmployees.Selected == false) {
                checkBoxEmployees.Click();
            }
            driver.FindElement(By.Name("save")).Click();
        }

        private void LoginToConfluence(IWebDriver driver) {
            driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/authenticate.action?destination=/admin/users/viewuser.action?username={_employee.Login}");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("login-container")).Displayed);
            driver.FindElement(By.Id("password")).SendKeys(_password);
            driver.FindElement(By.Id("authenticateButton")).Click();
            driver.Navigate().GoToUrl($"https://confluence.intetics.com/confluence/admin/users/createuser.action");
        }

        private void AddToJiraGroup(IWebDriver driver) {
            _wait.Until(webDriver => webDriver.FindElement(By.Id("user-filter-userSearchFilter")).Displayed);
            string editGroupHref = driver.FindElement(By.Id($"editgroups_{_employee.Login}")).GetAttribute("href");
            //go to Manage user groups
            driver.Navigate().GoToUrl(editGroupHref);

            driver.FindElement(By.Id("groupsToJoin-textarea")).SendKeys("jira-users-intetics");
            _wait.Until(webDriver => webDriver.FindElement(By.ClassName("ajs-layer")).Displayed);
            driver.FindElement(By.Id("groupsToJoin-textarea")).SendKeys(Keys.Enter);
            driver.FindElement(By.Id("user-edit-groups-join")).Click();

            driver.Navigate().GoToUrl($"https://jira.intetics.com/secure/admin/user/UserBrowser.jspa?createdUser={_employee.Login}");
        }

        private void ProcessUserCreatingInJira(IWebDriver driver) {
            //open new tab. Does not work
            //driver.FindElement(By.TagName("body")).SendKeys(Keys.Control+"t");

            driver.Navigate().GoToUrl("https://jira.intetics.com/secure/admin/user/AddUser!default.jspa");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("user-create")).Displayed);

            driver.FindElement(By.Id("user-create-email")).SendKeys(_employee.Mail);
            driver.FindElement(By.Id("user-create-fullname")).SendKeys(_employee.FullName);
            driver.FindElement(By.Id("user-create-username")).SendKeys(_employee.Login);
            if (_employee.Country.Equals(Country.UA) == true) {
                //in dropdown list need to select Kharkiv
                driver.FindElement(By.Id("user-create-directoryId")).SendKeys("Kharkiv");
            }
            driver.FindElement(By.Id("user-create-submit")).Click();
            //driver.FindElement(By.Id("user-create-cancel")).Click();
        }

        private void ProcessMicrosoftLogin(IWebDriver driver) {
            driver.Navigate().GoToUrl("https://login.microsoftonline.com");
            driver.Manage().Window.Maximize();

            _wait.Until(webDriver => webDriver.FindElement(By.Name("loginfmt")).Displayed);

            //Microsoft login form
            IWebElement msFormLogin = driver.FindElement(By.Name("loginfmt"));
            msFormLogin.SendKeys("v.kobzar@intetics.com");
            driver.FindElement(By.Id("idSIButton9")).Click();

            _wait.Until(webDriver => webDriver.FindElement(By.Name("passwd")).Displayed);
            driver.FindElement(By.Name("passwd")).SendKeys(_password);
            driver.FindElement(By.Id("idSIButton9")).Click();

            _wait.Until(webDriver => webDriver.FindElement(By.Id("idBtn_Back")).Displayed);
        }
    }
}
