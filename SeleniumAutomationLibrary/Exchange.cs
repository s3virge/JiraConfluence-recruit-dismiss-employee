using ActiveDirectoryLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeleniumAutomationLibrary {
    public class Exchange {
        private WebDriverWait _wait;
        private IWebDriver _driver;

        public Exchange(IWebDriver driver) {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

        private void Login(string login = null, string password = null) {
            login = "";
            password = "";

            _driver.Navigate().GoToUrl("https://mail.intetics.com/ecp/?exsvurl=1&p=Mailboxes");
            _driver.Manage().Window.Maximize();

            _wait.Until(webDriver => webDriver.FindElement(By.Id("username")).Displayed);

            //login form
            _driver.FindElement(By.Id("username")).SendKeys(login);
            _driver.FindElement(By.Id("password")).SendKeys(password);
            _driver.FindElement(By.ClassName("signinbutton")).Click();
        }

        private void CreareMailBox(Employee empl) {
            //_wait.Until(webDriver => webDriver.FindElement(By.Id("Menu_Mailboxes")).Displayed);
            _driver.Navigate().GoToUrl("https://mail.intetics.com/ecp/UsersGroups/NewMailboxOnPremises.aspx?pwmcid=2&ReturnObjectType=1");
            _wait.Until(webDriver => webDriver.FindElement(By.Id("ResultPanePlaceHolder_caption_textContainer")).Displayed);

            _driver.FindElement(By.ClassName("pickerBrowseButton")).Click();

            //_driver.Navigate().GoToUrl("https://mail.intetics.com/ecp/Pickers/CustomizedAdUserPicker.aspx?pwmcid=1&Launcher=ResultPanePlaceHolder_NewMailbox_contentContainer_pickerUser&mode=single&filter=RecipientTypeDetails%20-eq%20%27User%27");
            //_wait.Until(webDriver => webDriver.FindElement(By.Id("ResultPanePlaceHolder_pickerContent_txtSearch")).Displayed);
            var mainWindowHandle = _driver.CurrentWindowHandle;
            var wnd = _driver.WindowHandles;

            foreach (var wind in wnd) {
                if (!wind.Equals(mainWindowHandle)) {
                    _driver.SwitchTo().Window(wind);
                }
            }

            Thread.Sleep(5*1000);
            _driver.FindElement(By.Id("ResultPanePlaceHolder_pickerContent_txtSearch")).SendKeys(empl.FullName);
            _driver.FindElement(By.Id("ResultPanePlaceHolder_pickerContent_txtSearch_SearchButton_ImageSearchButton")).Click();
            Thread.Sleep(2 * 1000);
            //ok button
            _driver.FindElement(By.Id("ResultPanePlaceHolder_ButtonsPanel_btnCommit")).Click();
            
            _driver.SwitchTo().Window(mainWindowHandle);

            _driver.FindElement(By.Id("ResultPanePlaceHolder_ButtonsPanel_btnCommit")).Click(); 
        }

        public void CreateUserMailBox(Employee emp) {
            Login();
            CreareMailBox(emp);
        }
    }
}
