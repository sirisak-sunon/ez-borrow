using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ez.Borrow.Models
{
    public class BorrowLog
    {
        public string Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Borrower { get; set; }
        public string Witness { get; set; }
        public DateTime WitnessConfirmDate { get; set; }
        public string ReturnWitness { get; set; }
        public DateTime? ReturnDate { get; set; }
        public IEnumerable<Goods> BorrowGoods { get; set; }
    }
}
