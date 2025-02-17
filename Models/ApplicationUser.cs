using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;

namespace BatDongSan_api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
