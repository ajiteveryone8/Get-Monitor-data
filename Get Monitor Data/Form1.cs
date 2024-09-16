using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;

using WindowsDisplayAPI.Exceptions;
using WindowsDisplayAPI.Native;
using WindowsDisplayAPI.Native.DeviceContext;
using WindowsDisplayAPI.Native.DeviceContext.Structures;
using WindowsDisplayAPI.Native.Structures;
using WindowsDisplayAPI.DisplayConfig;

namespace Get_Monitor_Data
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PathDisplayTarget pathDisplayTarget;
        private void Form1_Load(object sender, EventArgs e)
        {
            LUID lUID = new LUID(97746, 0);
            PathDisplayAdapter pathDisplayAdapter = new PathDisplayAdapter(lUID);
             pathDisplayTarget = new PathDisplayTarget(pathDisplayAdapter, 198147);

           // PathDisplayAdapter

            // MonitorChanger.get_monitor_data();




            int index = 0;
            foreach (var target in PathDisplayTarget.GetDisplayTargets())
            {
                string deviceName = target.ToDisplayDevice().DisplayFullName.Replace("\\Monitor0", "");
                
                //MessageBox.Show(target.TargetId + "  ---  " + /*Screen.AllScreens[index].DeviceName + "  ---  " + */ Screen.AllScreens.Where(s=> s.DeviceName.Equals(deviceName)).First().Bounds);

            
                var key1 = ((string[])target.OpenDevicePnPKey().GetValue("HardwareID"))[0];

                richTextBox1.AppendText(key1);

                richTextBox1.AppendText(" -- ");

                var key2 = target.OpenDevicePnPKey().GetValue("ContainerID").ToString();

                richTextBox1.AppendText(key2);

                richTextBox1.AppendText("\n");

                index++;
            }

            //ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_MonitorDetails");
            //foreach (ManagementObject obj in searcher.Get())
            //    Console.WriteLine("Description: {0}", obj["Description"]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript("get-CimInstance WmimonitorID -Namespace root\\wmi | select InstanceName");
            //pipeline.Commands.Add("Out-String");
            Collection<PSObject> results = pipeline.Invoke();
            runspace.Close();

            StringBuilder stringBuilder = new StringBuilder();

            int monitorIndex = 1;
            
            foreach (var item in results)
            {
                stringBuilder.AppendLine($"Display : {monitorIndex} - {item.Properties["instanceName"].Value}");
                monitorIndex++;
            }

            richTextBox1.AppendText(stringBuilder.ToString());
        }

        private void RunScript(string scriptText)
        {
            Runspace myRunspace = RunspaceFactory.CreateRunspace();
            myRunspace.Open();

            RunspaceConfiguration rsConfig = RunspaceConfiguration.Create();
            PSSnapInException snapInException = null;
            PSSnapInInfo info = rsConfig.AddPSSnapIn("Microsoft.PowerShell.Core", out snapInException);
            Runspace myRunSpace = RunspaceFactory.CreateRunspace(rsConfig);
            myRunSpace.Open();

            Pipeline pipeLine = myRunSpace.CreatePipeline();
            Command myCommand = new Command(scriptText);
            pipeLine.Commands.Add(myCommand);
            var commandResults = pipeLine.Invoke();

            foreach (PSObject cmdlet in commandResults)
            {
                string cmdletName = cmdlet.Properties["Name"].Value.
                ToString();
               // listBox1.Items.Add(cmdletName);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //label1.Text = string.IsNullOrEmpty(pathDisplayTarget.DevicePath) ? "Disconnectee" : "Connected";
        }
    }
}
