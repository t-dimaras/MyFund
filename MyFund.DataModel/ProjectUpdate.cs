using System;
using System.Collections.Generic;

namespace MyFund.DataModel
{
    public partial class ProjectUpdate
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public long? AttatchmentSetId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public long StatusId { get; set; }

        public AttatchmentSet AttatchmentSet { get; set; }
        public Project Project { get; set; }
        public Status Status { get; set; }
    }
}
