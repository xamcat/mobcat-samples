using System;
using Microsoft.Win32.SafeHandles;

namespace MathFuncs
{
    internal class MyMathFuncsSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public MyMathFuncsSafeHandle() : base(true) { }

        public IntPtr Ptr => this.handle;

        protected override bool ReleaseHandle()
        {
            MyMathFuncsWrapper.DisposeMyMathFuncs(handle);
            return true;
        }
    }
}