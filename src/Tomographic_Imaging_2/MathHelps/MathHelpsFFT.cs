using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using fftwlib;


public static partial class MathHelps
{
    public static double[] FFT(double[] array)
    {
        IntPtr fplan2;
        double[] fout = new double[array.Length];
        unsafe
        {
            fixed (double* hin = array)
            {
                fixed (double* hout = fout)
                {
                    fplan2 = fftw.r2r_1d(array.Length, (IntPtr)hin, (IntPtr)hout, fftw_kind.DHT, fftw_flags.Estimate);
                    fftw.execute(fplan2);
                    fftw.destroy_plan(fplan2);
                }
            }
        }
        return fout;
    }
    public static double[] iFFT(double[] array)
    {
        IntPtr fplan2;
        double[] fout = new double[array.Length];
        unsafe
        {
            fixed (double* hin = array)
            {
                fixed (double* hout = fout)
                {
                    fplan2 = fftw.r2r_1d(array.Length, (IntPtr)hin, (IntPtr)hout, fftw_kind.DHT, fftw_flags.Estimate);
                    fftw.execute(fplan2);
                    fftw.destroy_plan(fplan2);
                }
            }
        }
        return fout;
    }
    public static complex[] FFT(complex[] array)
    {
        IntPtr fplan2;
        complex[] fout = new complex[array.Length];
        unsafe
        {
            fixed (complex* hin = array)
            {
                fixed (complex* hout = fout)
                {
                    fplan2 = fftw.dft_1d(array.Length, (IntPtr)hin, (IntPtr)hout, fftw_direction.Forward, fftw_flags.Estimate);
                    fftw.execute(fplan2);
                    fftw.destroy_plan(fplan2);
                }
            }
        }
        return fout;
    }
    public static complex[] iFFT(complex[] array)
    {
        IntPtr fplan2;
        complex[] fout = new complex[array.Length];
        unsafe
        {
            fixed (complex* hin = array)
            {
                fixed (complex* hout = fout)
                {
                    fplan2 = fftw.dft_1d(array.Length, (IntPtr)hin, (IntPtr)hout, fftw_direction.Backward, fftw_flags.Estimate);
                    fftw.execute(fplan2);
                    fftw.destroy_plan(fplan2);
                }
            }
        }
        return fout;
    }
    public static complex[] FFTc(double[] array)
    {
        IntPtr fplan2;
        complex[] fout = new complex[array.Length];
        unsafe
        {
            fixed (double* hin = array)
            {
                fixed (complex* hout = fout)
                {
                    fplan2 = fftw.dft_r2c_1d(array.Length, (IntPtr)hin, (IntPtr)hout, fftw_flags.Estimate);
                    fftw.execute(fplan2);
                    fftw.destroy_plan(fplan2);
                }
            }
        }
        return fout;
    }
    public static double[] iFFTc(complex[] array)
    {
        IntPtr fplan2;
        double[] fout = new double[array.Length];
        unsafe
        {
            fixed (complex* hin = array)
            {
                fixed (double* hout = fout)
                {
                    fplan2 = fftw.dft_c2r_1d(array.Length, (IntPtr)hin, (IntPtr)hout, fftw_flags.Estimate);
                    fftw.execute(fplan2);
                    fftw.destroy_plan(fplan2);
                }
            }
        }
        return fout;
    }

    public static double[] ConvoluteFFT(double[] Function, double[] impulse)
    {
        int Length = 0;
        double[] array1;
        double[] array2;
        double[] arrayOut;
        if (Function.Length > impulse.Length)
            Length = Function.Length;
        else
            Length = impulse.Length;
        if (Function.Length != impulse.Length)
        {
            Length = (int)MathHelps.NearestPowerOf2(Length);
            array1 = Function.ZeroPadArray(Length);
            array2 = impulse.ZeroPadArray(Length);
        }
        else
        {
            array1 = Function;
            array2 = impulse;
        }
        array2.MakeFFTHumanReadable();

        complex[] cArray1 = FFTc(array1);
        complex[] cArray2 = FFTc(array2);

        cArray1.MultiplyInPlace(cArray2);
        arrayOut = iFFTc(cArray1).MakeFFTHumanReadable();
        return arrayOut;
    }
    public static double[] ConvoluteFFT(int nPoints, double[] Function, double[] impulse)
    {
        int Length = nPoints;
        double[] array1;
        double[] array2;
        double[] arrayOut;

        //    Length = (int)MathHelps.NearestPowerOf2(Length);
        array1 = Function.ZeroPadArray(Length);
        array2 = impulse.ZeroPadArray(Length);

        array2.MakeFFTHumanReadable();

        complex[] cArray1 = FFTc(array1);
        complex[] cArray2 = FFTc(array2);

        cArray1.MultiplyInPlace(cArray2);
        arrayOut = iFFTc(cArray1).MakeFFTHumanReadable();
        arrayOut.CenterShortenArray(Function.Length);
        return arrayOut;
    }
}
