using ActiveDirectoryLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Windows;

namespace SeleniumAutomationLibrary {
    public class DismissingAnEmployee {
        public void Launch(Employee employee) {
            var driverService = ChromeDriverService.CreateDefaultService();
            //var driverService = ChromeDriverService.CreateDefaultService(@"D:\VSProjects\SeleniumAutomation\SeleniumAutomation");
            driverService.HideCommandPromptWindow = true;

            using (IWebDriver driver = new ChromeDriver(driverService, new ChromeOptions())) {

                try {
                    new Microsoft(driver).Login();
                }
                catch (Exception msExeption) {
                    throw new Exception(msExeption.Message);
                }

                try {                    
                    new Jira(driver).Dismiss(employee);
                }
                catch (Exception jiraExeption) {
                    MessageBoxResult result = MessageBox.Show($"{jiraExeption.Message} \n\n Continue the execution of the script?", "Something went wrong!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.No) {
                        throw new Exception(jiraExeption.Message);
                    }
                }

                try {
                    driver.SwitchTo().NewWindow(WindowType.Tab);
                    new Confluence(driver).Dismiss(employee);
                }
                catch (Exception confluenceExeption) {
                    throw new Exception(confluenceExeption.Message);
                }

                OnProcessCompleted(EventArgs.Empty);
            }
        }

        // declaring an event using built-in EventHandler
        public event EventHandler ProcessCompleted;
        protected virtual void OnProcessCompleted(EventArgs e) {
            ProcessCompleted?.Invoke(this, e);
        }
    }
}





