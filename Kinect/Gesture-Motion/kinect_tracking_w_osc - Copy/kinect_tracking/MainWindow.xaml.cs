using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Microsoft.Kinect;
using System.Diagnostics;
//using System.Threading;
using System.Net;
using Bespoke.Common.Osc;

namespace kinect_tracking
{
    public partial class MainWindow : Window
    {
        #region Class Globals

        //Maximum number of tracked skeletons
        private const int MAX_TRACKED_SKELETONS = 2;

        //Need a seperate gesture recognizer for each tracked skeleton
        private DtwGestureRecognizer[] DTWRecognizers = new DtwGestureRecognizer[MAX_TRACKED_SKELETONS];

        //List of gesture info, updated and written to the data grid
        List<GestureInfo> gestureInfos = new List<GestureInfo>();

        //Skeleton point buffers (skeleton video) for each skeleton being tracked
        private ArrayList[] skeleton_buffers = new ArrayList[MAX_TRACKED_SKELETONS];
        //Number of frames to store in each buffer
        private int SKELETON_BUFFER_LENGTH = 30;

        //Ignore frames: ignore every 1 out every'IGNORE_FRAMES'
        private int IGNORE_FRAMES = 2;
        //Helper value to keep track of frames ignored for each skeleton
        private int[] IGNORE_COUNTERS = new int[MAX_TRACKED_SKELETONS];

        //Minimum number of frames needed to begin gesture recognition
        private int MIN_NUM_FRAMES = 6;

        //Currently recording gesture flag
        private bool recording_gesture = false;

        //String representing the name of an unknown gesture
        public static string UNKNOWN_GESTURE_NAME = "__UNKNOWN";

        //Countdown for beginning capturing gestures
        Timer captureBeginTimer = new Timer();
        DateTime captureBeginTime = DateTime.Now;
        int CAPTURE_BEGIN_COUNTDOWN_SECONDS = 5;
        int CAPTURE_BEGIN_COUNTDOWN_INTERVAL = 100;

        //How many tracked skeletons are there?
        int tracked_skeletons = 0;

        //Dictionary to look up joint colors
        Dictionary<JointType, Brush> jointColors = new Dictionary<JointType, Brush>
        { 
            {JointType.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointType.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointType.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointType.Head, new SolidColorBrush(Color.FromRgb(200, 0, 0))},
            {JointType.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79, 84, 33))},
            {JointType.ElbowLeft, new SolidColorBrush(Color.FromRgb(84, 33, 42))},
            {JointType.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointType.HandLeft, new SolidColorBrush(Color.FromRgb(215, 86, 0))},
            {JointType.ShoulderRight, new SolidColorBrush(Color.FromRgb(33, 79,  84))},
            {JointType.ElbowRight, new SolidColorBrush(Color.FromRgb(33, 33, 84))},
            {JointType.WristRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            {JointType.HandRight, new SolidColorBrush(Color.FromRgb(37,  69, 243))},
            {JointType.HipLeft, new SolidColorBrush(Color.FromRgb(77, 109, 243))},
            {JointType.KneeLeft, new SolidColorBrush(Color.FromRgb(69, 33, 84))},
            {JointType.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointType.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointType.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointType.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222, 76))},
            {JointType.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointType.FootRight, new SolidColorBrush(Color.FromRgb(77, 109, 243))}
        };

        //Kinect sensor object
        private KinectSensor kinect_sensor;

        //OSC trans and receiv
        ITransmitter osc_transmitter;
        OscServer osc_receiver;
        string UDP_STRING = "UDP";
        string TCP_STRING = "TCP";
        //string UDP_MULTICAST_STRING = "Multicast";
        //OSC methods are like file paths - must start with root dir
        string DEMO_METHOD = "/DemoMethod";
        string SKELETON_METHOD = "/kinect/skeleton";
        IPEndPoint transmitter_ipendpoint;
        IPEndPoint receiver_ipendpoint;
        //Track number of packets sent
        long sent_bundles = 0;


        /// <summary>
        /// Skeleton2DdataCoordEventHandler delegate
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="a">Skeleton 2Ddata Coord Event Args</param>
        public delegate void Skeleton2DdataCoordEventHandler(object sender, Skeleton2DdataCoordEventArgs a);

        /// <summary>
        /// The Skeleton 2Ddata Coord Ready event
        /// </summary>
        public static event Skeleton2DdataCoordEventHandler Skeleton2DdataCoordReady;

        #endregion
        
        #region Kinect Actions/Events
        private void kinect_init()
        {
            //Initilize various things
            for (int i = 0; i < MAX_TRACKED_SKELETONS; i++)
            {
                IGNORE_COUNTERS[i] = 0;
                DTWRecognizers[i] = new DtwGestureRecognizer(12, 0.6, 2, 2, 10);
                skeleton_buffers[i] = new ArrayList();
            }

            //Smoothing "good for gesture recognition"
            //http://msdn.microsoft.com/en-us/library/jj131024.aspx
            TransformSmoothParameters smoothingParam = new TransformSmoothParameters();
            {
                smoothingParam.Smoothing = 0.5f;
                smoothingParam.Correction = 0.5f;
                smoothingParam.Prediction = 0.5f;
                smoothingParam.JitterRadius = 0.05f;
                smoothingParam.MaxDeviationRadius = 0.04f;
            };
            //Depth stream
            //kinect_sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            //Color stream
            //kinect_sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            //Skeleton stream with smoothing
            kinect_sensor.SkeletonStream.Enable(smoothingParam);
            //Start streams
            kinect_sensor.Start();

            //Setup kinect events
            //Depth frame ready event
            //kinect_sensor.DepthFrameReady += DepthFrameReady;
            //Color frame ready
            //kinect_sensor.ColorFrameReady += ColorFrameReady;
            //Skeleton FrameEdges ready event
            kinect_sensor.SkeletonFrameReady += SkeletonFrameReady;
            //Add another code block tied to the same event - this one for DTW processing
            kinect_sensor.SkeletonFrameReady += SkeletonFrameReady_Init_DTW_Process;

            //Tie the KinectDTW processor to an event here in main - event processes the 2D skeleton data
            Skeleton2DdataCoordReady += Skeleton2DFrameReady_Do_DTW_Process;
            //Also tie that event to sending network packets
            Skeleton2DdataCoordReady += Skeleton2DFrameReady_Do_Packet_Send;
        }

        void updateSkeletonBuffersPreRecognize(Skeleton2DdataCoordEventArgs e)
        {
            //Note: skeleton points sometimes come in as NaN - check this
            if (!double.IsNaN(e.GetPoint(0).X))
            {
                //Only store one out of every IGNORE_FRAMES frames
                IGNORE_COUNTERS[e.index] = (IGNORE_COUNTERS[e.index] + 1) % IGNORE_FRAMES;
                if (IGNORE_COUNTERS[e.index] == 0)
                {
                    //Store this current single skeleton info in the appropriate skeleton buffer
                    skeleton_buffers[e.index].Add(e.GetCoords());
                }
            }
        }

        void addGestureToRecognizer()
        {
            //Use values from form
            int skeleton_index = cmbbSkeletonCaptureSelect.SelectedIndex;
            string gesture_name = txtGestureName.Text;
            DTWRecognizers[skeleton_index].AddOrUpdate(skeleton_buffers[skeleton_index], gesture_name);
            //No longer capturing
            recording_gesture = false;
            largeStatusText.Text = "Capturing complete!";
        }

        void updateSkeletonBuffersPostRecognize(Skeleton2DdataCoordEventArgs e)
        {
            //Slide the window after addignthe frame at the end of the buffer
            if (skeleton_buffers[e.index].Count > SKELETON_BUFFER_LENGTH)
            {
                // If we are currently capturing and we reach the maximum buffer size then automatically store
                if (recording_gesture)
                {
                    //We are capturing and have reached the maximum number of frame
                    //Write data into recognizer
                    //This function checks which skeleton, name...etc.
                    addGestureToRecognizer();
                }
                else
                {
                    //Remove the first frame in the buffer to slide window
                    skeleton_buffers[e.index].RemoveAt(0);
                }
            }
        }

        string recognizeGesture(Skeleton2DdataCoordEventArgs e)
        {
            //Check that an appropriate number of frame have been captured for this skeleton
            //And that we are not currently recording a gesture
            if ((skeleton_buffers[e.index].Count > MIN_NUM_FRAMES) && !recording_gesture)
            {
                //Get the string name of the gesture in the video buffer
                return DTWRecognizers[e.index].Recognize(skeleton_buffers[e.index]);
            }
            else
            {
                return UNKNOWN_GESTURE_NAME;
            }
        }

        //Event that actual calls the dtw processor to do work
        void Skeleton2DFrameReady_Do_DTW_Process(object sender, Skeleton2DdataCoordEventArgs e)
        {
            //arg contains shoulder ponts at index 6 and 7
            //make new array that does not have them
            Skeleton2DdataCoordEventArgs newe = new Skeleton2DdataCoordEventArgs(e._points, e.index, e.center_point, e.distance_normalizer);
            Point[] newpoints = new Point[6];
            for(int i =0 ; i < 6; i++)
            {
                newpoints[i]=e._points[i];
            }
            newe._points = newpoints;
            
            //Pass 2d points to audio demo fnuction
            audioDemoTracking(newe);

            //Update the skeleton buffers
            updateSkeletonBuffersPreRecognize(newe);

            string gesture = recognizeGesture(newe);
            //If we don't recognize the gesture
            //Reset the buffer somewhere in here?
            if (!gesture.Contains(UNKNOWN_GESTURE_NAME))
            {
                //Do something with the gesture name here
                largeStatusText.Text = gesture;
            }
            else
            {
                //Not recognized
            }

            //Update buffer
            updateSkeletonBuffersPostRecognize(newe);
        }

         //Skeleton FrameEdges ready event - start DTW processing
        void SkeletonFrameReady_Init_DTW_Process(object sender, SkeletonFrameReadyEventArgs e)
        {
            //Get the skeletons from the skeleton frame
            //Some frames come back empty - check for null
            SkeletonFrame skeleton_frame = e.OpenSkeletonFrame();
            if (skeleton_frame == null)
            {
                //Do nothin'
                return;
            }

            //Init a new skeletons array of proper size
            Skeleton[] skeletons = new Skeleton[skeleton_frame.SkeletonArrayLength];
            //Copy data into array
            skeleton_frame.CopySkeletonDataTo(skeletons);

            //For each skeleton, process the data
            //Keep an 'index' of which of the two tracked skeletons is being passed
            int num_tracked = 0;
            for (int i = 0; i < skeletons.Count<Skeleton>(); i++)
            {
                //Only do DTW for skeletons which are fully tracked
                if(skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                {
                    //Get the event args
                    Skeleton2DdataCoordEventArgs args = Skeleton2DDataExtract.skeletonToSkeleton2DdataCoordEventArgs(skeletons[i],num_tracked);
                    //Launch the events
                    Skeleton2DdataCoordReady(null, args);
                    num_tracked++;
                    //Stop looping if this was the second (max) tracked skeleton 
                    if(num_tracked >= MAX_TRACKED_SKELETONS)
                    {
                        break;
                    }
                }
            }

            //A warning told me to do this - and I am code monkey.
            skeleton_frame.Dispose();
        }
        

        //Skeleton FrameEdges ready event - for drawing to screen
        void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //Get the skeletons from the skeleton frame
            //Some frames come back empty - check for null
            SkeletonFrame skeleton_frame = e.OpenSkeletonFrame();
            if (skeleton_frame == null)
            {
                //Do nothin'
                return;
            }

            //Init a new skeletons array of proper size
            Skeleton[] skeletons = new Skeleton[skeleton_frame.SkeletonArrayLength];
            //Copy data into array
            skeleton_frame.CopySkeletonDataTo(skeletons);

            //Draw skeletons to screen
            drawSkeletons(ref skeletonCanvas, ref skeletons, skeleton_frame.SkeletonArrayLength, ref kinect_sensor);

            //A warning told me to do this - and I am code monkey.
            skeleton_frame.Dispose();
        }

        //Depth frame ready event
        void DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            //Do nothing, enable in kinect_init
        }

        //Color frame ready
        void ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            //Do nothing, enable in kinect_init
        }
        #endregion

        #region Form Controls Actions/Events
        //Window Constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        //Start OSC receiver
        private void btnStartOSCRecv_Click(object sender, RoutedEventArgs e)
        {
            //Get the transport type
            if ((string)cmbbOSCRecvType.SelectedItem == UDP_STRING)
            {
                receiver_ipendpoint = new IPEndPoint(IPAddress.Loopback, Convert.ToInt32(txtOSCRecvPort.Text));
                osc_receiver = new OscServer(TransportType.Udp, IPAddress.Loopback, Convert.ToInt32(txtOSCRecvPort.Text));
            }
            else if ((string)cmbbOSCRecvType.SelectedItem == TCP_STRING)
            {
                receiver_ipendpoint = new IPEndPoint(IPAddress.Loopback, Convert.ToInt32(txtOSCRecvPort.Text));
                osc_receiver = new OscServer(TransportType.Tcp, IPAddress.Loopback, Convert.ToInt32(txtOSCRecvPort.Text));
            }
            else
            {
                //Do nothing
                Debug.WriteLine("Receiver transport type not recognized");
                return;
            }


            osc_receiver.FilterRegisteredMethods = false;
            osc_receiver.RegisterMethod(DEMO_METHOD);
            //To receive from OSC demo app
            string AliveMethod = "/osctest/alive";
            string TestMethod = "/osctest/test";
            osc_receiver.RegisterMethod(AliveMethod);
            osc_receiver.RegisterMethod(TestMethod);

            osc_receiver.BundleReceived += new EventHandler<OscBundleReceivedEventArgs>(osc_receiver_BundleReceived);
            osc_receiver.MessageReceived += new EventHandler<OscMessageReceivedEventArgs>(osc_receiver_MessageReceived);
            osc_receiver.ReceiveErrored += new EventHandler<Bespoke.Common.ExceptionEventArgs>(osc_receiver_ReceiveErrored);
            osc_receiver.ConsumeParsingExceptions = false;
            osc_receiver.Start();
            btnStartOSCRecv.IsEnabled = false;
            cmbbOSCRecvType.IsEnabled = false;
            txtOSCRecvPort.IsEnabled = false;
        }

        //Start OSC transmitter
        private void btnStartOSCTrans_Click(object sender, RoutedEventArgs e)
        {
            //Get the transport type
            if ((string)cmbbOSCTransType.SelectedItem == UDP_STRING)
            {
                transmitter_ipendpoint = new IPEndPoint(IPAddress.Parse(txtOSCTransIP.Text), Convert.ToInt32(txtOSCTransPort.Text));
                osc_transmitter = new UdpTransmitter(transmitter_ipendpoint);
                
            }
            else if ((string)cmbbOSCTransType.SelectedItem == TCP_STRING)
            {
                transmitter_ipendpoint = new IPEndPoint(IPAddress.Parse(txtOSCTransIP.Text), Convert.ToInt32(txtOSCTransPort.Text));
                osc_transmitter = new TcpTransmitter(transmitter_ipendpoint);  
            }
            else
            {
                //Do nothing
                Debug.WriteLine("Transmitter transport type not recognized.");
                return;
            }
            //osc_transmitter.Start(bundle);
            btnStartOSCTrans.IsEnabled = false;
            cmbbOSCTransType.IsEnabled = false;
            txtOSCTransIP.IsEnabled = false;
            txtOSCTransPort.IsEnabled = false;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Init some things
            //Tie event for ticks
            captureBeginTimer.Tick += captureBeginTimer_Tick;

            //Add items to drop down boxes
            cmbbOSCRecvType.Items.Add(UDP_STRING);
            cmbbOSCRecvType.Items.Add(TCP_STRING);
            cmbbOSCTransType.Items.Add(UDP_STRING);
            cmbbOSCTransType.Items.Add(TCP_STRING);

            //Test
            //updateOrAddGestureData(ref this.gestureDataGrid, 0, "Test", 3.14f, 2.13f, 5.6f);
        }

        private void startCapture()
        {
            //Tell the user
            largeStatusText.Text = "Recording gesture!";

            // Set the buttons disabled
            btnCaptureGesture.IsEnabled = false;
            btnSaveGestureToFile.IsEnabled = false;
            btnLoadGestureFromFile.IsEnabled = false;
            
            //Set capturing flag
            recording_gesture = true;

            //Assumes the index in the list is the skeleton number...
            skeleton_buffers[cmbbSkeletonCaptureSelect.SelectedIndex] = new ArrayList();
        }

        //Tick for capture begin countdown
        void captureBeginTimer_Tick(object sender, EventArgs e)
        {
            //If it is too soon
            if (DateTime.Now < captureBeginTime)
            {
                //Use big text box for countdown text
                largeStatusText.Text = "Capturing in " + ((captureBeginTime - DateTime.Now).Seconds + 1) + " seconds";
            }
            else
            {
                captureBeginTimer.Stop();
                startCapture();
            }
        }

        //Update or add info to the System.Windows.Controls.DataGrid
        private void updateOrAddGestureData(ref System.Windows.Controls.DataGrid dg, int Skeleton, string Gesture,float Last_Frame_Distance, float DTW_Distance,float Frames)
        {
            //Loop through the existing data
            bool found = false;
            foreach(GestureInfo gi in gestureInfos)
            {
                if( (Skeleton == gi.Skeleton) && (Gesture == gi.Gesture))
                {
                    //An entry for this skeleton and gesture has been found
                    //Update the info
                    gi.Last_Frame_Distance = Last_Frame_Distance;
                    gi.DTW_Distance = DTW_Distance;
                    gi.Frames = Frames;
                    //Done
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                //Not found, Add a new one
                gestureInfos.Add(new GestureInfo(Skeleton, Gesture, Last_Frame_Distance, DTW_Distance, Frames));
            }

            //Update data grid
            dg.ItemsSource = gestureInfos;
        }
        
        private void btnConnectFirstKinect_Click(object sender, RoutedEventArgs e)
        {
            //Check that a kinect is connected
            if (KinectSensor.KinectSensors.Count > 0)
            {
                //If the first kinect is connected
                if (KinectSensor.KinectSensors.FirstOrDefault().Status == KinectStatus.Connected)
                {
                    //Running this line prints to the screen without a newline
                    //Dumb
                    kinect_sensor = KinectSensor.KinectSensors.FirstOrDefault();
                    Debug.WriteLine("\nFirst Kinect connected.");
                    //Run kinect initialize
                    kinect_init();
                    btnConnectFirstKinect.Content = "Connected.";
                    btnConnectFirstKinect.IsEnabled = false;
                }
                else
                {
                    Debug.WriteLine("First Kinect exists but cannot be connected to.");
                }
            }
            else
            {
                Debug.WriteLine("No Kinects connected");
            }
        }

        private void btnCaptureGesture_Click(object sender, RoutedEventArgs e)
        {
            if(cmbbSkeletonCaptureSelect.SelectedIndex < 0)
            {
                //No selection made
                //Tell user
                largeStatusText.Text = "Cannot Capture: No Skeleton Selected!";
            }
            else
            {
                //Set the time for some seconds from now
                captureBeginTime = DateTime.Now.AddSeconds(CAPTURE_BEGIN_COUNTDOWN_SECONDS);

                //Start the timer
                //Check in every so often
                captureBeginTimer.Interval = CAPTURE_BEGIN_COUNTDOWN_INTERVAL;
                //Start the timer
                captureBeginTimer.Start();
            }
        }

        private void updateUI()
        {
            //Turn off caputure buttons when no skeletons
            if (tracked_skeletons <= 0)
            {
                btnCaptureGesture.IsEnabled = false;
                btnSaveGestureToFile.IsEnabled = false;
                btnLoadGestureFromFile.IsEnabled = false;
                cmbbSkeletonCaptureSelect.IsEnabled = false;
            }
            //If recording a gesture
            else if (recording_gesture)
            {
                grpGesture.IsEnabled = false;
            }
            else
            {
                //Populate skeletons combo box
                cmbbSkeletonCaptureSelect.Items.Clear();
                for (int i = 0; i < tracked_skeletons; i++)
                {
                    cmbbSkeletonCaptureSelect.Items.Add("Skeleton " + i);
                }
                cmbbSkeletonCaptureSelect.SelectedIndex = 0;

                //Turn on appropriate buttons
                grpGesture.IsEnabled = true;
                btnCaptureGesture.IsEnabled = true;
                btnSaveGestureToFile.IsEnabled = true;
                btnLoadGestureFromFile.IsEnabled = true;
                txtGestureName.IsEnabled = true;
                cmbbSkeletonCaptureSelect.IsEnabled = true;
            }
        }

        //Whole bunch of checks to make sure user input is ok
        void checkOSCTransParams()
        {
            //This runs too soon
            if (this.IsLoaded == false) return;

            //Check that valid entries exist
            if (cmbbOSCTransType.SelectedIndex == -1)
            {
                btnStartOSCTrans.IsEnabled = false;
                return;
            }

            IPAddress tmp;
            if (!IPAddress.TryParse(txtOSCTransIP.Text, out tmp) || txtOSCTransIP.Text == String.Empty)
            {
                btnStartOSCTrans.IsEnabled = false;
                return;
            }

            int tmpint;
            if (!int.TryParse(txtOSCTransPort.Text, out tmpint) || txtOSCTransPort.Text == String.Empty)
            {
                btnStartOSCTrans.IsEnabled = false;
                return;
            }
            btnStartOSCTrans.IsEnabled = true;
        }

        private void cmbbOSCTransType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkOSCTransParams();
        }

        private void txtOSCTransIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkOSCTransParams();
        }

        private void txtOSCTransPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkOSCTransParams();
        }

        void checkOSCRecvParams()
        {
            //This runs too soon
            if (this.IsLoaded == false) return;

            //Check that valid entries exist
            if (cmbbOSCRecvType.SelectedIndex == -1)
            {
                btnStartOSCRecv.IsEnabled = false;
                return;
            }

            int tmpint;
            if (!int.TryParse(txtOSCRecvPort.Text, out tmpint) || txtOSCRecvPort.Text == String.Empty)
            {
                btnStartOSCRecv.IsEnabled = false;
                return;
            }

            btnStartOSCRecv.IsEnabled = true;
        }

        private void cmbbOSCRecvType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkOSCRecvParams();
        }

        private void txtOSCRecvPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkOSCRecvParams();
        }

        #endregion

        #region Functions to draw skeletons to the screen, ref for changes/speed
        void drawSkeletons(ref Canvas canvas, ref Skeleton[] skeletons, int numSkeletons, ref KinectSensor _nui)
        {
            //Code stolen from KinectDTW for drawing skeletons to screen
            int iSkeleton = 0;
            var brushes = new Brush[6];
            brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
            brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
            brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
            brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

            canvas.Children.Clear();
            tracked_skeletons = 0;
            foreach (var data in skeletons)
            {
                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {
                    //One more tracked skeleton
                    tracked_skeletons++;

                    // Draw bones
                    Brush brush = brushes[iSkeleton % brushes.Length];
                    canvas.Children.Add(GetBodySegment(data.Joints, brush, ref canvas, ref _nui, JointType.HipCenter, JointType.Spine, JointType.ShoulderCenter, JointType.Head));
                    canvas.Children.Add(GetBodySegment(data.Joints, brush, ref canvas, ref _nui, JointType.ShoulderCenter, JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft));
                    canvas.Children.Add(GetBodySegment(data.Joints, brush, ref canvas, ref _nui, JointType.ShoulderCenter, JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight));
                    canvas.Children.Add(GetBodySegment(data.Joints, brush, ref canvas, ref _nui, JointType.HipCenter, JointType.HipLeft, JointType.KneeLeft, JointType.AnkleLeft, JointType.FootLeft));
                    canvas.Children.Add(GetBodySegment(data.Joints, brush, ref canvas, ref _nui, JointType.HipCenter, JointType.HipRight, JointType.KneeRight, JointType.AnkleRight, JointType.FootRight));

                    // Draw joints
                    foreach (Joint joint in data.Joints)
                    {
                        Point jointPos = GetDisplayPosition(joint, ref canvas, ref _nui);
                        var jointLine = new Line();
                        jointLine.X1 = jointPos.X - 3;
                        jointLine.X2 = jointLine.X1 + 6;
                        jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                        jointLine.Stroke = jointColors[joint.JointType];
                        jointLine.StrokeThickness = 6;
                        canvas.Children.Add(jointLine);
                    }
                }

                iSkeleton++;
            } // for each skeleton
            //Update the UI
            updateUI();
        }

        /// <summary>
        /// Works out how to draw a line ('bone') for sent Joints
        /// </summary>
        /// <param name="joints">Kinect NUI Joints</param>
        /// <param name="brush">The brush we'll use to colour the joints</param>
        /// <param name="ids">The JointsIDs we're interested in</param>
        /// <returns>A line or lines</returns>
        private Polyline GetBodySegment(JointCollection joints, Brush brush, ref Canvas canvas, ref KinectSensor _nui, params JointType[] ids)
        {

            var points = new PointCollection(ids.Length);
            foreach (JointType t in ids)
            {
                points.Add(GetDisplayPosition(joints[t], ref canvas, ref _nui));
            }

            var polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = brush;
            polyline.StrokeThickness = 5;
            return polyline;
        }

        /// <summary>
        /// Gets the display position (i.e. where in the display image) of a Joint
        /// </summary>
        /// <param name="joint">Kinect NUI Joint</param>
        /// <returns>Point mapped location of sent joint</returns>
        private Point GetDisplayPosition(Joint joint, ref Canvas canvas, ref KinectSensor _nui)
        {
            //float depthX, depthY;
            //var pos = _nui.MapSkeletonPointToDepth(joint.Position, DepthImageFormat.Resolution320x240Fps30);
            //var pos = _nui.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, DepthImageFormat.Resolution320x240Fps30);

            //depthX = pos.X;
            //depthY = pos.Y;

            int colorX, colorY;

            // Only ImageResolution.Resolution640x480 is supported at this point
            //var pos2 = _nui.MapSkeletonPointToColor(joint.Position, ColorImageFormat.RgbResolution640x480Fps30);
            var pos2 = _nui.CoordinateMapper.MapSkeletonPointToColorPoint(joint.Position, ColorImageFormat.RgbResolution640x480Fps30);
            colorX = pos2.X;
            colorY = pos2.Y;


            return new Point((int)(canvas.ActualWidth * (colorX / 640.0)), (int)(canvas.ActualHeight * (colorY / 480.0)));
        }

        #endregion

        #region OSC Packet Handling
        private static void osc_receiver_BundleReceived(object sender, OscBundleReceivedEventArgs e)
        {
            OscBundle bundle = e.Bundle;
            Debug.WriteLine(string.Format("\nBundle Received [{0}:{1}]: Nested Bundles: {2} Nested Messages: {3}", bundle.SourceEndPoint.Address, bundle.TimeStamp, bundle.Bundles.Count, bundle.Messages.Count));
        }

        private static void osc_receiver_MessageReceived(object sender, OscMessageReceivedEventArgs e)
        {
            OscMessage message = e.Message;

            Debug.WriteLine(string.Format("\nMessage Received [{0}]: {1}", message.SourceEndPoint.Address, message.Address));
            Debug.WriteLine(string.Format("Message contains {0} objects.", message.Data.Count));

            for (int i = 0; i < message.Data.Count; i++)
            {
                string dataString;

                if (message.Data[i] == null)
                {
                    dataString = "Nil";
                }
                else
                {
                    dataString = (message.Data[i] is byte[] ? BitConverter.ToString((byte[])message.Data[i]) : message.Data[i].ToString());
                }
                Debug.WriteLine(string.Format("[{0}]: {1}", i, dataString));
            }
        }

        private static void osc_receiver_ReceiveErrored(object sender, Bespoke.Common.ExceptionEventArgs e)
        {
            Debug.WriteLine("Error during reception of packet: {0}",e.Exception.Message);
        }

        //Send packets
        void Skeleton2DFrameReady_Do_Packet_Send(object sender, Skeleton2DdataCoordEventArgs e)
        {
            //If we are not setup to transmit transmitting, quit
            if (transmitter_ipendpoint == null)
            {
                return;
            }

            //Get the OSC bundle
            OscBundle bundle = skeleton2DdataCoordEventArgsToOSCBundle(e);

            //Send it
            bundle.Send(transmitter_ipendpoint);
            //osc_transmitter.Send(bundle);
            sent_bundles++;
        }

        //A skeleton point is relative to shoulder center and shoulder width
        //From testing the max multiple of shoulder widths is 4 in the Y direction, 3 in the X
        Point normalize_skeleton_point(Point skeleton_point)
        {
            double newX = skeleton_point.X / 2.0;
            if (newX > 1 || newX == double.PositiveInfinity || newX == double.NegativeInfinity)
            {
                newX = 1;
            }
            double newY = skeleton_point.Y / 3.0;
            if (newY > 1 || newY == double.PositiveInfinity || newY == double.NegativeInfinity)
            {
                newY = 1;
            }
            return new Point(newX, newY);
        }

        //Return an OSC bundle for skeleton index provided
        /*
        OscBundle skeletonToOSCBundle(Skeleton s, int index)
        {
            Skeleton2DdataCoordEventArgs e = Skeleton2DDataExtract.skeletonToSkeleton2DdataCoordEventArgs(s, index);
            return skeleton2DdataCoordEventArgsToOSCBundle(e);
        }
         */

        OscBundle skeleton2DdataCoordEventArgsToOSCBundle(Skeleton2DdataCoordEventArgs e)
        { 
            //Bundles are collections of messages
            OscBundle bundle = new OscBundle(transmitter_ipendpoint);
            //Messages are like packets, label with their type by method name
            OscMessage message = new OscMessage(transmitter_ipendpoint, SKELETON_METHOD);
            //Both bundles and messages can be nested
            //Must have at least one bundle and on message

            //Send the whole skeleton
            //Send packet number
            message.Append(sent_bundles);
            //Debug.WriteLine("Sent bundle: " + sent_bundles);
            //Send the index
            message.Append(e.index);
            //Then append each point in order, x then y
            int num_points = e._points.Count();
            for (int i = 0; i < num_points; i++)
            {
                Point normalized_point = normalize_skeleton_point(e._points[i]);
                message.Append((float)normalized_point.X);
                message.Append((float)normalized_point.Y);
            }
            bundle.Append(message);
            return bundle;
        }
        #endregion

        #region Audio Demo
        string audioFileName;
        MediaPlayer mp = new MediaPlayer();
        bool isPaused = false;
        bool isPlaying = false;
        private void selectAudioButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            audioFileName = dlg.FileName;

            //Handle opening audio
            mp.MediaOpened += new EventHandler(mp_MediaOpened);
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                //mp.Play();
                isPlaying = true;
                isPaused = false;
            }
            else
            {
                //Start a new audio player object
                     mp.Open(new Uri(audioFileName));
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            //mp.Pause();
            isPlaying = false;
            isPaused = true;
        }

        void mp_MediaOpened(object sender, EventArgs e)
        {
            //Opening complete, play
            mp.Volume = 0.5;
            mp.Play();
            isPlaying = true;
        }

        void audioDemoTracking(Skeleton2DdataCoordEventArgs e)
        {
            Point [] points = e._points;
            //Do something for each joint
            for (int i = 0; i < points.Count<Point>(); i++)
            {
                //Normalize
                Point tmp = normalize_skeleton_point(points[i]);

                if (i == 0) //handleft
                {
                    

                }
                else if (i == 1)//wrist left
                {

                }
                else if (i == 2)//elbow left
                {

                }
                else if (i == 3)//elbowright
                {

                }
                else if (i == 4)//wristright
                {

                }
                else if (i == 5)//handright
                {
                    if (isPlaying)
                    {
                        //Shift up by about half for nicer looking demo
                        mp.Volume = Math.Abs(tmp.Y+0.5);
                        mp.Balance = tmp.X;                       
                    }
                }
            }
        }
    

        #endregion 
    }
}
