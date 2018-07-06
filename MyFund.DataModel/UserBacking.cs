using System;
using System.Collections.Generic;

namespace MyFund.DataModel
{
    public partial class UserBacking
    {
        public long UserId { get; set; }
        public long BackingId { get; set; }
        public long? TransactionId { get; set; }
        public decimal Amount { get; set; }

        public BackingPackage Backing { get; set; }
        public User User { get; set; }
    }
}
