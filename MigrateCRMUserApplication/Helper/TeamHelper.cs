using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateCRMUserApplication.Helper
{
    public static class TeamHelper
    {
        public static EntityCollection GetAllTeams(IOrganizationService service)
        {
            QueryExpression exp = new QueryExpression("team");
            exp.ColumnSet = new ColumnSet(true);

            var records = service.RetrieveMultiple(exp);
            return records;
        }

        public static Entity GetTeamByName(string name ,IOrganizationService service)
        {
            ConditionExpression cnd = new ConditionExpression();
            cnd.AttributeName = "name";
            cnd.Operator = ConditionOperator.Equal;
            cnd.Values.Add(name);


            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(cnd);

            QueryExpression exp = new QueryExpression("team");
            exp.ColumnSet = new ColumnSet(true);
            exp.Criteria.AddFilter(filter);

            var records = service.RetrieveMultiple(exp).Entities.FirstOrDefault();
            return records;
        }

        public static EntityCollection GetTeamByBusinessUnit(Guid businessUnitId, IOrganizationService service)
        {

            ConditionExpression cnd = new ConditionExpression();
            cnd.AttributeName = "businessunitid";
            cnd.Operator = ConditionOperator.Equal;
            cnd.Values.Add(businessUnitId);

            ConditionExpression cnd2 = new ConditionExpression();
            cnd2.AttributeName = "isdefault";
            cnd2.Operator = ConditionOperator.Equal;
            cnd2.Values.Add(false);

            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            filter.Conditions.Add(cnd);
            filter.Conditions.Add(cnd2);

            


            QueryExpression exp = new QueryExpression("team");
            exp.ColumnSet = new ColumnSet(true);
            exp.Criteria.AddFilter(filter);


            var records = service.RetrieveMultiple(exp);
            return records;
        }

        public static EntityCollection GetTeamMembers(Guid teamId, IOrganizationService service)
        {
            ConditionExpression cnd = new ConditionExpression();
            cnd.AttributeName = "teamid";
            cnd.Operator = ConditionOperator.Equal;
            cnd.Values.Add(teamId);


            FilterExpression filter = new FilterExpression();
            filter.Conditions.Add(cnd);

            QueryExpression exp = new QueryExpression("teammembership");
            exp.ColumnSet = new ColumnSet(true);
            exp.Criteria.AddFilter(filter);

            var records = service.RetrieveMultiple(exp);
            return records;
        }

        public static Guid CreateTeam(Entity ent, Guid businessUnitId,IOrganizationService service)
        {
            Entity newBusinessUnitTeam = new Entity("team");
            newBusinessUnitTeam.Id = ent.Id;
            newBusinessUnitTeam["name"] = ent.GetAttributeValue<string>("name");
            newBusinessUnitTeam["businessunitid"] = new EntityReference() { Id = businessUnitId, LogicalName = "businessunit" };
            newBusinessUnitTeam["administratorid"] = new EntityReference() { Id = new Guid("EB4DA98A-945B-E911-80DC-FA163E10693A"), LogicalName = "systemuser" };


            return service.Create(newBusinessUnitTeam);
        }

        public static EntityCollection GetTeamRoles(Guid teamId, IOrganizationService service)
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
                                    LinkToEntityName = "teamroles",
                                    LinkToAttributeName = "roleid",
                                    LinkCriteria = new FilterExpression
                                    {
                                        FilterOperator = LogicalOperator.And,
                                            Conditions =
                                            {
                                                new ConditionExpression
                                                {
                                                    AttributeName = "teamid",
                                                    Operator = ConditionOperator.Equal,
                                                    Values = { teamId }
                                                }
                                            }
                                    }
                                }
                            }
            };

            var records = service.RetrieveMultiple(rolesQE);
            return records;

        }

        public static void AddTeamMember(Guid teamId,Guid userId,IOrganizationService service)
        {

            var addToTeamRequest = new AddMembersTeamRequest
            {
                TeamId = teamId,
                MemberIds = new[] { userId }
            };
            service.Execute(addToTeamRequest);
        }

        public static void AssociateTeamRole(Guid teamId, Guid roleId, IOrganizationService service)
        {
            service.Associate("team", teamId, new Relationship("teamroles_association"), new EntityReferenceCollection() { new EntityReference("role", roleId) });
        }
    }
}
