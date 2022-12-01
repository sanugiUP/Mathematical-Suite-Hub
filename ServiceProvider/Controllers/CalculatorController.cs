using System.Web.Http;
using ServiceProviderGUIClasses;
using Authenticator;
using System.ServiceModel;
using System.Windows;

namespace ServiceProvider.Controllers
{
    [RoutePrefix("api/calculator")]
    public class CalculatorController : ApiController
    {
        private static ChannelFactory<AuthenticatorInterface> foobFactory = new ChannelFactory<AuthenticatorInterface>(new NetTcpBinding(), "net.tcp://localhost:8100/AuthenticationService");
        private AuthenticatorInterface foob = foobFactory.CreateChannel();

        [Route("addtwo")]
        [HttpPost]
        public CalculatorData AddTwo([FromBody] CalculatorInputData input)
        {
            CalculatorData data = new CalculatorData();
            string isValid = foob.Validate(input.token);
            if (isValid.Equals("Validated") && input.operands.Count == 2)
            {
                data.result = input.operands[0] + input.operands[1];
                data.status = "Granted";
                data.reason = "Authentication Success";
            }
            else if(isValid.Equals("Not Validated"))
            {
                data.status = "Denied";
                data.reason = "Authentication Error";
            }
            else
            {
                data.status = "Input Error";
                data.reason = "An Error Occured! Please Check Input";
            }
            return data;
        }

        [Route("addthree")]
        [HttpPost]
        public CalculatorData AddThree([FromBody] CalculatorInputData input)
        {
            CalculatorData data = new CalculatorData();
            string isValid = foob.Validate(input.token);
            if (isValid.Equals("Validated") && input.operands.Count == 3)
            {
                data.result = input.operands[0] + input.operands[1] + input.operands[2];
                data.status = "Granted";
                data.reason = "Authentication Success";
            }
            else if (isValid.Equals("Not Validated"))
            {
                data.status = "Denied";
                data.reason = "Authentication Error";
            }
            else
            {
                data.status = "Input Error";
                data.reason = "An Error Occured! Please Check Input";
            }
            return data;
        }


        [Route("multwo")]
        [HttpPost]
        public CalculatorData MulTwo([FromBody] CalculatorInputData input)
        {
            CalculatorData data = new CalculatorData();
            string isValid = foob.Validate(input.token);
            if (isValid.Equals("Validated") && input.operands.Count == 2)
            {
                data.result = input.operands[0] * input.operands[1];
                data.status = "Granted";
                data.reason = "Authentication Success";
            }
            else if (isValid.Equals("Not Validated"))
            {
                data.status = "Denied";
                data.reason = "Authentication Error";
            }
            else
            {
                data.status = "Input Error";
                data.reason = "An Error Occured! Please Check Input";
            }
            return data;
        }

        
        [Route("multhree")]
        [HttpPost]
        public CalculatorData MulThree([FromBody] CalculatorInputData input)
        {
            CalculatorData data = new CalculatorData();
            string isValid = foob.Validate(input.token);
            if (isValid.Equals("Validated") && input.operands.Count == 3)
            {
                data.result = input.operands[0] * input.operands[1] * input.operands[2];
                data.status = "Granted";
                data.reason = "Authentication Success";
            }
            else if (isValid.Equals("Not Validated"))
            {
                data.status = "Denied";
                data.reason = "Authentication Error";
            }
            else
            {
                data.status = "Input Error";
                data.reason = "An Error Occured! Please Check Input";
            }
            return data;
        }
    }
}

/* REF : https://www.javatpoint.com/prime-number-program-in-csharp */
