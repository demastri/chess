using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QueueCommon;

namespace QueuePublish
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitMQWrapper subQueue = new RabbitMQWrapper("AnalysisFarm", "AnalysisRequest", "localhost");

            while (!Console.KeyAvailable)
            {
                if (!subQueue.QueueEmpty())
                    subQueue.PullMessages();
            }
            subQueue.CloseConnections();
        }
    }
}
