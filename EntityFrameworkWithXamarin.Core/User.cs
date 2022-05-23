
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EntityFrameworkWithXamarin.Core
{
        public class User 
        {
        //public int Id { get; set; }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public  List<Reguest> Reguests { get; set; }
        public  List<TableItem> tbItems { get; set; }

       /* public User() {  }
        public User(long userId) { UserId = userId; Reguests = new List<Reguest>(); tbItems = new List<TableItem>(); }
        public User(long userId, string name="" ) 
        { UserId = userId; Name = name; Reguests = new List<Reguest>(); tbItems = new List<TableItem>();  }*/


    }
    
}
