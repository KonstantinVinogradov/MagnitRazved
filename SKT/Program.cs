using SKT;
using SKT.Interfaces;
using SKT.Mesh;

var rez = GeneratePoints(0, 100, 100, 5, 10, 2, 11);
{
    var str = System.Text.Json.JsonSerializer.Serialize(rez);
    File.WriteAllText("C:\\Users\\sergopavlov\\Documents\\Тест Кишлак\\points", str);
}
int nx = 20;
int ny = 1;
int nz = 7;

MeshBuilder builder = new();
builder.X = Enumerable.Range(0, nx+1).Select(t => t * 100.0/nx).ToArray();
builder.Y = Enumerable.Range(0, ny+1).Select(t => t * 10.0/ny).ToArray();
builder.Z = Enumerable.Range(0, nz+1).Select(t => t * 10.0 / nz).ToArray();
var mesh = builder.Build();
var directSolver = new DirectSolver(mesh);
List<Material> materials = [];
foreach (var item in mesh.Elements)
{
    materials.Add(new Material());
}
materials[100].P = new(4, 4, 4);
var generator = directSolver.Bind(materials);
var data = rez.Select(t => (t, generator.Value(t))).ToList();
var reverseSolver = new ReverseSolver(data,mesh);
reverseSolver.AlphaRegularization = 0;
materials[100].P = new();
var asd =  reverseSolver.Minimize(new LeastSquaresFunctional(data, materials.Count * 3), directSolver, materials);

Console.WriteLine("Hello, World!");


List<Vector3D> GeneratePoints(double x0, double x1, int xn, double y0, double y1, int yn, double z)
{
    List<Vector3D> points = [];
    for (int i = 0; i < xn; i++)
    {
        for (int j = 0; j < yn-1; j++)
        {
            points.Add(new Vector3D(x0 + i * (x1 - x0) / (xn - 1), y0 + j * (y1 - y0) / (yn - 1), z));
        }
    }
    return points;
}


