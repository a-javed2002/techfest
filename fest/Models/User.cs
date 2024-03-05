using System;
using System.Collections.Generic;

namespace fest.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Passwords { get; set; } = null!;
        public string? Images { get; set; }
        public int? RoleIdfk { get; set; }

        public virtual Role? RoleIdfkNavigation { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
