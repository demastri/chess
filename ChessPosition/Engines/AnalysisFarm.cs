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
        private System.Timers.Timer EngineManager;

        RabbitMQWrapper myPositionQueue;
        RabbitMQWrapper myResultsQueue;

        List<AnalysisRequest> rawRequests;
        int engineCount;

        public AnalysisFarm(int nbrEngines, System.ComponentModel.ISynchronizeInvoke syncObj)
        {
            engineCount = nbrEngines;
            InitTick(syncObj);
        }
        private void EngineTick(object sender, EventArgs e)
        {
            foreach (Engine en in engines)
                en.CheckProgress();
        }

        public void Start()
        {
            rawRequests = new List<AnalysisRequest>();
            InitEngines();
            InitQueues();
            StartTick();
        }

        private void InitTick(System.ComponentModel.ISynchronizeInvoke syncObj)
        {
            EngineManager = new System.Timers.Timer(250);
            EngineManager.SynchronizingObject = syncObj;
            EngineManager.AutoReset = true;
            EngineManager.Elapsed += EngineTick;
        }
        private void StartTick()
        {
            EngineManager.Start();
        }

        public void Shutdown()
        { 
            QuitEngines();
            QuitQueues();
        }
        public void QuitEngines()
        {
            foreach (Engine e in engines)
                e.Quit();
        }
        public void QuitQueues()
        {
            try
            {
                myPositionQueue.CloseConnections();
                myResultsQueue.CloseConnections();
            }
            catch (Exception e) { }
        }

        private void InitEngines()
        {
            engines = new List<Engine>();
            Engine nextEngine;

            for (int i = 0; i < engineCount; i++)
            {
                nextEngine = new Engines.Stockfish();
                nextEngine.AnalysisCompleteEvent += AnalysisEngine_AnalysisComplete;
                engines.Add(nextEngine);
            }
            //nextEngine.Status();
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

            rawRequests.Add(new AnalysisRequest(System.Text.Encoding.Default.GetString(result)));
            ScheduleTask();
        }
        private void ScheduleTask()
        {
            int scheduled = -1;
            for (int i = 0; i < rawRequests.Count && scheduled < 0; i++)
            {
                AnalysisRequest ar = rawRequests[i];
                if (ar.Status == "Waiting")
                {
                    for (int j = 0; j < engines.Count; j++)
                    {
                        Engine e = engines[j];
                        if (e.AnalysisComplete())
                        {
                            Console.WriteLine("Analysis started: " + ar.thisID);
                            scheduled = i;
                            ar.Status = "In Process";
                            ar.EngineInstance = j;
                            e.SetPostion(ar);
                            break;
                        }
                    }
                }
            }
        }
        void AnalysisEngine_AnalysisComplete(int thisID)  // needs to refer to the actual analysis request...
        {
            AnalysisRequest ar = rawRequests[thisID - 1];
            Engine en = engines[ar.EngineInstance];
            ///find the right ar
            ///copy the curanalysis
            ///post the curanalysis as a new message
            Console.WriteLine("Analysis completed: " + thisID + " " + engines[ar.EngineInstance].curAnalysis.Score.ToString() + " " + engines[ar.EngineInstance].curAnalysis.bestLine[0]);
            myResultsQueue.PostMessage(en.curAnalysis.ToQueueString());

            ScheduleTask();
        }

    }
}
