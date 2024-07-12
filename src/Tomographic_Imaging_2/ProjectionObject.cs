using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tomographic_Imaging_2
{
    public class ProjectionObject
    {
        protected complex[,] FFTGrid;
        protected double[,] DensityGrid;

        protected List<ProjectionSlice> AllSlices = new List<ProjectionSlice>();

        protected double PhysicalWidth;
        protected double PhysicalHeight;

        protected double ProjectionStartX;
        protected double ProjectionEndX;
        protected double ProjectionStepX;
        protected int nProjectionSteps;

        #region Conversions
        private double ConvertXToFFTGrid(double x)
        {
            return (x + PhysicalWidth / 2) / (PhysicalWidth) * FFTGrid.GetLength(0);
        }
        private double ConvertYToFFTGrid(double y)
        {
            return (y + PhysicalHeight / 2) / (PhysicalHeight) * FFTGrid.GetLength(1);
        }
        #endregion
        public void ClearGrid(bool ProjectionExtendsToCorners, double PhysicalWidth, double PhysicalHeight, int nCols, int nRows)
        {

            FFTGrid = new complex[nCols, nRows];
            DensityGrid = new double[nCols, nRows];

            AllSlices = new List<ProjectionSlice>();

            this.PhysicalWidth = PhysicalWidth;
            this.PhysicalHeight = PhysicalHeight;

            if (ProjectionExtendsToCorners)
            {
                ProjectionStartX = -1 * Math.Sqrt(Math.Pow(PhysicalWidth / 2, 2) + Math.Pow(PhysicalHeight / 2, 2));
                ProjectionEndX = ProjectionStartX * -1;
            }
            else
            {
                ProjectionStartX = -1 * PhysicalWidth / 2;
                ProjectionEndX = PhysicalWidth / 2;
            }

            double stepX = PhysicalWidth / nCols;
            double stepY = PhysicalHeight / nRows;
            if (stepX < stepY)
                ProjectionStepX = stepX;
            else
                ProjectionStepX = stepY;
            nProjectionSteps = (int)((ProjectionEndX - ProjectionStartX) / ProjectionStepX);
        }


        /// <summary>
        /// Adds offsetX slice to the collection.  Nothing is done with the mData
        /// </summary>
        /// <param name="ProjectionSlice"></param>
        public void AddSlice(ProjectionSlice ProjectionSlice)
        {
            AllSlices.Add(ProjectionSlice);
        }

        #region SimulationCode

        /// <summary>
        /// Creates offsetX simulated Projection from DensityData
        /// </summary>
        /// <param name="Angle">Desired projection angle in degrees</param>
        /// <returns></returns>
        public virtual ProjectionSlice CreateSimulatedProjection(double Angle)
        {
            double rAngle = Angle / 180d * Math.PI;

            double sTheta = Math.Sin(rAngle);
            double cTheta = Math.Cos(rAngle);

            double rX = ProjectionStartX * cTheta;
            double rY = ProjectionStartX * sTheta;

            PointD Origin = new PointD(ProjectionStartX, ProjectionStartX);
            PointD StepAxis = new PointD(ProjectionStepX, 0);
            PointD Step = new PointD(0, ProjectionStepX);

            Origin = MathHelps.RotatePoint(rAngle, Origin);
            StepAxis = MathHelps.RotatePoint(rAngle, StepAxis);
            Step = MathHelps.RotatePoint(rAngle, Step);


            // double tAngle = Math.Atan2(StepAxis.Y, StepAxis.X);

            double[] Sums = new double[nProjectionSteps];
            double x; double xp;
            double y; double yp;
            double sum;
            for (int i = 0; i < nProjectionSteps; i++)
            {
                sum = 0;
                for (int j = 0; j < nProjectionSteps; j++)
                {
                    x = Origin.X + i * StepAxis.X + j * Step.X;
                    y = Origin.Y + i * StepAxis.Y + j * Step.Y;
                    xp = (x + PhysicalWidth / 2) / PhysicalWidth * DensityGrid.GetLength(0);
                    yp = (y + PhysicalHeight / 2) / PhysicalHeight * DensityGrid.GetLength(1);
                    if ((xp >= 0 && xp < DensityGrid.GetLength(0)) &&
                         (yp >= 0 && yp < DensityGrid.GetLength(1)))
                    {
                        sum += (DensityGrid[(int)xp, (int)yp]);
                    }
                }
                Sums[i] = sum;
            }

            ProjectionSlice ps = new ProjectionSlice();
            ps.Angle = rAngle;
            ps.Projection = new PhysicalArray(Sums,ProjectionStartX,ProjectionEndX );
          
            return ps;
        }

        /// <summary>
        /// Adds offsetX ellipse to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddEllipse(double CenterX, double CenterY, double MajorAxis, double MinorAxis, double RotationAngle, double DensityChange)
        {
            double Rotation = RotationAngle / 180d * Math.PI;
            double cX = (PhysicalWidth / 2) / (DensityGrid.GetLength(0) / 2);
            double cY = (PhysicalHeight / 2) / (DensityGrid.GetLength(1) / 2);
            for (int i = 0; i < DensityGrid.GetLength(0); i++)
            {
                double x = cX * (i - DensityGrid.GetLength(0) / 2);
                for (int j = 0; j < DensityGrid.GetLength(1); j++)
                {
                    double y = cY * (j - DensityGrid.GetLength(1) / 2);
                    if (MathHelps.IsInsideEllipse(x, y, CenterX, CenterY, MajorAxis, MinorAxis, Rotation))
                    {
                        DensityGrid[i, j] += DensityChange;
                    }
                }
            }
        }

        /// <summary>
        /// Adds offsetX rectangle to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddRectangle(double CenterX, double CenterY, double MajorAxis, double MinorAxis, double RotationAngle, double DensityChange)
        {
            double Rotation = RotationAngle / 180d * Math.PI;
            double cX = (PhysicalWidth / 2) / (DensityGrid.GetLength(0) / 2);
            double cY = (PhysicalHeight / 2) / (DensityGrid.GetLength(1) / 2);
            for (int i = 0; i < DensityGrid.GetLength(0); i++)
            {
                double x = cX * (i - DensityGrid.GetLength(0) / 2);
                for (int j = 0; j < DensityGrid.GetLength(1); j++)
                {
                    double y = cY * (j - DensityGrid.GetLength(1) / 2);
                    if (MathHelps.IsInsideRectangle(x, y, CenterX, CenterY, MajorAxis, MinorAxis, Rotation))
                    {
                        DensityGrid[i, j] += DensityChange;
                    }
                }
            }
        }

        /// <summary>
        /// Adds offsetX rectangle to density grid.  
        /// </summary>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        /// <param name="MajorAxis"></param>
        /// <param name="MinorAxis"></param>
        /// <param name="Rotation">Rotation in degrees</param>
        /// <param name="DensityChange"></param>
        public virtual void AddRectangle(double MajorAxis, double MinorAxis, double DensityChange)
        {

            int sX = (int)((DensityGrid.GetLength(0) / 2) - DensityGrid.GetLength(0) / 2 * MajorAxis);
            int eX = (int)((DensityGrid.GetLength(0) / 2) + DensityGrid.GetLength(0) / 2 * MajorAxis);
            int sY = (int)((DensityGrid.GetLength(1) / 2) - DensityGrid.GetLength(1) / 2 * MinorAxis);
            int eY = (int)((DensityGrid.GetLength(1) / 2) + DensityGrid.GetLength(1) / 2 * MinorAxis);

            for (int i = sX; i < eX; i++)
            {
                for (int j = sY; j < eY; j++)
                {
                    DensityGrid[i, j] += DensityChange;
                }
            }
        }

        /// <summary>
        /// Creates the phantom based on the work of shepp and logan
        /// </summary>
        public void CreateShepAndLogan()
        {
            AddEllipse(0, 0, .92, .69, 90, 2);
            AddEllipse(0, -0.0184, .874, .6624, 90, -0.98);
            AddEllipse(.22, 0, .31, .11, 72, -.2);
            AddEllipse(-.22, 0, .41, .16, 108, -.2);
            AddEllipse(0, .35, .25, .21, 90, .1);
            AddEllipse(0, .01, .046, .046, 0, .1);
            AddEllipse(0, -.01, .046, .046, 0, .1);
            AddEllipse(-.08, -.605, .046, .023, 0, .1);
            AddEllipse(0, -.605, .023, .023, 0, .1);
            AddEllipse(.06, -.605, .046, .023, 90, .1);

        }
        #endregion


        public static double[] CreateStandardImpulseFunction(int nPoints, double PhysicalStep)
        {
            double[] impulse = new double[nPoints];
            double tau = PhysicalStep;
            double halfI = impulse.Length / 2;
            double tauP = Math.PI * tau * Math.PI * tau;
            double offsetP;
            for (int i = 0; i < impulse.Length; i++)
            {
                if (i == halfI)
                    impulse[i] = .25 / tau / tau;
                else if ((i % 2) == 0)
                    impulse[i] = 0;
                else
                {
                    offsetP = i - halfI;
                    impulse[i] = -1 / (offsetP * offsetP * tauP);
                }
            }
            return impulse;
        }


        public void DoBackProjection(double[] impulse)
        {

            double[,] Grid = null;
            double halfX = 0;
            double halfY = 0;
            double xx, yy, rx, ry, t;
            double[] tempBackProjection;
            double[] Ys;
            int L1, L2;

            AllSlices[0].DoBackProjection(impulse);
            Grid = new double[AllSlices[0].BackProjection.Length, AllSlices[0].BackProjection.Length];
            halfX = Grid.GetLength(0) / 2;
            halfY = Grid.GetLength(1) / 2;
            L1 = Grid.GetLength(0);
            L2 = Grid.GetLength(1);
            Ys = new double[L2];

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            unsafe
            {

                fixed (double* pGrid = Grid)
                {
                    double* pGridI;
                    fixed (double* pYs = Ys)
                    {
                        double* pYsI;

                        // for (int TestLoop = 0; TestLoop < 25; TestLoop++)
                        // {
                        for (int i = 0; i < AllSlices.Count; i++)
                        {
                            if (AllSlices[i].BackProjection == null)
                                AllSlices[i].DoBackProjection(1d, impulse);

                            rx = Math.Cos(AllSlices[i].Angle);
                            ry = Math.Sin(AllSlices[i].Angle);
                            tempBackProjection = AllSlices[i].BackProjection.ToDoubleArray ();
                            #region SafeCode
                            /*
                    for (int x = 0; x < Grid.GetLength(0); x++)
                    {
                        xx = x - halfX;
                        for (int y = 0; y < Grid.GetLength(1); y++)
                        {
                            yy = y - halfY;
                            //get t value for projection
                            t = xx * rx + yy * ry;
                            //now center it on the mData
                            t += halfX;
                            if (t > 0 && t < tempBackProjection.Length)
                                Grid[x, y] += tempBackProjection[(int)t];
                        }
                    }*/
                            #endregion

                            for (int y = 0; y < L2; y++)
                                Ys[y] = ry * (y - halfY) + halfY;

                            for (int x = 0; x < L1; x++)
                            {
                                xx = (x - halfX) * rx;
                                pYsI = pYs;
                                pGridI = pGrid + L2 * x;
                                for (int y = 0; y < L2; y++)
                                {
                                    //get t value for projection
                                    t = xx + (*pYsI);
                                    //now center it on the mData
                                    if (t > 0 && t < L1)
                                        *pGridI += tempBackProjection[(int)t];

                                    pGridI++;
                                    pYsI++;
                                }
                            }
                            //  }
                        }
                    }
                }

            }
            System.Diagnostics.Debug.Print(sw.ElapsedTicks.ToString());
            System.Diagnostics.Debug.Print(sw.ElapsedMilliseconds.ToString());
            double[,] reducedGrid = Grid.CenterShortenArray((int)(AllSlices[0].Projection.Length / 1.4));

            DensityGrid =  (reducedGrid);
        }

        public Bitmap ViewObject()
        {
            return DensityGrid.MakeBitmap();
        }

        public Bitmap ViewFFT()
        {
            return FFTGrid.MakeBitmap();
        }



    }
}
