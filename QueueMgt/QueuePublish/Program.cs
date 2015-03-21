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
            RabbitMQWrapper pubQueue = new RabbitMQWrapper("AnalysisFarm", "AnalysisRequest", "localhost");

            while (!Console.KeyAvailable)
            {
                if (pubQueue.QueueEmpty())
                    pubQueue.PostTestMessages();
            }
            pubQueue.CloseConnections();
        }
    }
}
