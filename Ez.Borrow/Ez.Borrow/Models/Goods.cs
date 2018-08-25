using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ez.Borrow.Models
{
    public class Goods
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Position { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime? LastEditDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string DeleteBy { get; set; }
        public DateTime? LastMoveDate { get; set; }
        public string MoveBy { get; set; }
    }
}
