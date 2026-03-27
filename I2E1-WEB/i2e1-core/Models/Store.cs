using i2e1_basics.Utilities;

namespace i2e1_core.Models
{
    public class Store
    {
        public LongIdInfo nasid { get; set; }

        public int controllerid { get; set; }

        public string storeName { get; set; }

        public string storeNameAlias { get; set; }

        public string address { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string category { get; set; }

        public string contactPerson { get; set; }

        public string contactNumber { get; set; }

        public string storeImg { get; set; }

        public string googleCategories { get; set; }

        public string googlePlaceId { get; set; }
        public string zohoTicketId { get; set; }

    }
}
