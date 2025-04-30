using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k
{
    public static class PixelPerfectCollision
    {
        public static byte[] CreateMask(Texture tx)
        {
            var img = tx.CopyToImage();
            int W = (int)tx.Size.X, H = (int)tx.Size.Y;
            var mask = new byte[W * H];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    mask[x + y * W] = img.GetPixel((uint)x, (uint)y).A;
            return mask;
        }

        public static bool Test(Sprite s1, byte[] mask1, Sprite s2, byte[] mask2, byte alphaLimit = 0)
        {
            var r1 = s1.GetGlobalBounds();
            var r2 = s2.GetGlobalBounds();

            if (!r1.Intersects(r2, out FloatRect inter))
                return false;
            for (int yi = 0; yi < inter.Height; yi++)
            {
                for (int xi = 0; xi < inter.Width; xi++)
                {
                    float wx = inter.Left + xi;
                    float wy = inter.Top + yi;

                    var p1 = (Vector2f)s1.InverseTransform.TransformPoint(wx, wy);
                    var p2 = (Vector2f)s2.InverseTransform.TransformPoint(wx, wy);

                    int ix1 = (int)p1.X, iy1 = (int)p1.Y;
                    int ix2 = (int)p2.X, iy2 = (int)p2.Y;

                    if (ix1 >= 0 && iy1 >= 0 && ix2 >= 0 && iy2 >= 0 &&
                        ix1 < s1.TextureRect.Width && iy1 < s1.TextureRect.Height &&
                        ix2 < s2.TextureRect.Width && iy2 < s2.TextureRect.Height &&
                        mask1[ix1 + iy1 * s1.TextureRect.Width] > alphaLimit &&
                        mask2[ix2 + iy2 * s2.TextureRect.Width] > alphaLimit)
                        return true;
                }
            }

            return false;


        }

    }
}
