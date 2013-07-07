using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Class to mainly makes it easier to print into the gesture data grid

namespace kinect_tracking
{
    public class GestureInfo
    {
        public GestureInfo()
        {
            Skeleton = 0;
            Gesture = MainWindow.UNKNOWN_GESTURE_NAME;
            Last_Frame_Distance = 0;
            DTW_Distance = 0;
            Frames = 0;
        }

        public GestureInfo(int _Skeleton, string _Gesture, float _Last_Frame_Distance, float _DTW_Distance, float _Frames)
        {
            Skeleton = _Skeleton;
            Gesture = _Gesture;
            Last_Frame_Distance = _Last_Frame_Distance;
            DTW_Distance = _DTW_Distance;
            Frames = _Frames;
        }

        public int Skeleton{ get; set; }
        public string Gesture{ get; set; }
        public float Last_Frame_Distance{ get; set; }
        public float DTW_Distance{ get; set; }
        public float Frames{ get; set; }
    }
}
