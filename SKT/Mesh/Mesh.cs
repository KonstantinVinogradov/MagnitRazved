using SKT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SKT.Mesh;
public class Mesh : IMesh
{
    public List<Element> Elements {  get; set; }

    public List<Vector3D> Points { get; set; }

    [JsonIgnore]
    IReadOnlyList<Element> IMesh.Elements => Elements;

    [JsonIgnore]
    IReadOnlyList<Vector3D> IMesh.Points => Points;

    public Vector3D this[int index] => Points[index];

    public Mesh(List<Element> elements, List<Vector3D> points)
    {
        Elements= elements;
        Points= points;
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
            point.Z >= v1.Z && point.Z <= v2.Z)
            return true;
        return false;
    }
}
