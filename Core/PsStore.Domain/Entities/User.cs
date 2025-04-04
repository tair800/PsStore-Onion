﻿using Microsoft.AspNetCore.Identity;

namespace PsStore.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Basket> Baskets { get; set; }
    }
}
