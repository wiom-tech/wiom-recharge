using i2e1_basics.Utilities;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace i2e1_core.QueueHandler
{
	public enum ServiceBusQueue
	{
		payment,
		wallet,
		plan_start_check,
		ticket,
		isp_integration
	}

	public class Worker : BackgroundService
    {
        private readonly List<IQueueHandler> _queueHandlers;

        public Worker(IEnumerable<IQueueHandler> sqsHandlers)
        {
            _queueHandlers = new List<IQueueHandler>(sqsHandlers);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var taskList = new List<Task>();
            foreach (var handler in _queueHandlers)
            {
                taskList.Add(handler.Register());
                Console.WriteLine(handler.GetType() + " Started");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                for(int i = 0; i < taskList.Count; i++)
                {
                    if (taskList[i].IsFaulted)
                    {
                        Logger.GetInstance().Info($"Handler {_queueHandlers[i].GetType()} is in faulty state. Exception: {taskList[i].Exception.InnerException}");
                    }
                }
                await Task.Delay(10*1000, stoppingToken);
            }
        }
    }
}
