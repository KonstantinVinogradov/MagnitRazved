using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
   public interface IReverseSolver : IOptimizator<ILeastSquaresFunctional<Vector, Vector>, IDifferentiableFunction<Vector, Vector>, List<Material>, Vector, Vector>
   {

   }
   public class ReverseSolver : IReverseSolver
   {
      /// <summary>
      /// Точки измерений
      /// </summary>
      public List<(Vector point, Vector B)> Data { get; set; }
      private IDirectSolver _solver;

      public ReverseSolver(List<(Vector point, Vector B)> data, IMesh mesh)
      {
         Data = data;
         _solver = new DirectSolver(mesh);
      }

      private class LeasTSquaresFunctional : ILeastSquaresFunctional<Vector, Vector>
      {
         private readonly List<(Vector point, Vector B)> Data;

         public LeasTSquaresFunctional(List<(Vector point, Vector B)> data)
         {
            Data = data;
         }

         public IMatrix Jacobian(IDifferentiableFunction<Vector, Vector> function)
         {
                throw new NotImplementedException();
         }

         public IVector Residual(IDifferentiableFunction<Vector, Vector> function)
         {
            IVector result = null;
            for (int i = 0; i < Data.Count; i++)
            {
               (Vector point, Vector B) data = Data[i];
               var cur = function.Gradient(data.point);
               if (result is null)
               {
                  result = new MyList();
                  for (int j = 0; j < cur.Count; j++)
                  {
                     //result.Add(cur[]);

                  }
               }

            }
            throw new NotImplementedException();
         }

         public double Value(IDifferentiableFunction<Vector, Vector> function)
         {
            double res = 0;
            foreach (var point in Data)
            {
               res += (function.Value(point.point) - point.B).Norm;
            }
            return res;
         }
      }

  

        public List<Material> Minimize(ILeastSquaresFunctional<Vector, Vector> objective, IParametricFunction<List<Material>, Vector, Vector> function, List<Material> initialParameters, IVector minimumParameters = null, IVector maximumParameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
