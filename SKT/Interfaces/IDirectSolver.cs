﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
   public interface IDirectSolver : IParametricFunction<List<Material>, Vector3D, Vector3D>
   {

   }
   public class DirectSolver : IDirectSolver
   {
      private readonly IMesh _mesh;

      public DirectSolver(IMesh mesh)
      {
         _mesh = mesh;
      }

      public IFunction<Vector3D, Vector3D> Bind(List<Material> parameters)
      {
         return new Sollution(_mesh, parameters);
      }

      private class Sollution : IFunction<Vector3D, Vector3D>
      {
         private readonly IMesh _mesh;
         private readonly List<Material> _parameters;

         public Sollution(IMesh mesh, List<Material> parameters)
         {
            _mesh = mesh;
            _parameters = parameters;
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
                  P.Z * (3 * r.Z * r.Z / rnorm / rnorm-1));
               result += new Vector3D(Bx, By, Bz);
            }
            return result;
         }
      }
   }
}
