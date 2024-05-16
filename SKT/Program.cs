using SKT;
using SKT.Interfaces;
using SKT.Mesh;

var builder = new MeshBuilder()
{
    X = [0, 10, 20, 30, 40, 50, 60],
    Y = [0, 10, 20, 30, 40, 50, 60],
    Z = [0, 0.5, 1, 1.5, 2, 2.5, 3]
};
var mesh = builder.Build();
IDirectSolver solver = new DirectSolver(mesh);
var trueMaterials = new List<Material>();
foreach (var element in mesh.Elements)
{
    trueMaterials.Add(new Material(new Vector3D(1, 1, 1)));
}
trueMaterials[trueMaterials.Count / 2].P = new Vector3D(200,200,200);

var trueSolution = solver.Bind(trueMaterials);
List<Vector3D> MeasurementPoints = [];
for (int i = 0; i < 36; i++)
{
    for (int j = 0; j < 36; j++)
    {
        MeasurementPoints.Add(new Vector3D(10.0/12 + 10.0/6 * i, 10.0 / 12 + 10.0 / 6 * j, 3.5));
    }
}
var data = MeasurementPoints.Select(t => (t, trueSolution.Value(t))).ToList();
IReverseSolver revSolvwer = new ReverseSolver(data, mesh);

var materials = new List<Material>();
foreach (var element in mesh.Elements)
{
    materials.Add(new Material(new Vector3D(0, 0, 0)));
}
var asd = revSolvwer.Minimize(new LeastSquaresFunctional(data, mesh.Elements.Count() * 3), solver, materials);


Console.WriteLine("Hello, World!");
