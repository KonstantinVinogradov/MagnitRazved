using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SKT.Interfaces
{
    public interface IVector : IList<double> { }

    public class Vector : List<double>, IVector
    {
        public Vector() { }
        public Vector(int N) : base(Enumerable.Repeat(0.0, N)) { }
        public Vector(IEnumerable<double> data) : base(data) { }
        public Vector Clone()
        {
            return new Vector(this);
        }

        public static Vector operator -(Vector vec1, Vector vec2)
        {
            if (vec1.Count != vec2.Count)
                throw new Exception("Длины векторов не равны");
            var res = new Vector(vec1.Count);
            for (int i = 0; i < vec1.Count; i++)
                res[i] = vec1[i] - vec2[i];
            return res;
        }

        public static Vector operator +(Vector vec1, Vector vec2)
        {
            if (vec1.Count != vec2.Count)
                throw new Exception("Длины векторов не равны");
            var res = new Vector(vec1.Count);
            for (int i = 0; i < vec1.Count; i++)
                res[i] = vec1[i] + vec2[i];
            return res;
        }

        public static double operator *(Vector vec1, Vector vec2)
        {
            if (vec1.Count != vec2.Count)
                throw new Exception("Длины векторов не равны");
            double product = 0;
            for (int i = 0; i < vec1.Count; i++)
                product += vec1[i] * vec2[i];
            return product;
        }

        public static Vector operator *(double number, Vector vec)
        {
            Vector product = new Vector(vec.Count);
            for (int i = 0; i < vec.Count; i++)
                product[i] += number * vec[i];
            return product;
        }
    }

    public interface IParametricFunction<TFunction, TParameter, TInput, TOutput> where TFunction : IFunction<TInput, TOutput>
    {
        TFunction Bind(TParameter parameters);
    }

    public interface IFunction<TInput, TOutput>
    {
        TOutput Value(TInput point);
    }

    public interface IDifferentiableFunction<TInput, TOutput> : IFunction<TInput, TOutput>
    {
        // По параметрам исходной IParametricFunction
        IVector Gradient(TInput point, int i);
    }
    public interface IFunctional<TFunction, TInput, TOutput> where TFunction : IFunction<TInput, TOutput>
    {
        double Value(TFunction function);
    }
    public interface IDifferentiableFunctional<TFunction, TInput, TOutput> : IFunctional<TFunction, TInput, TOutput> where TFunction : IDifferentiableFunction<TInput, TOutput>
    {
        IVector Gradient(TFunction function);
    }
    public interface IMatrix : IList<IList<double>>
    {
        public IVector SolveSLAE(IVector rightPart);
    }
    public class Matrix : List<IList<double>>, IMatrix
    {
        public Matrix(int n, int m) : base(n)
        {
            N = n;
            M = m;
            for (int i = 0; i < n; i++)
            {
                Add(new List<double>(Enumerable.Repeat(0.0, m)));
            }
        }

        public int N { get; init; }
        public int M { get; init; }
        public static Vector operator *(Matrix matrix, IVector vector)
        {
            var product = new Vector(matrix.N);
            Parallel.For(0, matrix.N, i =>
                {
                    for (int j = 0; j < matrix.M; j++)
                        product[i] += matrix[i][j] * vector[j];
                });

            return product;
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.M != matrix2.N)
                throw new Exception("матрицы нельзя перемножить");
            var res = new Matrix(matrix1.N, matrix2.M);
            Parallel.For(0, matrix1.N, i =>
            {
                for (int j = 0; j < matrix2.M; j++)
                {
                    for (int k = 0; k < matrix1.M; k++)
                    {
                        res[i][j] += matrix1[i][k] * matrix2[k][j];
                    }
                }
            });
            return res;
        }

        public Matrix Transpose()
        {
            var res = new Matrix(this.M, this.N);
            Parallel.For(0, this.M, i =>
            {
                for (int j = 0; j < this.N; j++)
                    res[i][j] = this[j][i];
            });


            return res;
        }

        public IVector SolveSLAE(IVector rightPart)
        {
            var slae = new SLAE(this, rightPart as Vector, 1e-128);
            return slae.LOS().solution;
        }

        public class SLAE
        {
            Matrix A;
            Vector B;
            int maxIters = 100000;
            double eps = 1e-128;
            public SLAE(Matrix A, Vector B, double eps)
            {
                this.A = A;
                this.B = B;
                this.eps = eps;
            }
            public SLAE() { }

            public (Vector solution, double residual) LOS()
            {
                int N = this.B.Count;

                double alpha, beta;
                double squareNorm;
                Vector q = new(N);
                Vector r;
                Vector z;
                Vector p;
                Vector tmp;

                r = this.B - this.A * q;

                z = r.Clone();

                p = this.A * z;

                squareNorm = r * r;

                for (int index = 0; index < this.maxIters && squareNorm > eps; index++)
                {
                    alpha = (p * r) / (p * p);
                    q = q + alpha * z;
                    squareNorm = (r * r) - (alpha * alpha) * (p * p);
                    r = r - alpha * p;

                    tmp = this.A * r;

                    beta = -(p * tmp) / (p * p);
                    z = r + beta * z;
                    p = tmp + beta * p;
                    if (index == this.maxIters - 1)
                        Console.WriteLine("MaxIter");
                }
                return (q, squareNorm);
            }
        }
    }

    public interface ILeastSquaresFunctional<TInput, TOutput> : IFunctional<IDifferentiableFunction<TInput, TOutput>, TInput, TOutput>
    {
        IVector Residual(IDifferentiableFunction<TInput, TOutput> function);
        IMatrix Jacobian(IDifferentiableFunction<TInput, TOutput> function);
    }
    public interface IOptimizator<TFunctional, TFunction, TParameter, TInput, TOutput> where TFunctional : IFunctional<TFunction, TInput, TOutput> where TFunction : IFunction<TInput, TOutput>
    {
        TParameter Minimize(TFunctional objective, IParametricFunction<IDifferentiableFunction<TInput, TOutput>, TParameter, TInput, TOutput> function, TParameter initialParameters, IVector minimumParameters = default, IVector maximumParameters = default);
    }

}
