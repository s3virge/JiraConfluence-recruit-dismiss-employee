using System.DirectoryServices.ActiveDirectory;

namespace ActiveDirectoryLibrary {
    public class Domains {
        private const string UADomain = "intetics.com.ua";
        private const string BYDomain = "atwss.com";
        private static string anotherDomain;
        
        /// <summary>
        /// name of current domain like atwss.com
        /// </summary>
        private static string Current { 
            get {
                return Domain.GetComputerDomain().ToString();
            } 
        } 
      
        public static string LdapPathToCurrentDomain { 
            get {
                if (Current.Equals(UADomain)) {
                    currentDomainCountry = Country.UA;
                }
                else if (Current.Equals(BYDomain)) {
                    currentDomainCountry = Country.BY;
                }
                return $"LDAP://{Current}"; 
            } 
        }

        private static string currentDomainCountry = null;
        public static string CurrentDomainCountry { 
            get {               
                if (Current.Equals(UADomain)) {
                    currentDomainCountry = Country.UA;
                }
                else if (Current.Equals(BYDomain)) {
                    currentDomainCountry = Country.BY;
                }
                return currentDomainCountry;                
            }
            private set { currentDomainCountry = value; }
        }

        private static string anotherDomainCountry = null;
        public static string AnotherDomainCountry { 
            get {               
                if (Current.Equals(UADomain)) {
                    anotherDomainCountry = Country.BY;
                }
                else if (Current.Equals(BYDomain)) {
                    anotherDomainCountry = Country.UA;
                }
                return anotherDomainCountry;                
            }
            private set { anotherDomainCountry = value; }
        }

        public static string LdapPathToAnotherDomain {
            get {
                if (Current.Equals(UADomain) == true) {
                    anotherDomain = BYDomain;
                    anotherDomainCountry = Country.BY;
                }
                else if (Current.Equals(BYDomain) == true) {
                    anotherDomain = UADomain;
                    anotherDomainCountry = Country.UA;
                }
                return $"LDAP://{anotherDomain}";
            }
        }
    }
}
