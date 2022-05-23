using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkWithXamarin.Core
{
   // [Serializable]
    public class TableItem
    {
        public int Id { get; set; }
       // public User User { get; set; }
       // public int UserId { get; set; }
        public Reguest Reguest { get; set; }
        public int ReguestId { get; set; }
        
        public string A { get; set; }
        public string Txt { get; set; }
        public string Cost { get; set; }
        public string Quele { get; set; }
        public string Locacion { get; set; }
        public string Url { get; set; }

        public TableItem() { }
        public TableItem(string a, string txt, string cost, string quele, string location, string url)
        { A = a; Txt = txt; Cost = cost; Quele = quele; Locacion = location; Url = url; }
        public TableItem(string a, string txt, string cost, string quele, string location)
        { A = a; Txt = txt; Cost = cost; Quele = quele; Locacion = location; }
        public TableItem(string a, string txt, string cost, string location)
        { A = a; Txt = txt; Cost = cost; Quele = ""; Locacion = location; }
        public TableItem(string a, string txt, string cost)
        { A = a; Txt = txt; Cost = cost; Quele = ""; Locacion = ""; Url = ""; }
    }
}
