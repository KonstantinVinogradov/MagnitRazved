using SKT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKT;
public class MeshBuilder
{
    public double[] X { get; set; } = [];
    public double[] Y { get; set; } = [];
    public double[] Z { get; set; } = [];
    public IMesh Build()
    {
        List<Vector3D> vertices = [];
        List<Element> elements = [];
        for (int i = 0; i < Z.Length; i++)
        {
            for (int j = 0; j < Y.Length; j++)
            {
                for (int k = 0; k < X.Length; k++)
                {
                    vertices.Add(new Vector3D(X[k], Y[j], Z[i]));
                }
            }
        }
        int counter = 0;
        int dy = X.Length;
        int dz = X.Length * Y.Length;
        for (int i = 0; i < Z.Length - 1; i++)
        {
            for (int j = 0; j < Y.Length - 1; j++)
            {
                for (int k = 0; k < X.Length - 1; k++)
                {
                    elements.Add(new Element(
                        vernums:
                        [
                            (i)*dz+ (j)*dy+(k),
                            (i)*dz+ (j)*dy+(k+1),
                            (i)*dz+ (j+1)*dy+(k),
                            (i)*dz+ (j+1)*dy+(k+1),
                            (i+1)*dz+ (j)*dy+(k),
                            (i+1)*dz+ (j)*dy+(k+1),
                            (i+1)*dz+ (j+1)*dy+(k),
                            (i+1)*dz+ (j+1)*dy+(k+1)
                        ],
                        counter++));
                }
            }
        }
        return new Mesh.Mesh(elements,vertices);
    }
}
