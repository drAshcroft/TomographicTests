using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tomographic_Imaging_2
{

    public class ProjectionImage
    {
        public double Angle;
        private double[,] mProjection;
        private complex[,] mFFTProjection;
        private double[,] mBackProjection;

        public int[] MidPoint=new int[2];
        public double[] StartR=new double [2];
        public double[] EndR=new double[2];

        public double StepR(int index)
        {
                return (EndR[index] - StartR[index]) / (double)mProjection.GetLength(index );
        }

        public double[,] Projection
        {
            get { return mProjection; }
            set
            {
                mProjection = value;
                mFFTProjection = null;
                mBackProjection = null;
            }
        }

        public ProjectionImage()
        {


        }

        public int GetProjectionLength(int Dimension)
        {
            return mProjection.GetLength(Dimension);
        }

        public int GetProjectionLength(Axis DesiredAxis)
        {
            return mProjection.GetLength((int)DesiredAxis );
        }

        public ProjectionSlice GetSlice(Axis DesiredAxis, int SliceNumber)
        {
            ProjectionSlice ps = new ProjectionSlice();
            int Dim = mProjection.GetLength((int)DesiredAxis);
            double[] SliceInfo = new double[Dim];
            if (DesiredAxis == Axis.XAxis)
            {
                for (int i = 0; i < Dim; i++)
                {
                    SliceInfo[i] = mProjection[i, SliceNumber];
                }
               
                ps.Projection = new PhysicalArray( SliceInfo, StartR[0],EndR[0]);
            }
            else
            {
                for (int i = 0; i < Dim; i++)
                {
                    SliceInfo[i] = mProjection[SliceNumber,i ];
                }
               
                ps.Projection = new PhysicalArray(SliceInfo, StartR[1], EndR[1]);
            }

            if (mBackProjection != null)
            {
                Dim=mBackProjection.GetLength((int)DesiredAxis);
                double[] SliceBack = new double[Dim ];
                if (DesiredAxis == Axis.XAxis)
                {
                    for (int i = 0; i < Dim; i++)
                    {
                        SliceBack[i] = mBackProjection[i, SliceNumber];
                    }
                    ps.SetBackProjection ( new PhysicalArray(SliceBack, StartR[0], EndR[0]));
                }
                else
                {
                    for (int i = 0; i < Dim; i++)
                    {
                        SliceBack[i] = mProjection[SliceNumber, i];
                    }
                    ps.SetBackProjection ( new PhysicalArray(SliceBack, StartR[1], EndR[1]));
                }

            }
            return ps;

        }

        /// <summary>
        /// this would be an expensive operation to call repeatedly.  Make offsetX local copy to do the math
        /// you must call DoBackProjection for this to have offsetX value
        /// </summary>
        public double[,] BackProjection
        {
            get
            {
                return mBackProjection;
            }

        }

        /// <summary>
        /// Performs fft of mData.  Zeropadding is the total number of points that should be used,  useful for padding mData to 2^n points
        /// </summary>
        /// <param name="ZeroPadding"></param>
        /// <returns></returns>
        public complex[] DoFFT(int ZeroPadding)
        {
            throw new Exception("not yet implemented");
        }


        /// <summary>
        /// Creates offsetX FFT of the projection slice
        /// </summary>
        /// <returns></returns>
        public complex[] DoFFT()
        {
            throw new Exception("not yet implemented");
        }


        /// <summary>
        /// Creates the backprojecting slice,  if you wish to add zero padding, you can specify how many times more 
        /// the mData should be extended
        /// </summary>
        /// <param name="ZeroPadding">n is the number of points passed to the convolution. Useful for ffts</param>
        /// <param name="impulse"></param>
        /// <returns></returns>
        public double[,] DoBackProjection(double[] impulse, int ZeroPadding)
        {
            double[,] ProjectionT = null;
            ProjectionT = mProjection.ZeroPadArray((int)(ZeroPadding));

            DoBackProjection(ProjectionT, impulse);
            return mBackProjection;
        }

        /// <summary>
        /// Creates the backprojecting slice,  if you wish to add zero padding, you can specify how many times more 
        /// the mData should be extended
        /// </summary>
        /// <param name="ZeroPadding">1 means that the mData stays the same length, 2 means twice as long...</param>
        /// <param name="impulse"></param>
        /// <returns></returns>
        public double[,] DoBackProjection(double ZeroPadding, double[] impulse)
        {
            double[,] ProjectionT = null;
            if (ZeroPadding == 1 || ZeroPadding == 0)
                ProjectionT = mProjection;
            else
                ProjectionT = mProjection.ZeroPadArray((int)(mProjection.Length * ZeroPadding));

            DoBackProjection(ProjectionT, impulse);
            return mBackProjection;
        }

        /// <summary>
        /// Creates the backprojecting slice
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns></returns>
        public double[,] DoBackProjection(double[] impulse)
        {
            double[,] ProjectionT = null;
            ProjectionT = mProjection;

            DoBackProjection(ProjectionT, impulse);
            return mBackProjection;
        }

        private void DoBackProjection(double[,] ProjectionT, double[] impulse)
        {
            double tau = EndR[0] - StartR[0] / (ProjectionT.GetLength(0));
            mBackProjection = MathHelps.ConvoluteChop1D(Axis.XAxis , ProjectionT, impulse);
            //  mBackProjection = FFT_Processor.Convolute( (int) ( MathHelps.NearestPowerOf2( impulse.Length)*2),   ProjectionT, impulse);
            // mBackProjection = mBackProjection.CenterShortenArray(mProjection.Length);
            mBackProjection.MultiplyInPlace(tau);
        }

        public Bitmap ViewProjection()
        {
            return mProjection.MakeBitmap();
        }

        public Bitmap ViewBackProjection()
        {
            return mBackProjection.MakeBitmap();
        }
    }
}
