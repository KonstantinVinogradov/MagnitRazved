using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
    public interface IReverseSolver : IOptimizator<ILeastSquaresFunctional<Vector3D, Vector3D>, IDifferentiableFunction<Vector3D, Vector3D>, List<Material>, Vector3D, Vector3D>
    {
        public double AlphaRegularization { get; set; }
    }

    public class LeastSquaresFunctional : ILeastSquaresFunctional<Vector3D, Vector3D>
    {
        private readonly List<(Vector3D point, Vector3D B)> Data;
        private readonly int _parametersCount;

        public LeastSquaresFunctional(List<(Vector3D point, Vector3D B)> data, int parametersCount)
        {
            Data = data;
            _parametersCount = parametersCount;
        }

        public IMatrix Jacobian(IDifferentiableFunction<Vector3D, Vector3D> function)
        {
            var matrix = new Matrix(Data.Count * 3, _parametersCount);
            for (int i = 0; i < Data.Count; i++)
            {

                for (int k = 0; k < 3; k++)
                {
                    var curgrad = function.Gradient(Data[i].point, k);
                    Parallel.For(0, _parametersCount, j =>
                    {
                        matrix[3 * i + k][j] += curgrad[j];
                    }
                    );
                }

            }
            return matrix;
        }

        public IVector Residual(IDifferentiableFunction<Vector3D, Vector3D> function)
        {
            IVector result = new Vector();
            for (int i = 0; i < Data.Count; i++)
            {
                (Vector3D point, Vector3D B) data = Data[i];
                var f = function.Value(data.point);
                result.Add(data.B.X - f.X);
                result.Add(data.B.Y - f.Y);
                result.Add(data.B.Z - f.Z);
            }
            return result;
        }

        public double Value(IDifferentiableFunction<Vector3D, Vector3D> function)
        {
            var residual = Residual(function);
            return residual.Sum(t => t * t);
        }
    }
    public class ReverseSolver : IReverseSolver
    {
        /// <summary>
        /// Точки измерений
        /// </summary>
        public List<(Vector3D point, Vector3D B)> Data { get; set; }
        public double AlphaRegularization { get; set; }

        private IDirectSolver _solver;
        private readonly IMesh _mesh;

        public ReverseSolver(List<(Vector3D point, Vector3D B)> data, IMesh mesh)
        {
            Data = data;
            _mesh = mesh;
            _solver = new DirectSolver(mesh);
        }

        public static double Goldenratio(Func<double, double> func, double a, double b, double eps)
        {
            double k1 = (3 - Math.Sqrt(5)) / 2;
            double k2 = (Math.Sqrt(5) - 1) / 2;
            bool flag = true;
            double x1 = 0, x2 = 0;
            int i = 1;
            int lastchosen = 0;
            int k = 0;
            while (flag)
            {
                if (i == 1)
                {
                    x1 = a + k1 * (b - a);
                    x2 = a + k2 * (b - a);
                }
                else
                {
                    if (lastchosen == 1)
                    {
                        x1 = x2;
                        x2 = a + k2 * (b - a);
                    }
                    else
                    {
                        x2 = x1;
                        x1 = a + k1 * (b - a);
                    }
                }
                double f1 = func(x1);
                double f2 = func(x2);
                if (b - a < eps)
                {
                    flag = false;
                }
                else
                {
                    if (Math.Abs((f1 - f2) / f1) < 1e-15)
                    {
                        a = x1;
                        b = x2;
                    }
                    else
                    {
                        i++;
                        if (f1 < f2)
                        {
                            b = x2;
                            lastchosen = 2;
                        }
                        else
                        {
                            a = x1;
                            lastchosen = 1;
                        }
                    }
                }
            }
            return (x1 + x2) / 2;
        }



        private const int Maxiter = 150;
        public List<Material> Minimize(ILeastSquaresFunctional<Vector3D, Vector3D> objective, IParametricFunction<IDifferentiableFunction<Vector3D, Vector3D>, List<Material>, Vector3D, Vector3D> function, List<Material> initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
        {
            int k = 0;
            var f = _solver.Bind(initialParameters);
            var value = objective.Value(f);
            using var sw = new StreamWriter("C:\\Users\\Konstantin\\OneDrive\\Рабочий стол\\Новая папка\\log.log");
            sw.WriteLine($"iteration {k}, value {value}");
            Stopwatch watch = new Stopwatch();
            while (value > 1e-12 && k < Maxiter)
            {
                watch.Restart();
                var Jacobi = objective.Jacobian(f) as Matrix;
                var mat = Jacobi.Transpose() * Jacobi;
                var residual = objective.Residual(f);
                var b = Jacobi.Transpose() * residual;
                for (int i = 0; i < mat.Count; i++)
                {
                    mat[i][i] += AlphaRegularization;
                }
                var dmaterial = mat.SolveSLAE(b);
                for (int i = 0; i < initialParameters.Count; i++)
                {
                    initialParameters[i].P.X += dmaterial[3 * i];
                    initialParameters[i].P.Y += dmaterial[3 * i + 1];
                    initialParameters[i].P.Z += dmaterial[3 * i + 2];
                }
                k++;
                value = objective.Value(f);
                watch.Stop();
                sw.WriteLine($"iteration {k}, value {value}, elapsed {watch.Elapsed}");
            }
            return initialParameters;
        }
    }
}
