using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Configuration;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace GUI
{
   public class Chart : UserControl
   {
      public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register("Series", typeof(ObservableCollection<Series>), typeof(Chart), new PropertyMetadata(new ObservableCollection<Series>()));
      private Matrix3x2 modelView;
      private double xmin, xmax, ymin, ymax;
      public ObservableCollection<Series> Series
      {
         get
         {
            return (ObservableCollection<Series>)GetValue(SeriesProperty);
         }
         set
         {
            SetValue(SeriesProperty, value);
         }
      }
      public Chart()
      {
         xmin = -10;
         xmax = 10;
         ymin = -10;
         ymax = 10;
         modelView = Matrix3x2.Identity;

      }
      private bool matrixInitiallized = false;
      protected override void OnRender(DrawingContext drawingContext)
      {

         drawingContext.DrawRectangle(new SolidColorBrush(Colors.White), new Pen(new SolidColorBrush(Colors.White), 1), new Rect(0, 0, ActualWidth, ActualHeight));
         if (matrixInitiallized)
         {
            DrawGrid(drawingContext);
            foreach (var item in Series)
            {
               DrawLine(drawingContext, item);
            }
         }

      }
      private void DrawGrid(DrawingContext drawingContext)
      {
         var topleft = ToGlobal(new PointWrapper(0, 0));
         var botRight = ToGlobal(new PointWrapper(ActualWidth, ActualHeight));
         double xlength = botRight.X - topleft.X;
         double ylength = topleft.Y - botRight.Y;
         double minlength = Math.Min(xlength, ylength);
         var log = Math.Round(Math.Log10(minlength));
         double h = Math.Pow(10, log - 1);
         var pen = new Pen(new SolidColorBrush(Colors.Black), 1);
         #region xgrid

         double x0 = topleft.X;
         var remainder = x0 / h;
         remainder = Math.Ceiling(remainder) - remainder;
         x0 = x0 + h * remainder;
         double x1 = botRight.X;
         remainder = x1 / h;
         remainder = Math.Ceiling(remainder) - remainder;
         x1 = x1 + h * remainder;
         int count = (int)Math.Round((x1 - x0) / h);
         var cur = x0;
         while (cur < x1)
         {
            drawingContext.DrawLine(pen, ToLocal(new(cur, botRight.Y)), ToLocal(new(cur, topleft.Y)));

            cur += h;
         }
         #endregion
         #region ygrid

         double y0 = botRight.Y;
         remainder = y0 / h;
         remainder = Math.Ceiling(remainder) - remainder;
         y0 = y0 + h * remainder;
         double y1 = topleft.Y;
         remainder = y1 / h;
         remainder = Math.Ceiling(remainder) - remainder;
         y1 = y1 + h * remainder;
         cur = y0;
         while (cur < y1)
         {
            drawingContext.DrawLine(pen, ToLocal(new(botRight.X, cur)), ToLocal(new(topleft.X, cur)));
            cur += h;
         }
         #endregion
         DrawNums(x0, x1, y0, y1, h, log, drawingContext);

      }
      private static Typeface tf = new Typeface("Cascadia");
      private void DrawNums(double x0, double x1, double y0, double y1, double h, double log, DrawingContext drawingContext)
      {
         var topleft = ToGlobal(new PointWrapper(0, 0));
         var botRight = ToGlobal(new PointWrapper(ActualWidth, ActualHeight));
         var pen = new Pen(new SolidColorBrush(Colors.Black), 1);
         double cur = x0;
         int count = (int)Math.Round((x1 - x0) / h);
         double hx = count > 9 ? 2 * h : h;
         while (cur < x1)
         {

            int digits = log <= 0 ? -(int)log + 1 : 0;
            double roundedx = Math.Round(cur, digits);
            FormattedText text = new(roundedx.ToString($"f{digits}"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 20, new SolidColorBrush(Colors.Black));
            PointWrapper p = ToLocal(new(cur, botRight.Y));
            p.Y -= 20;
            p.X += 5;
            drawingContext.DrawText(text, p);
            cur += hx;
         }
         cur = y0;
         count = (int)Math.Round((y1 - y0) / h);
         double hy = count > 9 ? 2 * h : h;
         while (cur < x1)
         {

            int digits = log <= 0 ? -(int)log + 1 : 0;
            double rounded = Math.Round(cur, digits);
            FormattedText text = new(rounded.ToString($"f{digits}"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 20, new SolidColorBrush(Colors.Black));
            PointWrapper p = ToLocal(new(topleft.X, cur));
            p.Y -= 20;
            p.X += 5;
            drawingContext.DrawText(text, p);
            cur += hy;
         }
      }
      private void DrawLine(DrawingContext drawingContext, Series series)
      {
         var pen = new Pen(new SolidColorBrush(series.Color), 3);
         switch (series.LineType)
         {
            case LineType.Line:
               for (int i = 0; i < series.Points.Count - 1; i++)
               {
                  drawingContext.DrawLine(pen, ToLocal(series.Points[i]), ToLocal(series.Points[i + 1]));
               }
               break;
            case LineType.Spline:
               /* var pqbs = new PolyQuadraticBezierSegment(series.Points.Select(t => ToLocal(t)), false);
                var geom = new PathGeometry();
                var pf = new PathFigure();
                pf.IsClosed = false;
                pf.Segments.Add(pqbs);
                geom.Figures.Add(pf);
                drawingContext.DrawGeometry(new SolidColorBrush(series.Color), pen, geom);*/
               for (int i = 0; i < series.Points.Count - 1; i++)
               {
                  //drawingContext.DrawLine(pen, series.Points[i], series.Points[i + 1]);
                  //drawingContext.DrawDrawing();яы
                  int n = series.Points.Count;
                  var ttat = new PathGeometry();
                  var ggag = new PathFigure();
                  var asddd = new QuadraticBezierSegment(ToLocal(new((series.Points[i].X + series.Points[(i + 2) % n].X) / 2.0, (series.Points[i].Y + series.Points[(i + 2) % n].Y) / 2.0)), ToLocal(series.Points[i + 1]), true);

                  ggag.Segments.Add(asddd);
                  ggag.StartPoint = ToLocal(series.Points[i]);
                  ttat.Figures.Add(ggag);
                  Path p = new Path();
                  p.Data = ttat;
                  p.Stroke = new SolidColorBrush(series.Color);
                  ggag.IsFilled = false;
                  drawingContext.DrawGeometry(p.Stroke, pen, ttat);



               }
               break;
            default:
               break;
         }
      }
      private static float scaleparam = 1.5f;
      private bool shouldTranslate = false;
      private Point lastPosition;
      protected override void OnMouseDown(MouseButtonEventArgs e)
      {
         shouldTranslate = true;
         lastPosition = e.GetPosition(this);
      }
      protected override void OnMouseUp(MouseButtonEventArgs e)
      {
         shouldTranslate = false;
      }
      protected override void OnMouseLeave(MouseEventArgs e)
      {
         shouldTranslate = false;
      }
      protected override void OnMouseMove(MouseEventArgs e)
      {
         if (shouldTranslate)
         {
            var cur = e.GetPosition(this);
            var delta = cur - lastPosition;
            modelView *= Matrix3x2.CreateTranslation((float)delta.X, (float)delta.Y);
            InvalidateVisual();
            lastPosition = cur;
         }
      }
      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {
         if (e.Delta < 0)
         {
            var asd = e.GetPosition(this);
            asd.X -= modelView.M31;
            asd.Y -= modelView.M32;
            modelView *= Matrix3x2.CreateTranslation((float)asd.X, (float)asd.Y);
            modelView *= Matrix3x2.CreateScale(1.0f / scaleparam);
            modelView.M31 *= scaleparam;
            modelView.M32 *= scaleparam;
            modelView *= Matrix3x2.CreateTranslation(-1.0f / scaleparam * (float)asd.X, -1.0f / scaleparam * (float)asd.Y);

         }
         else
         {
            var asd = e.GetPosition(this);
            asd.X -= modelView.M31;
            asd.Y -= modelView.M32;
            modelView *= Matrix3x2.CreateTranslation((float)asd.X, (float)asd.Y);
            modelView *= Matrix3x2.CreateScale(scaleparam);
            modelView.M31 /= scaleparam;
            modelView.M32 /= scaleparam;
            modelView *= Matrix3x2.CreateTranslation(-scaleparam * (float)asd.X, -scaleparam * (float)asd.Y);
         }
         InvalidateVisual();
      }
      protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
      {
         if (!matrixInitiallized)
         {
            modelView *= Matrix3x2.CreateScale(20, -20);
            modelView *= Matrix3x2.CreateTranslation((float)this.ActualWidth / 2, (float)this.ActualHeight / 2);
            matrixInitiallized = true;
            InvalidateVisual();
         }
         else
         {
            modelView *= Matrix3x2.CreateTranslation((float)(sizeInfo.NewSize.Width - sizeInfo.PreviousSize.Width) / 2, (float)(sizeInfo.NewSize.Height - sizeInfo.PreviousSize.Height) / 2);
            InvalidateVisual();
         }
      }
      private PointWrapper ToLocal(PointWrapper point)
      {
         return modelView.Mult(point);
      }
      private PointWrapper ToGlobal(PointWrapper point)
      {
         return modelView.SolveSlae(point);
      }
   }
   public static class Matrix3x2Extentions
   {
      public static PointWrapper Mult(this Matrix3x2 mat, PointWrapper point)
      {
         return new PointWrapper(mat.M11 * point.X + mat.M21 * point.Y + mat.M31,
                                 mat.M12 * point.X + mat.M22 * point.Y + mat.M32);
      }
      public static PointWrapper SolveSlae(this Matrix3x2 mat, PointWrapper point)
      {
         var det = mat.GetDeterminant();
         double x = ((point.X - mat.M31) * mat.M22 - (point.Y - mat.M32) * mat.M21) / det;
         double y = ((point.Y - mat.M32) * mat.M11 - (point.X - mat.M31) * mat.M12) / det;
         return new PointWrapper(x, y);
      }
   }

}