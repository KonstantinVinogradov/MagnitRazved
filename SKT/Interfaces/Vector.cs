using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
   public struct Vector
   {
      public Vector(double x, double y, double z)
      {
         X = x;
         Y = y;
         Z = z;
      }
      public static Vector Zero => new Vector(0, 0, 0);
      public double X { get; set; }
      public double Y { get; set; }
      public double Z { get; set; }
      public Vector Cross(Vector v)
      {
         return new Vector(Y * v.Z - Z * v.Y, v.X * Z - v.Z * X, X * v.Y - Y * v.X);
      }
      public double Norm => Math.Sqrt(this * this);
      public Vector Normallize() => this / Norm;
      public static Vector operator +(Vector left, Vector right)
      {
         return new Vector(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
      }
      public static Vector operator -(Vector left, Vector right)
      {
         return new Vector(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
      }
      public static double operator *(Vector left, Vector right)
      {
         return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
      }
      public static Vector operator *(Vector vector, double scalar)
      {
         return new Vector(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
      }
      public static Vector operator *(double scalar, Vector vector)
      {
         return new Vector(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
      }
      public static Vector operator /(Vector vector, double scalar)
      {
         return new Vector(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
      }
   }
}
