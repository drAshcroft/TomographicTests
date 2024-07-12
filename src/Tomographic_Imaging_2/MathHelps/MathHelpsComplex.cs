using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;


[StructLayout(LayoutKind.Explicit, Size = 16, CharSet = CharSet.Ansi)]
public struct complex
{
    [FieldOffset(0)]
    public double real;
    [FieldOffset(8)]
    public double imag;
    public complex(double real, double imag)
    {
        this.real = real;
        this.imag = imag;
    }
    public double Abs()
    {
        return Math.Sqrt(real * real + imag * imag);
    }

    #region Operators
    public static complex operator +(complex c1, complex c2)
    {
        return new complex(c1.real + c2.real, c1.imag + c2.imag);
    }

    public static complex operator +(complex c1, double c2)
    {
        return new complex(c1.real + c2, c1.imag);
    }

    public static complex operator -(complex c1, complex c2)
    {
        return new complex(c1.real - c2.real, c1.imag - c2.imag);
    }

    public static complex operator -(complex c1, double c2)
    {
        return new complex(c1.real - c2, c1.imag);
    }

    public static complex operator *(complex c1, complex c2)
    {
        return new complex(c1.real * c2.real - c1.imag * c2.imag, c1.real * c2.imag + c1.imag * c2.real);
    }

    public static complex operator *(complex c1, double c2)
    {
        return new complex(c1.real * c2, c1.imag * c2);
    }

    public static complex operator /(complex c1, complex c2)
    {
        double denom = c2.real * c2.real + c2.imag * c2.imag;
        return new complex((c1.real * c2.real + c1.imag * c2.imag) / denom, (c1.real * c2.imag - c1.imag * c2.real) / denom);
    }

    public static complex operator /(complex c1, double c2)
    {
        return new complex(c1.real / c2, c1.imag / c2);
    }

    public static complex operator +(double c2, complex c1)
    {
        return new complex(c1.real + c2, c1.imag);
    }

    public static complex operator -(double c2, complex c1)
    {
        return new complex(c2-c1.real, c1.imag);
    }

    public static complex operator *(double c2, complex c1)
    {
        return new complex(c1.real * c2, c1.imag * c2);
    }

    public static complex operator /(double c2, complex c1)
    {
        double denom = c1.real * c1.real + c1.imag * c1.imag;
        return new complex((c2 * c1.real) / denom, (c2 * c1.imag) / denom);
    }

    public static bool operator ==(double c2, complex c1)
    {
        return (c2 == c1.real && c1.imag==0);
    }
    public static bool operator !=(double c2, complex c1)
    {
        return !(c2 == c1.real && c1.imag == 0);
    }

    public static bool operator ==(complex c1,double c2)
    {
        return (c2 == c1.real && c1.imag == 0);
    }
    public static bool operator !=(complex c1, double c2)
    {
        return !(c2 == c1.real && c1.imag == 0);
    }

    public static bool operator ==(complex c1, complex c2)
    {
        return (c2.real  == c1.real && c1.imag == c2.imag );
    }
    public static bool operator !=(complex c1, complex c2)
    {
        return !(c2.real == c1.real && c1.imag == c2.imag);
    }

    #endregion

    public override string ToString()
    {
        return (String.Format("{0} + {1}i", real, imag));
    }


}

public static  partial  class MathHelps
{

    /// <summary>
    /// Converts offsetX 1D array of doubles to offsetX 1D array of complex with the doubles all becoming the real part
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static complex[] ConvertToComplex(this double[] array)
    {
        complex[] outArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            outArray[i] = new complex(array[i], 0);
        }
        return outArray;
    }

    public static double[] ConvertToDoubleMagnitude(this complex[] array)
    {
        double[] outArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            outArray[i] = array[i].Abs();
        }
        return outArray;

    }

    public static double[] ConvertToDoubleReal(this complex[] array)
    {
        double[] outArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            outArray[i] = array[i].real;
        }
        return outArray;

    }

    public static double[] ConvertToDoubleImag(this complex[] array)
    {
        double[] outArray = new double[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            outArray[i] = array[i].imag;
        }
        return outArray;

    }

    /// <summary>
    /// Converts offsetX 2D array to offsetX intensity bitmap
    /// </summary>
    /// <param name="ImageArray"></param>
    /// <returns></returns>
    public static Bitmap MakeBitmap(this complex[,] ImageArray)
    {
        int iWidth = ImageArray.GetLength(0);
        int iHeight = ImageArray.GetLength(1);
        double iMax = -10000;
        double iMin = 10000;

        for (int i = 0; i < iWidth; i++)
            for (int j = 0; j < iHeight; j++)
            {
                if (iMax < ImageArray[i, j].Abs()) iMax = ImageArray[i, j].Abs();
                if (iMin > ImageArray[i, j].Abs()) iMin = ImageArray[i, j].Abs();
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
                    int g = (int)(255d * (ImageArray[x, iHeight - y].Abs() - iMin) / iLength);
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

    public static Bitmap MakeBitmap(this complex[, ,] ImageArray, int ZIndex)
    {
        int iWidth = ImageArray.GetLength(0);
        int iHeight = ImageArray.GetLength(1);
        double iMax = -10000;
        double iMin = 10000;

        for (int i = 0; i < iWidth; i++)
            for (int j = 0; j < iHeight; j++)
            {
                if (iMax < ImageArray[i, j, ZIndex].Abs()) iMax = ImageArray[i, j, ZIndex].Abs();
                if (iMin > ImageArray[i, j, ZIndex].Abs()) iMin = ImageArray[i, j, ZIndex].Abs();
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
                    int g = (int)(255d * (ImageArray[x, iHeight - y, ZIndex].Abs() - iMin) / iLength);
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
    /// Converts offsetX 2D array to offsetX intensity bitmap
    /// </summary>
    /// <param name="ImageArray"></param>
    /// <returns></returns>
    public static Bitmap MakeBitmapReal(this complex[,] ImageArray)
    {
        int iWidth = ImageArray.GetLength(0);
        int iHeight = ImageArray.GetLength(1);
        double iMax = -10000;
        double iMin = 10000;

        for (int i = 0; i < iWidth; i++)
            for (int j = 0; j < iHeight; j++)
            {
                if (iMax < ImageArray[i, j].real) iMax = ImageArray[i, j].real;
                if (iMin > ImageArray[i, j].real) iMin = ImageArray[i, j].real;
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
                    int g = (int)(255d * (ImageArray[x, iHeight - y].real - iMin) / iLength);
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
    /// Converts offsetX 2D array to offsetX intensity bitmap
    /// </summary>
    /// <param name="ImageArray"></param>
    /// <returns></returns>
    public static Bitmap MakeBitmapImag(this complex[,] ImageArray)
    {
        int iWidth = ImageArray.GetLength(0);
        int iHeight = ImageArray.GetLength(1);
        double iMax = -10000;
        double iMin = 10000;

        for (int i = 0; i < iWidth; i++)
            for (int j = 0; j < iHeight; j++)
            {
                if (iMax < ImageArray[i, j].imag) iMax = ImageArray[i, j].imag;
                if (iMin > ImageArray[i, j].imag) iMin = ImageArray[i, j].imag;
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
                    int g = (int)(255d * (ImageArray[x, iHeight - y].imag - iMin) / iLength);
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

    public static complex[] MakeFFTHumanReadable(this complex[] MachineArray)
    {
        int d1 = MachineArray.Length;
        complex[] HumanReadable = new complex[d1];
        int Length = d1;
        int hLength = d1 / 2;
        for (int i = 0; i < hLength; i++)
        {
            HumanReadable[i] = MachineArray[hLength + i];
            HumanReadable[i + hLength] = MachineArray[i];
        }
        return HumanReadable;
    }

    #region Array_Arithmetic
    public static void AddInPlace(this complex[] array, double addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += addValue;
        }
    }

    public static complex[] AddToArray(this complex[] array, double addValue)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] + addValue;
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[] array, double addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] -= addValue;
        }
    }

    public static complex[] SubtractFromArray(this complex[] array, double addValue)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] - addValue;
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[] array, double Multiplicant)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] *= Multiplicant;
        }
    }

    public static complex[] MultiplyToArray(this complex[] array, double Multiplicant)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] * Multiplicant;
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[] array, double Divisor)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= Divisor;
        }
    }

    public static complex[] DivideToArray(this complex[] array, double Divisor)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] / Divisor;
        }
        return OutArray;
    }

    public static void AddInPlace(this complex[] array, complex[] addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += addValue[i];
        }
    }

    public static complex[] AddToArray(this complex[] array, complex[] addValue)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] + addValue[i];
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[] array, complex[] addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] -= addValue[i];
        }
    }

    public static complex[] SubtractFromArray(this complex[] array, complex[] addValue)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] - addValue[i];
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[] array, complex[] Multiplicant)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] *= Multiplicant[i];
        }
    }

    public static complex[] MultiplyToArray(this complex[] array, complex[] Multiplicant)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] * Multiplicant[i];
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[] array, complex[] Divisor)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= Divisor[i];
        }
    }

    public static complex[] DivideToArray(this complex[] array, complex[] Divisor)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] / Divisor[i];
        }
        return OutArray;
    }

    public static void AddInPlace(this complex[,] array, double addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue;
            }
        }
    }

    public static complex[,] AddToArray(this complex[,] array, double addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue;
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[,] array, double addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue;
            }
        }
    }

    public static complex[,] SubtractFromArray(this complex[,] array, double addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue;
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[,] array, double Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant;
            }
        }
    }

    public static complex[,] MultiplyToArray(this complex[,] array, double Multiplicant)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant;
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[,] array, double Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor;
            }
        }
    }

    public static complex[,] DivideToArray(this complex[,] array, double Divisor)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] / Divisor;
            }
        }
        return OutArray;
    }

    public static void AddInPlace(this complex[,] array, complex[,] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue[i, j];
            }
        }
    }

    public static complex[,] AddToArray(this complex[,] array, complex[,] addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue[i, j];
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[,] array, complex[,] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue[i, j];
            }
        }
    }

    public static complex[,] SubtractFromArray(this complex[,] array, complex[,] addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue[i, j];
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[,] array, complex[,] Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant[i, j];
            }
        }
    }

    public static complex[,] MultiplyToArray(this complex[,] array, complex[,] Multiplicant)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant[i, j];
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[,] array, complex[,] Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor[i, j];
            }
        }
    }

    public static complex[,] DivideToArray(this complex[,] array, complex[,] Divisor)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] / Divisor[i, j];
            }
        }
        return OutArray;
    }

    public static void AddInPlace(this complex[,] array, complex[] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue[i];
            }
        }
    }

    public static complex[,] AddToArray(this complex[,] array, complex[] addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue[i];
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[,] array, complex[] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue[i];
            }
        }
    }

    public static complex[,] SubtractFromArray(this complex[,] array, complex[] addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue[i];
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[,] array, complex[] Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant[i];
            }
        }
    }

    public static complex[,] MultiplyToArray(this complex[,] array, complex[] Multiplicant)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant[i];
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[,] array, complex[] Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor[i];
            }
        }
    }

    public static complex[,] DivideToArray(this complex[,] array, complex[] Divisor)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] / Divisor[i];
            }
        }
        return OutArray;
    }
    
    public static void AddInPlace(this complex[] array, complex addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += addValue;
        }
    }

    public static complex[] AddToArray(this complex[] array, complex addValue)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] + addValue;
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[] array, complex addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] -= addValue;
        }
    }

    public static complex[] SubtractFromArray(this complex[] array, complex addValue)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] - addValue;
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[] array, complex Multiplicant)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] *= Multiplicant;
        }
    }

    public static complex[] MultiplyToArray(this complex[] array, complex Multiplicant)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] * Multiplicant;
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[] array, complex Divisor)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= Divisor;
        }
    }

    public static complex[] DivideToArray(this complex[] array, complex Divisor)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] / Divisor;
        }
        return OutArray;
    }

    public static void AddInPlace(this complex[] array, double[] addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] += addValue[i];
        }
    }

    public static complex[] AddToArray(this complex[] array, double[] addValue)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] + addValue[i];
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[] array, double[] addValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] -= addValue[i];
        }
    }

    public static complex[] SubtractFromArray(this complex[] array, double[] addValue)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] - addValue[i];
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[] array, double[] Multiplicant)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] *= Multiplicant[i];
        }
    }

    public static complex[] MultiplyToArray(this complex[] array, double[] Multiplicant)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] * Multiplicant[i];
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[] array, double[] Divisor)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= Divisor[i];
        }
    }

    public static complex[] DivideToArray(this complex[] array, double[] Divisor)
    {
        complex[] OutArray = new complex[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            OutArray[i] = array[i] / Divisor[i];
        }
        return OutArray;
    }

    public static void AddInPlace(this complex[,] array, complex addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue;
            }
        }
    }

    public static complex[,] AddToArray(this complex[,] array, complex addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue;
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[,] array, complex addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue;
            }
        }
    }

    public static complex[,] SubtractFromArray(this complex[,] array, complex addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue;
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[,] array, complex Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant;
            }
        }
    }

    public static complex[,] MultiplyToArray(this complex[,] array, complex Multiplicant)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant;
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[,] array, complex Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor;
            }
        }
    }

    public static complex[,] DivideToArray(this complex[,] array, complex Divisor)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] / Divisor;
            }
        }
        return OutArray;
    }

    public static void AddInPlace(this complex[,] array, double[,] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue[i, j];
            }
        }
    }

    public static complex[,] AddToArray(this complex[,] array, double[,] addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue[i, j];
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[,] array, double[,] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue[i, j];
            }
        }
    }

    public static complex[,] SubtractFromArray(this complex[,] array, double[,] addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue[i, j];
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[,] array, double[,] Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant[i, j];
            }
        }
    }

    public static complex[,] MultiplyToArray(this complex[,] array, double[,] Multiplicant)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant[i, j];
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[,] array, double[,] Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor[i, j];
            }
        }
    }

    public static complex[,] DivideToArray(this complex[,] array, double[,] Divisor)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] / Divisor[i, j];
            }
        }
        return OutArray;
    }

    public static void AddInPlace(this complex[,] array, double[] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] += addValue[i];
            }
        }
    }

    public static complex[,] AddToArray(this complex[,] array, double[] addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] + addValue[i];
            }
        }
        return OutArray;
    }

    public static void SubtractInPlace(this complex[,] array, double[] addValue)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] -= addValue[i];
            }
        }
    }

    public static complex[,] SubtractFromArray(this complex[,] array, double[] addValue)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] - addValue[i];
            }
        }
        return OutArray;
    }

    public static void MultiplyInPlace(this complex[,] array, double[] Multiplicant)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] *= Multiplicant[i];
            }
        }
    }

    public static complex[,] MultiplyToArray(this complex[,] array, double[] Multiplicant)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                OutArray[i, j] = array[i, j] * Multiplicant[i];
            }
        }
        return OutArray;
    }

    public static void DivideInPlace(this complex[,] array, double[] Divisor)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] /= Divisor[i];
            }
        }
    }

    public static complex[,] DivideToArray(this complex[,] array, double[] Divisor)
    {
        complex[,] OutArray = new complex[array.GetLength(0), array.GetLength(1)];
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
    /// Performs offsetX full(slow) convolution of two arrays
    /// </summary>
    /// <param name="Array1"></param>
    /// <param name="Array2"></param>
    /// <returns></returns>
    public static complex[] Convolute(complex[] Array1, double[] Array2)
    {
        complex[] ArrayOut = new complex[Array1.Length + Array2.Length];
        int L1 = Array1.Length;
        int L2 = Array2.Length;

        unsafe
        {
            complex p1;
            double* p2;
            complex* pOut;
            fixed (double* pArray2 = Array2)
            {
                fixed (complex* pArrayOut = ArrayOut)
                {
                    for (int i = 0; i < L1; i++)
                    {
                        p1 = Array1[i];
                        p2 = pArray2;
                        pOut = pArrayOut + i;
                        for (int j = 0; j < L2; j++)
                        {
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
    public static complex[] ConvoluteChop(complex[] Array1, double[] Array2)
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
        complex[] ArrayOut = new complex[Length];

        Length2 = Array1.Length + Array2.Length;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;


        int L1 = Array1.Length;
        int L2 = Array2.Length;
        int sI, eI;

        unsafe
        {
            complex p1;
            double* p2;
            complex* pOut;
            fixed (double* pArray2 = Array2)
            {
                fixed (complex* pArrayOut = ArrayOut)
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

    public static complex[,] ConvoluteChop1D(Axis DesiredAxis, complex[,] Array1, double[] Array2)
    {
        if (DesiredAxis == Axis.XAxis)
            return ConvoluteChopXAxis(Array1, Array2);
        else
            return ConvoluteChopYAxis(Array1, Array2);
    }

    private static complex[,] ConvoluteChopXAxis(complex[,] Array1, double[] Array2)
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
        complex[,] ArrayOut = new complex[Length, Array1.GetLength(1)];

        Length2 = L1 + L2;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;

        int Array1LY = Array1.GetLength(1);
        int sI, eI;
        unsafe
        {
            complex p1;
            double* p2;
            complex* pOut;
            fixed (double* pArray2 = Array2)
            {
                fixed (complex* pArrayOut = ArrayOut)
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
                                pOut = pArrayOut + m;
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
    private static complex[,] ConvoluteChopYAxis(complex[,] Array1, double[] Array2)
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
        complex[,] ArrayOut = new complex[Array1.GetLength(0), Length];

        Length2 = L1 + L2;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;

        int Array1LY = Array1.GetLength(0);
        int sI, eI;
        unsafe
        {
            complex p1;
            double* p2;
            complex* pOut;
            fixed (double* pArray2 = Array2)
            {
                fixed (complex* pArrayOut = ArrayOut)
                {
                    for (int m = 0; m < Array1.GetLength(0); m++)
                    {
                        for (int i = 0; i < L1; i++)
                        {
                            p1 = Array1[m, i];
                            sI = StartI - i;
                            eI = EndI - i;
                            if (eI > L2) eI = L2;
                            if (sI < 0) sI = 0;
                            if (sI < eI)
                            {
                                p2 = pArray2 + sI;
                                pOut = pArrayOut + m * Array1LY;
                                for (int j = sI; j < eI; j++)
                                {
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
    public static complex[] ConvoluteChopSlow(complex[] Array1, double[] Array2)
    {
        complex[] ArrayOut = Convolute(Array1, Array2);

        int Length;

        if (Array1.Length < Array2.Length)
        {
            Length = Array1.Length;
        }
        else
        {
            Length = Array2.Length;
        }
        complex[] ArrayOut2 = new complex[Length];
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
    /// Performs offsetX full(slow) convolution of two arrays
    /// </summary>
    /// <param name="Array1"></param>
    /// <param name="Array2"></param>
    /// <returns></returns>
    public static complex[] Convolute(complex[] Array1, complex[] Array2)
    {
        complex[] ArrayOut = new complex[Array1.Length + Array2.Length];
        int L1 = Array1.Length;
        int L2 = Array2.Length;

        unsafe
        {
            complex p1;
            complex* p2;
            complex* pOut;
            fixed (complex* pArray2 = Array2)
            {
                fixed (complex* pArrayOut = ArrayOut)
                {
                    for (int i = 0; i < L1; i++)
                    {
                        p1 = Array1[i];
                        p2 = pArray2;
                        pOut = pArrayOut + i;
                        for (int j = 0; j < L2; j++)
                        {
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
    public static complex[] ConvoluteChop(complex[] Array1, complex[] Array2)
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
        complex[] ArrayOut = new complex[Length];

        Length2 = Array1.Length + Array2.Length;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;


        int L1 = Array1.Length;
        int L2 = Array2.Length;
        int sI, eI;

        unsafe
        {
            complex p1;
            complex* p2;
            complex* pOut;
            fixed (complex* pArray2 = Array2)
            {
                fixed (complex* pArrayOut = ArrayOut)
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

    public static complex[,] ConvoluteChop1D(Axis DesiredAxis, complex[,] Array1, complex[] Array2)
    {
        if (DesiredAxis == Axis.XAxis)
            return ConvoluteChopXAxis(Array1, Array2);
        else
            return ConvoluteChopYAxis(Array1, Array2);
    }

    private static complex[,] ConvoluteChopXAxis(complex[,] Array1, complex[] Array2)
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
        complex[,] ArrayOut = new complex[Length, Array1.GetLength(1)];

        Length2 = L1 + L2;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;

        int Array1LY = Array1.GetLength(1);
        int sI, eI;
        unsafe
        {
            complex p1;
            complex* p2;
            complex* pOut;
            fixed (complex* pArray2 = Array2)
            {
                fixed (complex* pArrayOut = ArrayOut)
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
                                pOut = pArrayOut + m;
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
    private static complex[,] ConvoluteChopYAxis(complex[,] Array1, complex[] Array2)
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
        complex[,] ArrayOut = new complex[Array1.GetLength(0), Length];

        Length2 = L1 + L2;

        int StartI = Length2 / 2 - Length / 2;
        int EndI = Length2 / 2 + Length / 2;

        int Array1LY = Array1.GetLength(0);
        int sI, eI;
        unsafe
        {
            complex p1;
            complex* p2;
            complex* pOut;
            fixed (complex* pArray2 = Array2)
            {
                fixed (complex* pArrayOut = ArrayOut)
                {
                    for (int m = 0; m < Array1.GetLength(0); m++)
                    {
                        for (int i = 0; i < L1; i++)
                        {
                            p1 = Array1[m, i];
                            sI = StartI - i;
                            eI = EndI - i;
                            if (eI > L2) eI = L2;
                            if (sI < 0) sI = 0;
                            if (sI < eI)
                            {
                                p2 = pArray2 + sI;
                                pOut = pArrayOut + m * Array1LY;
                                for (int j = sI; j < eI; j++)
                                {
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
    public static complex[] ConvoluteChopSlow(complex[] Array1, complex[] Array2)
    {
        complex[] ArrayOut = Convolute(Array1, Array2);

        int Length;

        if (Array1.Length < Array2.Length)
        {
            Length = Array1.Length;
        }
        else
        {
            Length = Array2.Length;
        }
        complex[] ArrayOut2 = new complex[Length];
        int cc = 0;
        int Length2 = ArrayOut.Length / 2 + Length / 2;
        for (int i = (int)(ArrayOut.Length / 2 - Length / 2); i < Length2; i++)
        {
            ArrayOut2[cc] = ArrayOut[i];
            cc++;
        }

        return ArrayOut2;
    }
}
