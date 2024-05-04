﻿using System.Collections.Generic;

namespace WebsiteBook.Models
{
    public class UserWithRolesModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
