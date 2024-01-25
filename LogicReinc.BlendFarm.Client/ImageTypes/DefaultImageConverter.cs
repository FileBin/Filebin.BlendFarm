using System;
using System.Drawing;
using System.IO;

namespace LogicReinc.BlendFarm.Client.ImageTypes {
    public class DefaultImageConverter : IImageConverter {
        public Image FromStream(Stream stream) {
#if DEBUG
            try {
#endif
                return new Bitmap(Image.FromStream(stream, false, false));
#if DEBUG
            } catch (ArgumentException e) {
                using (var fileStream = File.Create("./debugfile")) {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }
                throw e;
            }
#endif
        }
    }
}
