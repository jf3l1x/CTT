using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTT.Models;
using Raven.Client.Indexes;

namespace CTT.Infrastructure.Indexes
{
    public class AllowedProjectUsers:AbstractIndexCreationTask<Project>
    {
        public AllowedProjectUsers()
        {
            Map = projects => from project in projects from user in project.AllowedUsers select new {ProjectId=project.Id,UserId= user};
        }
    }
}