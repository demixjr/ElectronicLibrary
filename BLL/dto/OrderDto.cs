using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public virtual ICollection<BookDto>? Books { get; set; }
    }
}
