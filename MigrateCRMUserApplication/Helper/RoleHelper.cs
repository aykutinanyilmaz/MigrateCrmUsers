using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateCRMUserApplication.Helper
{
    public static class RoleHelper
    {
        public static Entity GetRoleById(Guid roleId,IOrganizationService service)
        {
            ConditionExpression cnd = new ConditionExpression();
            cnd.AttributeName = "roleid";
            cnd.Operator = ConditionOperator.Equal;
            cnd.Values.Add(roleId);


            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(cnd);

            QueryExpression exp = new QueryExpression("role");
            exp.ColumnSet = new ColumnSet(true);
            exp.Criteria.AddFilter(filter);

            var records = service.RetrieveMultiple(exp).Entities.FirstOrDefault();
            return records;
        }
        public static Entity GetRoleByName(string name,Guid BusinessUnitId, IOrganizationService service)
        {

            var fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='role'>
                                <attribute name='name' />
                                <attribute name='businessunitid' />
                                <attribute name='roleid' />
                                <order attribute='name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='businessunitid' operator='eq' value='{BusinessUnitId}' />
                                  <condition attribute='name' operator='eq' value='{name}' />
                                </filter>
                              </entity>
                            </fetch>";
            

            var records = service.RetrieveMultiple(new FetchExpression(fetch)).Entities.FirstOrDefault();
            return records;
        }
    }
}
