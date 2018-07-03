using System;
using System.Collections.Generic;

namespace MyFund.DataModel
{
    public partial class Status
    {
        public Status()
        {
            Project = new HashSet<Project>();
            ProjectUpdate = new HashSet<ProjectUpdate>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Project> Project { get; set; }
        public ICollection<ProjectUpdate> ProjectUpdate { get; set; }

        public enum StatusDescription : long
        {
            Inactive = 1,
            Active = 2,
            Success = 3,
            Fail =4
        }
    }
}
