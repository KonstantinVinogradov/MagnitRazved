using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
    public interface IDirectSolver : IParametricFunction<IDifferentiableFunction<Vector3D, Vector3D>, List<Material>, Vector3D, Vector3D>
    {

    }
    public class DirectSolver : IDirectSolver
    {
        private readonly IMesh _mesh;

        public DirectSolver(IMesh mesh)
        {
            _mesh = mesh;
        }

        public IDifferentiableFunction<Vector3D, Vector3D> Bind(List<Material> parameters)
        {
            return new Sollution(_mesh, parameters);
        }



        private class Sollution : IDifferentiableFunction<Vector3D, Vector3D>
        {
            private readonly IMesh _mesh;
            private readonly List<Material> _parameters;

            public Sollution(IMesh mesh, List<Material> parameters)
            {
                _mesh = mesh;
                _parameters = parameters;
            }

            public IVector Gradient(Vector3D point)
            {
                Vector res = [];
                foreach (Element elem in _mesh.Elements)
                {
                    if (_mesh.IsPointInsideElement(elem, point))
                    {
                        var value = Value(point);
                        var curmaterial = _parameters[elem.MaterialNumber];

                        double delta = Math.Abs(curmaterial.I) > 1e-7 ? curmaterial.I * 0.05 : 1e-7;
                        curmaterial.I += delta;
                        res.Add((Value(point) - value).Norm / delta);
                        curmaterial.I -= delta;

                        delta = Math.Abs(curmaterial.P.X) > 1e-7 ? curmaterial.P.X * 0.05 : 1e-7;
                        curmaterial.P.X += delta;
                        res.Add((Value(point) - value).Norm / delta);
                        curmaterial.P.X -= delta;

                        delta = Math.Abs(curmaterial.P.Y) > 1e-7 ? curmaterial.P.Y * 0.05 : 1e-7;
                        curmaterial.P.Y += delta;
                        res.Add((Value(point) - value).Norm / delta);
                        curmaterial.P.Y -= delta;

                        delta = Math.Abs(curmaterial.P.Z) > 1e-7 ? curmaterial.P.Z * 0.05 : 1e-7;
                        curmaterial.P.Z += delta;
                        res.Add((Value(point) - value).Norm / delta);
                        curmaterial.P.Z -= delta;
                    }
                    res.AddRange(Enumerable.Repeat(0.0, 4));
                }
                return res;
            }

            public Vector3D Value(Vector3D point)
            {
                Vector3D result = Vector3D.Zero;
                foreach (var elem in _mesh.Elements)
                {
                    var P = _parameters[elem.MaterialNumber].P;
                    double I = _parameters[elem.MaterialNumber].I;
                    Vector3D r = point - _mesh.GetElementCenter(elem);
                    double rnorm = r.Norm;
                    double Bx = I * _mesh.GetElementMeasure(elem) / (4 * Math.PI * rnorm * rnorm * rnorm) *
                       (P.X * (3 * r.X * r.X / rnorm / rnorm - 1) +
                       P.Y * (3 * r.X * r.Y / rnorm / rnorm) +
                       P.Z * (3 * r.X * r.Z / rnorm / rnorm));
                    double By = I * _mesh.GetElementMeasure(elem) / (4 * Math.PI * rnorm * rnorm * rnorm) *
                       (P.X * (3 * r.Y * r.X / rnorm / rnorm) +
                       P.Y * (3 * r.Y * r.Y / rnorm / rnorm - 1) +
                       P.Z * (3 * r.Y * r.Z / rnorm / rnorm));
                    double Bz = I * _mesh.GetElementMeasure(elem) / (4 * Math.PI * rnorm * rnorm * rnorm) *
                       (P.X * (3 * r.Z * r.X / rnorm / rnorm) +
                       P.Y * (3 * r.Z * r.Y / rnorm / rnorm) +
                       P.Z * (3 * r.Z * r.Z / rnorm / rnorm - 1));
                    result += new Vector3D(Bx, By, Bz);
                }
                return result;
            }
        }
    }
}
