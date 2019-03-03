using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

using Visionscape;
using Visionscape.Steps;
using Visionscape.Devices;
using Visionscape.Communications;

namespace Microscan_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VsCoordinator m_Coord;          // For starting avp backplane
        private JobStep m_Job;                  // For opening and saving jobs
        private VsDevice m_Dev;                 // For controlling the vision device
        private ReportConnection m_RepCon1;     // For getting report and image from camera
        private InspectionReport report;        // Contains report coming back from camera
        private IOConnection m_IO;              // IO connection to the device for physical and virtual IO
        private VisionSystemStep vs;            // Visionsystem Step for job handling
        private string DEVICENAME;              // Global to hold name of device

        private Visionscape.Display.Setup.SetupManager SetupManager1;
        private Visionscape.Display.Image.BufferView BufferView1;

        public MainWindow()
        {
            InitializeComponent();
            SetupManager1 = new Visionscape.Display.Setup.SetupManager();
            BufferView1 = new Visionscape.Display.Image.BufferView();

        }

        private void Microscan_App_Loaded(object sender, RoutedEventArgs e)
        {
            WFHost1.Child = BufferView1;

            m_Coord = new VsCoordinator();     // Starts Backplane         
            m_Job = new JobStep();
            m_RepCon1 = new ReportConnection();
            m_IO = new IOConnection();

            // Set DEVICENAME to your units name
            DEVICENAME = "SoftSys1";
            //DEVICENAME = "MicroHAWK1A48C4";
            m_RepCon1.NewReport += new EventHandler<ReportConnectionEventArgs>(m_RepCon1_NewReport);
            m_Coord.OnDeviceFocus += new _IVsCoordinatorEvents_OnDeviceFocusEventHandler(m_Coord_OnDeviceFocus);
            m_Coord.DeviceFocusSetOnDiscovery(DEVICENAME, -1);          
            
        }

        private void Microscan_App_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop all inspections
            if (m_Dev != null)
                m_Dev.StopAll();

            // Disconnect the reports
            if (m_RepCon1 != null)
                m_RepCon1.Disconnect();
            m_RepCon1 = null;

            // Disconnect the IO
            if (m_IO != null)
                m_IO.Disconnect();
            m_IO = null;

            // Disconnect from the camera
            if (m_Dev != null)
                m_Dev.Disconnect();
            m_Dev = null;

            // Set the RootStep of setup manager to null
            SetupManager1.RootStep = null;

            // Clear the job from pc memory
            while (m_Job.Count > 0)
                m_Job.Remove(1);
            m_Job = null;
        }

        void m_RepCon1_NewReport(object sender, ReportConnectionEventArgs e)
        {
            // Get the Inspection report
            report = e.Report;
            // Get Number of Blobs
            bool status;
            long numBlobs;
            status = report.Results[0].AsBool;
            textBox1.Text = status.ToString();
            numBlobs = report.Results[1].AsInt;
            textBox2.Text = numBlobs.ToString();

            //does our report contain any images, if so display it?
            if (report.Images.Count > 0)
            {
                //extract the image as a BufferDm,
                //and set it into the Buffer View
                BufferView1.Buffer = (BufferDm)report.Images[0];
            }

            // This is a good place to collect unused memory
            GC.Collect();
        }

        void m_Coord_OnDeviceFocus(VsDevice objDevice)
        {
            // Get handle to the Device
            m_Dev = objDevice;
            // Connect the IO control to the device. Only do this once.
            m_IO.Connect(m_Dev);

            // Create an event so that we see when IO on camera changes
            // m_IO.IOTransition += new EventHandler<IOConnectionEventArgs>(m_IO_IOTransition);

            // Initialize all IO to low, in this case Virtual 1
            m_IO.PointWrite(VsIoType.Virtual, 0, false);


            // Load our job from disk
            m_Job.Load("C:\\Microscan\\Vscape\\Jobs\\test.avp");

            // Get the first VisionSystemStep in the job
            vs = m_Job.VisionSystemStep();

            // Connect the VisionSystemStep to our device
            vs.SelectSystem(m_Dev.Name);
            //Now we can download to the device(vision system) and run the job on that device
            m_Dev.Download(vs, true);

            // Connect reports to the inspection on the device
            m_RepCon1.Connect(DEVICENAME, 1, ReportConnection.ConnectOptions.DEFAULT);

            // Make images part of the report
            m_RepCon1.AddSnapBuffers((Visionscape.Steps.InspectionStep)m_Job.FindBySymName("Insp1"));
            // Now start the device
            m_Dev.StartAll();
        }

        private void RunMode_btn_Click(object sender, RoutedEventArgs e)
        {
            // Show Buffer View
            //bufferView1.Show();
            // we can do this using two methods:
            // 1. change WFHost1.Child to either BufferView1 or SetupManager1
            // 2. use two WFHost1 and 2, assign one a BufferView, second a SetupManager
            // then use WFHost1.Visibility.hide or something
            if(WFHost1.Child != BufferView1)
                WFHost1.Child = BufferView1;

            // Hide Setup Manager
            //setupManager1.Hide();
            SetupManager1.RootStep = null;
            //WFHost1.Child
            // Download Job to Device
            m_Dev.Download(m_Job, true);

            // Connect reports to the inspection on the device
            m_RepCon1.Connect(DEVICENAME, 1, ReportConnection.ConnectOptions.DEFAULT);

            // Make images part of the report
            m_RepCon1.AddSnapBuffers((Visionscape.Steps.InspectionStep)m_Job.FindBySymName("Insp1"));

            // Now start the device
            m_Dev.StartAll();

        }

        private void Trigger_btn_Click(object sender, RoutedEventArgs e)
        {
            // Set IO to low, in this case Virtual 1
            m_IO.PointWrite(VsIoType.Virtual, 0, false);

            // Put in short delay
            System.Threading.Thread.Sleep(5);

            //Set IO to high, in this case Virtual 1
            m_IO.PointWrite(VsIoType.Virtual, 0, true);
        }

        private void SetupMode_btn_Click(object sender, RoutedEventArgs e)
        {
            // Stop the device and disconnect reports
            m_Dev.StopAll();
            m_RepCon1.Disconnect();

            // Show setupManager1 starting with Inspection as first step
            SetupManager1.ListViewSize = 80;
            SetupManager1.ImageViewSize = 300;
            SetupManager1.ZoomAuto();
            SetupManager1.RootStep = (Visionscape.Steps.Step)m_Job.FindBySymName("Insp1");
            WFHost1.Child = SetupManager1;
        }

        private void LoadAVP_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\Microscan\\Vscape\\Jobs";
            openFileDialog1.Filter = "AVP Files|*.avp";
            openFileDialog1.Title = "Select an AVP File";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_Dev.StopAll();
                m_RepCon1.Disconnect();

                SetupManager1.RootStep = null;

                while (m_Job.Count > 0) m_Job.Remove(1);

                m_Job = null;

                m_Job = new JobStep();

                m_Job.Load(openFileDialog1.FileName);
            }
        }

        private void CreateAVP_btn_Click(object sender, RoutedEventArgs e)
        {

            WFHost1.Child = SetupManager1;
            // stop all inspections
            m_Dev.StopAll();
            
            //disconnect the reports
            m_RepCon1.Disconnect();
            
            //set the RootStep of the setup manager to null
            SetupManager1.RootStep = null;
            
            //clear the job from PC memory
            while (m_Job.Count > 0)
                m_Job.Remove(1);
            
            //create new job
            m_Job = new JobStep();

            //add steps
            VisionSystemStep m_Vs;
            InspectionStep m_Insp;
            SnapshotStep m_Snap;
            Step m_Blob;
            Datum m_ROI;
            AcquireStep m_Acquire;

            m_Vs = (VisionSystemStep)(m_Job.AddStep("Step.VisionSystem"));
            m_Vs.SelectSystem(m_Dev.Name);
            
            // add rest of the steps
            m_Insp = (InspectionStep)(m_Vs.AddStep("Step.Inspection"));
            m_Snap = (SnapshotStep)(m_Insp.AddStep("Step.Snapshot"));
            m_Blob = (Step)(m_Snap.AddStep("Step.Blob"));
           
            //change aquire datum to load images from file. add die pip images
            
            m_Acquire = (AcquireStep)m_Job.Find("Step.Acquire", EnumAvpFindOption.FIND_BY_TYPE, EnumAvpStepCategory.S_ALL);
            m_Acquire.Datum("PicMode").Value = 1;
            m_Acquire.Datum("FileDir").Value = "C:\\Microscan\\Vscape\\Tutorials And Samples\\Micro Hawk\\Simple Blob Tool\\die*.tif";

            // set blob to find dark parts
            m_Blob.Datum("Polarity").Value = 0;
            m_Blob.Datum("FilterTouchROI").Value = true;

            //change ROI pos of blob
            
            m_ROI = (Datum)m_Blob.Datum("ROI");
            object[] a;
            a = (object[])m_ROI.Value;
            a[0] = 320;
            a[1] = 240;
            a[2] = 250;
            a[3] = 250;

            m_ROI.Value = a;
            //setup results
            m_Blob.TagForUpload("Status");
            m_Blob.TagForUpload("NumBlobs");

            SetupManager1.RootStep = (Step)m_Vs;

            //private void MainWindow_FromClosing(object sender, Form)
        }
    }
}
