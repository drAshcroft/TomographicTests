using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;




public enum Axis
{
    XAxis=0,YAxis=1,ZAxis=2
}
    public static partial  class MathHelps
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Angle">rotation in radians</param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point3D RotatePoint(double XYAngle, Point3D point)
        {
            return Point3D.Roll(point, XYAngle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Angle">rotation in radians</param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point3D RotatePoint(double XYAngle, double YZAngle, double XZangle, Point3D point)
        {
            Point3D temp=Point3D.Yaw(point, XZangle );
            temp.Pitch(YZAngle);
            temp.Roll(XYAngle);
            return temp;
        }

        /// <summary>
        /// Rotation in radians
        /// </summary>
        /// <param name="Angle"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static Point3D RotatePoint(double XYAngle, double YZAngle, double XZAngle, double X, double Y, double Z)
        {

           return  RotatePoint(XYAngle, YZAngle, XZAngle, new Point3D(X, Y, Z));
        }

        /// <summary>
        /// Normalizes offsetX point as if it is offsetX vector from (0,0,0)
        /// </summary>
        /// <param name="inPoint"></param>
        /// <returns></returns>
        public static Point3D NormalizePoint(Point3D inPoint)
        {
            double r = Math.Sqrt(inPoint.X * inPoint.X + inPoint.Y * inPoint.Y + inPoint.Z * inPoint.Z);
            return new Point3D(inPoint.X / r, inPoint.Y / r, inPoint.Z / r);
        }

        /// <summary>
        /// Normalizes offsetX point as if it is offsetX vector from (0,0,0)
        /// </summary>
        /// <param name="inPoint"></param>
        /// <returns></returns>
        public static Point3D NormalizePoint(double X, double Y, double Z)
        {
            double r = Math.Sqrt(X * X + Y * Y + Z * Z);
            return new Point3D(X / r, Y / r, Z / r);
        }


        public static bool IsInsideEllipse(double x, double y, double z, double CenterX, double CenterY, double CenterZ, double MajorAxis, double MinorAxis, double Rotation)
        {
            Point3D newPoint = new Point3D(x - CenterX, y - CenterY,z-CenterZ );
            newPoint = RotatePoint(-1 * Rotation, newPoint);

            double xp = Math.Abs(newPoint.X / MajorAxis);
            double yp = Math.Abs(newPoint.Y / MinorAxis);
            double zp = Math.Abs(newPoint.Z / MinorAxis);

            if (xp *xp + yp *yp + zp*zp  <1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// returns true is point (x,y) is within the rectangle defined by center (centerX,centerY) with specified axis and rotation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation is in radians</param>
        /// <returns></returns>
        public static bool IsInsideRectangle(double x, double y, double z, double CenterX, double CenterY, double CenterZ, double MajorAxis, double MinorAxis, double Rotation)
        {
            Point3D newPoint = new Point3D(x - CenterX, y - CenterY, z - CenterZ);
            newPoint = RotatePoint(-1 * Rotation, newPoint);
            double xp = Math.Abs(newPoint.X / MajorAxis);
            double yp = Math.Abs(newPoint.Y / MinorAxis);
            double zp = Math.Abs(newPoint.Z / MinorAxis);

            if (xp <= 1 && yp <= 1 && zp < 1)
                return true;
            else
                return false;

        }

        public static Bitmap MakeBitmap(this double[, ,] ImageArray, int ZIndex)
        {
            int iWidth = ImageArray.GetLength(0);
            int iHeight = ImageArray.GetLength(1);
            double iMax = -10000;
            double iMin = 10000;

            for (int i = 0; i < iWidth; i++)
                for (int j = 0; j < iHeight; j++)
                {
                    if (iMax < ImageArray[i, j, ZIndex]) iMax = ImageArray[i, j, ZIndex];
                    if (iMin > ImageArray[i, j, ZIndex]) iMin = ImageArray[i, j, ZIndex];
                }
            double iLength = iMax - iMin;


            Bitmap b = new Bitmap(iWidth, iHeight, PixelFormat.Format32bppRgb);
            BitmapData bmd = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.WriteOnly, b.PixelFormat);


            unsafe
            {
                for (int y = 1; y <= iHeight; y++)
                {
                    Int32* scanline = (Int32*)((byte*)bmd.Scan0 + (y - 1) * bmd.Stride);

                    for (int x = 0; x < iWidth; x++)
                    {
                        byte* bits = (byte*)scanline;
                        int g = (int)(255d * (ImageArray[x, iHeight - y, ZIndex] - iMin) / iLength);
                        if (g > 255) g = 255;
                        byte g2 = (byte)g;
                        bits[0] = g2;
                        bits[1] = g2;
                        bits[2] = g2;
                        scanline++;
                    }
                }
            }
            b.UnlockBits(bmd);
            return b;
        }
    }
