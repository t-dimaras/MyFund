using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyFund.Model
{
    public partial class Project : IResource
    {
        public Project()
        {
            BackingPackages = new HashSet<BackingPackage>();
            ProjectUpdates = new HashSet<ProjectUpdate>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public decimal Goal { get; set; }
        public decimal AmountGathered { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }

        public long StatusId { get; set; }
        public long ProjectCategoryId { get; set; }
        public string Url { get; set; }
        public long UserId { get; set; }
        public long? AttatchmentSetId { get; set; }
        public string MediaUrl { get; set; }

        public AttatchmentSet AttatchmentSet { get; set; }
        public ProjectCategory ProjectCategory { get; set; }
        public Status Status { get; set; }
        public User User { get; set; }
        public ICollection<BackingPackage> BackingPackages { get; set; }
        public ICollection<ProjectUpdate> ProjectUpdates { get; set; }

        public long GetResourceOwnerId()
        {
            return UserId;
        }
    }
}
