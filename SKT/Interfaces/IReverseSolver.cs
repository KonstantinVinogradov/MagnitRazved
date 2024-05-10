using System;
using System.Collections.Generic;
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

    }
    public class ReverseSolver : IReverseSolver
    {
        /// <summary>
        /// Точки измерений
        /// </summary>
        public List<(Vector3D point, Vector3D B)> Data { get; set; }
        private IDirectSolver _solver;
        private readonly IMesh _mesh;

        public ReverseSolver(List<(Vector3D point, Vector3D B)> data, IMesh mesh)
        {
            Data = data;
            _mesh = mesh;
            _solver = new DirectSolver(mesh);
        }


        private class LeasTSquaresFunctional : ILeastSquaresFunctional<Vector3D, Vector3D>
        {
            private readonly List<(Vector3D point, Vector3D B)> Data;
            private readonly int _parametersCount;

            public LeasTSquaresFunctional(List<(Vector3D point, Vector3D B)> data, int parametersCount)
            {
                Data = data;
                _parametersCount = parametersCount;
            }

            public IMatrix Jacobian(IDifferentiableFunction<Vector3D, Vector3D> function)
            {
                var matrix = new Matrix(Data.Count, _parametersCount);
                for (int i = 0; i < Data.Count; i++)
                {
                    var curgrad = function.Gradient(Data[i].point);
                    for (int j = 0; j < _parametersCount; j++)
                        matrix[i][j] += curgrad[j];
                }
                return matrix;
            }

            public IVector Residual(IDifferentiableFunction<Vector3D, Vector3D> function)
            {

                IVector result = new Vector();
                for (int i = 0; i < Data.Count; i++)
                {
                    (Vector3D point, Vector3D B) data = Data[i];
                    for (int j = 0; j < Data.Count; j++)
                    {
                        result.Add((function.Value(data.point) - data.B).Norm);
                    }
                }
                return result;
            }

            public double Value(IDifferentiableFunction<Vector3D, Vector3D> function)
            {
                double res = 0;
                foreach (var point in Data)
                {
                    res += (function.Value(point.point) - point.B).Norm;
                }
                return res;
            }
        }

        private const int Maxiter = 150;
        public List<Material> Minimize(ILeastSquaresFunctional<Vector3D, Vector3D> objective, IParametricFunction<IDifferentiableFunction<Vector3D, Vector3D>, List<Material>, Vector3D, Vector3D> function, List<Material> initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
        {
            int k = 0;
            var f = _solver.Bind(initialParameters);
            while (objective.Value(f) > 1e-15 && k < Maxiter)
            {
                var mat = objective.Jacobian(f);
                var b = objective.Residual(f);
                var dmaterial = mat.SolveSLAE(b);
                for (int i = 0; i < initialParameters.Count; i++)
                {
                    initialParameters[i].I += dmaterial[4 * i];

                    initialParameters[i].P.X += dmaterial[4 * i + 1];
                    initialParameters[i].P.Y += dmaterial[4 * i + 2];
                    initialParameters[i].P.Z += dmaterial[4 * i + 3];
                }
                k++;
            }
            return initialParameters;
        }
    }
}
