using ActiveDirectoryLibrary;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Windows;

namespace SeleniumAutomationLibrary {
    public class CreatingAnEmployee {
        // declaring an event using built-in EventHandler
        public event EventHandler ProcessCompleted;

        protected virtual void OnProcessCompleted(EventArgs e) {
            ProcessCompleted?.Invoke(this, e);
        }

        public void Launch(Employee coworker) {
            
            //IWebDriver driver = new ChromeDriver(); 
            /*OpenQA.Selenium.DriverServiceNotFoundException: 'The chromedriver.exe file does not exist in the current 
            * directory or in a directory on the PATH environment variable. The driver can be downloaded 
            * at http://chromedriver.storage.googleapis.com/ */

            var driverService = ChromeDriverService.CreateDefaultService();
            //var driverService = ChromeDriverService.CreateDefaultService(@"D:\VSProjects\SeleniumAutomation\SeleniumAutomation");
            driverService.HideCommandPromptWindow = true;

            using (IWebDriver driver = new ChromeDriver(driverService, new ChromeOptions())) {
                try {
                    try {
                        new Microsoft(driver).Login();
                    }
                    catch (Exception msExeption) {
                        throw new Exception(msExeption.Message);
                    }

                    try {
                        new Jira(driver).Recruit(coworker);                        
                    }
                    catch (Exception jiraExeption) {
                        MessageBoxResult result = MessageBox.Show($"{jiraExeption.Message} \n\n Continue the execution of the script?", "Something went wrong!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (result == MessageBoxResult.No) {
                            throw new Exception(jiraExeption.Message);
                        }
                    }

                    try {
                        driver.SwitchTo().NewWindow(WindowType.Tab);
                        new Confluence(driver).Recruit(coworker);
                    }
                    catch (Exception confluenceExeption) {
                        throw new Exception(confluenceExeption.Message);
                    }

                    OnProcessCompleted(EventArgs.Empty);
                }
                catch (Exception ex) {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
