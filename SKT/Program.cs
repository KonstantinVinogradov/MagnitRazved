using SKT;
using SKT.Interfaces;
using SKT.Mesh;

var rez = GeneratePoints(0, 1000, 100, 0, 1000, 100, 22);
{
   var str = System.Text.Json.JsonSerializer.Serialize(rez);
   File.WriteAllText("C:\\Users\\Konstantin\\OneDrive\\Рабочий стол\\Новая папка\\123", str);
}
Console.WriteLine("Hello, World!");


List<Vector3D> GeneratePoints(double x0,double x1,int xn,double y0 ,double y1,int yn,double z)
{
    List<Vector3D> points = [];
    for (int i = 0; i < xn; i++)
    {
        for (int j = 0; j < yn; j++)
        {
            points.Add(new Vector3D(x0 + i * (x1 - x0) / (xn - 1), y0 + j * (y1 - y0) / (yn - 1), z));
        }
    }
    return points;
}


