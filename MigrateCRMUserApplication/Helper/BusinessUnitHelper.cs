using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateCRMUserApplication.Helper
{
    public static class BusinessUnitHelper
    {
        public static EntityCollection GetAllBusinessUnits(IOrganizationService service)
        {
            QueryExpression exp = new QueryExpression("businessunit");
            exp.ColumnSet = new ColumnSet(true);

            var records = service.RetrieveMultiple(exp);
            return records;
        }

        public static EntityCollection GetBusinessUnitByName(string name, IOrganizationService service)
        {
            ConditionExpression cnd = new ConditionExpression();
            cnd.AttributeName = "name";
            cnd.Operator = ConditionOperator.Equal;
            cnd.Values.Add(name);


            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(cnd);

            QueryExpression exp = new QueryExpression("businessunit");
            exp.ColumnSet = new ColumnSet(true);
            exp.Criteria.AddFilter(filter);

            var records = service.RetrieveMultiple(exp);
            return records;
        }

        public static void CreateBusinessUnits(Entity ent, IOrganizationService service)
        {
            try
            {
                var newBusinessUnit = new Entity("businessunit");
                newBusinessUnit.Id = ent.Id;
                newBusinessUnit["name"] = ent["name"];
                newBusinessUnit["parentbusinessunitid"] = new EntityReference() { Id = new Guid("84087952-945B-E911-80DC-FA163E10693A"),LogicalName="businessunit" };

                service.Create(newBusinessUnit);
            }
            catch (Exception e)
            {

                throw;
            }

        }
    }
}
