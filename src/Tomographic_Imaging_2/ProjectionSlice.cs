using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomographic_Imaging_2
{
    public class ProjectionSlice
    {
        public double Angle;
        private PhysicalArray mProjection;
        private PhysicalArray mBackProjection;


        public double PhysicalStart
        {
            get { return mProjection.PhysicalStart; }
        }
        public double PhysicalEnd
        {
            get { return mProjection.PhysicalEnd; }
        }

        public double PhysicalStep
        {
            get
            {
                return mProjection.PhysicalStep;
            }
        }

        public PhysicalArray  Projection
        {
            get { return mProjection; }
            set
            {
                mProjection = value;
                mBackProjection = null;
            }
        }


        /// <summary>
        /// this would be an expensive operation to call repeatedly.  Make offsetX local copy to do the math
        /// you must call DoBackProjection for this to have offsetX value
        /// </summary>
        public PhysicalArray  BackProjection
        {
            get 
            {
                return mBackProjection;
            }
        }


        /// <summary>
        /// Creates the backprojecting slice,  if you wish to add zero padding, you can specify how many times more 
        /// the mData should be extended
        /// </summary>
        /// <param name="ZeroPadding">n is the number of points passed to the convolution. Useful for ffts</param>
        /// <param name="impulse"></param>
        /// <returns></returns>
        public PhysicalArray DoBackProjection(double[] impulse,int  ZeroPadding )
        {
            PhysicalArray ProjectionT = null;
            ProjectionT = mProjection.ZeroPadDataCentered(ZeroPadding);

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
        public PhysicalArray DoBackProjection(double ZeroPadding, double[] impulse)
        {
            PhysicalArray ProjectionT = null;
            if (ZeroPadding == 1 || ZeroPadding == 0)
                ProjectionT = mProjection;
            else
                ProjectionT = mProjection.ZeroPadDataCentered((int)(mProjection.Length * ZeroPadding));

            DoBackProjection(ProjectionT, impulse);
            return mBackProjection;
        }

        /// <summary>
        /// Creates the backprojecting slice
        /// </summary>
        /// <param name="impulse"></param>
        /// <returns></returns>
        public PhysicalArray  DoBackProjection( double[] impulse)
        {
            PhysicalArray ProjectionT = null;
            ProjectionT = mProjection;

            DoBackProjection(ProjectionT, impulse);
            return mBackProjection;
        }

        private void DoBackProjection(double[] ProjectionT, double[] impulse)
        {
            double tau = PhysicalEnd - PhysicalStart / (ProjectionT.Length);
            mBackProjection = new PhysicalArray( MathHelps.ConvoluteChop(ProjectionT, impulse),PhysicalStart,PhysicalEnd) ;

            mBackProjection *= tau;

        }

        private void DoBackProjection(PhysicalArray ProjectionT, double[] impulse)
        {
            double tau = PhysicalEnd - PhysicalStart / (ProjectionT.Length);
            mBackProjection = ProjectionT.ConvoluteChop(impulse);
            mBackProjection *= tau;

        }


        public void SetBackProjection(PhysicalArray BackProjection)
        {
            mBackProjection = BackProjection.TruncateData(Projection.PhysicalStart, Projection.PhysicalEnd);
        }

    }
}
