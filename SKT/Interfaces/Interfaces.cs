using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
   public interface IVector : IList<double> { }

   public class MyList : List<double>, IVector;

   public interface IParametricFunction<TParameter, TInput, TOutput>
   {
      IFunction<TInput, TOutput> Bind(TParameter parameters);
   }

   public interface IFunction<TInput, TOutput>
   {
      TOutput Value(TInput point);
   }

   public interface IDifferentiableFunction<TInput, TOutput> : IFunction<TInput, TOutput>
   {
      // По параметрам исходной IParametricFunction
      IVector Gradient(TInput point);
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

   }
   public interface ILeastSquaresFunctional<TInput, TOutput> : IFunctional<IDifferentiableFunction<TInput, TOutput>, TInput, TOutput>
   {
      IVector Residual(IDifferentiableFunction<TInput, TOutput> function);
      IMatrix Jacobian(IDifferentiableFunction<TInput, TOutput> function);
   }
   public interface IOptimizator<TFunctional, TFunction, TParameter, TInput, TOutput> where TFunctional : IFunctional<TFunction, TInput, TOutput> where TFunction : IFunction<TInput, TOutput>
   {
      TParameter Minimize(TFunctional objective, IParametricFunction<TParameter, TInput, TOutput> function, TParameter initialParameters, IVector minimumParameters = default, IVector maximumParameters = default);
   }

}
