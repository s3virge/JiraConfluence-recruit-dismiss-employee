using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryLibrary{
    public class ActiveDirectory {
        
        public List<string> GetListOfEmployee(string ldapRoot, string whoToSeek = null) {
            var emplList = new List<string>();            
            string searchFilter = $"(&(objectCategory=user)(cn=*{whoToSeek}*))";

            //seek by name
            DirectorySearcher dirSearcher = new DirectorySearcher(new DirectoryEntry(ldapRoot), searchFilter);

            if (string.IsNullOrEmpty(whoToSeek) == true) {
                dirSearcher.Filter = $"( &(objectCategory=User))";
            }

            foreach (SearchResult resEnt in dirSearcher.FindAll()) {
                DirectoryEntry directoryEntry = resEnt.GetDirectoryEntry();
                                
                string objName = directoryEntry.Name;

                if (objName.StartsWith("CN=")) {
                    objName = objName.Remove(0, "CN=".Length);
                }

                emplList.Add(objName);
            }

            if (emplList.Count > 0) {
                emplList.Sort();
            }

            dirSearcher.Dispose();

            return emplList;
        }
       
        /// <summary>
        /// get from ad object properties
        /// </summary>
        public Employee GetEmployee(string ldapRoot, string empoyeeNameOrLogin, bool searchByUserLogin = false) {
            Employee employee = new Employee();
            string searchFilter = $"(&(objectCategory=User)(Name={empoyeeNameOrLogin}))";

            if (searchByUserLogin) {
                searchFilter = $"(&(objectCategory=user)(SamAccountName=*{empoyeeNameOrLogin}*))";
            }

            string[] properties = new string[] {
                "name",             //0
                "samaccountname",   //1
                "department",       //2               
                "title",            //3 //job
                "mail",             //4
                "mobile",           //5
                "canonicalname",    //6
                "whencreated",      //7
                "whenchanged",      //8
                "pwdlastset",       //9
                "lastlogon",        //10
                "description",      //11
                "memberof",         //12
                "distinguishedname",//13
                "useraccountcontrol",//14                
                "msds-user-account-control-computed",//15
                "physicaldeliveryofficename", //16
                "manager"           //17
            };

            DirectorySearcher mySearcher = new DirectorySearcher(new DirectoryEntry(ldapRoot), searchFilter, properties);

            //mySearcher.PropertiesToLoad.Add("CanonicalName");
            //mySearcher.PropertiesToLoad.Add("*");
            SearchResult resEnt = mySearcher.FindOne();

#if DEBUG
            if (resEnt != null) {
                int propCount = resEnt.Properties.Count;
                string[] propNames = new string[propCount];
                //string[] propValues = new string[propCount];
                resEnt.Properties.PropertyNames.CopyTo(propNames, 0);
                for (int c = 0; c < propCount; c++) {
                    Console.WriteLine($"{c,-3} {propNames[c],-20} = {resEnt.Properties[propNames[c]][0]}");
                }
            }
#endif
            if (resEnt != null) {
                employee.FullName = SelectPropertie(resEnt, "name");
                employee.Login = SelectPropertie(resEnt, "SamAccountName");
                employee.Department = SelectPropertie(resEnt, "department");
                employee.Job = SelectPropertie(resEnt, "title");
                employee.Mail = SelectPropertie(resEnt, "mail");
                employee.Mobile = SelectPropertie(resEnt, "mobile");
                employee.CanonicalName = SelectPropertie(resEnt, "CanonicalName");
                employee.WhenCreated = SelectPropertie(resEnt, "whenCreated");
                employee.WhenChanged = SelectPropertie(resEnt, "whenChanged");
                employee.PasswordLastSet = SelectDataTimePropertie(resEnt, "pwdLastSet");
                employee.LastLogon = SelectDataTimePropertie(resEnt, "lastLogon");
                employee.MemderOf = SelectMemberOfPropertie(resEnt);
                employee.Description = SelectPropertie(resEnt, "description");
                employee.DistinguishedName = SelectPropertie(resEnt, "DistinguishedName");
                employee.Office = SelectPropertie(resEnt, "physicalDeliveryOfficeName");
                employee.Manager = ExtractNameFromCN(SelectPropertie(resEnt, "manager"));

                //string propVal = SelectPropertie(resEnt, "msDS-User-Account-Control-Computed");
                //if (string.IsNullOrEmpty(propVal) != true) {
                //    int attribute = Convert.ToInt32(propVal);
                //    employee.Locked = Convert.ToBoolean(attribute & (int)UserAccountControl.LOCKOUT);
                //}

                //employee.Disabled = IsUserAccountControlFlagSet(resEnt, (int)UserAccountControl.ACCOUNTDISABLE);
            }

            return employee;
        }

        private List<string> SelectMemberOfPropertie(SearchResult resEnt) {
            ResultPropertyValueCollection collection = resEnt.Properties["memberof"];
            List<string> groups = new List<string>();

            foreach (string item in collection) {
                //"CN=APP_Java_v8.0.172,OU=Applications,OU=Groups,DC=intetics,DC=com,DC=ua"
                //Match match = Regex.Match(temp, @"CN=\s*(?<g>\w*)\s*.");
                string temp = ExtractNameFromCN(item);
                groups.Add(temp);
            }

            return groups;
        }

        /// <summary>
        /// extract DataTime propertie and convert to string
        /// </summary>
        /// <param name="resEnt"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        private string SelectDataTimePropertie(SearchResult resEnt, string propName) {
            string result = "";
            if (resEnt.Properties.Contains(propName)) {
                long value = (long)resEnt.Properties[propName][0];
                if (value > 0) {
                    DateTime pwdLastSet = DateTime.FromFileTimeUtc(value);
                    result = pwdLastSet.ToString();
                }
            }
            return result;
        }

        /// <summary>
        /// select specified properties from SearchResult
        /// </summary>
        /// <param name="resEnt"></param>
        /// <param name="propertie"></param>
        /// <returns>empty string if propertie is not exist</returns>
        private string SelectPropertie(SearchResult resEnt, string propertie) {
            string result = "";
            if (resEnt.Properties[propertie] != null && resEnt.Properties[propertie].Count > 0)
                result = resEnt.Properties[propertie][0].ToString();
            return result;
        }

        private string ExtractNameFromCN(string item) {
            int firstSignIndex = item.IndexOf('=') + 1;
            int lastSignIndex = item.IndexOf(',');
            int length = lastSignIndex - firstSignIndex;
            string temp = "";
            if (length != -1) {
                temp = item.Substring(firstSignIndex, length);
            }
            return temp;
        }
    }
}
