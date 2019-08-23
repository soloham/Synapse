using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public static void CopyControl(Control sourceControl, Control targetControl)
        {
            // make sure these are the same
            if (sourceControl.GetType() != targetControl.GetType())
            {
                throw new Exception("Incorrect control types");
            }

            foreach (PropertyInfo sourceProperty in sourceControl.GetType().GetProperties())
            {
                object newValue = sourceProperty.GetValue(sourceControl, null);

                MethodInfo mi = sourceProperty.GetSetMethod(true);
                if (mi != null)
                {
                    sourceProperty.SetValue(targetControl, newValue, null);
                }
            }
        }
        public static object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();

            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }

            return p;
        }
    }
}
