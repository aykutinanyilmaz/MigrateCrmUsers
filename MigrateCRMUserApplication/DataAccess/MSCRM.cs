using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace MigrateCRMUserApplication.DataAccess
{
    public static class MSCRM
    {

        public static IOrganizationService GetService(string userName,string password, string domainName,string organizationServiceUrl)
        {

            ClientCredentials authCredentials = new ClientCredentials();
            authCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(userName, password, domainName);
            var orgServiceProxy = new OrganizationServiceProxy(new Uri(organizationServiceUrl), null, authCredentials, null);
            orgServiceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
            orgServiceProxy.EnableProxyTypes();
            orgServiceProxy.ServiceConfiguration.CurrentServiceEndpoint.Binding.SendTimeout = new TimeSpan(24, 0, 0);
            orgServiceProxy.ServiceConfiguration.CurrentServiceEndpoint.Binding.OpenTimeout = new TimeSpan(24, 0, 0);
            orgServiceProxy.ServiceConfiguration.CurrentServiceEndpoint.Binding.CloseTimeout = new TimeSpan(24, 0, 0);
            orgServiceProxy.Timeout = new TimeSpan(0, 30, 0);
            return orgServiceProxy;
        }
    }
}
