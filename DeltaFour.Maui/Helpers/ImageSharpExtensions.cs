using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Helpers
{
    public static class ImageSharpExtensions
    {
        public static float[][,] ToBgrPlanes(this SixLabors.ImageSharp.Image<Rgb24> image)
        {
            int w = image.Width;
            int h = image.Height;
            var planes = new float[3][,];
            planes[0] = new float[h, w];
            planes[1] = new float[h, w];
            planes[2] = new float[h, w];

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < h; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < w; x++)
                    {
                        var pixel = row[x];
                        planes[0][y, x] = pixel.B;
                        planes[1][y, x] = pixel.G;
                        planes[2][y, x] = pixel.R;
                    }
                }
            });

            return planes;
        }
    }
}
