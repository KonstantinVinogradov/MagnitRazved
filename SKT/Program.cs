using GUI;
using SKT;
using SKT.Interfaces;
using SKT.Mesh;
using System.Text.Json;


var meshtrue = JsonSerializer.Deserialize<MeshSaveModel>(File.ReadAllText("C:\\Users\\Konstantin\\OneDrive\\Рабочий стол\\Новая папка\\smilefacetrue"));

var meshres= JsonSerializer.Deserialize<MeshSaveModel>(File.ReadAllText("C:\\Users\\Konstantin\\OneDrive\\Рабочий стол\\Новая папка\\smilefaceresult"));


double z = 3.5;
double x = 2.5;
double y = 5;
for (int i = 0; i < 20;i++)
{
   var elemres = meshres.Mesh.Elements.FirstOrDefault(t => meshres.Mesh.IsPointInsideElement(t, new(x, y, z)));
   if(elemres != null)
   {
      var elemtrue = meshres.Mesh.Elements.FirstOrDefault(t => meshres.Mesh.IsPointInsideElement(t, new(x, y, z)));
      if (elemtrue != null)
      {
         Console.WriteLine($"{x} {(meshres.Materials[elemres.MaterialNumber].P - meshtrue.Materials[elemtrue.MaterialNumber].P).Norm}");
      }
   }
   x += 5;
}

//var rez = GeneratePoints(0, 100, 30, 0.5, 1, 2, 35);
//{
//    var str = System.Text.Json.JsonSerializer.Serialize(rez);
//    File.WriteAllText("C:\\Users\\Konstantin\\OneDrive\\Рабочий стол\\Новая папка\\big точки измерений.jjj", str);
//}
//int nx = 20;
//int ny = 1;
//int nz = 7;

//MeshBuilder builder = new();
//builder.X = Enumerable.Range(0, nx+1).Select(t => t * 100.0/nx).ToArray();
//builder.Y = Enumerable.Range(0, ny+1).Select(t => t * 10.0/ny).ToArray();
//builder.Z = Enumerable.Range(0, nz+1).Select(t => t * 10.0 / nz).ToArray();
//var mesh = builder.Build();
//var directSolver = new DirectSolver(mesh);
//List<Material> materials = [];
//foreach (var item in mesh.Elements)
//{
//    materials.Add(new Material());
//}
//materials[100].P = new(4, 4, 4);
//var generator = directSolver.Bind(materials);
//var data = rez.Select(t => (t, generator.Value(t))).ToList();
//var reverseSolver = new ReverseSolver(data,mesh);
//reverseSolver.AlphaRegularization = 0;
//materials[100].P = new();
//var asd =  reverseSolver.Minimize(new LeastSquaresFunctional(data, materials.Count * 3), directSolver, materials);

//Console.WriteLine("Hello, World!");


//List<Vector3D> GeneratePoints(double x0, double x1, int xn, double y0, double y1, int yn, double z)
//{
//    List<Vector3D> points = [];
//    for (int i = 0; i < xn; i++)
//    {
//        for (int j = 0; j < yn-1; j++)
//        {
//            points.Add(new Vector3D(x0 + i * (x1 - x0) / (xn - 1), y0 + j * (y1 - y0) / (yn - 1), z));
//        }
//    }
//    return points;
//}


