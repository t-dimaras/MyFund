using System;
using System.Collections.Generic;

namespace MyFund.DataModel
{
    public partial class Attatchment
    {
        public long Id { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public long AttatchmentSetId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public AttatchmentSet AttatchmentSet { get; set; }
    }
}
