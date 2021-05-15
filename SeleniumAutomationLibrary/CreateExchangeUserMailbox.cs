using ActiveDirectoryLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumAutomationLibrary {
    public class CreateExchangeUserMailbox {
        // declaring an event using built-in EventHandler
        public event EventHandler ProcessCompleted;

        protected virtual void OnProcessCompleted(EventArgs e) {
            ProcessCompleted?.Invoke(this, e);
        }

        public void Launch(Employee employee) {
            var driverService = ChromeDriverService.CreateDefaultService();           
            driverService.HideCommandPromptWindow = true;

            using (IWebDriver driver = new ChromeDriver(driverService, new ChromeOptions())) {
                try {
                    new Exchange(driver).CreateUserMailBox(employee);
                    OnProcessCompleted(EventArgs.Empty);
                }
                catch (Exception exchangeException) {
                    throw new Exception(exchangeException.Message);
                }
            }
        }
    }
}
