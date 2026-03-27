using i2e1_basics.Models;
using i2e1_core.Models.Client;
using System;

namespace i2e1_core.Models
{
    [Serializable]
    public class RouterSearchQuery : SearchQuery
    {
        public RouterState installState { get; set; }
    }
}