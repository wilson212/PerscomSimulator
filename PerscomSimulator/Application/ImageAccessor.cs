using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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
                image = GetFromDisk(filePath);
                reference = new WeakReference(image);
                References[filePath] = reference;
            }
            else
            {
                image = reference.Target as Bitmap;
                if (image == null)
                {
                    image = GetFromDisk(filePath);
                    reference.Target = image;
                }
            }

            return image;
        }

        private static Bitmap GetFromDisk(string filePath)
        {
            return new Bitmap(Path.Combine(Program.RootPath, "Images", filePath));
        }
    }
}
