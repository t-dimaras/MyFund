using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyFund.DataModel
{
    public partial class User: IdentityUser<long>
    {
        public User():base()
        {
            Projects = new HashSet<Project>();
            UserBackings = new HashSet<UserBacking>();
        }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        public string Url { get; set; }
        public string Organization { get; set; }
        public string Description { get; set; }

        public ICollection<Project> Projects { get; set; }
        public ICollection<UserBacking> UserBackings { get; set; }
    }
}
