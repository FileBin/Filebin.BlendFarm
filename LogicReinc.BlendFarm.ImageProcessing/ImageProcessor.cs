using Avalonia;
using Avalonia.Media.Imaging;

namespace LogicReinc.BlendFarm.ImageProcessing;

public class ImageProcessor {
    private RenderTargetBitmap bitmap;

    public ImageProcessor(int width, int height) {
        bitmap = new RenderTargetBitmap(new PixelSize(width, height));
    }

    public void DrawImage(byte[] imageData, int x, int y) {
        var ctx = bitmap.CreateDrawingContext(null);

        using (var ms = new MemoryStream(imageData)) {
            Bitmap image = new Bitmap(ms);
            var imageSize = image.PixelSize.ToSize(1.0);
            ctx.DrawBitmap(
                image.PlatformImpl,
                opacity: 1.0,
                sourceRect: new Rect(imageSize),
                destRect: new Rect(new Point(x, y), imageSize));
        }
    }

    public void Save(MemoryStream ms) {
        bitmap.Save(ms);
    }
}
