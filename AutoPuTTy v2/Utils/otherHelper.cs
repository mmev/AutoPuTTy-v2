using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace AutoPuTTY.Utils
{
    class OtherHelper
    {
        /// <summary>
        /// ???
        /// </summary>
        /// <param name="s">???</param>
        /// <param name="r">???</param>
        /// <param name="str">???</param>
        /// <returns>???</returns>
        public static string ReplaceA(string[] s, string[] r, string str)
        {
            int i = 0;
            if (s.Length > 0 && r.Length > 0 && s.Length == r.Length)
            {
                while (i < s.Length)
                {
                    str = str.Replace(s[i], r[i]);
                    i++;
                }
            }
            return str;
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="s">???</param>
        /// <param name="str">???</param>
        /// <returns>???</returns>
        public static string ReplaceU(string[] s, string str)
        {
            int i = 0;
            if (s.Length > 0)
            {
                while (i < s.Length)
                {
                    str = str.Replace(s[i], Uri.EscapeDataString(s[i]).ToUpper());
                    i++;
                }
            }
            str = str.Replace("*", "%2A");
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Change Image eye opacity when click
        /// </summary>
        /// <param name="image">image</param>
        /// <param name="opacity">opacity param</param>
        /// <returns>new image with custom opacity</returns>
        public Image Set(Image image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            ColorMatrix cmx = new ColorMatrix();
            cmx.Matrix33 = opacity;

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cmx, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
            ia.Dispose();
            gfx.Dispose();

            return bmp;
        }
    }
}
