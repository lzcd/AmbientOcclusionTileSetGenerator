using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AmbientOcclusionTileSetGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var shadowPoints = new List<Point>()
                {
                    new Point(0,0),
                    new Point(7,0),
                    new Point(15,0),
                    new Point(15,7),
                    new Point(15,15),
                    new Point(7,15),
                    new Point(0,15),
                    new Point(0,7)
                };

            for (var permutation = 0; permutation < 256; permutation++)
            {
                var permutationShadowPoints = new List<Point>();

                var nameBuilder = new StringBuilder();
                for (var i = 0; i < 8; i++)
                {
                    var digit = (int)Math.Pow(2, i);
                    if ((digit & permutation) > 0)
                    {
                        nameBuilder.Append("1");
                        permutationShadowPoints.Add(shadowPoints[i]);
                    }
                    else
                    {
                        nameBuilder.Append("0");
                    }
                }
                nameBuilder.Append(".png");
                
                var shadowPointSize = 2f;
                var shadowColor = Color.Black;

                using (var bitmap = new Bitmap(16, 16))
                {
                    for (var y = 0; y < bitmap.Height; y++)
                        for (var x = 0; x < bitmap.Width; x++)
                        {
                            var shadowInfluence = 0f;
                            foreach (var shadowPoint in permutationShadowPoints)
                            {
                                var xDelta = x - shadowPoint.X;
                                var yDelta = y - shadowPoint.Y;
                                var distance = Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
                                var pointInfluence = shadowPointSize / (float)distance;
                                if (pointInfluence > 1f) pointInfluence = 1f;
                                if (pointInfluence < 0f) pointInfluence = 0f;
                                shadowInfluence += pointInfluence;
                            }
                            if (shadowInfluence > 1f) shadowInfluence = 1f;
                            var inverse = 1f - shadowInfluence;
                            var red = (byte)((shadowColor.R * shadowInfluence) + (Color.Gray.R * inverse));
                            var green = (byte)((shadowColor.G * shadowInfluence) + (Color.Gray.G * inverse));
                            var blue = (byte)((shadowColor.B * shadowInfluence) + (Color.Gray.B * inverse));
                            var alpha = (byte)((shadowColor.A * shadowInfluence) + (Color.Gray.A * inverse));
                            var color = Color.FromArgb(alpha, red, green, blue);
                            bitmap.SetPixel(x, y, color);
                        }
                    bitmap.Save(nameBuilder.ToString(), System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }
    }
}
