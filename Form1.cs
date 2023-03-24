using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace montoring
{
    public partial class Form1 : Form
    {
        private ManagementEventWatcher fileWatcher;
        private ManagementEventWatcher appWatcher;

        public Form1()
        {
            InitializeComponent(); textBox1.Multiline = true; textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new System.Drawing.Size(600, 400);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Set up the WMI event watcher to monitor for file system and app changes
            fileWatcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'CIM_DataFile'"));
            fileWatcher.EventArrived += OnFileEvent;

            appWatcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Product'"));
            appWatcher.EventArrived += OnAppEvent;

            // Start the event watchers to monitor in the background
            fileWatcher.Start();
            appWatcher.Start();

            textBox1.AppendText("Monitoring for file and app changes..." + Environment.NewLine);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Ready to monitor for file and app changes.";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the event watchers when the form is closed
            if (fileWatcher != null)
                fileWatcher.Stop();
            if (appWatcher != null)
                appWatcher.Stop();
        }


        private void OnFileEvent(object sender, EventArrivedEventArgs e)
        {
            string eventName = e.NewEvent.ClassPath.ClassName;
            string filePath = (string)((ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value)["Name"];

            // Use BeginInvoke to update the textbox on the UI thread
            BeginInvoke(new Action(() =>
            {
                textBox1.AppendText($"File {filePath} was {eventName.ToLower()}." + Environment.NewLine);
            }));
        }

        private void OnAppEvent(object sender, EventArrivedEventArgs e)
        {
            string eventName = e.NewEvent.ClassPath.ClassName;
            string appName = (string)((ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value)["Name"];

            // Use BeginInvoke to update the textbox on the UI thread
            BeginInvoke(new Action(() =>
            {
                textBox1.AppendText($"Application {appName} was {eventName.ToLower()}." + Environment.NewLine);
            }));
        }
    }
}
