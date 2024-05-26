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
                int xcount =  int.Parse(XCount.Text);


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
                    x[i] = x0 + (x1 - x0) / xcount;
                }

                for (int i = 0; i < y.Length; i++)
                {
                    y[i] = y0 + (y1 - y0) / ycount;
                }

                for (int i = 0; i < z.Length; i++)
                {
                    z[i] = z0 + (z1 - z0) / zcount;
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

        private void ProjXYButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void ProjYZButton_Click(object sender, RoutedEventArgs e)
        {
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
                    if(_mesh)
                    break;
                case ProjectionTypeEnum.YZ:
                    break;
                case ProjectionTypeEnum.XZ:
                    break;
                default:
                    break;
            }
        }
        private void Draw()
        {
            if(_mesh==null)
            {
                MessageBox.Show("Сетка не построена","Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}