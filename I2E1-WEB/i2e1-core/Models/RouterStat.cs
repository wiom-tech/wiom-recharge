using System;

namespace i2e1_core.Models
{
    [Serializable]
	public  class RouterStat: UserBaseModel
    {
        public string macId {get;set;}
        public int hour { get; set; }
        public int day { get; set; }
        public int count { get; set; }
    }

    [Serializable]
	public  class RouterStatCompressed: UserBaseModel
    {
        public string key { get; set; }

        public int count { get; set; }
    }
}