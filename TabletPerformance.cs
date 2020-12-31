using Newtonsoft.Json;
using NLog;
using RabbitMQManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FakeTablet
{
    public class StartParams
    {
        public int testNumber { get; set; }
        public string EmployeeId { get; set; }
        public string ManagerEmployeeId { get; set; }
        public bool started { get; set; }
    }
    class TabletPerformance
    {
        private static Barrier barrier;
        RabbitMQManagersApprove m;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public List<RabbitMQManager.Agent> GetFakeAgentsBulk(int count) {
            List<RabbitMQManager.Agent> agents = new List<RabbitMQManager.Agent>();
            for (int i = 0; i < count; i++) {
                RabbitMQManager.Agent agent = new RabbitMQManager.Agent();
                agent.EmployeeId = (1000 + i).ToString();
                agent.ManagerEmployeeId = (3000 + i).ToString();
                agents.Add(agent);
            }
            return agents;
        }
        public void BasicTest()
        {
            int numAgents = int.Parse(ConfigurationManager.AppSettings["numberOfAgents"]);
            m = new RabbitMQManagersApprove();

            List<StartParams> spList = new List<StartParams>();

            List<RabbitMQManager.Agent> allAgents = GetFakeAgentsBulk(numAgents);//m.GetAllAgents();
            numAgents = allAgents.Count;
            barrier = new Barrier(numAgents + 1);
            try
            {
                // SNIP configure logging

                // Create the specified number of clients, to carry out test operations, each on their own threads
                Thread[] threads = new Thread[numAgents];
                for (int count = 0; count < numAgents; ++count)
                {
                    var index = count;
                    ParameterizedThreadStart ts = new ParameterizedThreadStart(this.RunClient);

                    threads[count] = new Thread(ts);
                    threads[count].Name = $"Agent {count}"; // for debugging
                    StartParams sp = new StartParams() { testNumber = count, EmployeeId = allAgents[count].EmployeeId, ManagerEmployeeId = allAgents[count].ManagerEmployeeId };
                    spList.Add(sp);
                    threads[count].Start(sp);
                }

                //for (int count = 0; count < numManagers; ++count)
                //{
                //    threads[count].Sleep(1000);
                //}
                // We loose the convenience of awaiting all tasks,
                // but use a thread barrier to block this thread until all the others are done.
                //System.Threading.Thread.Sleep(5000); //10 SEC
                barrier.SignalAndWait();
                logger.Info(JsonConvert.SerializeObject(spList));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private readonly object balanceLock = new object();
        private void RunClient(object startParams)
        {
            try
            {
                lock (balanceLock)
                {
                    System.Diagnostics.Debug.WriteLine($"test number {((StartParams)startParams).testNumber}");
                    SingleTablet sm = new SingleTablet(null, ((StartParams)startParams).EmployeeId, ((StartParams)startParams).ManagerEmployeeId, ((StartParams)startParams).testNumber);
                    sm.getAllManagersConnected();
                    //sm.askApprove();
                    //System.Threading.Thread.Sleep(5000); //10 SEC
                    if (sm.isBound)
                        ((StartParams)startParams).started = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                barrier.SignalAndWait();
            }
        }
    }
}
