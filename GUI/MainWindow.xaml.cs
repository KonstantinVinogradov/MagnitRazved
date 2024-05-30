using Microsoft.Win32;
using SKT;
using SKT.Interfaces;
using SKT.Mesh;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private IMesh _mesh;
      private List<Material> _materials = [];
      public ObservableCollection<Vector3D> Points { get; private set; } = [];
      public MainWindow()
      {
         InitializeComponent();
         Chart.PointClicked += PointClickedHandler;
         this.DataContext = this;
      }
      private void PointClickedHandler(object? sender, PointWrapper e)
      {
         Vector3D vector = new();
         if (_mesh == null)
            return;
         switch (projectionType)
         {
            case ProjectionTypeEnum.XY:
               vector = new Vector3D(e.X, e.Y, coord);
               break;
            case ProjectionTypeEnum.YZ:
               vector = new Vector3D(coord, e.X, e.Y);
               break;
            case ProjectionTypeEnum.XZ:
               vector = new Vector3D(e.X, coord, e.Y);
               break;
         }
         var elements = _mesh.Elements.Where(t => _mesh.IsPointInsideElement(t, vector)).ToList();
         if (elements.Count == 1)
         {
            Vector3D P = _materials[elements[0].MaterialNumber].P;

            bool result = false;
            do
            {
               Vector3DRequestWindow window = new Vector3DRequestWindow(ref P);
               result = (bool)window.ShowDialog();
            } while (result);
         }
         Draw();

      }


      private void BuildMeshButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            MeshBuilder builder = new();

            double x0 = double.Parse(XFrom.Text);
            double x1 = double.Parse(XTo.Text);
            int xcount = int.Parse(XCount.Text);


            double y0 = double.Parse(YFrom.Text);
            double y1 = double.Parse(YTo.Text);
            int ycount = int.Parse(YCount.Text);

            double z0 = double.Parse(ZFrom.Text);
            double z1 = double.Parse(ZTo.Text);
            int zcount = int.Parse(ZCount.Text);

            double[] x = new double[(xcount) + 1];
            double[] y = new double[(ycount) + 1];
            double[] z = new double[(zcount) + 1];

            for (int i = 0; i < x.Length; i++)
            {
               x[i] = x0 + i * (x1 - x0) / xcount;
            }

            for (int i = 0; i < y.Length; i++)
            {
               y[i] = y0 + i * (y1 - y0) / ycount;
            }

            for (int i = 0; i < z.Length; i++)
            {
               z[i] = z0 + i * (z1 - z0) / zcount;
            }
            builder.X = x;
            builder.Y = y;
            builder.Z = z;
            _mesh = builder.Build();
            foreach (var item in _mesh.Elements)
            {
               _materials.Add(new Material());
            }
            Chart.Elements.Clear();
            Chart.InvalidateVisual();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message);
         }
      }
      private ProjectionTypeEnum projectionType;
      private double coord;
      private enum ProjectionTypeEnum
      {
         XY,
         YZ,
         XZ
      }


      private void ProjXYButton_Click(object sender, RoutedEventArgs e)
      {
         if (_mesh == null)
         {
            MessageBox.Show("Сетка не построена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }
         try
         {
            projectionType = ProjectionTypeEnum.XY;
            coord = double.Parse(ProjXYTextBox.Text);
            Draw();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message);
         }

      }

      private void Draw()
      {
         var minP = _materials.Min(t => t.P.Norm);
         var maxP = _materials.Max(t => t.P.Norm);
         Chart.Elements.Clear();
         var pen = new Pen(new SolidColorBrush(Colors.Black), 3);

         foreach (var element in _mesh.Elements.Where(el => IsElementInProjectionPlane(el)))
         {
            var P = _materials[element.MaterialNumber].P.Norm;
            var brush = new SolidColorBrush(Color.FromArgb(128, (byte)(255 * (P - minP) / (maxP - minP)), (byte)(255 * (maxP - P) / (maxP - minP)), 0));
                switch (projectionType)
                {
                    case ProjectionTypeEnum.XY:
                        Chart.Elements.Add(
                        new DrawingElement(pen,
                                           brush,
                                           new PointWrapper(_mesh[element.Vernums[0]].X, _mesh[element.Vernums[0]].Y),
                                           new PointWrapper(_mesh[element.Vernums[^1]].X, _mesh[element.Vernums[^1]].Y)));
                        break;
                    case ProjectionTypeEnum.YZ:
                        Chart.Elements.Add(
                new DrawingElement(pen,
                                   brush,
                                   new PointWrapper(_mesh[element.Vernums[0]].Y, _mesh[element.Vernums[0]].Z),
                                   new PointWrapper(_mesh[element.Vernums[^1]].Y, _mesh[element.Vernums[^1]].Z)));
                        break;
                    case ProjectionTypeEnum.XZ:
                        Chart.Elements.Add(
                new DrawingElement(pen,
                                   brush,
                                   new PointWrapper(_mesh[element.Vernums[0]].X, _mesh[element.Vernums[0]].Z),
                                   new PointWrapper(_mesh[element.Vernums[^1]].X, _mesh[element.Vernums[^1]].Z)));
                        break;
                }
                
         }
         Chart.InvalidateVisual();
      }

      private void CalculateDirect_Click(object sender, RoutedEventArgs e)
      {
         if (_mesh == null)
            return;
         IDirectSolver solver = new DirectSolver(_mesh);
         var asd = solver.Bind(_materials);
         var P = Points.Select(p => asd.Value(p)).ToList();

         SaveFileDialog dialog = new SaveFileDialog();

         var res = dialog.ShowDialog();
         if ((bool)res)
         {
            var path = dialog.FileName;
            using StreamWriter writer = new StreamWriter(path);
            for (int i = 0; i < P.Count; i++)
            {
               writer.WriteLine($"{Points[i].ToString()} {P[i].ToString()}");
            }

         }

      }

      private void ClearMaterialsButton_Click(object sender, RoutedEventArgs e)
      {
         foreach (var material in _materials)
         {
            material.P = new();
         }
         Draw();
      }

      private async void CalculateReverse_Click(object sender, RoutedEventArgs e)
      {
         OpenFileDialog dialog = new OpenFileDialog();
         var res = dialog.ShowDialog();
         if ((bool)res)
         {
            List<(Vector3D, Vector3D)> data = [];
            using var sr = new StreamReader(dialog.FileName);
            while (!sr.EndOfStream)
            {
               var line = sr.ReadLine();
               var str = line.Split(' ');
               data.Add((new(double.Parse(str[0]), double.Parse(str[1]), double.Parse(str[2])), new(double.Parse(str[3]), double.Parse(str[4]), double.Parse(str[5]))));
            }
            IDirectSolver dirsolver = new DirectSolver(_mesh);

            IReverseSolver revSolvwer = new ReverseSolver(data, _mesh);
            revSolvwer.AlphaRegularization = double.Parse(RegularizationParameter.Text);

            foreach (var item in _materials)
            {
               item.P = new();
            }
            _materials = revSolvwer.Minimize(new LeastSquaresFunctional(data, _mesh.Elements.Count() * 3), dirsolver, _materials);
            Draw();
         }

      }

      private void RibbonApplicationMenuItem_Click(object sender, RoutedEventArgs e)
      {
         OpenFileDialog dialog = new OpenFileDialog();
         bool? res = dialog.ShowDialog();
         if ((bool)res)
         {
            var smesh = File.ReadAllText(dialog.FileName);
            var mesh = System.Text.Json.JsonSerializer.Deserialize<MeshSaveModel>(smesh);
            _mesh = mesh.Mesh;
            _materials = mesh.Materials;
         }
      }

      private void RibbonApplicationMenuItem_Click_1(object sender, RoutedEventArgs e)
      {
         if (_mesh == null)
            return;
         SaveFileDialog dialog = new SaveFileDialog();
         bool? res = dialog.ShowDialog();
         if ((bool)res)
         {
            var mesh = new MeshSaveModel() { Mesh = (Mesh)_mesh, Materials = _materials };
            var smesh = System.Text.Json.JsonSerializer.Serialize(mesh);
            File.WriteAllText(dialog.FileName, smesh);
         }
      }

      private void SavePointsButton_Click(object sender, RoutedEventArgs e)
      {
         SaveFileDialog dialog = new SaveFileDialog();
         bool? res = dialog.ShowDialog();
         if ((bool)res)
         {

            var spoints = System.Text.Json.JsonSerializer.Serialize(Points);
            File.WriteAllText(dialog.FileName, spoints);
         }
      }

      private void LoadPointButton_Click(object sender, RoutedEventArgs e)
      {
         OpenFileDialog dialog = new OpenFileDialog();
         bool? res = dialog.ShowDialog();
         if ((bool)res)
         {
            var spoints = File.ReadAllText(dialog.FileName);
            var points = System.Text.Json.JsonSerializer.Deserialize<List<Vector3D>>(spoints);
            Points.Clear();
            foreach (var item in points)
            {
               Points.Add(item);
            }
         }
      }

      private void ProjYZButton_Click(object sender, RoutedEventArgs e)
      {
         if (_mesh == null)
         {
            MessageBox.Show("Сетка не построена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }
         try
         {
            projectionType = ProjectionTypeEnum.YZ;
            coord = double.Parse(ProjYZTextBox.Text);
            Draw();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message);
         }
      }

      private void ProjXZButton_Click(object sender, RoutedEventArgs e)
      {
         if (_mesh == null)
         {
            MessageBox.Show("Сетка не построена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
         }
         try
         {
            projectionType = ProjectionTypeEnum.XZ;
            coord = double.Parse(ProjXZTextBox.Text);
            Draw();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message);
         }
      }
      private bool IsElementInProjectionPlane(Element element)
      {
         switch (projectionType)
         {
            case ProjectionTypeEnum.XY:
               if (_mesh[element.Vernums[0]].Z <= coord &&
                   _mesh[element.Vernums[^1]].Z >= coord)
                  return true;
               break;
            case ProjectionTypeEnum.YZ:
               if (_mesh[element.Vernums[0]].X <= coord &&
                   _mesh[element.Vernums[^1]].X >= coord)
                  return true;
               break;
            case ProjectionTypeEnum.XZ:
               if (_mesh[element.Vernums[0]].Y <= coord &&
                   _mesh[element.Vernums[^1]].Y >= coord)
                  return true;
               break;
            default:
               break;
         }
         return false;
      }
   }
}