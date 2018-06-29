using System;
using System.Collections.Generic;

namespace MyFund.Model
{
    public partial class ProjectCategory
    {
        public ProjectCategory()
        {
            Project = new HashSet<Project>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Project> Project { get; set; }
    }
}
