#include "MyMathFuncsWrapper.h"

MyMathFuncs* CreateMyMathFuncsClass()
{
    return new MyMathFuncs();
}

void DisposeMyMathFuncsClass(MyMathFuncs* ptr)
{
    if (ptr != nullptr)
    {
        delete ptr;
        ptr = nullptr;
    }
}

double MyMathFuncsAdd(MyMathFuncs *ptr, double a, double b)
{
    return ptr->Add(a, b);
}

double MyMathFuncsSubtract(MyMathFuncs *ptr, double a, double b)
{
    return ptr->Subtract(a, b);
}

double MyMathFuncsMultiply(MyMathFuncs *ptr, double a, double b)
{
    return ptr->Multiply(a, b);
}

double MyMathFuncsDivide(MyMathFuncs *ptr, double a, double b)
{
    return ptr->Divide(a, b);
}