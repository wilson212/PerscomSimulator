using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Telerik.WinControls;
using Telerik.WinControls.Svg;

namespace Perscom
{
    /// <summary>
    /// A class used to load and cache images in memory.
    /// </summary>
    public static class ImageAccessor
    {
        private static Dictionary<string, WeakReference> References;

        static ImageAccessor()
        {
            References = new Dictionary<string, WeakReference>();
        }

        /// <summary>
        /// Loads an image from the cache, or the Disk Drive if the image
        /// hasnt been loaded into memory yet.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string filePath)
        {
            Bitmap image = null;
            WeakReference reference = null;

            if (!References.TryGetValue(filePath, out reference))
            {
                image = GetImageFromDisk(filePath);
                reference = new WeakReference(image);
                References[filePath] = reference;
            }
            else
            {
                image = reference.Target as Bitmap;
                if (image == null)
                {
                    image = GetImageFromDisk(filePath);
                    reference.Target = image;
                }
            }

            return image;
        }

        public static RadSvgImage GetSvgImage(string filePath)
        {
            RadSvgImage image = null;
            WeakReference reference = null;
            if (String.IsNullOrEmpty(filePath))
                return null;

            if (!References.TryGetValue(filePath, out reference))
            {
                image = GetSvgImageFromDisk(filePath);
                reference = new WeakReference(image);
                References[filePath] = reference;
            }
            else
            {
                image = reference.Target as RadSvgImage;
                if (image == null)
                {
                    image = GetSvgImageFromDisk(filePath);
                    reference.Target = image;
                }
            }

            return image;
        }

        private static Bitmap GetImageFromDisk(string filePath)
        {
            filePath = Path.Combine(Program.RootPath, "Images", filePath);
            return (File.Exists(filePath)) ? new Bitmap(filePath) : null;
        }

        private static RadSvgImage GetSvgImageFromDisk(string filePath)
        {
            filePath = Path.Combine(Program.RootPath, "Images", filePath);
            return File.Exists(filePath) ? RadSvgImage.FromFile(filePath) : null;
        }
    }
}
