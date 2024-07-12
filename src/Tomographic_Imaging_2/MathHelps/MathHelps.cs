using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

#region Useful_Structs
public struct PointD
{
    public double X;
    public double Y;
    public PointD(double x, double y)
    {
        X = x;
        Y = y;
    }
}


#endregion

public static partial class MathHelps
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Angle">rotation in radians</param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static PointD RotatePoint(double Angle, PointD point)
    {
        return RotatePoint(Angle, point.X, point.Y);
    }

    /// <summary>
    /// Rotation in radians
    /// </summary>
    /// <param name="Angle"></param>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>
    public static PointD RotatePoint(double Angle, double X, double Y)
    {
        double c = Math.Cos(Angle);
        double s = Math.Sin(Angle);
        return new PointD(c * X - s * Y, s * X + c * Y);
    }



    /// <summary>
    /// Normalizes offsetX point as if it is offsetX vector from (0,0)
    /// </summary>
    /// <param name="inPoint"></param>
    /// <returns></returns>
    public static PointD NormalizePoint(PointD inPoint)
    {
        double r = Math.Sqrt(inPoint.X * inPoint.X + inPoint.Y * inPoint.Y);
        return new PointD(inPoint.X / r, inPoint.Y / r);
    }

    /// <summary>
    /// returns true is point (x,y) is within the ellipse defined by center (centerX,centerY) with specified axis and rotation
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="CenterX"></param>
    /// <param name="CenterY"></param>
    /// <param name="MajorAxis"></param>
    /// <param name="MinorAxis"></param>
    /// <param name="Rotation">Rotation is in radians</param>
    /// <returns></returns>
    public static bool IsInsideEllipse(double x, double y, double CenterX, double CenterY, double MajorAxis, double MinorAxis, double Rotation)
    {
        PointD newPoint = new PointD(x - CenterX, y - CenterY);
        newPoint = RotatePoint(-1 * Rotation, newPoint);
        double xp = newPoint.X / MajorAxis;
        double yp = newPoint.Y / MinorAxis;
        if (xp * xp + yp * yp <= 1)
            return true;
        else
            return false;


    }



    /// <summary>
    /// returns true is point (x,y) is within the ellipse defined by center (centerX,centerY) with specified axis and rotation
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="CenterX"></param>
    /// <param name="CenterY"></param>
    /// <param name="MajorAxis"></param>
    /// <param name="MinorAxis"></param>
    /// <param name="Rotation">Rotation is in radians</param>
    /// <returns></returns>
    public static bool IsInsideRectangle(double x, double y, double CenterX, double CenterY, double MajorAxis, double MinorAxis, double Rotation)
    {
        PointD newPoint = new PointD(x - CenterX, y - CenterY);
        newPoint = RotatePoint(-1 * Rotation, newPoint);
        double xp = Math.Abs(newPoint.X / MajorAxis);
        double yp = Math.Abs(newPoint.Y / MinorAxis);

        if (xp <= 1 && yp <= 1)
            return true;
        else
            return false;


    }


    /// <summary>
    /// Performs offsetX full(slow) convolution of two arrays
    /// </summary>
    /// <param name="Array1"></param>
    /// <param name="Array2"></param>
    /// <returns></returns>
    public static double[] Convolute(double[] Array1, double[] Array2)
    {
        double[] ArrayOut = new double[Array1.Length + Array2.Length];
        int L1 = Array1.Length;
        int L2 = Array2.Length;

        unsafe
        {
            double p1;
            double* p2;
            double* pOut;
            fixed (double* pArray2 = Array2)
            {
                fixed (double* pArrayOut = ArrayOut)
                {
                    for (int i = 0; i < L1; i++)
                    {
                        p1 = Array1[i];
                        p2 = pArray2;
                        pOut = pArrayOut + i;
                        for (int j = 0; j < L2; j++)
                        {
                            //ArrayOut[i + j] += p1 * (*p2);
                            *pOut += p1 * (*p2);
                            p2++;
                            pOut++;
                        }
                    }
                }
            }
        }
        return ArrayOut;
    }

    /// <summary>
    /// returns an array with length equal to the shorter array (results are centered)
    /// </summary>
    /// <param name="Array1"></param>
    /// <param name="Array2"></param>
    /// <returns></returns>
    public static double[] ConvoluteChop(double[] Array1, double[] Array2)
    {
        int Length;
        int Length2;

        if (Array1.Length < Array2.Length)
        {
            Length = Array1.Length;
        }
        else
        {
            Length = Array2.Length;
        }
        double[] ArrayOut = new double[Length];

        Length2 = Array1.Length + Array2.Length;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;


        int L1 = Array1.Length;
        int L2 = Array2.Length;
        int sI, eI;

        unsafe
        {
            double p1;
            double* p2;
            double* pOut;
            fixed (double* pArray2 = Array2)
            {
                fixed (double* pArrayOut = ArrayOut)
                {
                    for (int i = 0; i < L1; i++)
                    {
                        p1 = Array1[i];
                        sI = StartI - i;
                        eI = EndI - i;
                        if (eI > L2) eI = L2;
                        if (sI < 0) sI = 0;
                        if (sI < eI)
                        {
                            p2 = pArray2 + sI;
                            pOut = pArrayOut + i + sI - StartI;
                            for (int j = sI; j < eI; j++)
                            {
                                *pOut += p1 * (*p2);
                                pOut++;
                                p2++;
                            }
                        }
                    }
                }
            }
        }
        return ArrayOut;
    }

    public static double[,] ConvoluteChop1D(Axis DesiredAxis, double[,] Array1, double[] Array2)
    {
        if (DesiredAxis == Axis.XAxis)
            return ConvoluteChopXAxis(Array1, Array2);
        else
            return ConvoluteChopYAxis(Array1, Array2);
    }

    private static double[,] ConvoluteChopXAxis(double[,] Array1, double[] Array2)
    {
        int L1 = Array1.GetLength(0);
        int L2 = Array2.Length;

        int Length;
        int Length2;

        if (L1 < L2)
        {
            Length = L1;
        }
        else
        {
            Length = L2;
        }
        double[,] ArrayOut = new double[Length, Array1.GetLength(1)];

        Length2 = L1 + L2;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;

        int Array1LY = Array1.GetLength(1);
        int sI, eI;
        unsafe
        {
            double p1;
            double* p2;
            double* pOut;
            fixed (double* pArray2 = Array2)
            {
                fixed (double* pArrayOut = ArrayOut)
                {
                    for (int m = 0; m < Array1.GetLength(1); m++)
                    {
                        for (int i = 0; i < L1; i++)
                        {
                            p1 = Array1[i, m];
                            sI = StartI - i;
                            eI = EndI - i;
                            if (eI > L2) eI = L2;
                            if (sI < 0) sI = 0;
                            if (sI < eI)
                            {
                                p2 = pArray2 + sI;
                                pOut = pArrayOut+m ;
                                for (int j = sI; j < eI; j++)
                                {
                                    //ArrayOut[i + j - StartI, m] += p1 * (*p2);
                                    *pOut += p1 * (*p2);
                                    p2++;
                                    pOut += Array1LY;
                                }
                            }
                        }
                    }
                }
            }
        }
        return ArrayOut;
    }
    private static double[,] ConvoluteChopYAxis(double[,] Array1, double[] Array2)
    {
        int L1 = Array1.GetLength(1);
        int L2 = Array2.Length;

        int Length;
        int Length2;

        if (L1 < L2)
        {
            Length = L1;
        }
        else
        {
            Length = L2;
        }
        double[,] ArrayOut = new double[ Array1.GetLength(0),Length ];

        Length2 = L1 + L2;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;

        int Array1LY = Array1.GetLength(0);
        int sI, eI;
        unsafe
        {
            double p1;
            double* p2;
            double* pOut;
            fixed (double* pArray2 = Array2)
            {
                fixed (double* pArrayOut = ArrayOut)
                {
                    for (int m = 0; m < Array1.GetLength(0); m++)
                    {
                        for (int i = 0; i < L1; i++)
                        {
                            p1 = Array1[m,i];
                            sI = StartI - i;
                            eI = EndI - i;
                            if (eI > L2) eI = L2;
                            if (sI < 0) sI = 0;
                            if (sI < eI)
                            {
                                p2 = pArray2 + sI;
                                pOut = pArrayOut + m*Array1LY ;
                                for (int j = sI; j < eI; j++)
                                {
                                    //ArrayOut[i + j - StartI, m] += p1 * (*p2);
                                    *pOut += p1 * (*p2);
                                    p2++;
                                    pOut++;
                                }
                            }
                        }
                    }
                }
            }
        }
        return ArrayOut;
    }
    /// <summary>
    /// returns an array with length equal to the shorter array (results are centered)
    /// </summary>
    /// <param name="Array1"></param>
    /// <param name="Array2"></param>
    /// <returns></returns>
    public static double[] ConvoluteChopSlow(double[] Array1, double[] Array2)
    {
        double[] ArrayOut = Convolute(Array1, Array2);

        int Length;

        if (Array1.Length < Array2.Length)
        {
            Length = Array1.Length;
        }
        else
        {
            Length = Array2.Length;
        }
        double[] ArrayOut2 = new double[Length];
        int cc = 0;
        int Length2 = ArrayOut.Length / 2 + Length / 2;
        for (int i = (int)(ArrayOut.Length / 2 - Length / 2); i < Length2; i++)
        {
            ArrayOut2[cc] = ArrayOut[i];
            cc++;
        }

        return ArrayOut2;
    }


    /// <summary>
    /// Normalizes the second dimension of the array to 1 or -1 as the greatest value
    /// </summary>
    /// <param name="array"></param>
    public static void Normalize2DArray(this double[,] array)
    {
        double Max = double.MinValue;
        for (int i = 0; i < array.GetLength(1); i++)
        {
            if (Math.Abs(array[1, i]) > Max) Max = Math.Abs(array[1, i]);
        }
        for (int i = 0; i < array.GetLength(1); i++)
        {
            array[1, i] = array[1, i] / Max;
        }
    }

    /// <summary>
    /// Normalizes the array to 1 or -1 as the greatest value
    /// </summary>
    /// <param name="array"></param>
    public static void Normalize1DArray(this double[] array)
    {
        double Max = double.MinValue;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > Max) Max = array[i];
        }
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = array[i] / Max;
        }
    }

    /// <summary>
    /// Adds the specified value to all the elements of an array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="yShift"></param>
    public static void ShiftArray(this double[] array, double yShift)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = array[i] + yShift;
        }
    }

    /// <summary>
    /// Adds the specified yvalue to all the elements of the second dimension and the 
    /// specifed xvalue to all the elements of the 1st dimension
    /// </summary>
    /// <param name="array"></param>
    /// <param name="yShift"></param>
    /// <param name="xShift"></param>
    public static void ShiftArray(this double[,] array, double yShift, double xShift)
    {
        for (int i = 0; i < array.GetLength(1); i++)
        {
            array[1, i] += yShift;
            array[0, i] += xShift;
        }
    }

    #region ArrayArithmetic
    public static void AddInPlace(this double[] array, double addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += addValue;
        }
    }

    public static double[] AddToArray(this double[] array, double addValue)
    {
        double[] OutArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] + addValue;
        }
        return OutArray;
    }

    public static void SubtractInPlace(this double[] array, double addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] -= addValue;
        }
    }

    public static double[] SubtractFromArray(this double[] array, double addValue)
    {
        double[] OutArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] - addValue;
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this double[] array, double Multiplicant)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] *= Multiplicant;
        }
    }

    public static double[] MultiplyToArray(this double[] array, double Multiplicant)
    {
        double[] OutArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] * Multiplicant;
        }
        return OutArray;
    }

    public static void DivideInPlace(this double[] array, double Divisor)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= Divisor;
        }
    }

    public static double[] DivideToArray(this double[] array, double Divisor)
    {
        double[] OutArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] / Divisor;
        }
        return OutArray;
    }

    public static void AddInPlace(this double[] array, double[] addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += addValue[i];
        }
    }

    public static double[] AddToArray(this double[] array, double[] addValue)
    {
        double[] OutArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] + addValue[i];
        }
        return OutArray;
    }

    public static void SubtractInPlace(this double[] array, double[] addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] -= addValue[i];
        }
    }

    public static double[] SubtractFromArray(this double[] array, double[] addValue)
    {
        double[] OutArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] - addValue[i];
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this double[] array, double[] Multiplicant)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] *= Multiplicant[i];
        }
    }

    public static double[] MultiplyToArray(this double[] array, double[] Multiplicant)
    {
        double[] OutArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] * Multiplicant[i];
        }
        return OutArray;
    }

    public static void DivideInPlace(this double[] array, double[] Divisor)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= Divisor[i];
        }
    }

    public static double[] DivideToArray(this double[] array, double[] Divisor)
    {
        double[] OutArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] / Divisor[i];
        }
        return OutArray;
    }

    public static void AddInPlace(this double[,] array, double addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue;
            }
        }
    }

    public static double[,] AddToArray(this double[,] array, double addValue)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue;
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this double[,] array, double addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue;
            }
        }
    }

    public static double[,] SubtractFromArray(this double[,] array, double addValue)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue;
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this double[,] array, double Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant;
            }
        }
    }

    public static double[,] MultiplyToArray(this double[,] array, double Multiplicant)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant;
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this double[,] array, double Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor;
            }
        }
    }

    public static double[,] DivideToArray(this double[,] array, double Divisor)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] / Divisor;
            }
        }
        return OutArray;
    }

    public static void AddInPlace(this double[,] array, double[,] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue[i, j];
            }
        }
    }

    public static double[,] AddToArray(this double[,] array, double[,] addValue)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue[i, j];
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this double[,] array, double[,] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue[i, j];
            }
        }
    }

    public static double[,] SubtractFromArray(this double[,] array, double[,] addValue)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue[i, j];
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this double[,] array, double[,] Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant[i, j];
            }
        }
    }

    public static double[,] MultiplyToArray(this double[,] array, double[,] Multiplicant)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant[i, j];
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this double[,] array, double[,] Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor[i, j];
            }
        }
    }

    public static double[,] DivideToArray(this double[,] array, double[,] Divisor)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] / Divisor[i, j];
            }
        }
        return OutArray;
    }


    public static void AddInPlace(this double[,] array, double[] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue[i];
            }
        }
    }

    public static double[,] AddToArray(this double[,] array, double[] addValue)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue[i];
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this double[,] array, double[] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue[i];
            }
        }
    }

    public static double[,] SubtractFromArray(this double[,] array, double[] addValue)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue[i];
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this double[,] array, double[] Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant[i];
            }
        }
    }

    public static double[,] MultiplyToArray(this double[,] array, double[] Multiplicant)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant[i];
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this double[,] array, double[] Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor[i];
            }
        }
    }

    public static double[,] DivideToArray(this double[,] array, double[] Divisor)
    {
        double[,] OutArray = new double[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] / Divisor[i];
            }
        }
        return OutArray;
    }
    #endregion

    /// <summary>
    /// Makes the array graphable by setting up an X axis for the array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="MinX"></param>
    /// <param name="StepX"></param>
    /// <returns></returns>
    public static double[,] MakeGraphableArray(this double[] array, double MinX, double StepX)
    {
        double[,] outArray = new double[2, array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            outArray[0, i] = MinX + StepX * i;
            outArray[1, i] = array[i];
        }
        return outArray;
    }

    /// <summary>
    /// Converts offsetX 2D array to offsetX intensity bitmap
    /// </summary>
    /// <param name="ImageArray"></param>
    /// <returns></returns>
    public static Bitmap MakeBitmap(this double[,] ImageArray)
    {
        int iWidth = ImageArray.GetLength(0);
        int iHeight = ImageArray.GetLength(1);
        double iMax = -10000;
        double iMin = 10000;

        for (int i = 0; i < iWidth; i++)
            for (int j = 0; j < iHeight; j++)
            {
                if (iMax < ImageArray[i, j]) iMax = ImageArray[i, j];
                if (iMin > ImageArray[i, j]) iMin = ImageArray[i, j];
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
                    int g = (int)(255d * (ImageArray[x, iHeight - y] - iMin) / iLength);
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




    /// <summary>
    /// Cuts offsetX smaller array out of offsetX bigger array, using the center as the fixed point
    /// </summary>
    /// <param name="array"></param>
    /// <param name="NewLength"></param>
    /// <returns></returns>
    public static double[] CenterShortenArray(this double[] array, int NewLength)
    {
        double[] ArrayOut2 = new double[NewLength];
        int cc = 0;
        int Length2 = array.Length / 2 + NewLength / 2;
        for (int i = (int)(array.Length / 2 - NewLength / 2); i < Length2; i++)
        {
            ArrayOut2[cc] = array[i];
            cc++;
        }
        return ArrayOut2;
    }

    /// <summary>
    /// Cuts offsetX smaller array out of offsetX bigger array, using the center as the fixed point
    /// </summary>
    /// <param name="array"></param>
    /// <param name="NewLength"></param>
    /// <returns></returns>
    public static double[,] CenterShortenArray(this double[,] array, int NewLength)
    {
        if ((array.GetLength(0) == NewLength && array.GetLength(1) == NewLength) || array.GetLength(0) < NewLength || array.GetLength(1) < NewLength)
            return array;
        double[,] ArrayOut2 = new double[NewLength, NewLength];
        int cc = 0;
        int cc2 = 0;
        int Length2 = array.GetLength(1) / 2 + NewLength / 2;
        for (int i = (int)(array.GetLength(1) / 2 - NewLength / 2); i < Length2; i++)
        {
            cc2 = 0;
            for (int j = (int)(array.GetLength(1) / 2 - NewLength / 2); j < Length2; j++)
            {
                ArrayOut2[cc, cc2] = array[i, j];
                cc2++;
            }
            cc++;
        }
        return ArrayOut2;
    }

    /// <summary>
    /// Pads out an array, with the zeros going on the outer edges and the mData remaining in the center
    /// </summary>
    /// <param name="array"></param>
    /// <param name="NewLength"></param>
    /// <returns></returns>
    public static double[] ZeroPadArray(this double[] array, int NewLength)
    {
        double[] ArrayOut2 = new double[NewLength];
        int cc = 0;
        int Length2 = array.Length / 2 + NewLength / 2;
        for (int i = (int)(NewLength / 2 - array.Length / 2); i < Length2; i++)
        {
            ArrayOut2[i] = array[cc];
            cc++;
        }
        return ArrayOut2;
    }

    /// <summary>
    /// Pads out an array, with the zeros going on the outer edges and the mData remaining in the center
    /// </summary>
    /// <param name="array"></param>
    /// <param name="NewLength"></param>
    /// <returns></returns>
    public static double[,] ZeroPadArray(this double[,] array, int NewLength)
    {
        double[,] ArrayOut2 = new double[2, NewLength];
        int cc = 0;
        int Length2 = array.GetLength(1) / 2 + NewLength / 2;
        for (int i = (int)(NewLength / 2 - array.GetLength(1) / 2); i < Length2; i++)
        {
            ArrayOut2[0, i] = array[0, cc];
            ArrayOut2[1, i] = array[1, cc];
            cc++;
        }
        return ArrayOut2;
    }

    public static void DecimateArray(this double[] array, ref double[] Array1, ref double[] Array2)
    {
        int cc = 0;
        for (int i = 0; i < array.Length; i += 2)
        {
            Array1[cc] = array[i];
            cc++;
        }

        cc = 0;
        for (int i = 1; i < array.Length; i += 2)
        {
            Array2[cc] = array[i];
            cc++;
        }
    }
    public static void DecimateArray(this double[] array, ref double[] Array1)
    {
        int cc = 0;
        for (int i = 0; i < array.Length; i += 2)
        {
            Array1[cc] = array[i];
            cc++;
        }
    }

    public static double[] DecimateArray(this double[] array, int Period)
    {
        double[] array1 = new double[(int)(array.Length / Period)];
        int cc = 0;
        for (int i = 0; i < array.Length; i += Period)
        {
            array1[cc] = array[i];
            cc++;
        }
        return array1;
    }


    public static double NearestPowerOf2(int testNumber)
    {
        double denom = Math.Log(testNumber) / Math.Log(2);
        if (denom - Math.Ceiling(denom) == 0)
            return testNumber;
        else
            return Math.Pow(2, Math.Ceiling(denom) + 1);

    }


    public static double[] MakeFFTHumanReadable(this double[] MachineArray)
    {
        int d1 = MachineArray.Length;
        double[] HumanReadable = new double[d1];
        int Length = d1;
        int hLength = d1 / 2;
        for (int i = 0; i < hLength; i++)
        {
            HumanReadable[i] = MachineArray[hLength + i];
            HumanReadable[i + hLength] = MachineArray[i];
        }
        return HumanReadable;
    }

    public static double[,] MakeFFTHumanReadable(this double[,] MachineArray)
    {
        int d1 = MachineArray.GetLength(0);
        int d2 = MachineArray.GetLength(1);
        double[,] HumanReadable = new double[d1, d2];
        int hCols = d1 / 2;
        int hRows = d2 / 2;

        for (int i = 0; i < hCols; i++)
        {
            for (int j = 0; j < hRows; j++)
            {
                HumanReadable[i, j] = MachineArray[i + hCols, j + hRows];
                HumanReadable[i + hCols, j + hRows] = MachineArray[i, j];
                HumanReadable[i + hCols, j] = MachineArray[i, j + hRows];
                HumanReadable[i, j + hRows] = MachineArray[i + hCols, j];
            }
        }

        return HumanReadable;
    }
}

