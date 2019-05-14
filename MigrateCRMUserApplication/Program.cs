using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MigrateCRMUserApplication.DataAccess;
using MigrateCRMUserApplication.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrateCRMUserApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var mDevServiceUrl = "http://100.126.0.19/TCRMDEV/XRMServices/2011/Organization.svc";
            var mDevService = MSCRM.GetService("eseozde", "Ericsson2020", "comcel", mDevServiceUrl);

            var upliftServiceUrl = "http://100.126.0.217:5555/TCRMDEV2/XRMServices/2011/Organization.svc";
            var upliftService = MSCRM.GetService("TCRMINSDR5", "ClaroFULL2017**", "comcel", upliftServiceUrl);

            var businessUnits = BusinessUnitHelper.GetAllBusinessUnits(mDevService);


            foreach (var businessUnit in businessUnits?.Entities)
            {
                var businessUnitName = businessUnit.GetAttributeValue<string>("name");

                var businessUnitTeams = TeamHelper.GetTeamByBusinessUnit(businessUnit.Id, mDevService);
                foreach (var businessUnitTeam in businessUnitTeams?.Entities)
                {
                    var businessUnitTeamName = businessUnitTeam.GetAttributeValue<string>("name");
                    var checkTeam = TeamHelper.GetTeamByName(businessUnitTeamName, upliftService);

                    if (checkTeam == null)
                    {
                        var newTeamId = TeamHelper.CreateTeam(businessUnitTeam, businessUnit.Id, upliftService);
                        CommonHelper.TeamRoleProcess(newTeamId, businessUnit.Id,mDevService, upliftService);
                        CommonHelper.TeamMemberProcess(businessUnitTeam.Id, businessUnit.Id, mDevService, upliftService);
                    }
                    else
                    {
                        CommonHelper.TeamRoleProcess(checkTeam.Id, businessUnit.Id, mDevService, upliftService);
                        CommonHelper.TeamMemberProcess(checkTeam.Id, businessUnit.Id, mDevService, upliftService);
                    }
                }
            }
        }
    }


}
