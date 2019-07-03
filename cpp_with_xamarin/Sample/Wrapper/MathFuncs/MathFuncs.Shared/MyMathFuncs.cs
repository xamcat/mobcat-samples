using System;

namespace MathFuncs
{
    public class MyMathFuncs : IDisposable
    {
        readonly MyMathFuncsSafeHandle handle;

        public MyMathFuncs()
        {
            handle = MyMathFuncsWrapper.CreateMyMathFuncs();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (handle != null && !handle.IsInvalid)
                handle.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public double Add(double a, double b)
        {
            return MyMathFuncsWrapper.Add(handle, a, b);
        }

        public double Subtract(double a, double b)
        {
            return MyMathFuncsWrapper.Subtract(handle, a, b);
        }

        public double Multiply(double a, double b)
        {
            return MyMathFuncsWrapper.Multiply(handle, a, b);
        }

        public double Divide(double a, double b)
        {
            return MyMathFuncsWrapper.Divide(handle, a, b);
        }
    }
}