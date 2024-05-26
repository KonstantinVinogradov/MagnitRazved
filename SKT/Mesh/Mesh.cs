using SKT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace SKT.Mesh;
internal class Mesh : IMesh
{
    public IEnumerable<Element> Elements => _elements;
    private readonly List<Element> _elements;

    public IReadOnlyList<Vector3D> Points => _points;

    public Vector3D this[int index] => _points[index];

    private readonly List<Vector3D> _points;

    public Mesh(List<Element> elements, List<Vector3D> points)
    {
        _elements = elements;
        _points = points;
    }

    public Vector3D GetElementCenter(Element elem)
    {
        var v1 = Points[elem.Vernums[0]];
        var v2 = Points[elem.Vernums[^1]];
        return (v1 + v2) / 2;
    }

    public double GetElementMeasure(Element elem)
    {
        var v1 = Points[elem.Vernums[0]];
        var v2 = Points[elem.Vernums[^1]];
        var dv = v2 - v1;
        return dv.X * dv.Y * dv.Z;
    }

    public bool IsPointInsideElement(Element elem, Vector3D point)
    {
        var v1 = Points[elem.Vernums[0]];
        var v2 = Points[elem.Vernums[^1]];
        if (point.X >= v1.X && point.X <= v2.X &&
            point.Y >= v1.Y && point.Y <= v2.Y &&
            point.Z >= v1.Z && point.Z <= v2.Z )
            return true;
        return false;
    }
}
