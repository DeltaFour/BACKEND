using Microsoft.Maui.Graphics;

namespace DeltaFour.Maui.Local;

public class FaceOverlayDrawable : IDrawable
{
    public static readonly FaceOverlayDrawable Instance = new();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float ellipseWidth = 320f;
        float ellipseHeight = 420f;

        float x = (dirtyRect.Width - ellipseWidth) / 2f;
        float y = (dirtyRect.Height - ellipseHeight) / 2f + 30f; 

        var path = new PathF();
        path.AppendRectangle(dirtyRect);                 
        path.AppendEllipse(x, y, ellipseWidth, ellipseHeight); 

        canvas.FillColor = new Color(0, 0, 0, 0.5f);
        canvas.FillPath(path, WindingMode.EvenOdd);     

        canvas.StrokeColor = new Color(255,255,255,0.1f);
        canvas.StrokeSize = 1;
        canvas.DrawEllipse(x, y, ellipseWidth, ellipseHeight);
    }
}
