using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synapse.Utilities
{
    public class Functions
    {
        public static string FormatRectangle(RectangleF rect)
        {
            return string.Format("X:{0}, Y:{1}, W:{2}, H:{3}", (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
        public static void DrawBox(Graphics graphics, Color color, RectangleF rectangle, double scale, int alpha = 64, float borderWidth = 2, DashStyle dashStyle = DashStyle.Dot, DashCap dashCap = DashCap.Flat)
        {
            borderWidth *= (float)scale;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, color)))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            if (borderWidth == 0)
                return;

            using (Pen pen = new Pen(color, borderWidth)
            {
                DashStyle = dashStyle,
                DashCap = dashCap
            })
            {
                graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }
        public static void DrawBox(Graphics graphics, RectangleF rectangle, double scale, Color color, float borderWidth = 2, DashStyle dashStyle = DashStyle.Dot, DashCap dashCap = DashCap.Flat)
        {
            borderWidth *= (float)scale;

            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            if (borderWidth == 0)
                return;

            using (Pen pen = new Pen(color, borderWidth)
            {
                DashStyle = dashStyle,
                DashCap = dashCap
            })
            {
                graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

    }
}
