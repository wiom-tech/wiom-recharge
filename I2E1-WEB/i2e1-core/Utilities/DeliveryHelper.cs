using i2e1_core.Interfaces;
using i2e1_core.Models;

namespace i2e1_core.Utilities
{
    public abstract class DeliveryHelper : IDelivery
    {
        protected DeliveryMsg msg;

        public abstract void FindWho();

        public abstract void SendMessage();

        public virtual void StoreMessage()
        {
            DeliveryUtility.StoreMessages(msg);
        }

        private void ProcessMessage()
        {
            this.FindWho();
            this.StoreMessage();
            this.SendMessage();
        }

        public virtual void onMessageReceived()
        {
            this.ProcessMessage();
        }

        public abstract void handleResponse();
    }
}
