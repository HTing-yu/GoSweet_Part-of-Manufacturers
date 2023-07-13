using System.Security.AccessControl;

namespace Officel.Models
{
    public class RevenuepageModels
    {
    }


    //Json用類別
    public class RevenueSearch {

        private string _RevenueSearchDate = "";
        private string _RevenueTotal = "";

        public string orderDate
        {
            set { _RevenueSearchDate = Convert.ToDateTime(value).ToString("yyyy-MM-dd"); }
            get { return _RevenueSearchDate;}
        }

        public string name { get; set; }

        public string categories { get; set; }

        public int id { get; set; }

        public int quentity { get; set; }

        public decimal price { get; set; }

        public decimal total { get; set; }
    }
}
