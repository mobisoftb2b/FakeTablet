using NLog;
using RabbitMQ.Client.Events;
using RabbitMQManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeTablet
{
    class SingleTablet
    {
        public delegate void Del(string value);
        RabbitMQManagersApprove m = new RabbitMQManagersApprove();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        String queueNameGlobal = "managerAppQ1";

        //string deviceUniqueID = "1234";
        public string queueName;
        Del updateTextBoxDelegate;
        string ManagerEmplId;
        string EmployeeId;
        string DeviceUniqueID;
        string RequestID;
        int TestNumber = 0;
        public bool isBound { get; set; }

        public SingleTablet(Del callback, string employeeId, string managerEmplId, int testNumber = 0)
        {
            updateTextBoxDelegate = callback;
            ManagerEmplId = managerEmplId;
            EmployeeId = employeeId;
            TestNumber = testNumber;
            queueName = System.Guid.NewGuid().ToString();
            DeviceUniqueID = System.Guid.NewGuid().ToString();
            RequestID = System.Guid.NewGuid().ToString();
            if (m.Init($"testNumber {testNumber}"))
            {
                m.BindQueue(queueName);
                m.ReceiveMessageSubscribe(queueName, ReceiveSubscriber);
                isBound = true;
            }
        }

        private void ReceiveSubscriber(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger.Info($"Received! TestNumber={TestNumber} message {message}");
            updateTextBoxDelegate?.Invoke(message);
        }

        public void GetManagerAuthorizationGroupActivities()
        {
            //m.SendMessage("test", queueName);
            m.SendMessage($@"{{ ""command"": ""getManagerActivities"", ""EmployeeId"": ""{EmployeeId}"", ""consumerToken"": [{{ ""consumerToken"": ""674976"", ""AndroidId"": ""74894c93fdca0cd8"",  ""StatusChangeTime"": 0, ""queueName"": ""{queueName}"" }}] }}", queueNameGlobal);
        }
        public void Close()
        {
            m.Close();
        }

        public void askApprove()
        {
            m.SendMessage($@"{{""RequestID"": ""{RequestID}"",  ""testNumber"": ""{TestNumber}"", ""pTime"": ""'20201011111446'"",  ""StatusChangeTime"": ""'1602404086837'"", ""AgentId"": ""'5985'"", ""AgentName"": ""דואהדה עיסאם"", ""EmployeeId"": ""'{EmployeeId}'"", 
                     ""EmployeeName"": ""דואהדה עיסאם"",  ""ActivityCode"": ""'44'"", ""ActivityDescription"": ""'סימולצית המחרה'"",  ""Cust_Key"": ""'1'"",
                    ""CustName"": ""'Unknown client'"",  ""DocType"": ""''"",  ""DocNum"": ""''"",  ""DocName"": ""''"",  ""Comment"": ""''"",
                    ""ManagerEmployeeId"": ""{ManagerEmplId}"",  ""ManagerName"": ""'הופמן רועי'"", ""RequestStatus"": ""'0'"",  ""ManagerStatusTime"": ""'0'"",
                    ""ManagerComment"": ""''"", ""ManagerDeviceType"": ""'1'"",  ""TransmissionState"": ""'1'"",  ""IsTest"": ""'1'"",
                    ""consumerToken"": [{{ ""consumerToken"": ""674976"", ""AndroidId"": ""{DeviceUniqueID}"",  ""StatusChangeTime"": 0, ""queueName"": ""{queueName}"" }}],
                    ""Subject"": ""1 שורות, 12 קר, 0 יח"", ""command"": ""askApprove""}}", queueNameGlobal);

        }

        public void updateAck()
        {
            m.SendMessage($@"{{""RequestID"": ""{RequestID}"",  ""testNumber"": ""{TestNumber}"", ""pTime"": ""'20201011111446'"",  ""StatusChangeTime"": ""'1602404086837'"", ""EmployeeId"": ""'{EmployeeId}'"", 
                    ""ManagerEmployeeId"": ""{ManagerEmplId}"",  ""ManagerName"": ""'הופמן רועי'"",  ""ManagerStatusTime"": ""20201227125806"",
                    ""ManagerComment"": """", ""ManagerDeviceType"": ""2"",  ""IsTest"": ""'1'"",
                    ""consumerToken"": [{{ ""consumerToken"": ""674976"", ""AndroidId"": ""{DeviceUniqueID}"",  ""StatusChangeTime"": 0, ""queueName"": ""{queueName}"" }}],
                    ""Subject"": """", ""command"": ""updateAck""}}", queueNameGlobal);
        }

        public void cancel()
        {
            m.SendMessage($@"{{""RequestID"": ""'{RequestID}'"", ""pTime"": ""'20201011111719'"", ""StatusChangeTime"": ""'1602404239365'"",  ""ManagerEmployeeId"": ""{ManagerEmplId}"",
            ""EmployeeName"": ""'דואהדה עיסאם'"", ""RequestStatus"": ""'1001'"", ""consumerToken"": [{{""consumerToken"": ""674976"",
            ""AndroidId"": ""{DeviceUniqueID}"", ""StatusChangeTime"": 0, ""queueName"": ""{queueName}""  }}], 
            ""Subject"": """", ""command"": ""cancel""}}", queueNameGlobal);
        }

        public void getAllManagersConnected()
        {   if (isBound)
                m.SendMessage($@"{{ ""command"": ""getAllManagersConnected"", ""testNumber"": ""{TestNumber}"", ""EmployeeId"": ""{EmployeeId}"", ""consumerToken"": [{{ ""consumerToken"": ""674976"", ""AndroidId"": ""{DeviceUniqueID}"",  ""StatusChangeTime"": 0, ""queueName"": ""{queueName}"" }}] }}", queueNameGlobal);

        }

        public void GetAllManagersConnectedIsrael()
        {
            m.SendMessage($@"{{""command"":""getAllManagersConnected"", ""EmployeeId"":""27426"", ""consumerToken"":[{{ ""consumerToken"":""27426"",""AndroidId"":""{DeviceUniqueID}"",""StatusChangeTime"":0,""queueName"":""274263c2d93efe1c9ce13""}}],""Subject"":""""}}", queueNameGlobal);
        }
    }
}
