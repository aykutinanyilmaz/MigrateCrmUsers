using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateCRMUserApplication.Helper
{
    public static class UserHelper
    {
        public static Entity GetUser(Guid userId, IOrganizationService service)
        {
            ConditionExpression cnd = new ConditionExpression();
            cnd.AttributeName = "systemuserid";
            cnd.Operator = ConditionOperator.Equal;
            cnd.Values.Add(userId);


            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(cnd);

            QueryExpression exp = new QueryExpression("systemuser");
            exp.ColumnSet = new ColumnSet(true);
            exp.Criteria.AddFilter(filter);

            var records = service.RetrieveMultiple(exp).Entities.FirstOrDefault();
            return records;
        }

        public static EntityCollection GetUserRoles(Guid guidUserID, IOrganizationService service)
        {

            QueryExpression rolesQE = new QueryExpression
            {
                EntityName = "role",
                ColumnSet = new ColumnSet("name"),
                LinkEntities = {
                                new LinkEntity
                                {
                                    LinkFromEntityName = "role",
                                    LinkFromAttributeName = "roleid",
                                    LinkToEntityName = "systemuserroles",
                                    LinkToAttributeName = "roleid",
                                    LinkCriteria = new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.And,
                                            Conditions =
                                            {
                                                new ConditionExpression
                                                {
                                                    AttributeName = "systemuserid",
                                                    Operator = ConditionOperator.Equal,
                                                    Values = { guidUserID }
                                                }
                                            }
                                    }
                                }
                            }
            };

            var records = service.RetrieveMultiple(rolesQE);
            return records;

        }

        public static Guid CreateOrGetUser(Entity ent, Guid businessUnitId, IOrganizationService service)
        {
            var isExistUser = GetUserByDomainName(ent.GetAttributeValue<string>("domainname"), service);
            if (isExistUser ==null)
            {
                Entity newSystemUser = new Entity("systemuser");
                newSystemUser.Id = ent.Id;
                newSystemUser["firstname"] = ent["firstname"];
                newSystemUser["lastname"] = ent["lastname"];
                newSystemUser["fullname"] = ent["fullname"];
                newSystemUser["domainname"] = ent["domainname"];
                newSystemUser["businessunitid"] = new EntityReference() { Id = businessUnitId, LogicalName = "businessunit" };

                return service.Create(newSystemUser);
            }

            return isExistUser.Id;
        }

        public static void AssociateUserRole(Guid userId,Guid roleId,IOrganizationService service)
        {
            service.Associate("systemuser",userId,new Relationship("systemuserroles_association"), new EntityReferenceCollection() { new EntityReference("role", roleId) });
        }

        public static Entity GetUserByDomainName(string domainName, IOrganizationService service)
        {
            ConditionExpression cnd = new ConditionExpression();
            cnd.AttributeName = "domainname";
            cnd.Operator = ConditionOperator.Equal;
            cnd.Values.Add(domainName);


            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(cnd);

            QueryExpression exp = new QueryExpression("systemuser");
            exp.ColumnSet = new ColumnSet(true);
            exp.Criteria.AddFilter(filter);

            var records = service.RetrieveMultiple(exp).Entities.FirstOrDefault();
            return records;
        }

    }
}
