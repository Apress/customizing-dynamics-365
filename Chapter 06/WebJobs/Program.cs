using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using SBMA.ServiceProcessor.BusinessProcessLayer;
using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.ServiceModel.Description;

namespace SBMA.ServiceProcessor
{
    public class Program
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public static void Main(string[] args)
        {
            ExecuteLogic();
            Console.ReadLine();
        }

        /// <summary>
        /// Connecting to Dynamics 365 instance
        /// </summary>
        public static void ExecuteLogic()
        {
            try
            {
                log.Info("Connecting to Dyamics 365 instance...");

                CrmServiceClient crmConnection = new CrmServiceClient(ConfigurationManager.ConnectionStrings["Xrm"].ConnectionString);
                log.Info(crmConnection.IsReady);
                if (crmConnection.IsReady)
                {
                    log.Info("Application connected to server successfully.");
                }

                IOrganizationService service = (IOrganizationService)crmConnection;
                CrmServiceContext context = new CrmServiceContext(service);

                if (service != null)
                {
                    Guid orgId = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).OrganizationId;
                    if (orgId != Guid.Empty)
                    {
                        log.Info("Connection established successfully.");
                    }

                    // Call the process to create the 
                    SubscriptionProcessor subscriptionProcessor = new SubscriptionProcessor();
                    subscriptionProcessor.CreateRenewalSubscriptionProcess(service);
                }
                else
                {
                    Console.WriteLine("Connection failed...");
                }
            }
            catch (Exception ex)
            {
                log.Error("EXCEPTION: ", ex);
            }
        }

    }
}
