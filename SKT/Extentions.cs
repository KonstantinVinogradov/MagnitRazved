using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT;
public static class Extentions
{
    public static void ThreadSafeAdd(this ref double dest, double value)
    {
        if (double.IsNaN(value))
            throw new ArgumentException("Value is NAN!");

        double initialValue, computedValue;
        do
        {
            initialValue = dest;
            computedValue = initialValue + value;
        }
        while (initialValue != Interlocked.CompareExchange(ref dest, computedValue, initialValue));
    }
}
