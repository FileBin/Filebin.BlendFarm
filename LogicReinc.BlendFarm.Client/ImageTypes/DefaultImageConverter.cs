using System;
using System.IO;

namespace LogicReinc.BlendFarm.Client.ImageTypes {
    public class DefaultImageConverter : IImageConverter {
        public Image FromStream(Stream stream) {
                stream.Position = 0;
                return Image.FromStream(stream, false, false);
        }
    }
}
