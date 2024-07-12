using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public sealed class PhysicalArrayComplex : IEquatable<PhysicalArrayComplex>, IEnumerable<complex>
{
    private complex[] mData;
    private double mPhysicalStart;
    private double mPhysicalEnd;
    private double mPhysicalStep;

    GCHandle mhData;
    IntPtr mipData;
    public complex this[int index]
    {
        get { return mData[index]; }
        set { mData[index] = value; }
    }
    public complex[] ActualData
    {
        get { return mData; }
    }

    #region Physical_World_Interactions
    public complex GetValue(double Physical_Xvalue)
    {
        int index = (int)((Physical_Xvalue - mPhysicalStart) / mPhysicalStep);
        if (index >= 0 && index < mData.Length)
            return mData[index];
        else
            throw new Exception("this is not in the physical range of data");
    }
    public complex GetValueUnchecked(double Physical_Xvalue)
    {
        int index = (int)((Physical_Xvalue - mPhysicalStart) / mPhysicalStep);
        return mData[index];
    }
    public double[] GetPhysicalIndicies()
    {
        double[] Indices = new double[mData.Length];
        double step = (mPhysicalEnd - mPhysicalStart) / (double)mData.Length;
        for (int i = 0; i < mData.Length; i++)
        {
            Indices[i] = mPhysicalStart + step * i;
        }
        return Indices;
    }
    public double PhysicalStart
    {
        get { return mPhysicalStart; }
    }
    public double PhysicalEnd
    {
        get { return mPhysicalEnd; }
    }
    public double PhysicalStep
    {
        get { return (mPhysicalEnd - mPhysicalStart) / (double)mData.Length; }
    }
    public double PhysicalMidPoint
    {
        get { return (mPhysicalStart + mPhysicalEnd) / 2d; }
    }
    public double PhysicalLength
    {
        get { return mPhysicalEnd - mPhysicalStart; }
    }
    #endregion

    #region Constructors
    public PhysicalArrayComplex(int nPoints, double PhysicalStart, double PhysicalEnd)
        : this(new complex [nPoints], PhysicalStart, PhysicalEnd)
    {
    }

    public PhysicalArrayComplex(complex [] data, double PhysicalStart, double PhysicalEnd)
    {
        if (data == null) throw new ArgumentNullException("must have initialized mData");
        this.mData = data;
        mhData = GCHandle.Alloc(this.mData, GCHandleType.Pinned);
        mipData = mhData.AddrOfPinnedObject();
        mPhysicalEnd = PhysicalEnd;
        mPhysicalStart = PhysicalStart;
        mPhysicalStep = (PhysicalEnd - PhysicalStart) / (double)mData.Length;
    }

    public PhysicalArrayComplex(complex[] data, bool MakeCopy, double PhysicalStart, double PhysicalEnd)
    {
        if (data == null) throw new ArgumentNullException("must have initialized mData");

        if (MakeCopy)
        {
            mData = new complex [data.Length];
            Buffer.BlockCopy(data, 0, mData, 0, data.Length * 16);
            mhData = GCHandle.Alloc(this.mData, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd = PhysicalEnd;
            mPhysicalStart = PhysicalStart;
            mPhysicalStep = (PhysicalEnd - PhysicalStart) / (double)mData.Length;
        }
        else
        {
            if (data == null) throw new ArgumentNullException("must have initialized mData");
            this.mData = data;
            mhData = GCHandle.Alloc(this.mData, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd = PhysicalEnd;
            mPhysicalStart = PhysicalStart;
            mPhysicalStep = (PhysicalEnd - PhysicalStart) / (double)mData.Length;
        }
    }
    public PhysicalArrayComplex(double [] data, double PhysicalStart, double PhysicalEnd)
    {
        if (data == null) throw new ArgumentNullException("must have initialized mData");

            mData = data.ConvertToComplex();
            mhData = GCHandle.Alloc(this.mData, GCHandleType.Pinned);
            mipData = mhData.AddrOfPinnedObject();
            mPhysicalEnd = PhysicalEnd;
            mPhysicalStart = PhysicalStart;
            mPhysicalStep = (PhysicalEnd - PhysicalStart) / (double)mData.Length;
    }

    ~PhysicalArrayComplex()
    {
        mhData.Free();
    }
    #endregion

    #region Default
    private int? hash;
    public override int GetHashCode()
    {
        if (hash == null)
        {
            double result = 13;
            for (int i = 0; i < mData.Length; i++)
            {
                result = (result * 7) + mData[i].real ;
            }
            hash = (int)result;
        }
        return hash.GetValueOrDefault();
    }

    public int Length { get { return mData.Length; } }

    public IEnumerator<complex > GetEnumerator()
    {
        for (int i = 0; i < mData.Length; i++)
        {
            yield return mData[i];
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return (System.Collections.IEnumerator)GetEnumerator();
    }

    public override bool Equals(object obj)
    {
        return this == (obj as PhysicalArrayComplex);
    }
    public bool Equals(PhysicalArrayComplex obj)
    {
        return this == obj;
    }

    public static bool operator !=(PhysicalArrayComplex x, PhysicalArrayComplex y)
    {
        return !(x == y);
    }
    #endregion

    #region OperatorOverloads
    private static void GetOverlaps(PhysicalArrayComplex x, PhysicalArrayComplex y, out double stepX, out double stepY,
        out double IntervalStart, out double step, out double IntervalEnd, out PhysicalArrayComplex pa1, out PhysicalArrayComplex pa2)
    {
        double sX, eX;
        if (x.mPhysicalStart >= y.mPhysicalStart && x.mPhysicalStart <= y.mPhysicalEnd)
        {
            sX = x.mPhysicalStart;
        }
        else if (y.mPhysicalStart >= x.mPhysicalStart && y.mPhysicalStart <= x.mPhysicalEnd)
        {
            sX = y.mPhysicalStart;
        }
        else
        {
            throw new Exception("Arrays must overlap");
        }

        if (x.mPhysicalEnd >= y.mPhysicalStart && x.mPhysicalEnd <= y.mPhysicalEnd)
        {
            eX = x.mPhysicalEnd;
        }
        else if (y.mPhysicalEnd >= x.mPhysicalStart && y.mPhysicalEnd <= x.mPhysicalEnd)
        {
            eX = y.mPhysicalEnd;
        }
        else
        {
            throw new Exception("Arrays must overlap");
        }

        stepX = (x.mPhysicalEnd - x.mPhysicalStart) / (double)x.mData.Length;
        stepY = (y.mPhysicalEnd - y.mPhysicalStart) / (double)y.mData.Length;

        if (stepX < stepY)
        {
            step = stepX;
            pa1 = x;
            pa2 = y;
        }
        else
        {
            step = stepY;
            stepY = stepX;
            stepX = step;
            pa1 = y;
            pa2 = x;
        }
        IntervalStart = (Math.Floor(sX / step) + 1) * step;
        IntervalEnd = Math.Floor(eX / step) * step;
    }

    public static bool operator ==(PhysicalArrayComplex x, PhysicalArrayComplex y)
    {
        if (ReferenceEquals(x, y)) return true;

        if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

        if (x.hash.HasValue && y.hash.HasValue &&
            x.hash.GetValueOrDefault() != y.hash.GetValueOrDefault()) return false;

        if (x.mData.Length != y.mData.Length) return false;

        if (x.mPhysicalStart != y.mPhysicalStart) return false;
        if (x.mPhysicalEnd != y.mPhysicalEnd) return false;

        unsafe
        {
            double* pX = (double*)x.mipData;
            double* pY = (double*)y.mipData;
            int Length = x.mData.Length*2;
            for (int i = 0; i < Length; i++)
            {
                if ((*pX) != (*pY)) return false;
                pX++;
                pY++;
            }
        }
        return true;
    }

    private enum MathType
    {
        Addition, Subtraction, Division, Multiplication
    }

    private static PhysicalArrayComplex DoArithmeticShifted(PhysicalArrayComplex x, PhysicalArrayComplex y, MathType Operation)
    {
        double IntervalStart, IntervalEnd;
        double step;
        double step1;
        double step2;
        PhysicalArrayComplex pa1;
        PhysicalArrayComplex pa2;
        GetOverlaps(x, y, out step1, out step2, out IntervalStart, out step, out IntervalEnd, out pa1, out pa2);

        int sXI = (int)Math.Floor((IntervalStart - pa1.mPhysicalStart) / step1);
        int eXI = (int)Math.Floor((IntervalEnd - pa1.mPhysicalStart) / step1);

        complex[] result = new complex[eXI - sXI];
        int j = (int)((IntervalStart - pa2.mPhysicalStart) / step2 - 1);
        unsafe
        {
            fixed (complex* pResult = result)
            {
                complex* pX = ((complex*)pa1.mipData) + sXI - 1;
                complex* pY = ((complex*)pa2.mipData) + j;
                complex* pOut = pResult;

                switch (Operation)
                {
                    case MathType.Addition:
                        {
                            for (int i = sXI; i < eXI; i++)
                            {
                                *pOut = (*pX) + (*pY);
                                pOut++;
                                pX++;
                                pY++;
                            }
                        }
                        break;
                    case MathType.Multiplication:
                        {
                            for (int i = sXI; i < eXI; i++)
                            {
                                *pOut = (*pX) * (*pY);
                                pOut++;
                                pX++;
                                pY++;
                            }
                        }
                        break;
                    case MathType.Subtraction:
                        {
                            if (pa1 == x)
                            {
                                for (int i = sXI; i < eXI; i++)
                                {
                                    *pOut = (*pX) - (*pY);
                                    pOut++;
                                    pX++;
                                    pY++;
                                }
                            }
                            else
                            {
                                for (int i = sXI; i < eXI; i++)
                                {
                                    *pOut = (*pY) - (*pX);
                                    pOut++;
                                    pX++;
                                    pY++;
                                }
                            }

                        }
                        break;
                    case MathType.Division:
                        {
                            if (pa1 == x)
                            {
                                for (int i = sXI; i < eXI; i++)
                                {
                                    *pOut = (*pX) / (*pY);
                                    pOut++;
                                    pX++;
                                    pY++;
                                }
                            }
                            else
                            {
                                for (int i = sXI; i < eXI; i++)
                                {
                                    *pOut = (*pY) / (*pX);
                                    pOut++;
                                    pX++;
                                    pY++;
                                }
                            }

                        }
                        break;

                }
            }
        }
        return new PhysicalArrayComplex(result, IntervalStart, IntervalEnd);
    }
    private static PhysicalArrayComplex DoArithmeticUnEqualInterpolation(PhysicalArrayComplex x, PhysicalArrayComplex y, MathType Operation)
    {
        double IntervalStart, IntervalEnd;
        double step;
        double step1;
        double step2;
        PhysicalArrayComplex pa1;
        PhysicalArrayComplex pa2;
        GetOverlaps(x, y, out step1, out step2, out IntervalStart, out step, out IntervalEnd, out pa1, out pa2);

        int sXI = (int)Math.Floor((IntervalStart - pa1.mPhysicalStart) / step1);
        int eXI = (int)Math.Floor((IntervalEnd - pa1.mPhysicalStart) / step1);

        complex[] result = new complex[eXI - sXI];
        double Cx = IntervalStart;
        double Cy = step1 / step2;
        double jd = (IntervalStart - pa2.mPhysicalStart) / step2 - 1;
        double u = 0;
        int j = 0;
        unsafe
        {
            fixed (complex* pResult = result)
            {
                complex* pX = ((complex*)pa1.mipData) + sXI;
                complex* pY = ((complex*)pa2.mipData) + j;
                complex* pOut = pResult;
                switch (Operation)
                {
                    case MathType.Addition:
                        {
                            for (int i = sXI; i < eXI; i++)
                            {
                                jd += Cy;
                                j = (int)Math.Floor(jd);
                                u = jd - j;

                                *pOut = (*pX) + ((pa2.mData[j + 1] - pa2.mData[j]) * u + pa2.mData[j]);
                                pOut++;
                                pX++;
                                pY++;
                            }
                        }
                        break;
                    case MathType.Multiplication:
                        {
                            for (int i = sXI; i < eXI; i++)
                            {
                                jd += Cy;
                                j = (int)Math.Floor(jd);
                                u = jd - j;

                                *pOut = (*pX) * ((pa2.mData[j + 1] - pa2.mData[j]) * u + pa2.mData[j]);
                                pOut++;
                                pX++;
                                pY++;
                            }
                        }
                        break;
                    case MathType.Subtraction:
                        {
                            if (pa1 == x)
                            {
                                for (int i = sXI; i < eXI; i++)
                                {
                                    jd += Cy;
                                    j = (int)Math.Floor(jd);
                                    u = jd - j;


                                    *pOut = (*pX) - ((pa2.mData[j + 1] - pa2.mData[j]) * u + pa2.mData[j]);
                                    pOut++;
                                    pX++;
                                    pY++;
                                }
                            }
                            else
                            {
                                for (int i = sXI; i < eXI; i++)
                                {
                                    jd += Cy;
                                    j = (int)Math.Floor(jd);
                                    u = jd - j;


                                    *pOut = ((pa2.mData[j + 1] - pa2.mData[j]) * u + pa2.mData[j]) - (*pX);
                                    pOut++;
                                    pX++;
                                    pY++;
                                }

                            }
                        }
                        break;
                    case MathType.Division:
                        {
                            if (pa1 == x)
                            {
                                for (int i = sXI; i < eXI; i++)
                                {
                                    jd += Cy;
                                    j = (int)Math.Floor(jd);
                                    u = jd - j;


                                    *pOut = (*pX) / ((pa2.mData[j + 1] - pa2.mData[j]) * u + pa2.mData[j]);
                                    pOut++;
                                    pX++;
                                    pY++;
                                }
                            }
                            else
                            {
                                for (int i = sXI; i < eXI; i++)
                                {
                                    jd += Cy;
                                    j = (int)Math.Floor(jd);
                                    u = jd - j;


                                    *pOut = ((pa2.mData[j + 1] - pa2.mData[j]) * u + pa2.mData[j]) / (*pX);
                                    pOut++;
                                    pX++;
                                    pY++;
                                }

                            }
                        }
                        break;

                }
            }
        }
        return new PhysicalArrayComplex(result, IntervalStart, IntervalEnd);
    }
    private static PhysicalArrayComplex DoArithmeticInterpolation(PhysicalArrayComplex x, PhysicalArrayComplex y, MathType Operation)
    {

        if (x.PhysicalStep != y.PhysicalStep)
        {
            return DoArithmeticUnEqualInterpolation(x, y, Operation);
        }
        else
        {
            return DoArithmeticShifted(x, y, Operation);
        }

    }
    private static PhysicalArrayComplex DoArithmeticNoInterpolation(PhysicalArrayComplex x, PhysicalArrayComplex y, MathType Operation)
    {

        complex[] result = new complex[x.mData.Length];
        unsafe
        {
            fixed (complex* pResult = result)
            {
                complex* pX = (complex*)x.mipData;
                complex* pY = (complex*)y.mipData;
                complex* pOut = pResult;
                int Length = x.mData.Length;
                switch (Operation)
                {
                    case MathType.Addition:
                        {
                            for (int i = 0; i < Length; i++)
                            {
                                *pOut = ((*pX) + (*pY));
                                pOut++;
                                pX++;
                                pY++;
                            }
                        }
                        break;
                    case MathType.Subtraction:
                        {
                            for (int i = 0; i < Length; i++)
                            {
                                *pOut = ((*pX) - (*pY));
                                pOut++;
                                pX++;
                                pY++;
                            }
                        }
                        break;
                    case MathType.Multiplication:
                        {
                            for (int i = 0; i < Length; i++)
                            {
                                *pOut = ((*pX) * (*pY));
                                pOut++;
                                pX++;
                                pY++;
                            }
                        }
                        break;
                    case MathType.Division:
                        {
                            for (int i = 0; i < Length; i++)
                            {
                                *pOut = ((*pX) / (*pY));
                                pOut++;
                                pX++;
                                pY++;
                            }
                        }
                        break;

                }
            }
        }
        return new PhysicalArrayComplex(result, x.mPhysicalStart, x.mPhysicalEnd);
    }
    private static PhysicalArrayComplex DoArithmetic(PhysicalArrayComplex x, PhysicalArrayComplex y, MathType Operation)
    {
        if (x == null || y == null) throw new ArgumentNullException();
        if (x.mPhysicalStart == y.mPhysicalStart && x.mPhysicalEnd == y.mPhysicalEnd || x.mData.Length != y.mData.Length)
        {
            return DoArithmeticNoInterpolation(x, y, Operation);
        }
        else
        {
            return DoArithmeticInterpolation(x, y, Operation);
        }
    }

    public static PhysicalArrayComplex operator +(PhysicalArrayComplex x, PhysicalArrayComplex y)
    {
        return DoArithmetic(x, y, MathType.Addition);
    }
    public static PhysicalArrayComplex operator -(PhysicalArrayComplex x, PhysicalArrayComplex y)
    {
        return DoArithmetic(x, y, MathType.Subtraction);
    }
    public static PhysicalArrayComplex operator *(PhysicalArrayComplex x, PhysicalArrayComplex y)
    {
        return DoArithmetic(x, y, MathType.Multiplication);
    }
    public static PhysicalArrayComplex operator /(PhysicalArrayComplex x, PhysicalArrayComplex y)
    {
        return DoArithmetic(x, y, MathType.Division);
    }

    public static PhysicalArrayComplex operator +(PhysicalArrayComplex x, double y)
    {
        if (x == null) throw new ArgumentNullException();

        complex[] result = new complex[x.mData.Length];
        unsafe
        {
            fixed (complex* pResult = result)
            {
                complex* pX = (complex*)x.mipData;
                complex* pOut = pResult;
                int Length = x.mData.Length;
                for (int i = 0; i < Length; i++)
                {
                    *pOut = ((*pX) + y);
                    pOut++;
                    pX++;
                }
            }
        }
        return new PhysicalArrayComplex(result, x.mPhysicalStart, x.mPhysicalEnd);
    }
    public static PhysicalArrayComplex operator +(double y, PhysicalArrayComplex x)
    {
        return x + y;
    }

    public static PhysicalArrayComplex operator -(PhysicalArrayComplex x, double y)
    {
        return x + (-1 * y);
    }
    public static PhysicalArrayComplex operator -(double y, PhysicalArrayComplex x)
    {
        return x + (-1 * y);
    }

    public static PhysicalArrayComplex operator *(PhysicalArrayComplex x, double y)
    {
        if (x == null) throw new ArgumentNullException();

        complex[] result = new complex[x.mData.Length];
        unsafe
        {
            fixed (complex* pResult = result)
            {
                complex* pX = (complex*)x.mipData;
                complex* pOut = pResult;
                int Length = x.mData.Length;
                for (int i = 0; i < Length; i++)
                {
                    *pOut = ((*pX) * y);
                    pOut++;
                    pX++;
                }
            }
        }
        return new PhysicalArrayComplex(result, x.mPhysicalStart, x.mPhysicalEnd);
    }
    public static PhysicalArrayComplex operator *(double y, PhysicalArrayComplex x)
    {
        return x * y;
    }

    public static PhysicalArrayComplex operator /(PhysicalArrayComplex x, double y)
    {
        return x * (1 / y);
    }
    public static PhysicalArrayComplex operator /(double y, PhysicalArrayComplex x)
    {
        return x * (1 / y);
    }

    public static PhysicalArrayComplex operator +(PhysicalArrayComplex x, complex y)
    {
        if (x == null) throw new ArgumentNullException();

        complex[] result = new complex[x.mData.Length];
        unsafe
        {
            fixed (complex* pResult = result)
            {
                complex* pX = (complex*)x.mipData;
                complex* pOut = pResult;
                int Length = x.mData.Length;
                for (int i = 0; i < Length; i++)
                {
                    *pOut = ((*pX) + y);
                    pOut++;
                    pX++;
                }
            }
        }
        return new PhysicalArrayComplex(result, x.mPhysicalStart, x.mPhysicalEnd);
    }
    public static PhysicalArrayComplex operator +(complex y, PhysicalArrayComplex x)
    {
        return x + y;
    }

    public static PhysicalArrayComplex operator -(PhysicalArrayComplex x, complex y)
    {
        return x + (-1 * y);
    }
    public static PhysicalArrayComplex operator -(complex y, PhysicalArrayComplex x)
    {
        return x + (-1 * y);
    }

    public static PhysicalArrayComplex operator *(PhysicalArrayComplex x, complex y)
    {
        if (x == null) throw new ArgumentNullException();

        complex[] result = new complex[x.mData.Length];
        unsafe
        {
            fixed (complex* pResult = result)
            {
                complex* pX = (complex*)x.mipData;
                complex* pOut = pResult;
                int Length = x.mData.Length;
                for (int i = 0; i < Length; i++)
                {
                    *pOut = ((*pX) * y);
                    pOut++;
                    pX++;
                }
            }
        }
        return new PhysicalArrayComplex(result, x.mPhysicalStart, x.mPhysicalEnd);
    }
    public static PhysicalArrayComplex operator *(complex y, PhysicalArrayComplex x)
    {
        return x * y;
    }

    public static PhysicalArrayComplex operator /(PhysicalArrayComplex x, complex y)
    {
        return x * (1 / y);
    }
    public static PhysicalArrayComplex operator /(complex y, PhysicalArrayComplex x)
    {
        return x * (1 / y);
    }


    #endregion

    #region Resize_Array
    public void ZeroPadDataCenteredInPlace(int NumPoints)
    {
        if (NumPoints > mData.Length)
        {
            complex[] nData = new complex[NumPoints];
            GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
            IntPtr ipData = hData.AddrOfPinnedObject();

            int StartD = (NumPoints - mData.Length) / 2;
            unsafe
            {
                complex* pOut = ((complex*)ipData) + StartD;
                complex* pData = (complex*)mipData;
                for (int i = 0; i < mData.Length; i++)
                {
                    *pOut = *pData;
                    pOut++;
                    pData++;
                }
            }

            double a = NumPoints / (double)mData.Length;
            double L = mPhysicalEnd - mPhysicalStart;
            double nPhysicalEnd = (a * L + mPhysicalStart + mPhysicalEnd) / 2;
            double nPhysicalStart = (mPhysicalStart + mPhysicalEnd - nPhysicalEnd);


            mhData.Free();
            mhData = hData;
            mipData = ipData;
            mData = nData;

            mPhysicalStart = nPhysicalStart;
            mPhysicalEnd = nPhysicalEnd;
        }
    }

    public void ZeroPadZerosCenterInPlace(int NumPoints)
    {
        if (NumPoints > mData.Length)
        {
            complex[] nData = new complex[NumPoints];
            GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
            IntPtr ipData = hData.AddrOfPinnedObject();

            int StartD = (NumPoints - mData.Length) / 2;
            unsafe
            {
                complex* pOut = ((complex*)ipData);
                complex* pOutE = ((complex*)ipData) + nData.Length;
                complex* pData = (complex*)mipData;
                complex* pDataE = ((complex*)mipData) + mData.Length;
                for (int i = 0; i < mData.Length / 2; i++)
                {
                    *pOut = *pData;
                    pOut++;
                    pData++;

                    *pOutE = *pDataE;
                    pOutE--;
                    pDataE--;
                }
            }
            double a = NumPoints / (double)mData.Length;
            double L = mPhysicalEnd - mPhysicalStart;
            double nPhysicalEnd = (a * L + mPhysicalStart + mPhysicalEnd) / 2;
            double nPhysicalStart = (mPhysicalStart + mPhysicalEnd - nPhysicalEnd);

            mhData.Free();
            mhData = hData;
            mipData = ipData;
            mData = nData;

            mPhysicalStart = nPhysicalStart;
            mPhysicalEnd = nPhysicalEnd;
        }

    }

    public void TruncateDataInPlace(double PhysicalStart, double PhysicalEnd)
    {
        double step = PhysicalStep;
        int sI = (int)((PhysicalStart - mPhysicalStart) / step);
        int eI = (int)((PhysicalEnd - mPhysicalStart) / step);
        if (sI < 0 || eI > mData.Length)
            throw new Exception("Must truncate data within its physical range");


        complex[] nData = new complex[eI - sI];
        GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
        IntPtr ipData = hData.AddrOfPinnedObject();

        unsafe
        {
            complex* pOut = ((complex*)ipData);
            complex* pData = ((complex*)mipData) + sI;
            for (int i = sI; i < eI; i++)
            {
                *pOut = *pData;
                pOut++;
                pData++;
            }
        }

        mhData.Free();
        mhData = hData;
        mipData = ipData;
        mData = nData;

        mPhysicalStart = PhysicalStart;
        mPhysicalEnd = PhysicalEnd;
        mPhysicalStep = (mPhysicalEnd - mPhysicalStart) / (double)mData.Length;
    }

    public PhysicalArrayComplex ZeroPadDataCentered(int NumPoints)
    {
        if (NumPoints > mData.Length)
        {
            complex[] nData = new complex[NumPoints];
            GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
            IntPtr ipData = hData.AddrOfPinnedObject();

            int StartD = (NumPoints - mData.Length) / 2;
            unsafe
            {
                complex* pOut = ((complex*)ipData) + StartD;
                complex* pData = (complex*)mipData;
                for (int i = 0; i < mData.Length; i++)
                {
                    *pOut = *pData;
                    pOut++;
                    pData++;
                }
            }

            double a = NumPoints / (double)mData.Length;
            double L = mPhysicalEnd - mPhysicalStart;
            double nPhysicalEnd = (a * L + mPhysicalStart + mPhysicalEnd) / 2;
            double nPhysicalStart = (mPhysicalStart + mPhysicalEnd - nPhysicalEnd);

            return new PhysicalArrayComplex(nData, nPhysicalStart, nPhysicalEnd);
        }
        else
        {
            return new PhysicalArrayComplex(mData, true, mPhysicalStart, mPhysicalEnd);

        }
    }

    public PhysicalArrayComplex ZeroPadZerosCenter(int NumPoints)
    {
        if (NumPoints > mData.Length)
        {
            complex[] nData = new complex[NumPoints];
            GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
            IntPtr ipData = hData.AddrOfPinnedObject();

            int StartD = (NumPoints - mData.Length) / 2;
            unsafe
            {
                complex* pOut = ((complex*)ipData);
                complex* pOutE = ((complex*)ipData) + nData.Length;
                complex* pData = (complex*)mipData;
                complex* pDataE = ((complex*)mipData) + mData.Length;
                for (int i = 0; i < mData.Length / 2; i++)
                {
                    *pOut = *pData;
                    pOut++;
                    pData++;

                    *pOutE = *pDataE;
                    pOutE--;
                    pDataE--;
                }
            }
            double a = NumPoints / (double)mData.Length;
            double L = mPhysicalEnd - mPhysicalStart;
            double nPhysicalEnd = (a * L + mPhysicalStart + mPhysicalEnd) / 2;
            double nPhysicalStart = (mPhysicalStart + mPhysicalEnd - nPhysicalEnd);

            return new PhysicalArrayComplex(nData, nPhysicalStart, nPhysicalEnd);

        }
        else
        {
            return new PhysicalArrayComplex(mData, true, mPhysicalStart, mPhysicalEnd);

        }

    }

    public PhysicalArrayComplex TruncateData(double PhysicalStart, double PhysicalEnd)
    {
        double step = PhysicalStep;
        int sI = (int)((PhysicalStart - mPhysicalStart) / step);
        int eI = (int)((PhysicalEnd - mPhysicalStart) / step);
        if (sI < 0 || eI > mData.Length)
            throw new Exception("Must truncate data within its physical range");


        complex[] nData = new complex[eI - sI];
        GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
        IntPtr ipData = hData.AddrOfPinnedObject();

        unsafe
        {
            complex* pOut = ((complex*)ipData);
            complex* pData = ((complex*)mipData) + sI;
            for (int i = sI; i < eI; i++)
            {
                *pOut = *pData;
                pOut++;
                pData++;
            }
        }

        return new PhysicalArrayComplex(nData, PhysicalStart, PhysicalEnd);

    }

    #endregion

    #region To_Conversions

    public complex[] ToComplexArray()
    {
        complex[] ArrayOut = new complex[mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[i] = mData[i];
        }
        return ArrayOut;
    }

    public double[] ToDoubleArrayMagnitude()
    {
        double[] ArrayOut = new double[mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[i] = mData[i].Abs();
        }
        return ArrayOut;
    }
    public double[] ToDoubleArrayReal()
    {
        double[] ArrayOut = new double[mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[i] = mData[i].real ;
        }
        return ArrayOut;
    }

    public double[] ToDoubleArrayImag()
    {
        double[] ArrayOut = new double[mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[i] = mData[i].imag ;
        }
        return ArrayOut;
    }

    public double[,] ToDoubleArrayIndexedMagnitude()
    {
        double[,] ArrayOut = new double[2, mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[0, i] = mPhysicalStart + i * mPhysicalStep;
            ArrayOut[1, i] = mData[i].Abs();
        }
        return ArrayOut;
    }
    public double[,] ToDoubleArrayIndexedReal()
    {
        double[,] ArrayOut = new double[2, mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[0, i] = mPhysicalStart + i * mPhysicalStep;
            ArrayOut[1, i] = mData[i].real ;
        }
        return ArrayOut;
    }

    public double[,] ToDoubleArrayIndexedImag()
    {
        double[,] ArrayOut = new double[2, mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[0, i] = mPhysicalStart + i * mPhysicalStep;
            ArrayOut[1, i] = mData[i].imag ;
        }
        return ArrayOut;
    }

    public double[,] MakeGraphableArray()
    {
        return ToDoubleArrayIndexedMagnitude();
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("[");
        if (mData.Length > 0) sb.Append(mData[0]);
        for (int i = 1; i < mData.Length; i++)
        {
            sb.Append(',').Append(mData[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }
    #endregion

    #region MathOperations

    public PhysicalArrayComplex Convolute(double[] impulse)
    {
        complex[] arrayout = MathHelps.Convolute(mData, impulse);
        double NumPoints = arrayout.Length;
        double a = NumPoints / (double)mData.Length;
        double L = mPhysicalEnd - mPhysicalStart;
        double nPhysicalEnd = (a * L + mPhysicalStart + mPhysicalEnd) / 2;
        double nPhysicalStart = (mPhysicalStart + mPhysicalEnd - nPhysicalEnd);

        return new PhysicalArrayComplex(arrayout, nPhysicalStart, nPhysicalEnd);
    }
    public PhysicalArrayComplex ConvoluteChop(double[] impulse)
    {
        return new PhysicalArrayComplex(MathHelps.ConvoluteChop(mData, impulse), mPhysicalStart, mPhysicalEnd);
    }

    public PhysicalArrayComplex Convolute(complex[] impulse)
    {
        complex[] arrayout = MathHelps.Convolute(mData, impulse);
        double NumPoints = arrayout.Length;
        double a = NumPoints / (double)mData.Length;
        double L = mPhysicalEnd - mPhysicalStart;
        double nPhysicalEnd = (a * L + mPhysicalStart + mPhysicalEnd) / 2;
        double nPhysicalStart = (mPhysicalStart + mPhysicalEnd - nPhysicalEnd);

        return new PhysicalArrayComplex(arrayout, nPhysicalStart, nPhysicalEnd);
    }
    public PhysicalArrayComplex ConvoluteChop(complex[] impulse)
    {
        return new PhysicalArrayComplex(MathHelps.ConvoluteChop(mData, impulse), mPhysicalStart, mPhysicalEnd);
    }

    public PhysicalArrayComplex Convolute(PhysicalArrayComplex  impulse)
    {
        throw new Exception("This is not yet implemented.  Need to line up step sizes. Then convolution is correct");
    }
    public PhysicalArrayComplex ConvoluteChop(PhysicalArrayComplex  impulse)
    {
        throw new Exception("This is not yet implemented.  Need to line up step sizes. Then convolution is correct");
    }

    public PhysicalArrayComplex FFTc()
    {
        complex[] fft = MathHelps.FFT(mData);
        return new PhysicalArrayComplex(fft, -1 / mPhysicalStep * mData.Length / 2, 1 / mPhysicalStep * mData.Length / 2);
    }
    
    public PhysicalArray iFFTr()
    {
        double[] fft = MathHelps.iFFTc(mData);
        return new PhysicalArray(fft, -1 / mPhysicalStep * mData.Length / 2, 1 / mPhysicalStep * mData.Length / 2);
    }

    public complex Sum()
    {
        complex  result = new complex(0,0);
        unsafe
        {
            complex * pX = (complex *)mipData;
            int Length = mData.Length;
            for (int i = 0; i < Length; i++)
            {
                result += ((*pX));
                pX++;
            }
        }
        return result;
    }
    #endregion
}
