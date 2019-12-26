using System;
using System.Drawing;

namespace EC.Common.Util
{
    public class ImageUtils
    {
        public static Bitmap MakeSquarePhoto(Bitmap bmp, int size)
        {
            try
            {
                Bitmap res = new Bitmap(size, size);
                Graphics g = Graphics.FromImage(res);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, size, size);
                int t = 0, l = 0;
                if (bmp.Height > bmp.Width)
                    t = (bmp.Height - bmp.Width) / 2;
                else
                    l = (bmp.Width - bmp.Height) / 2;
                g.DrawImage(bmp, new Rectangle(0, 0, size, size), new Rectangle(l, t, bmp.Width - l * 2, bmp.Height - t * 2), GraphicsUnit.Pixel);
                return res;
            }
            catch
            {
            }

            return null;
        }

        public static void MakeSquarePhoto(string file, bool lessSize = true)
        {
            using (var img = Bitmap.FromFile(file))
            {
                using (var bmp = new Bitmap(img))
                {
                    var res = MakeSquarePhoto(bmp, lessSize ? Math.Min(bmp.Height, bmp.Width) : Math.Max(bmp.Height, bmp.Width));
                    if (res != null)
                    {
                        res.Save(file + "_tmp");
                    }
                }
            }
            if (System.IO.File.Exists(file + "_tmp"))
            {
                System.IO.File.Delete(file);
                System.IO.File.Move(file + "_tmp", file);
            }
        }
    }
}