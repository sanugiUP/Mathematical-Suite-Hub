using System.ServiceModel;
using System.Windows;
using Newtonsoft.Json;
using RestSharp;
using Authenticator;
using ServiceProviderGUIClasses;
using System;
using System.Threading.Tasks;
using RegistryAPIClasses;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ClientGUIApplication
{
    public delegate string Register();
    public delegate int Login();
    public delegate ServiceStatus searchAll();
    public delegate ServiceStatus search();
    public delegate CalculatorData testService();

    public partial class MainWindow : Window
    {
        private static ChannelFactory<AuthenticatorInterface> foobFactory = new ChannelFactory<AuthenticatorInterface>(new NetTcpBinding(), "net.tcp://localhost:8100/AuthenticationService");
        private static AuthenticatorInterface foob = foobFactory.CreateChannel();
        private static RestClient client = new RestClient("http://localhost:53020/");
        private string username, password, service_name, endpointAPI;
        private static int token;
        private static List<TextBox> textBoxes = new List<TextBox>();
        private static List<TextBlock> textBlocks = new List<TextBlock>();
        private static CalculatorInputData in_data = new CalculatorInputData();

        public MainWindow()
        {
            InitializeComponent();
            NetTcpBinding tcp = new NetTcpBinding();
            string authURL = "net.tcp://localhost:8100/AuthenticationService";
            ChannelFactory<AuthenticatorInterface> foobFactory = new ChannelFactory<AuthenticatorInterface>(tcp, authURL);
            foob = foobFactory.CreateChannel();
            hideServiceTester();

        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            username = user_name.Text;
            password = user_password.Text;
            FreezeLoginRegisterFunctions();

            if (username.Equals("") || password.Equals(""))
            {
                MessageBox.Show("Username and Password Fields cannot be Empty.", "Warning!");
                UnfreezeLoginRegisterFunctions();
            }
            else
            {
                int timeout = 20000; //timeout only 20 seconds
                Task<int> task = new Task<int>(login);
                task.Start();
                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    int session_token = await task;
                    if (session_token == 0)
                    {
                        MessageBox.Show("Username or password does not exist. Please try registering first.", "Login Failed!");
                        UnfreezeLoginRegisterFunctions();
                    }
                    else { MessageBox.Show("Login Successful!"); showServiceTester(); token = session_token; }
                }
                else
                {
                    MessageBox.Show("Login Timeout! Try Again!");
                    UnfreezeLoginRegisterFunctions();
                }
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            username = user_name.Text;
            password = user_password.Text;
            FreezeLoginRegisterFunctions();

            if (username.Equals("") || password.Equals(""))
            {
                MessageBox.Show("Username and Password Fields cannot be Empty.", "Warning!");
            }
            else
            {
                int timeout = 20000; //timeout only 20 seconds
                Task<string> task = new Task<string>(register);
                task.Start();
                
                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    string resgister_status = await task;
                    if (resgister_status.Equals("Registration Unsuccessful!")) { 
                        MessageBox.Show("Username and password already exist.", "Registration Failed!"); 
                    }
                    else { MessageBox.Show(resgister_status); }
                }
                else
                {
                    MessageBox.Show("Registration Timeout! Try Again!");
                }
            }
            UnfreezeLoginRegisterFunctions();
        }

        private int login()
        {
            int session_token = foob.Login(username, password);
            return session_token;
        }

        private string register()
        {
            String status = foob.Register(username, password);
            return status;
        }

        private void FreezeLoginRegisterFunctions()
        {
            user_name.IsReadOnly = true;
            user_password.IsReadOnly = true;
            user_name.Clear();
            user_password.Clear();
            RegisterButton.Visibility = Visibility.Hidden;
            LoginButton.Visibility = Visibility.Hidden;
        }

        private void UnfreezeLoginRegisterFunctions()
        {
            user_name.IsReadOnly = false;
            user_password.IsReadOnly = false;
            user_name.Clear();
            user_password.Clear();
            RegisterButton.Visibility = Visibility.Visible;
            LoginButton.Visibility = Visibility.Visible;
        }

        private void hideServiceTester()
        {
            ServiceNameBox.Visibility = Visibility.Hidden;
            ServiceDropList.Visibility = Visibility.Hidden;
            Welcome.Visibility = Visibility.Hidden;
            SearchNameButton.Visibility = Visibility.Hidden;
            SearchAllButton.Visibility = Visibility.Hidden;
            h1.Visibility = Visibility.Hidden;
            h2.Visibility = Visibility.Hidden;
            h3.Visibility = Visibility.Hidden;
            h4.Visibility = Visibility.Hidden;
            h5.Visibility = Visibility.Hidden;
            h7.Visibility = Visibility.Hidden;
            TestButton.Visibility = Visibility.Hidden;
            ResultBox.Visibility = Visibility.Hidden;
            NameBox.Visibility = Visibility.Hidden;
            DescBox.Visibility = Visibility.Hidden;
            APIBox.Visibility = Visibility.Hidden;
            NumOperandsBox.Visibility = Visibility.Hidden;
            OperandTypeBox.Visibility = Visibility.Hidden;
        }

        private void showServiceTester()
        {
            ServiceNameBox.Visibility = Visibility.Visible;
            ServiceDropList.Visibility = Visibility.Visible;
            Welcome.Visibility = Visibility.Visible;
            SearchNameButton.Visibility = Visibility.Visible;
            SearchAllButton.Visibility = Visibility.Visible;
            h1.Visibility = Visibility.Visible;
            h2.Visibility = Visibility.Visible;
            h3.Visibility = Visibility.Visible;
            h4.Visibility = Visibility.Visible;
            h5.Visibility = Visibility.Visible;
            h7.Visibility = Visibility.Visible;
            ResultBox.Visibility = Visibility.Visible;
            NameBox.Visibility = Visibility.Visible;
            DescBox.Visibility = Visibility.Visible;
            APIBox.Visibility = Visibility.Visible;
            NumOperandsBox.Visibility = Visibility.Visible;
            OperandTypeBox.Visibility = Visibility.Visible;
        }

        /* REF : https://stackoverflow.com/questions/12117119/creating-dynamic-textbox-in-wpf-according-to-location
        * REF : https://stackoverflow.com/questions/15008871/how-to-create-many-labels-and-textboxes-dynamically-depending-on-the-value-of-an?noredirect=1&lq=1
        * REF : https://stackoverflow.com/questions/40118174/input-from-c-sharp-dynamically-created-text-boxes
        * REF : https://www.c-sharpcorner.com/UploadFile/mahesh/wpf-textbox/ */
        private void showTestServiceInputFields(int num_of_operands)
        {
            for (int i = 0; i < num_of_operands; i++) {
                TextBox t = new TextBox();
                t.Margin = new Thickness(270, ((i * 20) + 545), 0, 0);
                t.HorizontalAlignment = HorizontalAlignment.Left;
                t.TextWrapping = TextWrapping.Wrap;
                t.VerticalAlignment = VerticalAlignment.Top;
                t.Height = 23;
                t.Width = 83;
                textBoxes.Add(t);
                Grid.Children.Add(t);

                TextBlock b = new TextBlock();
                b.Margin = new Thickness(130, ((i * 20) + 545), 0, 0);
                b.HorizontalAlignment = HorizontalAlignment.Left;
                b.TextWrapping = TextWrapping.Wrap;
                b.VerticalAlignment = VerticalAlignment.Top;
                b.Text = "Enter Operand " + (i + 1) + " :";
                textBlocks.Add(b);
                Grid.Children.Add(b);
            }
        }

        private void RemoveTestServiceInputFields()
        {
            for(int i = 0; i < textBoxes.Count; i++)
            {
                Grid.Children.Remove(textBoxes[i]);   
            }
            for (int i = 0; i < textBlocks.Count; i++)
            {
                Grid.Children.Remove(textBlocks[i]);
            }
            textBlocks.Clear();
            textBoxes.Clear();
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Clear();
            bool cannotProceed = true;
            for(int i = 0; i < textBoxes.Count; i++)
            {
                if (!textBoxes[i].Text.Equals(""))
                {
                    cannotProceed = false;
                }
            }

            if (cannotProceed)
            {
                MessageBox.Show("Operand Fields cannot be Empty.", "Warning!");
            }
            else
            {
                ServiceDescription serviceDescription = ServiceDropList.SelectedItem as ServiceDescription;
                if (serviceDescription != null)
                {
                    endpointAPI = serviceDescription.end_point_API;
                    in_data = new CalculatorInputData();
                    in_data.token = token;
                    in_data.operands = new List<int>();
                    for (int i = 0; i < serviceDescription.operands; i++)
                    {
                        try
                        {
                            int data = Int32.Parse(textBoxes[i].Text);
                            in_data.operands.Add(data);
                        }
                        catch (FormatException)
                        {
                            textBoxes[i].Clear();
                            break;
                        }
                    }

                    int timeout = 60000; //timeout only 60 seconds
                    Task<CalculatorData> task = new Task<CalculatorData>(testService);
                    task.Start();
                    ProgressBar.Visibility = Visibility.Visible;

                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                    {
                        CalculatorData out_obj = await task;
                        ProgressBar.Visibility = Visibility.Hidden;
                        if(out_obj.status.Equals("Granted")) 
                        { 
                            ResultBox.Text = out_obj.result.ToString();
                        }
                        else if(out_obj.reason.Equals("Authentication Error")) 
                        {
                            MessageBox.Show(out_obj.reason, out_obj.status);
                            RemoveTestServiceInputFields(); //Removing any existing operand boxes before creating any new ones.
                            UnfreezeLoginRegisterFunctions();
                            hideServiceTester();
                        }  
                        else { MessageBox.Show(out_obj.reason, out_obj.status); }
                    }
                    else
                    {
                        ProgressBar.Visibility = Visibility.Hidden;
                        MessageBox.Show("Search Timeout! Try Again!");
                    }
                }
                else
                {
                    ProgressBar.Visibility = Visibility.Hidden;
                    MessageBox.Show("Please select service from the drop down menu before proceeding.", "Warning!");
                }
            }
            
        }

        private CalculatorData testService()
        {
            RestClient clientTwo = new RestClient();
            RestRequest request = new RestRequest(endpointAPI, Method.Post);
            request.AddJsonBody(in_data);
            in_data = null;
            RestResponse response = clientTwo.Post(request);
            CalculatorData result = JsonConvert.DeserializeObject<CalculatorData>(response.Content);
            return result;
        }

        private async void SearchAllButton_Click(object sender, RoutedEventArgs e)
        {
            int timeout = 40000; //timeout only 40 seconds
            Task<ServiceStatus> task = new Task<ServiceStatus>(searchAll);
            task.Start();
            ProgressBar.Visibility = Visibility.Visible;

            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                ServiceStatus status_obj = await task;
                if (status_obj.status.Equals("Service Denied"))
                {
                    ProgressBar.Visibility = Visibility.Hidden;
                    MessageBox.Show(status_obj.reason, status_obj.status);
                    UnfreezeLoginRegisterFunctions();
                    hideServiceTester();
                }
                else
                {
                    ProgressBar.Visibility = Visibility.Hidden;
                    MessageBox.Show(status_obj.reason, status_obj.status);
                    setUpComboBox(status_obj.serviceDescriptions);
                    TestButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ProgressBar.Visibility = Visibility.Hidden;
                MessageBox.Show("Search Timeout! Try Again!");
            }
            
        }

        private ServiceStatus searchAll()
        {
            RestRequest request = new RestRequest("api/registryservices/allservices/{token}", Method.Get);
            request.AddUrlSegment("token", token);
            RestResponse response = client.Get(request);
            ServiceStatus result = JsonConvert.DeserializeObject<ServiceStatus>(response.Content);
            return result;  
        }

        private void setUpComboBox(List<ServiceDescription> serviceDescriptions)
        {
            /* REF : https://stackoverflow.com/questions/19071664/wpf-how-to-bind-object-to-combobox */
            ServiceDropList.ItemsSource = serviceDescriptions;
            ServiceDropList.DisplayMemberPath = "name";
        }

        private void ServiceDropList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ServiceDescription serviceDescription = ServiceDropList.SelectedItem as ServiceDescription;
            if(serviceDescription != null)
            {
                NameBox.IsReadOnly = false;
                DescBox.IsReadOnly = false;
                APIBox.IsReadOnly = false;
                NumOperandsBox.IsReadOnly = false;
                OperandTypeBox.IsReadOnly = false;
                NameBox.Text = serviceDescription.name;
                DescBox.Text = serviceDescription.description;
                APIBox.Text = serviceDescription.end_point_API;
                NumOperandsBox.Text = serviceDescription.operands.ToString();
                OperandTypeBox.Text = serviceDescription.operandType;
                RemoveTestServiceInputFields(); //Removing any existing operand boxes before creating any new ones.
                showTestServiceInputFields(serviceDescription.operands);
            }
        }

        private async void SearchNameButton_Click(object sender, RoutedEventArgs e)
        {
            service_name = ServiceNameBox.Text;

            if (service_name.Equals("")) { MessageBox.Show("Service Name Field cannot be Empty.", "Warning!"); }
            else
            {
                int timeout = 40000; //timeout only 40 seconds
                Task<ServiceStatus> task = new Task<ServiceStatus>(search);
                task.Start();
                ProgressBar.Visibility = Visibility.Visible;
                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    ServiceStatus status_obj = await task;
                    if (status_obj.status.Equals("Service Denied"))
                    {
                        ProgressBar.Visibility = Visibility.Hidden;
                        MessageBox.Show(status_obj.reason, status_obj.status);
                        UnfreezeLoginRegisterFunctions();
                        hideServiceTester();
                    }
                    else
                    {
                        ProgressBar.Visibility = Visibility.Hidden;
                        MessageBox.Show(status_obj.reason, status_obj.status);
                        setUpComboBox(status_obj.serviceDescriptions);
                        TestButton.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    ProgressBar.Visibility = Visibility.Hidden;
                    MessageBox.Show("Search All Timeout! Try Again!");
                }
            }
            
        }

        private ServiceStatus search()
        {
            RestRequest request = new RestRequest("api/registryservices/search/{searchtext}/{token}", Method.Get);
            request.AddUrlSegment("token", token);
            request.AddUrlSegment("searchtext", service_name);
            RestResponse response = client.Get(request);
            ServiceStatus result = JsonConvert.DeserializeObject<ServiceStatus>(response.Content);
            return result;
        }
    }
}