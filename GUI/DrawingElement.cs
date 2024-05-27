using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GUI
{
    public  class DrawingElement
    {
        public DrawingElement(Pen stroke, Brush fill, PointWrapper botLeft, PointWrapper topRight)
        {
            Stroke = stroke;
            Fill = fill;
            BotLeft = botLeft;
            TopRight = topRight;
        }

        public Pen Stroke { get; set; }
        public Brush Fill { get; set; }
      
        // Координаты глобальные

        public PointWrapper BotLeft { get; set; }
        public PointWrapper TopRight { get; set; }
    }
}
