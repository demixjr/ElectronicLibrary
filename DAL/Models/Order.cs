using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public virtual ICollection<Book> Books { get; set; }
        public decimal TotalPrice { get; set; }

        public decimal GetTotalPrice()
        {
            decimal totalPrice = 0;
            foreach(var book in Books)
            {
                totalPrice += book.Price;
            }
            return totalPrice;
        }
    }
}
