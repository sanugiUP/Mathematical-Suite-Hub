using Authenticator;
using Newtonsoft.Json;
using RegistryAPIClasses;
using RestSharp;
using System;
using System.ServiceModel;

namespace ServicePublishingConsoleApp
{
    public class ServicePublisher
    {
        private static ChannelFactory<AuthenticatorInterface> foobFactory = new ChannelFactory<AuthenticatorInterface>(new NetTcpBinding(), "net.tcp://localhost:8100/AuthenticationService");
        private static AuthenticatorInterface foob = foobFactory.CreateChannel();
        private static RestClient client = new RestClient("http://localhost:53020/");

        static void Main(string[] args)
        {
            int token = 0;
            bool login = false;
            bool relogin = false;

            Console.WriteLine("WELCOME TO SERVICES PUBLISHING SERVICE!");
            while (true)
            {
                while (login == false)
                {
                    int selection = DisplayFirstMenu();
                    Console.WriteLine("\n\nPlease Enter Username: ");
                    string username = Console.ReadLine();
                    Console.WriteLine("\nPlease Enter Password: ");
                    string password = Console.ReadLine();
                    if (selection == 1) { token = LogIn(username, password); if (token != 0) { login = true; } }
                    else if (selection == 2) { Registration(username, password); }
                }

                int sec_selection = DisplaySecondMenu();

                while (sec_selection != 3)
                {
                    if (sec_selection == 1) { relogin = PublishService(token); }
                    else if (sec_selection == 2) { relogin = UnpublishService(token); }

                    if(relogin == true) { sec_selection = 3; }
                    else { sec_selection = DisplaySecondMenu(); }    
                }
                login = false;
            }
        }

        public static int DisplayFirstMenu()
        {
            bool valid = true;
            int selection = 0;

            while (valid)
            {
                try
                {
                    Console.WriteLine("\n\nPlease Log In Or Register To Use The Services:\n");
                    Console.WriteLine("01) Log In");
                    Console.WriteLine("02) Registration");
                    Console.WriteLine("\nPlease Enter the Integer Corresponding To Your Choice : ");
                    string choice = Console.ReadLine();
                    selection = Int32.Parse(choice);

                    if (selection == 1 || selection == 2) { valid = false; }
                    else { Console.WriteLine("Incorrect Input! Try Again."); }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Incorrect Input! Try Again.");
                }
            }
            return selection;
        }


        public static int DisplaySecondMenu()
        {
            bool valid = true;
            int selection = 0;

            while (valid)
            {
                try
                {
                    Console.WriteLine("\n\nPlease Choose One Of The Following Services:\n");
                    Console.WriteLine("01) Publish Service");
                    Console.WriteLine("02) Unpublish Service");
                    Console.WriteLine("03) Go Back To Main Menu");
                    Console.WriteLine("\nPlease Enter the Integer Corresponding Your Choice : ");
                    string choice = Console.ReadLine();
                    selection = Int32.Parse(choice);

                    if (selection == 1 || selection == 2 || selection == 3) { valid = false; }
                    else { Console.WriteLine("Incorrect Input! Try Again."); }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Incorrect Input! Try Again.");
                }
            }
            return selection;
        }


        public static void Registration(string username, string password)
        {
            string regst_status = foob.Register(username, password);
            if (regst_status == null)
            {
                Console.WriteLine("Authenticator Error. Server Currently Unavailable!\n\n");
            } else if (regst_status.Equals("Registration Unsuccessful!"))
            {
                Console.WriteLine(regst_status + " " + "User Already Registered!\n\n");
            }
            else if (regst_status.Equals("Successfully Registered"))
            {
                Console.WriteLine("Registration Successful!");
            }
        }


        public static int LogIn(string username, string password)
        {
            int token = foob.Login(username, password);
            if (token == 0) { Console.WriteLine("Log In Unsuccessful! User Doesn't Exist! Please Register!"); }
            else { Console.WriteLine("Log In Successful!\n\n"); }
            return token;
        }


        public static bool PublishService(int token)
        {
            bool relogin = false;
            string ServiceProviderControllerURL = "http://localhost:51352/" + "api/calculator/";
            ServiceDescription serviceDescription = new ServiceDescription();
            bool formatCheck = true;

            Console.WriteLine("WELCOME TO SERVICE PUBLISH SERVICE!\n\n");

            Console.WriteLine("Please Enter Service Name: ");
            string name = Console.ReadLine();
            while(validateInput(name) == false)
            {
                Console.WriteLine("Invalid Input! Please Enter Again");
                Console.WriteLine("Please Enter Service Name: ");
                name = Console.ReadLine();
            }
            serviceDescription.name = name;

            Console.WriteLine("\nPlease Enter Service Description: ");
            string desc = Console.ReadLine();
            while (validateInput(desc) == false)
            {
                Console.WriteLine("Invalid Input! Please Enter Again");
                Console.WriteLine("Please Enter Service Description: ");
                desc = Console.ReadLine();
            }
            serviceDescription.description = desc;

            Console.WriteLine("\nPlease Enter Service End Point API: ");
            string endpoint = Console.ReadLine();
            while (validateInput(endpoint) == false)
            {
                Console.WriteLine("Invalid Input! Please Enter Again");
                Console.WriteLine("Please Enter Service End Point API: ");
                endpoint = Console.ReadLine();
            }
            serviceDescription.end_point_API = ServiceProviderControllerURL + endpoint.ToLower();

            while (formatCheck)
            {
                Console.WriteLine("\nPlease Enter Number Of Operands: ");
                try { serviceDescription.operands = Int32.Parse(Console.ReadLine()); formatCheck = false; }
                catch (FormatException) { Console.WriteLine("Incorrect Input! Please Try Again."); }
            }

            Console.WriteLine("\nPlease Enter Operand Type: ");
            string otype = Console.ReadLine();
            while (validateInput(otype) == false)
            {
                Console.WriteLine("Invalid Input! Please Enter Again");
                Console.WriteLine("Please Enter Operand Type: ");
                otype = Console.ReadLine();
            }
            serviceDescription.operandType = otype;

            RestRequest request = new RestRequest("api/registryservices/publish/{token}", Method.Post);
            request.AddUrlSegment("token", token);
            request.AddJsonBody(serviceDescription);
            RestResponse response = client.Execute(request);
            ServiceStatus result = JsonConvert.DeserializeObject<ServiceStatus>(response.Content);

            Console.WriteLine("\n\nREQUEST STATUS: " + result.status);
            Console.WriteLine("PUBLISH STATUS: " + result.reason);

            if(result.status.Equals("Service Denied")) { relogin = true; }
            return relogin;
        }


        public static bool UnpublishService(int token)
        {
            bool relogin = false;
            string ServiceProviderControllerURL = "http://localhost:51352/" + "api/calculator/";
            URLInput input = new URLInput();

            Console.WriteLine("WELCOME TO SERVICE UNPUBLISH SERVICE!\n\n");

            Console.WriteLine("Please Enter the Service API End Point: ");
            string endpoint = Console.ReadLine();
            while(validateInput(endpoint) == false)
            {
                Console.WriteLine("Invalid Input! Please Enter Again");
                Console.WriteLine("Please Enter the Service API End Point: ");
                endpoint = Console.ReadLine();
            }
            input.end_point_API = ServiceProviderControllerURL + endpoint;

            RestRequest request = new RestRequest("api/registryservices/unpublish/{token}", Method.Post);
            request.AddUrlSegment("token", token);
            request.AddJsonBody(input);
            RestResponse response = client.Post(request);
            ServiceStatus result = JsonConvert.DeserializeObject<ServiceStatus>(response.Content);

            Console.WriteLine("\n\nREQUEST STATUS: " + result.status);
            Console.WriteLine("PUBLISH STATUS: " + result.reason);

            if (result.status.Equals("Service Denied")) { relogin = true; }
            return relogin;
        }


        private static bool validateInput(string input)
        {
            if (input.Equals("")) { return false; }
            else { return true; }
        }
        
    }
}
