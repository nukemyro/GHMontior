using GHModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace ServiceSimulator
{
    public partial class Service : Form
    {        
        string url = "http://localhost:24316/api/updates"; //POST api to save to
        Timer mrT = new Timer();
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        /// <summary>This service will simulate a windows service running on a deployed client in production.</summary>
        public Service()
        {
            InitializeComponent();

            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            mrT.Enabled = true;
            mrT.Interval = 9000;
            mrT.Tick += MrT_Tick;
            mrT.Start();
        }

        private void MrT_Tick(object sender, EventArgs e)
        {
            SendUpdate();
        }

        public object getCPUCounter()
        {            
            // will always start at 0
            dynamic firstValue = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            dynamic secondValue = cpuCounter.NextValue();

            return secondValue;
        }

        public float getAvailableRAM()
        {
            return ramCounter.NextValue();//MB
        }


        private void SendUpdate()
        {
            /* construct the update object so send */
            HealthUpdate dt = new HealthUpdate();

            //get the machine name
            dt.Name = Environment.MachineName;

            //get the cpu info
            var cpu = getCPUCounter();
            if (cpu != null)
            {
                dt.CPU = (float)cpu;
            }

            //get the ram info
            var ram = getAvailableRAM();
            if (!float.IsNaN(ram))
            {
                dt.RAM = (decimal)ram;
            }

            //get hard disk space
            DriveInfo cDrive = DriveInfo.GetDrives()[0];//1st disk only
            dt.DISK = (float)cDrive.AvailableFreeSpace / 10000000000;

            //now transmit it to the server
            Transmit(dt);
        }


        /// <summary>Transmit the HealthUpdate to the central server</summary>
        /// <param name="hu">The HealthUpdate to send.</param>
        private void Transmit(HealthUpdate hu)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            var json = new JavaScriptSerializer().Serialize(hu);
            
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendUpdate();
        }
    }
}
