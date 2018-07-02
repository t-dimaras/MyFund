using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public string PackageDescription { get; set; }

        [DisplayName("Pledge")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public decimal BackingAmount { get; set; }

        [DisplayName("Benefits")]
        [DataType(DataType.MultilineText)]
        public string RewardDescription { get; set; }

        [DisplayName("Created on")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Updated on")]
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
