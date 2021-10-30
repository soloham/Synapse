namespace Synapse.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    public class Functions
    {
        public static T[,] Make2DArray<T>(T[] input, int height, int width)
        {
            var output = new T[height, width];
            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                output[i, j] = input[i * width + j];
            return output;
        }

        public static string FormatRectangle(RectangleF rect)
        {
            return string.Format("X:{0}, Y:{1}, W:{2}, H:{3}", (int)rect.X, (int)rect.Y, (int)rect.Width,
                (int)rect.Height);
        }

        public static void DrawBox(Graphics graphics, Color color, RectangleF rectangle, double scale, int alpha = 64,
            float borderWidth = 2, DashStyle dashStyle = DashStyle.Dot, DashCap dashCap = DashCap.Flat)
        {
            borderWidth *= (float)scale;

            using (var brush = new SolidBrush(Color.FromArgb(alpha, color)))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            if (borderWidth == 0)
            {
                return;
            }

            using (var pen = new Pen(color, borderWidth)
            {
                DashStyle = dashStyle,
                DashCap = dashCap
            })
            {
                graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
        }

        public static void DrawBox(Graphics graphics, RectangleF rectangle, double scale, Color color,
            float borderWidth = 2, DashStyle dashStyle = DashStyle.Dot, DashCap dashCap = DashCap.Flat)
        {
            borderWidth *= (float)scale;

            using (var brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, rectangle);
            }

            if (borderWidth == 0)
            {
                return;
            }

            using (var pen = new Pen(color, borderWidth)
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
            var results = new RectangleF[rectangleFs.Length];

            var xScaleRatio = newSize.Width / curSize.Width;
            var yScaleRatio = newSize.Height / curSize.Height;

            for (var i = 0; i < results.Length; i++)
                results[i] = new RectangleF(new PointF(rectangleFs[i].X * xScaleRatio, rectangleFs[i].Y * yScaleRatio),
                    new SizeF(rectangleFs[i].Size.Width * xScaleRatio, rectangleFs[i].Size.Height * yScaleRatio));
            return results;
        }

        public static RectangleF ResizeRegion(RectangleF rectangleF, SizeF curSize, SizeF newSize)
        {
            var result = RectangleF.Empty;

            var xScaleRatio = newSize.Width / curSize.Width;
            var yScaleRatio = newSize.Height / curSize.Height;

            result = new RectangleF(new PointF(rectangleF.X * xScaleRatio, rectangleF.Y * yScaleRatio),
                new SizeF(rectangleF.Size.Width * xScaleRatio, rectangleF.Size.Height * yScaleRatio));
            return result;
        }

        public static PointF[] ResizePoints(PointF[] rectangleFs, SizeF curSize, SizeF newSize)
        {
            var results = new PointF[rectangleFs.Length];

            var xScaleRatio = newSize.Width / curSize.Width;
            var yScaleRatio = newSize.Height / curSize.Height;

            for (var i = 0; i < results.Length; i++)
                results[i] = new PointF(rectangleFs[i].X * xScaleRatio, rectangleFs[i].Y * yScaleRatio);
            return results;
        }

        public static void CopyControl(Control sourceControl, Control targetControl)
        {
            // make sure these are the same
            if (sourceControl.GetType() != targetControl.GetType())
            {
                throw new Exception("Incorrect control types");
            }

            foreach (var sourceProperty in sourceControl.GetType().GetProperties())
            {
                var newValue = sourceProperty.GetValue(sourceControl, null);

                var mi = sourceProperty.GetSetMethod(true);
                if (mi != null)
                {
                    sourceProperty.SetValue(targetControl, newValue, null);
                }
            }
        }

        public static object CloneObject(object o)
        {
            var t = o.GetType();
            var properties = t.GetProperties();

            var p = t.InvokeMember("", BindingFlags.CreateInstance, null, o, null);

            foreach (var pi in properties)
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }

            return p;
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            //Take use of the IDictionary implementation
            var expandoDict = (IDictionary<string, object>)expando;
            if (expandoDict.ContainsKey(propertyName))
            {
                expandoDict[propertyName] = propertyValue;
            }
            else
            {
                expandoDict.Add(propertyName, propertyValue);
            }
        }

        public static object GetProperty(ExpandoObject expando, string propertyName)
        {
            //Take use of the IDictionary implementation
            var expandoDict = (IDictionary<string, object>)expando;
            if (expandoDict.ContainsKey(propertyName))
            {
                return expandoDict[propertyName];
            }

            return null;
        }

        public static object GetPropertyByIndex(ExpandoObject expando, int propertyIndex)
        {
            //Take use of the IDictionary implementation
            var expandoDict = (IDictionary<string, object>)expando;
            var propertyNames = expandoDict.Keys.ToArray();
            if (propertyNames.Length <= propertyIndex)
            {
                return null;
            }

            var propertyName = propertyNames[propertyIndex];
            if (expandoDict.ContainsKey(propertyName))
            {
                return expandoDict[propertyName];
            }

            return null;
        }
    }
}