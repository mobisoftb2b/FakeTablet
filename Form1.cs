using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client.Events;
using RabbitMQManager;


namespace FakeTablet
{
    public partial class Agent : Form
    {
        SingleTablet st;

        public Agent()
        {
            st = new SingleTablet(UpdateTextBox, "123", "44107");
            InitializeComponent();
            this.Load += new System.EventHandler(this.Agent_Load);
        }

        private void Agent_Load(object sender, EventArgs e)
        {
            queueName.Text = st.queueName;
        }
        public void UpdateTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateTextBox), new object[] { value });
                return;
            }
            richTextBox1.Text = value;
        }

        private void GetManagerAuthorizationGroupActivities_Click(object sender, EventArgs e)
        {
            //m.SendMessage("test", queueName);
            st.GetManagerAuthorizationGroupActivities();
        }
        protected override void Dispose(bool disposing)
        {
            st.Close();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void askApprove_Click(object sender, EventArgs e)
        {
            st.askApprove();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            st.cancel();
        }

        private void getAllManagersConnected_Click(object sender, EventArgs e)
        {
            st.getAllManagersConnected();
        }

        private void GetAllManagersConnectedIsrael_Click(object sender, EventArgs e)
        {
            st.GetAllManagersConnectedIsrael();
        }

        private void test1_Click(object sender, EventArgs e)
        {
            TabletPerformance tp = new TabletPerformance();
            tp.BasicTest();
            richTextBox1.Text = "finished";
        }

        private void updateAck_Click(object sender, EventArgs e)
        {
            st.updateAck();
        }
    }
}
