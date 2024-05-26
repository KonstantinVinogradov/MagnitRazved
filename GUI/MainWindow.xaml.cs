using SKT;
using SKT.Interfaces;
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
        public MainWindow()
        {
            InitializeComponent();
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

        double minx, maxx, miny, maxy;

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
                minx = _mesh.Elements.Where(el => IsElementInProjectionPlane(el)).Min(el => _mesh.Points[el.Vernums[0]].X);
                maxx = _mesh.Elements.Where(el => IsElementInProjectionPlane(el)).Max(el => _mesh.Points[el.Vernums[^1]].X);
                miny = _mesh.Elements.Where(el => IsElementInProjectionPlane(el)).Min(el => _mesh.Points[el.Vernums[0]].Y);
                maxy = _mesh.Elements.Where(el => IsElementInProjectionPlane(el)).Max(el => _mesh.Points[el.Vernums[^1]].Y);
                if(maxx-minx>maxy-miny)
                {
                    maxy = miny + maxx-miny;
                }
                Draw();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private Path GetLocalRectangle(Element element)
        {
            double wcanv = Canvas.ActualWidth;
            double hcanv = Canvas.ActualHeight;
            double canvcoef = Math.Min(wcanv, hcanv);
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Rect
                ((_mesh[element.Vernums[0]].X - minx) / (maxx - minx) * canvcoef,
                (_mesh[element.Vernums[0]].Y - miny) / (maxy - miny) * canvcoef,
                (_mesh[element.Vernums[^1]].X - _mesh[element.Vernums[0]].X) / (maxx - minx) * canvcoef,
                (_mesh[element.Vernums[^1]].Y - _mesh[element.Vernums[0]].Y) / (maxy - miny) * canvcoef);

            Path path = new Path();
            path.Fill = new SolidColorBrush(Colors.Yellow);
            path.Stroke = new SolidColorBrush(Colors.Red);
            path.StrokeThickness = 5;
            path.Data = geometry;
            return path;
        }
        private void Draw()
        {
            if (_mesh == null)
            {
                MessageBox.Show("Сетка не построена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var elements = _mesh.Elements.Where(el => IsElementInProjectionPlane(el)).ToList();
            Canvas.Children.Clear();

            foreach (var item in elements)
            {
                Canvas.Children.Add(GetLocalRectangle(item));
            }

        }
    }
}