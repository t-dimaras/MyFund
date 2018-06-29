using System;
using System.Collections.Generic;

namespace MyFund.Model
{
    public partial class BackingPackage : IResource
    {
        public BackingPackage()
        {
            UserBackings = new HashSet<UserBacking>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string PackageDescription { get; set; }
        public decimal BackingAmount { get; set; }
        public string RewardDescription { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public long ProjectId { get; set; }
        public long? AttatchmentSetId { get; set; }

        public AttatchmentSet AttatchmentSet { get; set; }
        public Project Project { get; set; }
        public ICollection<UserBacking> UserBackings { get; set; }

        public long GetResourceOwnerId()
        {
            return Project.GetResourceOwnerId();
        }
    }
}
