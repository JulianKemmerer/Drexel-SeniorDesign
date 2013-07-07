//-----------------------------------------------------------------------
// <copyright file="Skeleton2DDataExtract.cs" company="Rhemyst and Rymix">
//     Open Source. Do with this as you will. Include this statement or 
//     don't - whatever you like.
//
//     No warranty or support given. No guarantees this will work or meet
//     your needs. Some elements of this project have been tailored to
//     the authors' needs and therefore don't necessarily follow best
//     practice. Subsequent releases of this project will (probably) not
//     be compatible with different versions, so whatever you do, don't
//     overwrite your implementation with any new releases of this
//     project!
//
//     Enjoy working with Kinect!
// </copyright>
//-----------------------------------------------------------------------

namespace kinect_tracking
{
    using System;
    using System.Windows;
    using Microsoft.Kinect;

    /// <summary>
    /// This class is used to transform the data of the skeleton
    /// </summary>
    internal class Skeleton2DDataExtract
    {
        /// <summary>
        /// Crunches Kinect SDK's Skeleton Data and spits out a format more useful for DTW
        /// </summary>
        /// <param name="data">Kinect SDK's Skeleton Data</param>
        /// 

        public static Skeleton2DdataCoordEventArgs skeletonToSkeleton2DdataCoordEventArgs(Skeleton data, int index)
        {
            // Extract the coordinates of the points.
            var p = new Point[8];
            int num_points = 8;
            Point shoulderRight = new Point(), shoulderLeft = new Point();
            foreach (Joint j in data.Joints)
            {
                switch (j.JointType)
                {
                    case JointType.HandLeft:
                        p[0] = new Point(j.Position.X, j.Position.Y);
                        break;
                    case JointType.WristLeft:
                        p[1] = new Point(j.Position.X, j.Position.Y);
                        break;
                    case JointType.ElbowLeft:
                        p[2] = new Point(j.Position.X, j.Position.Y);
                        break;
                    case JointType.ElbowRight:
                        p[3] = new Point(j.Position.X, j.Position.Y);
                        break;
                    case JointType.WristRight:
                        p[4] = new Point(j.Position.X, j.Position.Y);
                        break;
                    case JointType.HandRight:
                        p[5] = new Point(j.Position.X, j.Position.Y);
                        break;
                    case JointType.ShoulderLeft:
                        shoulderLeft = new Point(j.Position.X, j.Position.Y);
                        p[6] = shoulderLeft;
                        break;
                    case JointType.ShoulderRight:
                        shoulderRight = new Point(j.Position.X, j.Position.Y);
                        p[7] = shoulderRight;
                        break;
                }
            }

            // Centre the data
            var center = new Point((shoulderLeft.X + shoulderRight.X) / 2, (shoulderLeft.Y + shoulderRight.Y) / 2);
            for (int i = 0; i < num_points; i++)
            {
                p[i].X -= center.X;
                p[i].Y -= center.Y;
            }

            // Normalization of the coordinates
            float shoulderDist =
                (float)Math.Sqrt(Math.Pow((shoulderLeft.X - shoulderRight.X), 2) +
                          Math.Pow((shoulderLeft.Y - shoulderRight.Y), 2));
            for (int i = 0; i < num_points; i++)
            {
                p[i].X /= shoulderDist;
                p[i].Y /= shoulderDist;
            }

            return new Skeleton2DdataCoordEventArgs(p,index,center,shoulderDist);
        }
    }
}