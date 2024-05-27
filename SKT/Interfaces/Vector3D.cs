using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SKT.Interfaces
{
   public class Vector3D
   {
        public Vector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
        public Vector3D(double x, double y, double z)
      {
         X = x;
         Y = y;
         Z = z;
      }
      public static Vector3D Zero => new Vector3D(0, 0, 0);
      public double X { get; set; }
      public double Y { get; set; }
      public double Z { get; set; }
      public Vector3D Cross(Vector3D v)
      {
         return new Vector3D(Y * v.Z - Z * v.Y, v.X * Z - v.Z * X, X * v.Y - Y * v.X);
      }
        [JsonIgnore]
      public double Norm => Math.Sqrt(this * this);
      public Vector3D Normallize() => this / Norm;
      public static Vector3D operator +(Vector3D left, Vector3D right)
      {
         return new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
      }
      public static Vector3D operator -(Vector3D left, Vector3D right)
      {
         return new Vector3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
      }
      public static double operator *(Vector3D left, Vector3D right)
      {
         return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
      }
      public static Vector3D operator *(Vector3D vector, double scalar)
      {
         return new Vector3D(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
      }
      public static Vector3D operator *(double scalar, Vector3D vector)
      {
         return new Vector3D(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
      }
      public static Vector3D operator /(Vector3D vector, double scalar)
      {
         return new Vector3D(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
      }
        public override string ToString()
        {
            return $"{X} {Y} {Z}";
        }
    }
}
