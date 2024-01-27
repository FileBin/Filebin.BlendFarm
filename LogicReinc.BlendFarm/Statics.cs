using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.BlendFarm {
    public static class Util {
        private static object _previewImageLock = new object();
        private static IBitmap? _noPreviewImage { get; set; }
        public static IBitmap NoPreviewImage {
            get {
                lock (_previewImageLock) {
                    if (_noPreviewImage == null) {
                        _noPreviewImage = new WriteableBitmap(
                            new PixelSize(700, 200),
                            new Vector(96, 96),
                            PixelFormat.Rgba8888,
                            AlphaFormat.Opaque
                            );
                    }
                }
                return _noPreviewImage;
                //TODO: draw normal text instead of image
            }
        }

        public static string SanitizePath(string inputPath) {
            //Fix Linux Space escape
            inputPath = inputPath.Replace("\\040", " ");
            return inputPath;
        }
        
        public static IBitmap BitmapFromBuffer(byte[] buffer) {
            using(var ms = new MemoryStream(buffer)) {
                return new Bitmap(ms);
            }
        }
    }
}
