using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyFund.DataModel
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

        [DisplayName("Short description")]
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal Goal { get; set; }

        [DisplayName("Progress")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public decimal AmountGathered { get; set; }

        [DisplayName("Created on")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Updated on")]
        public DateTime? DateUpdated { get; set; }

        [DisplayName("Target date")]
        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }

        public long StatusId { get; set; }
        public long ProjectCategoryId { get; set; }

        [DisplayName("Project site")]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        public long UserId { get; set; }
        public long? AttatchmentSetId { get; set; }

        [DisplayName("Photo url")]
        [DataType(DataType.ImageUrl)]
        public string MediaUrl { get; set; }


        //Ignored in context. Check mappings of entity Project
        [DisplayName("Photo")]
        [DataType(DataType.Upload)]
        public IFormFile Media { get; set; }

        public AttatchmentSet AttatchmentSet { get; set; }

        [DisplayName("Category")]
        public ProjectCategory ProjectCategory { get; set; }

        [DisplayName("Status")]
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
