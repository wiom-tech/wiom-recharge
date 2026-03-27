using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i2e1_core.Interfaces
{
    public interface IDelivery
    {
        public void FindWho();
        public void StoreMessage();
        public void SendMessage();
    }
}
