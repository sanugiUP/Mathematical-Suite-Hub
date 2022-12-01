using Authenticator;
using Newtonsoft.Json;
using RegistryAPIClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web.Http;

namespace Registry.Controllers
{
    [RoutePrefix("api/registryservices")]
    public class RegistryServicesController : ApiController
    {
        private static ChannelFactory<AuthenticatorInterface> foobFactory = new ChannelFactory<AuthenticatorInterface>(new NetTcpBinding(), "net.tcp://localhost:8100/AuthenticationService");
        private static AuthenticatorInterface foob = foobFactory.CreateChannel();
        /* REF : https://stackoverflow.com/questions/54155392/why-is-environment-currentdirectory-set-to-c-program-files-iis-express*/
        private static string registryDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RegistryData.txt");
        
        
        [Route("publish/{token}")]
        [Route("publish")]
        [HttpPost]
        public ServiceStatus Publish([FromBody]ServiceDescription serviceDescription, int token)
        {
            ServiceStatus publish_Status = new ServiceStatus();
            string validatedResult = foob.Validate(token);

            if (validatedResult.Equals("Validated"))
            {
                /* REF : https://code-maze.com/csharp-write-json-into-a-file/ */
                /* REF : https://stackoverflow.com/questions/16921652/how-to-write-a-json-file-in-c*/
                /* REF : https://stackoverflow.com/questions/33081102/json-add-new-object-to-existing-json-file-c-sharp*/

                var fileList = File.ReadAllText(registryDataPath);

                var servicesDescList = JsonConvert.DeserializeObject<List<ServiceDescription>>(fileList);

                if(servicesDescList == null)
                {
                    //Initializing servicesDescList when creating the RegistryData.txt for the first time before adding data
                    servicesDescList = new List<ServiceDescription>();
                }

                servicesDescList.Add(serviceDescription);
                var converted_serarch_description = JsonConvert.SerializeObject(servicesDescList, Formatting.Indented);
                File.WriteAllText(registryDataPath, converted_serarch_description); // Overwrites the file

                publish_Status.status = "Service Complete";
                publish_Status.reason = "Publish Successful";
                publish_Status.serviceDescriptions = new List<ServiceDescription>();
            }
            else
            {
                publish_Status.status = "Service Denied";
                publish_Status.reason = "Authentication Error";
                publish_Status.serviceDescriptions = new List<ServiceDescription>();
            }

            return publish_Status;
        }



        [Route("search/{searchtext}/{token}")]
        [HttpGet]
        public ServiceStatus Search(string searchText, int token)
        {
            ServiceStatus search_Status = new ServiceStatus();
            List<ServiceDescription> searchedResults = new List<ServiceDescription>();
            
            string validatedResult = foob.Validate(token);

            if (validatedResult.Equals("Validated"))
            {
                var fileList = File.ReadAllText(registryDataPath);
                var serviceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(fileList);

                for (int i = 0; i < serviceDescriptions.Count<ServiceDescription>(); i++)
                {
                    if (serviceDescriptions[i].name.ToString().ToLower().Contains(searchText.ToLower()))
                    {
                        searchedResults.Add(serviceDescriptions[i]);
                    }
                }

                search_Status.status = "Service Complete";
                search_Status.reason = "Search Successful";
                search_Status.serviceDescriptions = searchedResults;

            }
            else
            {
                search_Status.status = "Service Denied";
                search_Status.reason = "Authentication Error";
                search_Status.serviceDescriptions = searchedResults;
            }
            return search_Status;
        }



        [Route("allservices/{token}")]
        [Route("allservices")]
        [HttpGet]
        public ServiceStatus AllServices(int token)
        {
            ServiceStatus all_service_Status = new ServiceStatus();
            string validatedResult = foob.Validate(token);

            if (validatedResult.Equals("Validated"))
            {
                var fileList = File.ReadAllText(registryDataPath);
                var serviceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(fileList);

                all_service_Status.status = "Service Complete";
                all_service_Status.reason = "All Services Search Successful";
                all_service_Status.serviceDescriptions = serviceDescriptions;
            }
            else
            {
                all_service_Status.status = "Service Denied";
                all_service_Status.reason = "Authentication Error";
                all_service_Status.serviceDescriptions = null;
            }
            return all_service_Status;
        }



        [Route("unpublish/{token}")]
        [Route("unpublish")]
        [HttpPost]
        public ServiceStatus Unpublish([FromBody]URLInput endPoint, int token)
        {
            List<ServiceDescription> modifiedServices = new List<ServiceDescription>();
            ServiceStatus unpublish_Status = new ServiceStatus();
            string validatedResult = foob.Validate(token);

            if (validatedResult.Equals("Validated"))
            {
                var fileList = File.ReadAllText(registryDataPath);
                var serviceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(fileList);

                for (int i = 0; i < serviceDescriptions.Count<ServiceDescription>(); i++)
                {
                    if (!serviceDescriptions[i].end_point_API.ToString().Equals(endPoint.end_point_API))
                    {
                        modifiedServices.Add(serviceDescriptions[i]);
                    }
                }

                // Overwrite the RegistryData.txt file after unpublishing services
                var modified_descriptions = JsonConvert.SerializeObject(modifiedServices, Formatting.Indented);
                File.WriteAllText(registryDataPath, modified_descriptions);

                unpublish_Status.status = "Service Complete";
                unpublish_Status.reason = "Unpublish Successful";
                unpublish_Status.serviceDescriptions = modifiedServices;
            }
            else
            {
                unpublish_Status.status = "Service Denied";
                unpublish_Status.reason = "Authentication Error";
                unpublish_Status.serviceDescriptions = modifiedServices;
            }
            return unpublish_Status;
        }

    }
}
