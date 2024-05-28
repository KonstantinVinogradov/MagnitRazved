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
            const double diffparameter = 0.0005;
            public IVector Gradient(Vector3D point, int i)
            {
                Vector res = [];
                foreach (Element elem in _mesh.Elements)
                {
                    var value = Value(point);
                    var curmaterial = _parameters[elem.MaterialNumber];

                    var delta = Math.Abs(curmaterial.P.X) > 1e-7 ? curmaterial.P.X * diffparameter : 1e-7;
                    curmaterial.P.X += delta;
                    AddToList(res, (Value(point) - value) / delta, i);
                    curmaterial.P.X -= delta;

                    delta = Math.Abs(curmaterial.P.Y) > 1e-7 ? curmaterial.P.Y * diffparameter : 1e-7;
                    curmaterial.P.Y += delta;
                    AddToList(res, (Value(point) - value) / delta, i);
                    curmaterial.P.Y -= delta;

                    delta = Math.Abs(curmaterial.P.Z) > 1e-7 ? curmaterial.P.Z * diffparameter : 1e-7;
                    curmaterial.P.Z += delta;
                    AddToList(res, (Value(point) - value) / delta, i);
                    curmaterial.P.Z -= delta;
                }
                return res;
            }
            private void AddToList(Vector vec, Vector3D value, int i)
            {
                switch (i)
                {
                    case 0:
                        vec.Add(value.X);
                        break;
                    case 1:
                        vec.Add(value.Y);
                        break;
                    case 2:
                        vec.Add(value.Z);
                        break;
                }
            }

            public Vector3D Value(Vector3D point)
            {
                double x = 0.0, y = 0.0, z = 0.0;
                Parallel.For(0, _mesh.Elements.Count(),
                    (i) =>
                    {
                        var elem = _mesh.Elements[i];
                        var P = _parameters[elem.MaterialNumber].P;
                        Vector3D r = point - _mesh.GetElementCenter(elem);
                        double rnorm = r.Norm;
                        double Bx = _mesh.GetElementMeasure(elem) / (4 * Math.PI * rnorm * rnorm * rnorm) *
                           (P.X * (3 * r.X * r.X / rnorm / rnorm - 1) +
                           P.Y * (3 * r.X * r.Y / rnorm / rnorm) +
                           P.Z * (3 * r.X * r.Z / rnorm / rnorm));
                        double By = _mesh.GetElementMeasure(elem) / (4 * Math.PI * rnorm * rnorm * rnorm) *
                           (P.X * (3 * r.Y * r.X / rnorm / rnorm) +
                           P.Y * (3 * r.Y * r.Y / rnorm / rnorm - 1) +
                           P.Z * (3 * r.Y * r.Z / rnorm / rnorm));
                        double Bz = _mesh.GetElementMeasure(elem) / (4 * Math.PI * rnorm * rnorm * rnorm) *
                           (P.X * (3 * r.Z * r.X / rnorm / rnorm) +
                           P.Y * (3 * r.Z * r.Y / rnorm / rnorm) +
                           P.Z * (3 * r.Z * r.Z / rnorm / rnorm - 1));

                        x.ThreadSafeAdd(Bx);
                        y.ThreadSafeAdd(By);
                        z.ThreadSafeAdd(Bz);
                    });

                
                return new Vector3D(x,y,z);
            }
        }
    }
}
