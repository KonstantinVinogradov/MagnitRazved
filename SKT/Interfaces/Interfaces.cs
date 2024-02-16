using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
   public interface IVector : IList<double> { }

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
   public interface ILeastSquaresFunctional<TFunction, TInput, TOutput> : IFunctional<TFunction, TInput, TOutput> where TFunction : IFunction<TInput, TOutput>
   {
      IVector Residual(TFunction function);
      IMatrix Jacobian(TFunction function);
   }
   public interface IOptimizator<TFunctional, TFunction, TParameter, TInput, TOutput> where TFunctional : IFunctional<TFunction, TInput, TOutput> where TFunction : IFunction<TInput, TOutput>
   {
      IVector Minimize(TFunctional objective, IParametricFunction<TParameter, TInput, TOutput> function, TParameter initialParameters, IVector minimumParameters = default, IVector maximumParameters = default);
   }
   
}
