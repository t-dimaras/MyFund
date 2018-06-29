using System;
using System.Collections.Generic;

namespace MyFund.Model
{
    public partial class AttatchmentSet
    {
        public AttatchmentSet()
        {
            Attatchments = new HashSet<Attatchment>();
            BackingPackage = new HashSet<BackingPackage>();
            Project = new HashSet<Project>();
            ProjectUpdate = new HashSet<ProjectUpdate>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<Attatchment> Attatchments { get; set; }
        public ICollection<BackingPackage> BackingPackage { get; set; }
        public ICollection<Project> Project { get; set; }
        public ICollection<ProjectUpdate> ProjectUpdate { get; set; }
    }
}
