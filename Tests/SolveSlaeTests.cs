using SKT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests;
public class SolveSlaeTests
{
    [Fact]
    public void TestSumpleMatrix()
    {
        var matrix = new Matrix(2, 2);
        matrix[0][0] = 1;
        matrix[0][1] = 2;
        matrix[1][0] = 3;
        matrix[1][1] = 4;
        var res = matrix.SolveSLAE(new Vector([3, 7]));
        Assert.Equal(0, Math.Sqrt((res[0] - 1) * (res[0] - 1) + (res[1] - 1) * (res[1] - 1)), 1e-14);
    }
}
