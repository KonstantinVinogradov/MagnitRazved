using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GUI
{
   public class PointWrapper : INotifyPropertyChanged
   {

      private System.Windows.Point _point;

      public PointWrapper(System.Windows.Point point)
      {
         _point = point;
      }
      public PointWrapper()
      {

      }
      public PointWrapper(double x, double y)
      {
         _point = new System.Windows.Point(x, y);
      }
      public static implicit operator System.Windows.Point(PointWrapper wrapper)
      {
         return wrapper._point;
      }
      public double X
      {
         get => _point.X;
         set
         {
            if (_point.X != value)
            {
               _point.X = value;
               OnPropertyChanged();
            }
         }
      }
      public double Y
      {
         get => _point.Y;
         set
         {
            if (_point.Y != value)
            {
               _point.Y = value;
               OnPropertyChanged();
            }
         }
      }

      public event PropertyChangedEventHandler? PropertyChanged;
      public void OnPropertyChanged([CallerMemberName] string prop = "")
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
      }
   }

   public class Series : INotifyPropertyChanged
   {
      [JsonInclude]
      public ObservableCollection<PointWrapper> Points { get; set; } = new();
      private LineType lineType = LineType.Line;

      public LineType LineType { get => lineType; set { lineType = value; OnPropertyChanged(); } }
      private System.Windows.Media.Color color;

      public event PropertyChangedEventHandler? PropertyChanged;
      public void OnPropertyChanged([CallerMemberName] string prop = "")
      {
         if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
      }

      public System.Windows.Media.Color Color
      {
         get

         { return color; }
         set
         {
            color = value;
            OnPropertyChanged();
            Brush = new SolidColorBrush(color);
         }

      }
      private Brush brush;
      private static int count = 1;
      public Series()
      {
         Color = Colors.Black;
         LineType = LineType.Line;
         Name = $"Line {count++}";
      }

      [JsonIgnore]
      public Brush Brush { get => brush; set { brush = value; OnPropertyChanged(); } }
      public string Name { get => name; set { name = value; OnPropertyChanged(); } }
      private RelayCommand addPoint;
      private string name;

      [JsonIgnore]
      public RelayCommand AddPoint
      {
         get
         {
            return addPoint ??
              (addPoint = new RelayCommand(obj =>
              {

                 Points.Add(new(0, 0));

              }));
         }
      }
   }
   public enum LineType
   {
      Line,
      Spline
   }
}
