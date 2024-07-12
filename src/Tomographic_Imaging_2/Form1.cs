using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using fftwlib;
using ZedGraph;
using MogreTest;

namespace Tomographic_Imaging_2
{
    public partial class Form1 : Form
    {
        ProjectionObject Phantom;
        ProjectionObject Reconstruct;

        ProjectionObject3D Phantom3D;
        ProjectionObject3D Recon3D;

        public Form1()
        {
            InitializeComponent();
        }
        #region TestFFTW
        /*
        private void button1_Click(object sender, EventArgs e)
        {
           int n=512;

            //managed arrays
            complex [] fin, fout;
            double[] finD, foutD;
          
            //pointers to the FFTW plan objects
            IntPtr fplan1, fplan2;

       
            //create two managed arrays, possibly misalinged
            //n*2 because we are dealing with complex numbers
            fin = new complex[n];
            fout = new complex[n];

            finD = new double[n * 2];
            foutD = new double[n * 2];


            double[] Guassian = new double[n];
            
            for (int i = 0; i < n; i++)
            {
                double x = (i - n / 2d) / n;
                Guassian[i] = Math.Exp(-1 * x * x * 1000);
            }

            //fill our arrays with offsetX sawtooth signal
            fin = Guassian.ConvertToComplex();
            for (int i=0;i<n;i++)
                fout[i] = new complex( i%50,0);

            int cc=0;
            for (int i = 0; i < n * 2; i += 2)
            {
                finD[i] =Guassian[cc] ;
                cc++;
            }

            unsafe
            {
                fixed (complex* hin = fin)
                {
                    double* hD = (double*)hin;
                    fixed (double* hinD = finD)
                    {
                        for (int i = 0; i < n * 2; i++)
                        {
                            if (hD[i] != hinD[i])
                                System.Diagnostics.Debug.Print("");

                        }
                    }
                }

            }

            //create offsetX few test transforms

            unsafe
            {
                fixed (complex* hin = fin)
                {
                    fixed (complex* hout = fout)
                    {
                        fplan2 = fftw.dft_1d(n,(IntPtr) hin,(IntPtr) hout,fftw_direction.Forward, fftw_flags.Estimate);
                        fftw.execute(fplan2);
                        fftw.destroy_plan(fplan2);
                    }
                }
            }

            unsafe
            {
                fixed (double * hinD = finD)
                {
                    fixed (double * houtD = foutD)
                    {
                        fplan1 = fftw.dft_1d(n, (IntPtr) hinD ,(IntPtr) houtD,fftw_direction.Forward, fftw_flags.Estimate);
                        fftw.execute(fplan1);
                        fftw.destroy_plan(fplan1);
                    }
                }
            }

            double[,] temp = finD.DecimateArray(2).MakeGraphableArray(0, 1);
            temp.Normalize2DArray();
            GraphLine(temp);

            temp = foutD.DecimateArray(2).MakeGraphableArray(0, 1);
            temp.Normalize2DArray();
            GraphLine2(temp);

            
            double[,] temp = fin.ConvertToDoubleReal().MakeGraphableArray(0, 1);
            temp.Normalize2DArray();
            GraphLine(temp);

            temp = fout.ConvertToDoubleReal().MakeGraphableArray(0, 1);
            temp.Normalize2DArray();
            GraphLine2(temp);

        }
    */
        #endregion
        #region GraphingHelps
        private void GraphLine(double[,] Data)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zg1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Distance";
            myPane.YAxis.Title.Text = "Absorption";

            // Make up some mData points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                double x = Data[0, i];
                double y = Data[1, i];
                list.Add(x, y);
            }

            myPane.CurveList.Clear();

            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurve = myPane.AddCurve("Alpha",
                list, Color.Red, SymbolType.None);
            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;

            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;

            // Enable scrollbars if needed
            zg1.IsShowHScrollBar = true;
            zg1.IsShowVScrollBar = true;
            zg1.IsAutoScrollRange = true;


            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            zg1.AxisChange();
            // Make sure the Graph gets redrawn
            zg1.Invalidate();
        }
        private void GraphLine2(double[,] Data)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zg1.GraphPane;

            // Make up some mData points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                double x = Data[0, i];
                double y = Data[1, i];
                list.Add(x, y);
            }


            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurve = myPane.AddCurve("Alpha2",
                list, Color.Blue, SymbolType.None);
            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            zg1.AxisChange();
            // Make sure the Graph gets redrawn
            zg1.Invalidate();
        }
        private void GraphLine3(double[,] Data)
        {
            // Get offsetX reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = zg1.GraphPane;

            // Make up some mData points based on the Sine function
            PointPairList list = new PointPairList();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                double x = Data[0, i];
                double y = Data[1, i];
                list.Add(x, y);
            }


            // Generate offsetX red curve with diamond symbols, and "Alpha" in the legend
            LineItem myCurve = myPane.AddCurve("Alpha3",
                list, Color.Green, SymbolType.None);
            // Fill the symbols with white
            myCurve.Symbol.Fill = new Fill(Color.White);

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            zg1.AxisChange();
            // Make sure the Graph gets redrawn
            zg1.Invalidate();
        }
        #endregion

        private void bCreatePhantom_Click(object sender, EventArgs e)
        {
            int Size =512;
            Phantom = new ProjectionObject();
            Phantom.ClearGrid(true, 2, 2, Size  , Size );
            Phantom.CreateShepAndLogan();

            Reconstruct = new ProjectionObject();
            Reconstruct.ClearGrid(true, 2, 2, Size , Size );

            Bitmap b=  Phantom.ViewObject();
            pictureBox1.Image = b;
            pictureBox1.Invalidate();
        }

        private void bDoProjections_Click(object sender, EventArgs e)
        {
            double SliceDegree = 0;
            double numSlices =(double) nSlices.Value;
            SliceDegree = 180d / numSlices;

            for (int i = 0; i < numSlices; i++)
            {
                ProjectionSlice Projection = Phantom.CreateSimulatedProjection( SliceDegree * i);
                GraphLine(Projection.Projection.ToDoubleArrayIndexed() );
                Reconstruct.AddSlice(Projection);
                Application.DoEvents();
            }
        }

        private void bBackProjection_Click(object sender, EventArgs e)
        {
            double[] impulse = ProjectionObject.CreateStandardImpulseFunction(1024,.1 );
            Reconstruct.DoBackProjection(impulse);
            Bitmap b = Reconstruct.ViewObject();

            pictureBox1.Image = b;
            pictureBox1.Invalidate();
        }

       
        private void b3DPhantom_Click(object sender, EventArgs e)
        {
            int GridSize =128;
            Phantom3D = new ProjectionObject3D();
            Phantom3D.ClearGrid(false , 2, 2,2, GridSize ,GridSize ,GridSize );
            Phantom3D.CreateShepAndLogan();

            Recon3D = new ProjectionObject3D();
            Recon3D.ClearGrid(false , 2, 2, 2, GridSize, GridSize, GridSize);

            Bitmap b = Phantom3D.ViewObject();
            pictureBox1.Image = b;
            pictureBox1.Invalidate();
            hScrollBar1.Enabled = true;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            double p = (double)hScrollBar1.Value / 100d;

            if (Recon3D != null && Recon3D.IsBackProjected )
            {
                Bitmap b =Recon3D.ViewObject(p);
                pictureBox1.Image = b;
                pictureBox1.Invalidate();
            }
            else if (Phantom3D != null)
            {
                Bitmap b = Phantom3D.ViewObject(p);
                pictureBox1.Image = b;
                pictureBox1.Invalidate();

            }
        }

        private void bDo3dProjections_Click(object sender, EventArgs e)
        {
            double SliceDegree = 0;
            double numSlices = (double)nSlices.Value;
            SliceDegree = 180d / numSlices;

            for (int i = 0; i < numSlices; i++)
            {
                ProjectionImage Projection = Phantom3D.CreateSimulatedProjection(Axis.YAxis  , SliceDegree * i);

                Bitmap b = Projection.ViewProjection();
                pictureBox1.Image = b;
                pictureBox1.Invalidate();
                Application.DoEvents();

                Recon3D.AddSlice(Projection);
                Application.DoEvents();
                label3.Text = "Finished Projection " + (i + 1).ToString();
            }
            label3.Text = "Finished All Projections";
        }

        private void bDo1Projection_Click(object sender, EventArgs e)
        {
            double SliceDegree = 0;
            ProjectionImage Projection = Phantom3D.CreateSimulatedProjection(Axis.YAxis , SliceDegree);

            Projection.DoBackProjection(ProjectionObject.CreateStandardImpulseFunction(1024, .1));

            Bitmap b = Projection.ViewBackProjection();
            pictureBox1.Image = b;
            pictureBox1.Invalidate();
            Application.DoEvents();

            ProjectionSlice ps= Projection.GetSlice(Axis.XAxis, Projection.GetProjectionLength(Axis.YAxis   ) / 2);


            double[,] temp = ps.Projection.ToDoubleArrayIndexed();
            temp.Normalize2DArray();
            GraphLine(temp);
            Application.DoEvents();

            temp  = ps.BackProjection.MakeGraphableArray();
            temp.Normalize2DArray();
            GraphLine2(temp);
        }

        private void bDo3DBackProjection_Click(object sender, EventArgs e)
        {
            Phantom3D = null;

            double[] impulse = ProjectionObject.CreateStandardImpulseFunction(1024, .1);
            Recon3D.DoBackProjection(impulse);
            Bitmap b = Recon3D.ViewObject();

            pictureBox1.Image = b;
            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PhysicalArray P1 = new PhysicalArray(new double[100], 0, 1);
            PhysicalArray P2 = new PhysicalArray(new double[100], 0.5, 2);

            
            double[] xs = P1.GetPhysicalIndicies();
            for (int i = 0; i < P1.Length; i++)
            {
                P1[i] = Math.Sin(xs[i]);
            }

            P1.ZeroPadDataCentered(200);

            xs = P2.GetPhysicalIndicies();
            for (int i = 0; i < P2.Length; i++)
            {
                P2[i] = Math.Sin(xs[i]);
            }

            PhysicalArray P3 = P2 - P1;

            double[,] temp = P3.MakeGraphableArray();
            GraphLine(temp);

            temp = P1.MakeGraphableArray();
            GraphLine2(temp);

            temp = P2.MakeGraphableArray();
            GraphLine3(temp);
        }
        MarchingCubes cubes;
        protected OgreWindow mogreWin;
        private void b3DDisplay_Click(object sender, EventArgs e)
        {
            mogreWin = new OgreWindow(new Point(100, 30), pictureBox1.Handle);
            mogreWin.InitMogre();

            cubes = new MarchingCubes();
            int GridSize = 256;
            Phantom3D = new ProjectionObject3D();
            Phantom3D.ClearGrid(true, 2, 2, 2, GridSize, GridSize, GridSize);
            Phantom3D.CreateShepAndLogan();

            Recon3D = new ProjectionObject3D();
            Recon3D.ClearGrid(true, 1, 2, 2, GridSize, GridSize, GridSize);


            
            cubes.CreateSurface(Phantom3D.ActualDensityGrid, 1, 50, 50, 50);
            cubes.RotateY = Math.PI / 2d;
            //Bitmap b = cubes.drawPoints(pictureBox1.Width, pictureBox1.Height);
 
            //pictureBox1.Image = b;
            //pictureBox1.Invalidate();

            cubes.CenterAndNormalizePoints(25);

            mogreWin.CreateMesh("Sphere1", cubes.VertexList, cubes.TriangleIndexs);

            timer1.Enabled = true;
        }

        private void hAngle_ValueChanged(object sender, EventArgs e)
        {
            render();
        }

        private void hScrollBar3_ValueChanged(object sender, EventArgs e)
        {
            render();
        }

        private void hScrollBar2_ValueChanged(object sender, EventArgs e)
        {
            render();
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            cubes = new MarchingCubes();
            double[, ,] DataPoints = new double[256, 256, 256];
            double mid = (double)DataPoints.GetLength(0) / 2d;
            double r;
            double R = 100;
            R *= R;
            for (int i = 0; i < DataPoints.GetLength(0); i++)
            {
                for (int j = 0; j < DataPoints.GetLength(1); j++)
                {
                    for (int m = 0; m < DataPoints.GetLength(2); m++)
                    {
                        r = ((i - mid) * (i - mid) + (j - mid) * (j - mid) + (m - mid) * (m - mid)) / R;
                        if (r < 1)
                        {
                            DataPoints[i, j, m] = 10;
                        }
                    }
                }
            }
            cubes.CreateSurface(DataPoints, 5, 20, 20, 20);
            render();
            hAngle.Enabled = true;
            hScrollBar2.Enabled = true;
            hScrollBar3.Enabled = true;
        }
        private void render()
        {
            if (mogreWin == null && cubes!=null)
            {
                cubes.RotateY = Math.PI / 2d + (hScrollBar3.Value - 50) / 50d * Math.PI;
                cubes.RotateX = (hAngle.Value - 50) / 50d * Math.PI;
                cubes.RotateZ = (hScrollBar2.Value - 50) / 50d * Math.PI;
                Bitmap b = cubes.drawLines(pictureBox1.Width, pictureBox1.Height);

                pictureBox1.Image = b;
                pictureBox1.Invalidate();
            }
            else
            {

            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (mogreWin != null)
                mogreWin.Paint();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mogreWin != null)
                mogreWin.Paint();
        }

      
    }
}