using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse.Utilities
{
    public class Functions
    {
        public static T[,] Make2DArray<T>(T[] input, int height, int width)
        {
            T[,] output = new T[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    output[i, j] = input[i * width + j];
                }
            }
            return output;
        }

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

        public static RectangleF[] ResizeRegions(RectangleF[] rectangleFs, SizeF curSize, SizeF newSize)
        {
            RectangleF[] results = new RectangleF[rectangleFs.Length];

            float xScaleRatio = newSize.Width / curSize.Width;
            float yScaleRatio = newSize.Height / curSize.Height;

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new RectangleF(new PointF(rectangleFs[i].X * xScaleRatio, rectangleFs[i].Y * yScaleRatio), new SizeF(rectangleFs[i].Size.Width * xScaleRatio, rectangleFs[i].Size.Height * yScaleRatio));
            }
            return results;
        }
        public static RectangleF ResizeRegion(RectangleF rectangleF, SizeF curSize, SizeF newSize)
        {
            RectangleF result = RectangleF.Empty;

            float xScaleRatio = newSize.Width / curSize.Width;
            float yScaleRatio = newSize.Height / curSize.Height;

            result = new RectangleF(new PointF(rectangleF.X * xScaleRatio, rectangleF.Y * yScaleRatio), new SizeF(rectangleF.Size.Width * xScaleRatio, rectangleF.Size.Height * yScaleRatio));
            return result;
        }

        public static PointF[] ResizePoints(PointF[] rectangleFs, SizeF curSize, SizeF newSize)
        {
            PointF[] results = new PointF[rectangleFs.Length];

            float xScaleRatio = newSize.Width / curSize.Width;
            float yScaleRatio = newSize.Height / curSize.Height;

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new PointF(rectangleFs[i].X * xScaleRatio, rectangleFs[i].Y * yScaleRatio);
            }
            return results;
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

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            //Take use of the IDictionary implementation
            var expandoDict = (IDictionary<string, object>)expando;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
        public static object GetProperty(ExpandoObject expando, string propertyName)
        {
            //Take use of the IDictionary implementation
            var expandoDict = (IDictionary<string, object>)expando;
            if (expandoDict.ContainsKey(propertyName))
                return expandoDict[propertyName];
            else
                return null;
        }
        public static object GetPropertyByIndex(ExpandoObject expando, int propertyIndex)
        {
            //Take use of the IDictionary implementation
            var expandoDict = (IDictionary<string, object>)expando;
            var propertyNames = expandoDict.Keys.ToArray();
            if (propertyNames.Length <= propertyIndex)
                return null;

            string propertyName = propertyNames[propertyIndex];
            if (expandoDict.ContainsKey(propertyName))
                return expandoDict[propertyName];
            else
                return null;
        }
    }
}
