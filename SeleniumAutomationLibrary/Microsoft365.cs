using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace SeleniumAutomationLibrary {
    public class Microsoft365 {
        private WebDriverWait _wait;
        //private IWebDriver _driver;
        
        private string _userNameToMailMigrate;

        public Microsoft365() {
        }
        // declaring an event using built-in EventHandler
        public event EventHandler ProcessCompleted;

        protected virtual void OnProcessCompleted(EventArgs e) {
            ProcessCompleted?.Invoke(this, e);
        }

        public void Migrate(string userNameToMigrate) {
            _userNameToMailMigrate = userNameToMigrate;
            //IWebDriver driver = new ChromeDriver(); 
            /*OpenQA.Selenium.DriverServiceNotFoundException: 'The chromedriver.exe file does not exist in the current 
            * directory or in a directory on the PATH environment variable. The driver can be downloaded 
            * at http://chromedriver.storage.googleapis.com/ */

            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            using (IWebDriver driver = new ChromeDriver(driverService, new ChromeOptions())) {
                try {
                    _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                    try {
                        new Microsoft(driver).Login("admin");
                    }
                    catch (Exception msExeption) {
                        throw new Exception(msExeption.Message);
                    }

                    ProcessMailMigration(driver);
                }
                catch (Exception ex) {
                    throw new Exception(ex.Message);
                }
            }
        }

     

        private void ProcessMailMigration(IWebDriver driver) {
            //driver.Navigate().GoToUrl("https://outlook.office365.com/ecp/");
            /*
            _wait.Until(webDriver => webDriver.FindElement(By.XPath("//*[@id='Menu_UsersGroups']/span/a")).Displayed);

            driver.FindElement(By.XPath("//*[@id='Menu_UsersGroups']/span/a")).Click();
            driver.FindElement(By.XPath("//*[@id='Menu_MigrationBatches']/a")).Click();
            
            _wait.Until(webDriver => webDriver.FindElement(By.XPath("//*[@id='Menu_UsersGroups']/span/a")).Displayed);
            */

            driver.Navigate().GoToUrl("https://admin.exchange.microsoft.com");
            _wait.Until(webDriver => webDriver.FindElement(By.Name("Migration")).Displayed);

            driver.Navigate().GoToUrl("https://admin.exchange.microsoft.com/#/migrationbatch");
            _wait.Until(webDriver => webDriver.FindElement(By.Name("Add migration batch")).Displayed);

            driver.FindElement(By.Name("Add migration batch")).Click();

            //////////////////////////////
            //Migration path
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("input[data-automation-id='MigrationPath_MigrationBatchName']")).Displayed);
            IWebElement elem = driver.FindElement(By.CssSelector("input[data-automation-id='MigrationPath_MigrationBatchName']"));
            elem.SendKeys(_userNameToMailMigrate);
            driver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Next']")).Click();

            /////////////////////////////
            ///Migration type
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("div[data-automation-id='MigrationWizard_MigrationType']")).Displayed);
            var migrationType = driver.FindElement(By.CssSelector("div[data-automation-id='MigrationWizard_MigrationType']"));
            migrationType.SendKeys("Remote move migration");
            driver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Next']")).Click();

            ///////////////////////////////
            ///Prerequisite
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("button[data-automation-id='Prerequisite_Remote_Migration']")).Displayed);
            driver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Next']")).Click();

            //////////////////////////////
            ///Set endpoint
            Thread.Sleep(2000);
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("div[data-automation-id='SetEndpoint_SelectMigrationEndpoint']")).Displayed);
            driver.FindElement(By.CssSelector("div[data-automation-id='SetEndpoint_SelectMigrationEndpoint']:first-child")).SendKeys("mail.intetics.com");
            driver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Next']")).Click();

            //////////////////////////////
            ///Add users
            Thread.Sleep(1000);
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("div[data-automation-id='SelectUser']")).Displayed);
            //string text = driver.FindElement(By.CssSelector("div[role='radiogroup'].ms-ChoiceFieldGroup:nth-child(2)")).Text;
            driver.FindElement(By.CssSelector("span[id$='-PeoplePicker'].ms-ChoiceFieldLabel")).Click();
            //string text = driver.FindElement(By.CssSelector("span[id$='-PeoplePicker'].ms-ChoiceFieldLabel")).Text;
            //MessageBox.Show(text);
            driver.FindElement(By.CssSelector("input[class^='ms-BasePicker-input'")).SendKeys(_userNameToMailMigrate);
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("div.ms-PeoplePicker-personaContent")).Displayed);
            driver.FindElement(By.CssSelector("div.ms-PeoplePicker-personaContent")).Click();
            driver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Next']")).Click();

            ////////////////////////////////
            ///Move configuration
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("button[data-automation-id='configuration_Learn_More']")).Displayed);
            driver.FindElement(By.CssSelector("div[data-automation-id='configuration_Target_Delivery_Domain']")).SendKeys("intetics.mail.onmicrosoft.com");
            driver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Next']")).Click();

            //////////////////////////////
            //Schedule batch migration
            //select ho to send the end of migration mail
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("button[data-automation-id='BatchSchedule_LearnMore']")).Displayed);
            driver.FindElement(By.CssSelector("input[class^='ms-BasePicker-input'")).Clear();
            driver.FindElement(By.CssSelector("input[class^='ms-BasePicker-input'")).SendKeys("Vitaliy Kobzar");
            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("div.ms-PeoplePicker-personaContent")).Displayed);
            driver.FindElement(By.CssSelector("div.ms-PeoplePicker-personaContent")).Click();
            //
            //string text = driver.FindElement(By.CssSelector("div[data-automation-id='BatchSchedule_EndBy']+span.ms-ChoiceFieldLabel")).Text;
            //string text = driver.FindElement(By.CssSelector("span.ms-ChoiceFieldLabel")).Text;
            var parentBlock = driver.FindElement(By.CssSelector("div[data-automation-id='BatchSchedule_EndBy']"));
            var elements = parentBlock.FindElements(By.ClassName("ms-ChoiceField-field"));
            //MessageBox.Show(text);
            elements[1].Click(); //press the button Automaticly complite the batch

            driver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Save']")).Click();

            _wait.Until(webDriver => webDriver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Done']")).Displayed);
            driver.FindElement(By.CssSelector("button[data-automation-id='MigrationWizard_Done']")).Click();
        }
    }
}
