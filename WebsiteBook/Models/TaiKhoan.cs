using Microsoft.Data.SqlClient;

namespace WebsiteBook.Models
{
    public class TaiKhoan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        //public bool isBlocked { get; set; }
    }
}
