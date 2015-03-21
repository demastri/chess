using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QueueCommon;

namespace ChessPosition
{
    public class AnalysisFarm
    {
        /// ok - here's the deal
        /// we should instantiate some Engines on init and wait for incoming positions via queue
        /// once we see them, we should start analysis on the engines
        /// when any return, we should post the analysis back to the return queue

        List<Engine> engines;

        RabbitMQWrapper myPositionQueue;
        RabbitMQWrapper myResultsQueue;

        List<AnalysisRequest> rawRequests;

        public AnalysisFarm()
        {
        }

        public void Start()
        {
            rawRequests = new List<AnalysisRequest>();
            InitEngines();
            InitQueues();
        }

        public void Shutdown()
        {
            myPositionQueue.CloseConnections();
            myResultsQueue.CloseConnections();
        }

        private void InitEngines()
        {
            engines = new List<Engine>();
            Engine nextEngine = new Engines.Stockfish();
            nextEngine.AnalysisCompleteEvent += AnalysisEngine_AnalysisComplete;
            engines.Add(nextEngine);
        }
        private void InitQueues()
        {
            myPositionQueue = new RabbitMQWrapper("AnalysisFarm", "AnalysisRequest", "request", "localhost");
            myPositionQueue.SetListenerCallback(ListenerCallback);

            myResultsQueue = new RabbitMQWrapper("AnalysisFarm", "AnalysisResults", "result", "localhost");
        }
        private void ListenerCallback(byte[] result)    // ok, this means we got a request
        {
            // turn this into an analysis request, find an engine and go
            // ###requests will certainly outstrip our ability to process them
            Console.WriteLine("something happened: " + System.Text.Encoding.Default.GetString(result));
            //myResultsQueue.PostMessage("Returning..." + System.Text.Encoding.Default.GetString(result));

            rawRequests.Add(new AnalysisRequest(System.Text.Encoding.Default.GetString(result)));
            ScheduleTask();
        }
        private void ScheduleTask()
        {
            foreach (AnalysisRequest ar in rawRequests)
            {
                if (ar.Status == "Waiting")
                {
                    foreach (Engine e in engines)
                    {
                        if (e.AnalysisComplete())
                        {
                            rawRequests.RemoveAt(0);
                            ar.Status = "In Process";
                            e.SetPostion(ar.FEN, ar.param);
                        }
                    }
                }
            }
        }
        void AnalysisEngine_AnalysisComplete(int thisID)  // needs to refer to the actual analysis request...
        {
            ///find the right ar
            ///copy the curanalysis
            ///post the curanalysis as a new message
        }

    }
}
