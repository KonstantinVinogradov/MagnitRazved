using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public ReverseSolver(List<(Vector3D point, Vector3D B)> data, IMesh mesh)
        {
            Data = data;
            _solver = new DirectSolver(mesh);
        }

        private class LeasTSquaresFunctional : ILeastSquaresFunctional<Vector3D, Vector3D>
        {
            private readonly List<(Vector3D point, Vector3D B)> Data;

            public LeasTSquaresFunctional(List<(Vector3D point, Vector3D B)> data)
            {
                Data = data;
            }

            public IMatrix Jacobian(IDifferentiableFunction<Vector3D, Vector3D> function)
            {
                throw new NotImplementedException();
            }

            public IVector Residual(IDifferentiableFunction<Vector3D, Vector3D> function)
            {

                IVector result = new Vector();
                for (int i = 0; i < Data.Count; i++)
                {
                    (Vector3D point, Vector3D B) data = Data[i];
                    var cur = function.Gradient(data.point);
                    for (int j = 0; j < cur.Count; j++)
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



        public List<Material> Minimize(ILeastSquaresFunctional<Vector3D, Vector3D> objective, IParametricFunction<List<Material>, Vector3D, Vector3D> function, List<Material> initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
