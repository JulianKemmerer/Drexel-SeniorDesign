﻿using System;
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
            Skeleton2DDataExtract.Skeleton2DdataCoordReady += Skeleton2DFrameReady_Do_DTW_Process;
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
            //Pass 2d points to audio demo fnuction
            audioDemoTracking(e);

            //Update the skeleton buffers
            updateSkeletonBuffersPreRecognize(e);

            string gesture = recognizeGesture(e);
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
            updateSkeletonBuffersPostRecognize(e);
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
                    Skeleton2DDataExtract.ProcessData(skeletons[i],num_tracked);
                    num_tracked++;
                    //Stop looping if this was the second (max) tracked skeleton 
                    if(num_tracked >= MAX_TRACKED_SKELETONS)
                    {
                        break;
                    }
                }
            }
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Init some things
            //Tie event for ticks
            captureBeginTimer.Tick += captureBeginTimer_Tick;
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
                mp.Play();
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
            mp.Pause();
            isPlaying = false;
            isPaused = true;
        }

        void mp_MediaOpened(object sender, EventArgs e)
        {
            //Opening complete, play
            mp.Volume = 0.05;
            mp.Play();
            isPlaying = true;
        }

        void audioDemoTracking(Skeleton2DdataCoordEventArgs e)
        {
            Point [] points = e._points;
            //Do something for each joint
            for (int i = 0; i < points.Count<Point>(); i++)
            {
                //Convert back to pure pixels
                Point tmp = new Point((points[i].X * e.distance_normalizer) + e.center_point.X, (points[i].Y * e.distance_normalizer) + e.center_point.Y);
                //Convert to screen percentages

                //Debug.WriteLine(tmp.Y);
                //tmp.X =tmp.X / 640.0;
                //tmp.Y = tmp.Y / 480.0;
                //Debug.WriteLine(tmp.X);

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
                        mp.Volume = Math.Abs(tmp.Y);
                    }
                }
            }
        }
    

        #endregion
    }
}
