using SKT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для Vector3DRequestWindow.xaml
    /// </summary>
    public partial class Vector3DRequestWindow : Window
    {
        private Vector3D vector;
        public bool Finished { get; private set; } = false;
        public Vector3DRequestWindow(ref Vector3D vector)
        {
            this.vector = vector;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = double.Parse(textBoxX.Text);
                double y = double.Parse(textBoxY.Text);
                double z = double.Parse(textBoxZ.Text);
                vector.X = x;
                vector.Y = y;
                vector.Z = z;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не получается запарсить вектор");
            }
            Finished = true;
            this.Close();
        }
    }
}
