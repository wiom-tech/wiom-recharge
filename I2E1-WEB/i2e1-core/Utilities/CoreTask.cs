using i2e1_basics.Utilities;
using System;
using System.Threading.Tasks;

namespace i2e1_core.Utilities
{
    public class CoreTask
    {
        public static Task Run(string name, Action action)
        {
            return Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Logger.GetInstance().Error($"Error Executing Task:{name}, Exception:{ex}");
                }
            });
        }
    }
}
