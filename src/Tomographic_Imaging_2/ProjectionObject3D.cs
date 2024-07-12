using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tomographic_Imaging_2
{
    public class ProjectionObject3D
    {
        protected complex[, ,] FFTGrid;
        protected double[, ,] DensityGrid;

        protected List<ProjectionImage> AllSlices = new List<ProjectionImage>();

        protected double PhysicalXWidth;
        protected double PhysicalYWidth;
        protected double PhysicalHeight;

        protected double ProjectionStartX;
        protected double ProjectionEndX;
        protected double ProjectionStepX;
        protected int nProjectionSteps;

        public bool IsBackProjected = false;

        public double[, ,] ActualDensityGrid
        {
            get { return DensityGrid; }
        }

        public void ClearGrid(bool ProjectionExtendsToCorners, double PhysicalXWidth, double PhysicalYWidth, double PhysicalHeight, int nCols, int nRows, int nZCols)
        {

            //FFTGrid = new complex[nCols, nRows, nZCols];
            DensityGrid = new double[nCols, nRows, nZCols];

            AllSlices = new List<ProjectionImage>();

            this.PhysicalXWidth = PhysicalXWidth;
            this.PhysicalYWidth = PhysicalYWidth;
            this.PhysicalHeight = PhysicalHeight;

            if (ProjectionExtendsToCorners)
            {
                ProjectionStartX = -1 * Math.Sqrt(Math.Pow(PhysicalXWidth / 2d, 2) + PhysicalYWidth * PhysicalYWidth / 4d + Math.Pow(PhysicalHeight / 2d, 2));
                ProjectionEndX = ProjectionStartX * -1;
            }
            else
            {
                ProjectionStartX = -1 * PhysicalXWidth / 2;
                ProjectionEndX = PhysicalXWidth / 2;
            }

            double stepX = PhysicalXWidth / nCols;
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
        public void AddSlice(ProjectionImage ProjectionImage)
        {
            AllSlices.Add(ProjectionImage);
        }

        #region SimulationCode

        public virtual ProjectionImage CreateSimulatedProjection(Axis DesiredRotationAxis, double Angle)
        {
            if (DesiredRotationAxis == Axis.XAxis)
                return CreateSimulatedProjectionXAxis(Angle);
            else if (DesiredRotationAxis == Axis.YAxis)
                return CreateSimulatedProjectionYAxis(Angle);
            else if (DesiredRotationAxis == Axis.ZAxis)
                return CreateSimulatedProjectionZAxis(Angle);
            return null;
        }

        /// <summary>
        /// Creates offsetX simulated Projection from DensityData
        /// </summary>
        /// <param name="Angle">Desired projection angle in degrees</param>
        /// <returns></returns>
        private ProjectionImage CreateSimulatedProjectionYAxis(double Angle)
        {
            double rAngle = Angle / 180d * Math.PI;

            double sTheta = Math.Sin(rAngle);
            double cTheta = Math.Cos(rAngle);

            double rX = ProjectionStartX * cTheta;
            double rZ = ProjectionStartX * sTheta;

            double[,] PImage = new double[nProjectionSteps, DensityGrid.GetLength(1)];

            PointD Origin = new PointD(ProjectionStartX, ProjectionStartX);
            PointD StepAxis = new PointD(ProjectionStepX, 0);
            PointD Step = new PointD(0, ProjectionStepX);

            Origin = MathHelps.RotatePoint(rAngle, Origin);
            StepAxis = MathHelps.RotatePoint(rAngle, StepAxis);
            Step = MathHelps.RotatePoint(rAngle, Step);

            double x; double xp;
            double z; double zp;

            double x1, z1;
            double sum;

            double angle1 = Math.Atan2(Step.Y, Step.X);
            if (Step.Y < 0 && Step.X > 0)
                angle1 += Math.PI * 3 / 2;
            else if (Step.Y > 0 && Step.X < 0)
                angle1 += Math.PI / 2;
            else if (Step.Y < 0 && Step.X < 0)
                angle1 += Math.PI;

            angle1 = angle1 / Math.PI * 180;


            int LX = DensityGrid.GetLength(0);
            int LY = DensityGrid.GetLength(1);
            int LZ = DensityGrid.GetLength(2);

            double a = 1 / PhysicalXWidth * DensityGrid.GetLength(0);
            double b = 1 / PhysicalHeight * DensityGrid.GetLength(2);
            double pX = PhysicalXWidth / 2d;
            double pZ = PhysicalHeight / 2d;
            for (int yI = 0; yI < LY; yI++)
            {
                for (int i = 0; i < nProjectionSteps; i++)
                {
                    sum = 0;
                    x1 = Origin.X + i * StepAxis.X;
                    z1 = Origin.Y + i * StepAxis.Y;
                    for (int j = 0; j < nProjectionSteps; j++)
                    {
                        x = x1 + j * Step.X;
                        z = z1 + j * Step.Y;

                        xp = (x + pX) * a;
                        zp = (z + pZ) * b;

                        if ((xp >= 1 && xp < LX - 1) &&
                             (zp >=1 && zp < LZ - 1))
                        {
                          
                                int xII = (int)Math.Floor(xp);
                                int zII = (int)Math.Floor(zp);

                                double uX = xp - xII;
                                double uZ = zp - zII;
                                
                                double V1 = DensityGrid[xII, yI, zII] * uX + (1 - uX) * DensityGrid[xII - 1, yI, zII];
                                double V2 = DensityGrid[xII, yI, zII - 1] * uX + (1 - uX) * DensityGrid[xII - 1, yI, zII - 1];

                                sum += V1 * uZ + (1 - uZ) * V2;
                           
                           // sum+=DensityGrid[(int)(Math.Round ( xp)), yI,(int)Math.Round( zp)];
                        }
                    }
                    PImage[i, yI] = sum;
                }
            }

            ProjectionImage ps = new ProjectionImage();
            ps.Angle = rAngle;
            ps.Projection = PImage;
            ps.StartR[0] = ProjectionStartX;
            ps.EndR[0] = ProjectionEndX;
            ps.StartR[1] = -1 * PhysicalYWidth / 2;
            ps.EndR[1] = PhysicalYWidth / 2;

            ps.MidPoint[0] = (int)(-1 * ProjectionStartX / ProjectionStepX);
            ps.MidPoint[1] = (int)(DensityGrid.GetLength(1) / 2);
            return ps;
        }

        private ProjectionImage CreateSimulatedProjectionXAxis(double Angle)
        {
            double rAngle = Angle / 180d * Math.PI;

            double sTheta = Math.Sin(rAngle);
            double cTheta = Math.Cos(rAngle);

            double rY = ProjectionStartX * cTheta;
            double rZ = ProjectionStartX * sTheta;

            double[,] PImage = new double[nProjectionSteps, DensityGrid.GetLength(0)];

            PointD Origin = new PointD(ProjectionStartX, ProjectionStartX);
            PointD StepAxis = new PointD(ProjectionStepX, 0);
            PointD Step = new PointD(0, ProjectionStepX);

            Origin = MathHelps.RotatePoint(rAngle, Origin);
            StepAxis = MathHelps.RotatePoint(rAngle, StepAxis);
            Step = MathHelps.RotatePoint(rAngle, Step);

            double y; double yp;
            double z; double zp;

            double y1, z1;
            double sum;

            int LX = DensityGrid.GetLength(0);
            int LY = DensityGrid.GetLength(1);
            int LZ = DensityGrid.GetLength(2);

            double a = 1 / PhysicalYWidth * DensityGrid.GetLength(1);
            double b = 1 / PhysicalHeight * DensityGrid.GetLength(2);
            double pY = PhysicalYWidth / 2d;
            double pZ = PhysicalHeight / 2d;
            for (int xI = 0; xI < LX; xI++)
            {
                for (int i = 0; i < nProjectionSteps; i++)
                {
                    sum = 0;
                    y1 = Origin.X + i * StepAxis.X;
                    z1 = Origin.Y + i * StepAxis.Y;
                    for (int j = 0; j < nProjectionSteps; j++)
                    {
                        y = y1 + j * Step.X;
                        z = z1 + j * Step.Y;

                        yp = Math.Round((y + pY) * a);
                        zp = Math.Round((z + pZ) * b);

                        if ((yp >= 0 && yp < LY) &&
                             (zp >= 0 && zp < LZ))
                        {
                            sum += (DensityGrid[(int)xI, (int)yp, (int)(zp)]);
                        }
                    }
                    PImage[i, xI] = sum;
                }
            }

            ProjectionImage ps = new ProjectionImage();
            ps.Angle = rAngle;
            ps.Projection = PImage;
            ps.StartR[0] = ProjectionStartX;
            ps.EndR[0] = ProjectionEndX;
            ps.StartR[1] = -1 * PhysicalXWidth / 2;
            ps.EndR[1] = PhysicalXWidth / 2;

            ps.MidPoint[0] = (int)(-1 * ProjectionStartX / ProjectionStepX);
            ps.MidPoint[1] = (int)(DensityGrid.GetLength(0) / 2);
            return ps;
        }

        private ProjectionImage CreateSimulatedProjectionZAxis(double Angle)
        {
            double rAngle = Angle / 180d * Math.PI;

            double sTheta = Math.Sin(rAngle);
            double cTheta = Math.Cos(rAngle);

            double rX = ProjectionStartX * cTheta;
            double rY = ProjectionStartX * sTheta;

            double[,] PImage = new double[nProjectionSteps, DensityGrid.GetLength(2)];

            PointD Origin = new PointD(ProjectionStartX, ProjectionStartX);
            PointD StepAxis = new PointD(ProjectionStepX, 0);
            PointD Step = new PointD(0, ProjectionStepX);

            Origin = MathHelps.RotatePoint(rAngle, Origin);
            StepAxis = MathHelps.RotatePoint(rAngle, StepAxis);
            Step = MathHelps.RotatePoint(rAngle, Step);

            double x; double xp;
            double y; double yp;

            double x1, y1;
            double sum;

            int LX = DensityGrid.GetLength(0);
            int LY = DensityGrid.GetLength(1);
            int LZ = DensityGrid.GetLength(2);

            double a = 1 / PhysicalXWidth * DensityGrid.GetLength(0);
            double b = 1 / PhysicalYWidth * DensityGrid.GetLength(1);
            double pX = PhysicalXWidth / 2d;
            double pY = PhysicalYWidth / 2d;
            for (int zI = 0; zI < LZ; zI++)
            {
                for (int i = 0; i < nProjectionSteps; i++)
                {
                    sum = 0;
                    x1 = Origin.X + i * StepAxis.X;
                    y1 = Origin.Y + i * StepAxis.Y;
                    for (int j = 0; j < nProjectionSteps; j++)
                    {
                        x = x1 + j * Step.X;
                        y = y1 + j * Step.Y;

                        xp = Math.Round((x + pX) * a);
                        yp = Math.Round((y + pY) * b);

                        if ((xp >= 0 && xp < LX) &&
                             (yp >= 0 && yp < LY))
                        {
                            sum += (DensityGrid[(int)(xp), (int)(yp), (int)zI]);
                        }
                    }
                    PImage[i, zI] = sum;
                }
            }

            ProjectionImage ps = new ProjectionImage();
            ps.Angle = rAngle;
            ps.Projection = PImage;
            ps.StartR[0] = ProjectionStartX;
            ps.EndR[0] = ProjectionEndX;
            ps.StartR[1] = -1 * PhysicalHeight / 2;
            ps.EndR[1] = PhysicalHeight / 2;

            ps.MidPoint[0] = (int)(-1 * ProjectionStartX / ProjectionStepX);
            ps.MidPoint[1] = (int)(DensityGrid.GetLength(2) / 2);
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
        public virtual void AddEllipse(double CenterX, double CenterY, double CenterZ, double MajorAxis, double MinorAxis, double RotationAngle, double DensityChange)
        {
            double Rotation = RotationAngle / 180d * Math.PI;
            double cX = (PhysicalXWidth) / ((double)DensityGrid.GetLength(0));
            double cY = (PhysicalYWidth) / ((double)DensityGrid.GetLength(1));
            double cZ = (PhysicalHeight) / ((double)DensityGrid.GetLength(2));

            double aX = Math.Sqrt(Math.Pow(MajorAxis * Math.Cos(Rotation), 2) + Math.Pow(MinorAxis * Math.Sin(Rotation), 2));
            double aY = Math.Sqrt(Math.Pow(MajorAxis * Math.Cos(Rotation + Math.PI / 2), 2) + Math.Pow(MinorAxis * Math.Sin(Rotation + Math.PI / 2), 2));
            double aZ = MinorAxis;

            double hX, hY, hZ;
            hX = DensityGrid.GetLength(0) / 2;
            hY = DensityGrid.GetLength(1) / 2;
            hZ = DensityGrid.GetLength(2) / 2;


            int Sx = (int)(hX + (CenterX - aX) / cX - 1);
            int Ex = (int)(hX + (CenterX + aX) / cX + 1);
            int Sy = (int)(hY + (CenterY - aY) / cY - 1);
            int Ey = (int)(hY + (CenterY + aY) / cY + 1);
            int Sz = (int)(hZ + (CenterZ - aZ) / cZ - 1);
            int Ez = (int)(hZ + (CenterZ + aZ) / cZ + 1);

            double c = Math.Cos(-1 * Rotation);
            double s = Math.Sin(-1 * Rotation);

            double x, y, z, x1, y1;
            double offsetX, offsetY, ccX, ssX, offsetZ;

            offsetX = -1 * (cX * hX + CenterX);
            offsetY = -1 * (cY * hY + CenterY);
            offsetZ = -1 * (cZ * hZ + CenterZ);

            for (int i = Sx; i < Ex; i++)
            {
                x = cX * i + offsetX;
                ccX = c * x;
                ssX = s * x;
                for (int j = Sy; j < Ey; j++)
                {
                    y = cY * j + offsetY;
                    x1 = (ccX - s * y) / MajorAxis;
                    y1 = (ssX + c * y) / MinorAxis;

                    x1 = x1 * x1;
                    y1 = y1 * y1;

                    for (int m = Sz; m <= Ez; m++)
                    {
                        z = (cZ * m + offsetZ) / MinorAxis;
                        if (x1 + y1 + z * z < 1)
                        {
                            DensityGrid[i, j, m] += DensityChange;
                        }
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
            double cX = (PhysicalXWidth / 2) / (DensityGrid.GetLength(0) / 2);
            double cY = (PhysicalYWidth / 2) / (DensityGrid.GetLength(1) / 2);
            double cZ = (PhysicalHeight / 2) / (DensityGrid.GetLength(2) / 2);
            for (int i = 0; i < DensityGrid.GetLength(0); i++)
            {
                double x = cX * (i - DensityGrid.GetLength(0) / 2);
                for (int j = 0; j < DensityGrid.GetLength(1); j++)
                {
                    double y = cY * (j - DensityGrid.GetLength(1) / 2);
                    for (int m = 0; m < DensityGrid.GetLength(2); m++)
                    {
                        double z = cZ * (m - DensityGrid.GetLength(2));
                        if (MathHelps.IsInsideRectangle(x, y, z, CenterX, CenterY, 0, MajorAxis, MinorAxis, Rotation))
                        {
                            DensityGrid[i, j, m] += DensityChange;
                        }
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

            int sZ = (int)((DensityGrid.GetLength(2) / 2) - DensityGrid.GetLength(2) / 2 * MinorAxis);
            int eZ = (int)((DensityGrid.GetLength(2) / 2) + DensityGrid.GetLength(2) / 2 * MinorAxis);

            for (int i = sX; i < eX; i++)
            {
                for (int j = sY; j < eY; j++)
                {
                    for (int m = sZ; m < eZ; m++)
                    {
                        DensityGrid[i, j, m] += DensityChange;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the phantom based on the work of shepp and logan
        /// </summary>
        public void CreateShepAndLogan()
        {
            AddEllipse(0, 0, 0, .92, .69, 90, 2);
            AddEllipse(0, -0.0184, 0, .874, .6624, 90, -0.98);
            AddEllipse(.22, 0, 0, .31, .11, 72, -.2);
            AddEllipse(-.22, 0, 0, .41, .16, 108, -.2);
            AddEllipse(0, .35, 0, .25, .21, 90, .1);
            AddEllipse(0, .01, 0, .046, .046, 0, .1);
            AddEllipse(0, -.01, 0, .046, .046, 0, .1);
            AddEllipse(-.08, -.605, 0, .046, .023, 0, .1);
            AddEllipse(0, -.605, 0, .023, .023, 0, .1);
            AddEllipse(.06, -.605, 0, .046, .023, 90, .1);

        }
        #endregion


        public static double[] CreateStandardImpulseFunction(int nPoints, double PhysicalStep)
        {
            return ProjectionObject.CreateStandardImpulseFunction(nPoints, PhysicalStep);
        }


        public void DoBackProjection(double[] impulse)
        {
            IsBackProjected = true;

            double[, ,] Grid = null;
            double halfX = 0;
            double halfY = 0;
            double xx, yy, rx, ry, t;
            double[,] tempBackProjection;
            double[] Ys;
            int L1, L2;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            for (int i = 0; i < AllSlices.Count; i++)
            {
                if (AllSlices[i].BackProjection == null)
                    AllSlices[i].DoBackProjection(1d, impulse);
            }

            Grid = new double[AllSlices[0].BackProjection.GetLength(1), AllSlices[0].BackProjection.GetLength(0), AllSlices[0].BackProjection.GetLength(0)];

            halfX = Grid.GetLength(1) / 2;
            halfY = Grid.GetLength(2) / 2;
            L1 = Grid.GetLength(1);
            L2 = Grid.GetLength(2);
            Ys = new double[L2];

            unsafe
            {

                fixed (double* pGrid = Grid)
                {
                    double* pGridI;
                    fixed (double* pYs = Ys)
                    {
                        double* pYsI;
                        for (int i = 0; i < AllSlices.Count; i++)
                        {
                            rx = Math.Cos(AllSlices[i].Angle);
                            ry = Math.Sin(AllSlices[i].Angle);
                            tempBackProjection = AllSlices[i].BackProjection;
                            for (int y = 0; y < L2; y++)
                                Ys[y] = ry * (y - halfY) + halfY;

                            for (int m = 0; m < Grid.GetLength(0); m++)
                            {
                                double* pGridII = pGrid + m * L2 * L1;
                                for (int x = 0; x < L1; x++)
                                {
                                    xx = (x - halfX) * rx;
                                    pYsI = pYs;
                                    pGridI = pGridII + L2 * x;
                                    for (int y = 0; y < L2; y++)
                                    {
                                        t = xx + (*pYsI);
                                        if (t > 1 && t < L1-1)
                                        {
                                            int fT =(int)Math.Floor(t);
                                            double uT =  t - fT;
                                           // uT *= -1;
                                            double V1= tempBackProjection[fT, m];
                                            double V2 = tempBackProjection[fT+1, m];
                                            *pGridI += V1*uT+V2*(1-uT);
                                        }
                                        pGridI++;
                                        pYsI++;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            System.Diagnostics.Debug.Print(sw.ElapsedTicks.ToString());
            System.Diagnostics.Debug.Print(sw.ElapsedMilliseconds.ToString());

            DensityGrid = (Grid);
        }

        public Bitmap ViewObject()
        {
            return DensityGrid.MakeBitmap((int)(DensityGrid.GetLength(2) / 2));
        }

        public Bitmap ViewObject(double ZFraction)
        {
            return DensityGrid.MakeBitmap((int)(DensityGrid.GetLength(2) * ZFraction));
        }


        public Bitmap ViewFFT()
        {
            return FFTGrid.MakeBitmap((int)(DensityGrid.GetLength(2) / 2));
        }



    }
}
