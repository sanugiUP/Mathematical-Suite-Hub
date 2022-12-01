using System;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace Authenticator
{
    public class AuthenticatorServer
    {
        public delegate void CleanUp(int num_of_mins);
        private static int num_of_mins = 5; //Default Num Of Minutes To Clean Token File
        private static ServiceHost host;

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome To The Authentication Service!");
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(typeof(AuthenticatorImplementation));
            host.AddServiceEndpoint(typeof(AuthenticatorInterface), tcp, "net.tcp://localhost:8100/AuthenticationService");
            host.Open();
            Console.WriteLine("Authentication System Online");

            Console.WriteLine("\n\nPlease Enter The Number Of Minutes For The Periodical Clean-Up Of Saved Tokens?");
            string u_input = Console.ReadLine();
            num_of_mins = int.Parse(u_input);

            Task task = new Task(CleanTokens);
            task.Start();

            Console.ReadLine();
            host.Close();
        }

        private static void CleanTokens()
        {
            while(host.State.ToString().Equals("Opened")) //Keep the method running on a seperate thread until server state changes
            {
                Thread.Sleep(num_of_mins * 10000); //Multiplied by 10000 to convert to minutes
                File.WriteAllText(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "TokenData.txt"), string.Empty);
            }            
        }
    }
}

/* REF : https://stackoverflow.com/questions/4999988/clear-the-contents-of-a-file 
 * REF : https://learn.microsoft.com/en-us/dotnet/api/system.servicemodel.communicationstate?view=netframework-4.8#system-servicemodel-communicationstate-opening */ 