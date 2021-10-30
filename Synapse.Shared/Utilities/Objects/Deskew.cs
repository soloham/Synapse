namespace Synapse.Utilities.Objects
{
    using System;
    using System.Diagnostics;
    using System.Drawing;

    using Emgu.CV;
    using Emgu.CV.Structure;

    public class Deskew
    {
        public enum DeskewType
        {
            Auto,
            Custom
        }

        // Representation of a line in the image.  
        private class HougLine
        {
            // Count of points in the line.
            public int Count;

            // Index in Matrix.
            public int Index;

            // The line is represented as all x,y that solve y*cos(alpha)-x*sin(alpha)=d
            public double Alpha;
        }


        // The Bitmap
        private static Bitmap _internalBmp;

        // The range of angles to search for lines
        private const double ALPHA_START = -20;
        private const double ALPHA_STEP = 0.2;
        private const int STEPS = 40 * 5;
        private const double STEP = 1;

        // Precalculation of sin and cos.
        private static double[] _sinA;
        private static double[] _cosA;

        // Range of d
        private static double _min;


        private static int _count;

        // Count of points that fit in a line.
        private static int[] _hMatrix;

        public static Bitmap DeskewImage(Bitmap image, double binarizeThreshold, out double deskewAngle)
        {
            var img = new Image<Gray, byte>(image);
            var binary = img.ThresholdBinary(new Gray(binarizeThreshold), new Gray(255));
            _internalBmp = img.Bitmap;

            deskewAngle = -GetSkewAngle();
            return img.Rotate(deskewAngle, new Gray(255), false).ToBitmap();
        }

        public static Bitmap DeskewImage(Bitmap image, double deskewAngle)
        {
            var img = new Image<Gray, byte>(image);
            return img.Rotate(deskewAngle, new Gray(255), false).ToBitmap();
        }

        public static Image<Gray, byte> DeskewImage(Image<Gray, byte> image, double binarizeThreshold,
            out double deskewAngle)
        {
            var binarized = image.ThresholdBinary(new Gray(binarizeThreshold), new Gray(255));
            _internalBmp = image.Bitmap;

            deskewAngle = -GetSkewAngle();
            return image.Rotate(deskewAngle, new Gray(255), false);
        }

        public static Image<Gray, byte> DeskewImage(Image<Gray, byte> image, double deskewAngle)
        {
            return image.Rotate(deskewAngle, new Gray(255), false);
        }


        // Calculate the skew angle of the image cBmp.
        private static double GetSkewAngle()
        {
            // Hough Transformation
            Calc();

            // Top 20 of the detected lines in the image.
            var hl = GetTop(20);

            // Average angle of the lines
            double sum = 0;
            var count = 0;
            for (var i = 0; i <= 19; i++)
            {
                sum += hl[i].Alpha;
                count += 1;
            }

            return sum / count;
        }

        // Calculate the Count lines in the image with most points.
        private static HougLine[] GetTop(int count)
        {
            var hl = new HougLine[count];

            for (var i = 0; i <= count - 1; i++) hl[i] = new HougLine();
            for (var i = 0; i <= _hMatrix.Length - 1; i++)
                if (_hMatrix[i] > hl[count - 1].Count)
                {
                    hl[count - 1].Count = _hMatrix[i];
                    hl[count - 1].Index = i;
                    var j = count - 1;
                    while (j > 0 && hl[j].Count > hl[j - 1].Count)
                    {
                        var tmp = hl[j];
                        hl[j] = hl[j - 1];
                        hl[j - 1] = tmp;
                        j -= 1;
                    }
                }

            for (var i = 0; i <= count - 1; i++)
            {
                var dIndex = hl[i].Index / STEPS;
                var alphaIndex = hl[i].Index - dIndex * STEPS;
                hl[i].Alpha = GetAlpha(alphaIndex);
                //hl[i].D = dIndex + _min;
            }

            return hl;
        }


        // Hough Transforamtion:
        private static void Calc()
        {
            var hMin = _internalBmp.Height / 4;
            var hMax = _internalBmp.Height * 3 / 4;

            Init();
            for (var y = hMin; y <= hMax; y++)
            for (var x = 1; x <= _internalBmp.Width - 2; x++)
                // Only lower edges are considered.
                if (IsBlack(x, y))
                {
                    if (!IsBlack(x, y + 1))
                    {
                        Calc(x, y);
                    }
                }
        }

        // Calculate all lines through the point (x,y).
        private static void Calc(int x, int y)
        {
            int alpha;

            for (alpha = 0; alpha <= STEPS - 1; alpha++)
            {
                var d = y * _cosA[alpha] - x * _sinA[alpha];
                var calculatedIndex = (int)CalcDIndex(d);
                var index = calculatedIndex * STEPS + alpha;
                try
                {
                    _hMatrix[index] += 1;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private static double CalcDIndex(double d)
        {
            return Convert.ToInt32(d - _min);
        }

        private static bool IsBlack(int x, int y)
        {
            var c = _internalBmp.GetPixel(x, y);
            var luminance = c.R * 0.299 + c.G * 0.587 + c.B * 0.114;
            return luminance < 140;
        }

        private static void Init()
        {
            // Precalculation of sin and cos.
            _cosA = new double[STEPS];
            _sinA = new double[STEPS];

            for (var i = 0; i < STEPS; i++)
            {
                var angle = GetAlpha(i) * Math.PI / 180.0;
                _sinA[i] = Math.Sin(angle);
                _cosA[i] = Math.Cos(angle);
            }

            // Range of d:            
            _min = -_internalBmp.Width;
            _count = (int)(2 * (_internalBmp.Width + _internalBmp.Height) / STEP);
            _hMatrix = new int[_count * STEPS];
        }

        private static double GetAlpha(int index)
        {
            return ALPHA_START + index * ALPHA_STEP;
        }
    }
}