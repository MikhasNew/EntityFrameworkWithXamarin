
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkWithXamarin.Core
{
   public class Reguest
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string RGSt { get; set; }
        public List<TableItem> TdItems { get; set; }

    }
}
