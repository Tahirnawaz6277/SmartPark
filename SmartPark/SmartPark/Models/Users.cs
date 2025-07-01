using Microsoft.AspNetCore.Identity;

namespace SmartPark.Models
{
    public class Users : IdentityUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CNIC { get; set; }
        public string Role { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
 
    }
}
