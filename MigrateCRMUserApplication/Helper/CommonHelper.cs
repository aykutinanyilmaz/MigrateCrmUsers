using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateCRMUserApplication.Helper
{
    public static class CommonHelper
    {
        public static void TeamMemberProcess(Guid teamId,Guid businessUnitId,IOrganizationService mDevService,IOrganizationService upliftService)
        {
            var teamMembers = TeamHelper.GetTeamMembers(teamId, mDevService);

            foreach (var teamMember in teamMembers.Entities)
            {
                var systemUserId = teamMember.GetAttributeValue<Guid>("systemuserid");
                var user = UserHelper.GetUser(systemUserId, mDevService);
                var userRoles = UserHelper.GetUserRoles(user.Id,mDevService);

                var newUserId = UserHelper.CreateOrGetUser(user, businessUnitId, upliftService);
                TeamHelper.AddTeamMember(teamId, newUserId, upliftService);

                foreach (var userRole in userRoles.Entities)
                {
                    try
                    {
                        var roleId = userRole.GetAttributeValue<Guid>("roleid");
                        var role = RoleHelper.GetRoleById(roleId, mDevService);
                        var roleByNameUplift = RoleHelper.GetRoleByName(role.GetAttributeValue<string>("name"), user.GetAttributeValue<EntityReference>("businessunitid").Id, upliftService);
                        UserHelper.AssociateUserRole(newUserId, roleByNameUplift.Id, upliftService);
                    }
                    catch
                    {


                    }

                }
            }
        }

        public static void TeamRoleProcess(Guid teamId,Guid businessUnitId,IOrganizationService mDevService,IOrganizationService upliftService)
        {
            try
            {
                var teamRoles = TeamHelper.GetTeamRoles(teamId, mDevService);

                foreach (var teamRole in teamRoles?.Entities)
                {
                    var roleId = teamRole.GetAttributeValue<Guid>("roleid");
                    var role = RoleHelper.GetRoleById(roleId, mDevService);
                    var roleByNameUplift = RoleHelper.GetRoleByName(role.GetAttributeValue<string>("name"), businessUnitId, upliftService);
                    TeamHelper.AssociateTeamRole(teamId, roleByNameUplift.Id, upliftService);
                }
            }
            catch
            {
            }
          
        }
    }
}
