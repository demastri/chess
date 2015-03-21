using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProcessWrappers;
using QueueCommon;

namespace ChessPosition
{
    public class AnalysisFarmClient
    {
        /// <summary>
        /// 
        /// actually, the idea of an analysisfarm implies that even the client side is decoupled from the server (engine implementation)
        /// we should create an analysis farm server that provisions the actual engines available for execution
        /// 
        /// ok - there are 2 broad models that we intend to call an engine for
        ///  1 - serial (not necessarily synchronous) mode
        ///   we're looking at a single position at a time
        ///   there may be value in reviewing incremental updates to analysis
        ///   we may want to cut the analysis short
        /// 
        ///     interface
        ///         set engine parameters
        ///         load a position (request analysis)
        ///         while running   - this can be event driven as well - analysis update/complete
        ///             review incremental results
        ///             potentially stop    - based on some async event (
        ///         review final results
        /// 
        /// 2 - parallel (almost certainly asynchronous) mode
        /// 
        ///     here we have a number of positions to analyze (or other things to do in the interim)
        ///     incremental analysis is not as important here
        ///     
        /// as always, implementing the proper parallel model makes the serial one easy to manage
        ///     allow the client to submit as many positions as they care about
        ///     the client will subscribe to analysis update events as needed
        ///     the client will almost certainly subscribe to analysis complete events
        ///     they should actually care about VERY little else than this...
        ///
        ///     if they call an engine directly, they get serial performance, regardless of the
        ///     number of positions they queue up for execution
        ///     if they call a farm, they get performance based on the parameters of the farm
        /// 
        ///  for the analysis farm model, we're not actually writing to a process at all
        ///  we're posting positions to a queue that will resolve them whenever it's complete
        ///  ideally we should restructure the engine model as well to operate asynchronously for consistency
        /// 
        /// InitAnalysisParameters()
        /// requestID = SubmitAnalysisRequest()
        /// 
        /// either List<Analysis> = PullComplpetedAnalysisRequests()
        /// or Analysis = GetComplpetedAnalysis( id )    // null if incomplete
        /// 
        /// for synchronous usage, there's only one ever in queue
        /// for each position - Submit - while( completed == null ) - process analysis
        /// in actuality, the current synchronous usage is currently really async...
        /// 
        /// for asynchronous usage, there's only one ever in queue
        /// for each position - Submit... - while( incomplete analysis remains ) pull completed analysis, process
        /// 
        /// for now, connect to a single engine, and chain the complete event as the engine is done
        /// 
        /// </summary>

        public delegate void AnalysisUpdateHandler(int analysisID);
        public event AnalysisUpdateHandler AnalysisUpdateEvent;
        public event AnalysisUpdateHandler AnalysisCompleteEvent;

        List<AnalysisRequest> queuedRequests = null;
        int requestIDSeed;
        private System.Timers.Timer QueueManager;
        List<Engine> myEngineFarm;

        RabbitMQWrapper myPositionQueue;
        RabbitMQWrapper myResultsQueue;

        public AnalysisFarmClient(System.ComponentModel.ISynchronizeInvoke syncObj)
        {
            QueueManager = new System.Timers.Timer();

            requestIDSeed = 0;
            queuedRequests = new List<AnalysisRequest>();
            myEngineFarm = new List<Engine>();

            InitQueues();

        }
        public void Stop()
        {
        }
        public void Quit()
        {
            myPositionQueue.CloseConnections();
            myResultsQueue.CloseConnections();
        }
        int positionssent = 0;
        public int SubmitAnalysisRequest(EngineParameters eParams, string fenString)
        {
            // sotre the request in a place where someone will look at it
            // instantiate an engine/farm if needed and point it at the first one if needed
            // event handlers for that engine should chain through to the events requested by the client
            AnalysisRequest req = new AnalysisRequest(eParams, fenString, ++requestIDSeed);
            queuedRequests.Add(req);
            // ### write it to the queue;
            myPositionQueue.PostMessage(req.ToQueueString());

            return req.thisID;
        }

        private void InitQueues()
        {
            myPositionQueue = new RabbitMQWrapper("AnalysisFarm", "AnalysisRequest", "request", "localhost");
            myResultsQueue = new RabbitMQWrapper("AnalysisFarm", "AnalysisResults", "result", "localhost");
            myResultsQueue.SetListenerCallback(ListenerCallback);
        }
        void ListenerCallback(byte[] result)
        {
            // turn this into an analysis no
            Console.WriteLine("Returned..." + System.Text.Encoding.Default.GetString(result));
        }
    }
}
