using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

public sealed class PhysicalArray : IEquatable<PhysicalArray>, IEnumerable<double>
{
    private double[] mData;
    private double mPhysicalStart;
    private double mPhysicalEnd;
    private double mPhysicalStep;

    GCHandle mhData;
    IntPtr mipData;
    public double this[int index]
    {
        get { return mData[index]; }
        set { mData[index] = value; }
    }
    public double[] ActualData
    {
        get { return mData; }
    }

    #region Physical_World_Interactions
    public double GetValue(double Physical_Xvalue)
    {
        int index = (int)((Physical_Xvalue - mPhysicalStart) / mPhysicalStep);
        if (index >= 0 && index < mData.Length)
            return mData[index];
        else
            throw new Exception("this is not in the physical range of data");
    }
    public double GetValueUnchecked(double Physical_Xvalue)
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
    public PhysicalArray(int nPoints, double PhysicalStart, double PhysicalEnd)
        : this(new double[nPoints], PhysicalStart, PhysicalEnd)
    {
    }

    public PhysicalArray(double[] data, double PhysicalStart, double PhysicalEnd)
    {
        if (data == null) throw new ArgumentNullException("must have initialized mData");
        this.mData = data;
        mhData = GCHandle.Alloc(this.mData, GCHandleType.Pinned);
        mipData = mhData.AddrOfPinnedObject();
        mPhysicalEnd = PhysicalEnd;
        mPhysicalStart = PhysicalStart;
        mPhysicalStep = (PhysicalEnd - PhysicalStart) / (double)mData.Length;
    }

    public PhysicalArray(double[] data, bool MakeCopy, double PhysicalStart, double PhysicalEnd)
    {
        if (data == null) throw new ArgumentNullException("must have initialized mData");

        if (MakeCopy)
        {
            mData = new double[data.Length];
            Buffer.BlockCopy(data, 0, mData, 0, data.Length * 8);
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

    ~PhysicalArray()
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
                result = (result * 7) + mData[i];
            }
            hash = (int)result;
        }
        return hash.GetValueOrDefault();
    }

    public int Length { get { return mData.Length; } }

    public IEnumerator<double> GetEnumerator()
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
        return this == (obj as PhysicalArray);
    }
    public bool Equals(PhysicalArray obj)
    {
        return this == obj;
    }

    public static bool operator !=(PhysicalArray x, PhysicalArray y)
    {
        return !(x == y);
    }
    #endregion

    #region OperatorOverloads

    public static bool operator ==(PhysicalArray x, PhysicalArray y)
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
            int Length = x.mData.Length;
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

    private static void GetOverlaps(PhysicalArray x, PhysicalArray y, out double stepX, out double stepY,
        out double IntervalStart, out double step, out double IntervalEnd, out PhysicalArray pa1, out PhysicalArray pa2)
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

    private static PhysicalArray DoArithmeticShifted(PhysicalArray x, PhysicalArray y, MathType Operation)
    {
        double IntervalStart, IntervalEnd;
        double step;
        double step1;
        double step2;
        PhysicalArray pa1;
        PhysicalArray pa2;
        GetOverlaps(x, y, out step1, out step2, out IntervalStart, out step, out IntervalEnd, out pa1, out pa2);

        int sXI = (int)Math.Floor((IntervalStart - pa1.mPhysicalStart) / step1);
        int eXI = (int)Math.Floor((IntervalEnd - pa1.mPhysicalStart) / step1);

        double[] result = new double[eXI - sXI];
        int j = (int)((IntervalStart - pa2.mPhysicalStart) / step2 - 1);
        unsafe
        {
            fixed (double* pResult = result)
            {
                double* pX = ((double*)pa1.mipData) + sXI - 1;
                double* pY = ((double*)pa2.mipData) + j;
                double* pOut = pResult;

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
        return new PhysicalArray(result, IntervalStart, IntervalEnd);
    }
    private static PhysicalArray DoArithmeticUnEqualInterpolation(PhysicalArray x, PhysicalArray y, MathType Operation)
    {
        double IntervalStart, IntervalEnd;
        double step;
        double step1;
        double step2;
        PhysicalArray pa1;
        PhysicalArray pa2;
        GetOverlaps(x, y, out step1, out step2, out IntervalStart, out step, out IntervalEnd, out pa1, out pa2);

        int sXI = (int)Math.Floor((IntervalStart - pa1.mPhysicalStart) / step1);
        int eXI = (int)Math.Floor((IntervalEnd - pa1.mPhysicalStart) / step1);

        double[] result = new double[eXI - sXI];
        double Cx = IntervalStart;
        double Cy = step1 / step2;
        double jd = (IntervalStart - pa2.mPhysicalStart) / step2 - 1;
        double u = 0;
        int j = 0;
        unsafe
        {
            fixed (double* pResult = result)
            {
                double* pX = ((double*)pa1.mipData) + sXI;
                double* pY = ((double*)pa2.mipData) + j;
                double* pOut = pResult;
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
        return new PhysicalArray(result, IntervalStart, IntervalEnd);
    }
    private static PhysicalArray DoArithmeticInterpolation(PhysicalArray x, PhysicalArray y, MathType Operation)
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
    private static PhysicalArray DoArithmeticNoInterpolation(PhysicalArray x, PhysicalArray y, MathType Operation)
    {

        double[] result = new double[x.mData.Length];
        unsafe
        {
            fixed (double* pResult = result)
            {
                double* pX = (double*)x.mipData;
                double* pY = (double*)y.mipData;
                double* pOut = pResult;
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
        return new PhysicalArray(result, x.mPhysicalStart, x.mPhysicalEnd);
    }
    private static PhysicalArray DoArithmetic(PhysicalArray x, PhysicalArray y, MathType Operation)
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



    public static PhysicalArray operator +(PhysicalArray x, PhysicalArray y)
    {
        return DoArithmetic(x, y, MathType.Addition);
    }
    public static PhysicalArray operator -(PhysicalArray x, PhysicalArray y)
    {
        return DoArithmetic(x, y, MathType.Subtraction);
    }
    public static PhysicalArray operator *(PhysicalArray x, PhysicalArray y)
    {
        return DoArithmetic(x, y, MathType.Multiplication);
    }
    public static PhysicalArray operator /(PhysicalArray x, PhysicalArray y)
    {
        return DoArithmetic(x, y, MathType.Division);
    }

    public static PhysicalArray operator +(PhysicalArray x, double y)
    {
        if (x == null) throw new ArgumentNullException();

        double[] result = new double[x.mData.Length];
        unsafe
        {
            fixed (double* pResult = result)
            {
                double* pX = (double*)x.mipData;
                double* pOut = pResult;
                int Length = x.mData.Length;
                for (int i = 0; i < Length; i++)
                {
                    *pOut = ((*pX) + y);
                    pOut++;
                    pX++;
                }
            }
        }
        return new PhysicalArray(result, x.mPhysicalStart, x.mPhysicalEnd);
    }
    public static PhysicalArray operator +(double y, PhysicalArray x)
    {
        return x + y;
    }

    public static PhysicalArray operator -(PhysicalArray x, double y)
    {
        return x + (-1 * y);
    }
    public static PhysicalArray operator -(double y, PhysicalArray x)
    {
        return x + (-1 * y);
    }

    public static PhysicalArray operator *(PhysicalArray x, double y)
    {
        if (x == null) throw new ArgumentNullException();

        double[] result = new double[x.mData.Length];
        unsafe
        {
            fixed (double* pResult = result)
            {
                double* pX = (double*)x.mipData;
                double* pOut = pResult;
                int Length = x.mData.Length;
                for (int i = 0; i < Length; i++)
                {
                    *pOut = ((*pX) * y);
                    pOut++;
                    pX++;
                }
            }
        }
        return new PhysicalArray(result, x.mPhysicalStart, x.mPhysicalEnd);
    }
    public static PhysicalArray operator *(double y, PhysicalArray x)
    {
        return x * y;
    }

    public static PhysicalArray operator /(PhysicalArray x, double y)
    {
        return x * (1 / y);
    }
    public static PhysicalArray operator /(double y, PhysicalArray x)
    {
        return x * (1 / y);
    }



    #endregion

    #region Resize_Array
    public void ZeroPadDataCenteredInPlace(int NumPoints)
    {
        if (NumPoints > mData.Length)
        {
            double[] nData = new double[NumPoints];
            GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
            IntPtr ipData = hData.AddrOfPinnedObject();

            int StartD = (NumPoints - mData.Length) / 2;
            unsafe
            {
                double* pOut = ((double*)ipData) + StartD;
                double* pData = (double*)mipData;
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
            double[] nData = new double[NumPoints];
            GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
            IntPtr ipData = hData.AddrOfPinnedObject();

            int StartD = (NumPoints - mData.Length) / 2;
            unsafe
            {
                double* pOut = ((double*)ipData);
                double* pOutE = ((double*)ipData) + nData.Length;
                double* pData = (double*)mipData;
                double* pDataE = ((double*)mipData) + mData.Length;
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


        double[] nData = new double[eI - sI];
        GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
        IntPtr ipData = hData.AddrOfPinnedObject();

        unsafe
        {
            double* pOut = ((double*)ipData);
            double* pData = ((double*)mipData) + sI;
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

    public PhysicalArray ZeroPadDataCentered(int NumPoints)
    {
        if (NumPoints > mData.Length)
        {
            double[] nData = new double[NumPoints];
            GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
            IntPtr ipData = hData.AddrOfPinnedObject();

            int StartD = (NumPoints - mData.Length) / 2;
            unsafe
            {
                double* pOut = ((double*)ipData) + StartD;
                double* pData = (double*)mipData;
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

            return new PhysicalArray(nData, nPhysicalStart, nPhysicalEnd);
        }
        else
        {
            return new PhysicalArray(mData, true, mPhysicalStart, mPhysicalEnd);

        }
    }

    public PhysicalArray ZeroPadZerosCenter(int NumPoints)
    {
        if (NumPoints > mData.Length)
        {
            double[] nData = new double[NumPoints];
            GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
            IntPtr ipData = hData.AddrOfPinnedObject();

            int StartD = (NumPoints - mData.Length) / 2;
            unsafe
            {
                double* pOut = ((double*)ipData);
                double* pOutE = ((double*)ipData) + nData.Length;
                double* pData = (double*)mipData;
                double* pDataE = ((double*)mipData) + mData.Length;
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

            return new PhysicalArray(nData, nPhysicalStart, nPhysicalEnd);

        }
        else
        {
            return new PhysicalArray(mData, true, mPhysicalStart, mPhysicalEnd);

        }

    }

    public PhysicalArray TruncateData(double PhysicalStart, double PhysicalEnd)
    {
        double step = PhysicalStep;
        int sI = (int)((PhysicalStart - mPhysicalStart) / step);
        int eI = (int)((PhysicalEnd - mPhysicalStart) / step);
        if (sI < 0 || eI > mData.Length)
            throw new Exception("Must truncate data within its physical range");


        double[] nData = new double[eI - sI];
        GCHandle hData = GCHandle.Alloc(nData, GCHandleType.Pinned);
        IntPtr ipData = hData.AddrOfPinnedObject();

        unsafe
        {
            double* pOut = ((double*)ipData);
            double* pData = ((double*)mipData) + sI;
            for (int i = sI; i < eI; i++)
            {
                *pOut = *pData;
                pOut++;
                pData++;
            }
        }

        return new PhysicalArray(nData, PhysicalStart, PhysicalEnd);

    }

    #endregion

    #region To_Conversions

    public double[] ToDoubleArray()
    {
        double[] ArrayOut = new double[mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[i] = mData[i];
        }
        return ArrayOut;
    }
    public double[,] ToDoubleArrayIndexed()
    {
        double[,] ArrayOut = new double[2, mData.Length];
        for (int i = 0; i < mData.Length; i++)
        {
            ArrayOut[0, i] = mPhysicalStart + i * mPhysicalStep;
            ArrayOut[1, i] = mData[i];
        }
        return ArrayOut;
    }
    public double[,] MakeGraphableArray()
    {
        return ToDoubleArrayIndexed();
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


    #region Math_Operations
    public PhysicalArray Convolute(double[] impulse)
    {
        double[] arrayout = MathHelps.Convolute(mData, impulse);
        double NumPoints = arrayout.Length;
        double a = NumPoints / (double)mData.Length;
        double L = mPhysicalEnd - mPhysicalStart;
        double nPhysicalEnd = (a * L + mPhysicalStart + mPhysicalEnd) / 2;
        double nPhysicalStart = (mPhysicalStart + mPhysicalEnd - nPhysicalEnd);

        return new PhysicalArray(arrayout, nPhysicalStart, nPhysicalEnd);
    }
    public PhysicalArray ConvoluteChop(double[] impulse)
    {
        return new PhysicalArray(MathHelps.ConvoluteChop(mData, impulse), mPhysicalStart, mPhysicalEnd);
    }

    public PhysicalArray Convolute(PhysicalArray impulse)
    {
        throw new Exception("This is not yet implemented.  Need to line up step sizes. Then convolution is correct");
    }
    public PhysicalArray ConvoluteChop(PhysicalArray impulse)
    {
        throw new Exception("This is not yet implemented.  Need to line up step sizes. Then convolution is correct");
    }

    public PhysicalArrayComplex FFT()
    {
        complex[] fft = MathHelps.FFTc(mData);
        return new PhysicalArrayComplex(fft, -1 / mPhysicalStep * mData.Length / 2, 1 / mPhysicalStep * mData.Length / 2);
    }
    public PhysicalArray FFTr()
    {
        double[] fft = MathHelps.FFT(mData);
        return new PhysicalArray(fft, -1 / mPhysicalStep * mData.Length / 2, 1 / mPhysicalStep * mData.Length / 2);
    }
    public PhysicalArray iFFTr()
    {
        double[] fft = MathHelps.iFFT(mData);
        return new PhysicalArray(fft, -1 / mPhysicalStep * mData.Length / 2, 1 / mPhysicalStep * mData.Length / 2);
    }

    public double Sum()
    {
        double result = 0;
        unsafe
        {
            double* pX = (double*)mipData;
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

/* public PhysicalArray(params double[] mData)
    {
        if (mData == null) throw new ArgumentNullException("must have initialized mData");
        this.mData = (double[])mData.Clone();
        mhData = GCHandle.Alloc(this.mData, GCHandleType.Pinned);
        mipData = mhData.AddrOfPinnedObject();
    }*/

/* double Cx = IntervalStart;
             double Cy = step1 / step2;
             double jd = (IntervalStart - pa2.mPhysicalStart) / step2 - 1;
             double u = 0;
             int j = 0;
             for (int i = sXI; i < eXI; i++)
             {
                 jd += Cy;
                 j = (int)Math.Floor(jd);
                 u = jd - j;
                 result[i - sXI] = pa1.mData[i] - ((pa2.mData[j + 1] - pa2.mData[j]) * u + pa2.mData[j]);
                 Cx += step1;
             }
             return new PhysicalArray(result, IntervalStart, IntervalEnd);*/