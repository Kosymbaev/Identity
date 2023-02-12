using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MyProject.Models
{
    public class User : IdentityUser
    {
        public override string Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime Register { get; set; }
        public DateTime LastLogin { get; set; }
        public bool Selected { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

    }
}
