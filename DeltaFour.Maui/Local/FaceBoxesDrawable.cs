#if ANDROID
using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using MLFace = Xamarin.Google.MLKit.Vision.Face.Face;

namespace DeltaFour.Maui.Local
{
    public sealed class FaceBoxesDrawable : IDrawable
    {
        readonly object _sync = new();
        IList<MLFace> _faces = Array.Empty<MLFace>();
        int _imageWidth;
        int _imageHeight;
        int _rotation;
        bool _isFront;

        // Controle de frames sem detecção (para evitar "piscar")
        int _missingFrames;
        const int MaxMissingFrames = 3;

        // Suavização da caixa principal
        RectF? _lastRect;

        public static FaceBoxesDrawable Instance { get; } = new();

        public void UpdateFaces(
            IList<MLFace> faces,
            int imageWidth,
            int imageHeight,
            int rotation,
            bool isFrontCamera)
        {
            lock (_sync)
            {
                if (faces == null || faces.Count == 0)
                {
                    _missingFrames++;

                    // Só "zera" depois de alguns frames sem rosto
                    if (_missingFrames > MaxMissingFrames)
                    {
                        _faces = Array.Empty<MLFace>();
                    }

                    return;
                }

                _missingFrames = 0;

                _faces = faces;
                _imageWidth = imageWidth;
                _imageHeight = imageHeight;
                _rotation = rotation;
                _isFront = isFrontCamera;
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            IList<MLFace> faces;
            int iw, ih;
            bool isFront;

            lock (_sync)
            {
                faces = _faces;
                iw = _imageWidth;
                ih = _imageHeight;
                isFront = _isFront;
            }

            if (faces == null || faces.Count == 0 || iw <= 0 || ih <= 0)
            {
                _lastRect = null;
                return;
            }

            canvas.SaveState();

            canvas.StrokeColor = Color.FromArgb("#F5F8FB");
            canvas.StrokeSize = 3;
            canvas.Alpha = 0.9f;

            float viewW = dirtyRect.Width;
            float viewH = dirtyRect.Height;

            // ML Kit (portrait) -> PreviewView (portrait) com rotação de 90°
            float scaleX = viewW / ih;
            float scaleY = viewH / iw;

            // Escala uniforme para não distorcer
            float scale = MathF.Min(scaleX, scaleY);

            float scaledW = ih * scale;
            float scaledH = iw * scale;

            float offsetX = dirtyRect.Left + (viewW - scaledW) / 2f;
            float offsetY = dirtyRect.Top + (viewH - scaledH) / 2f;

            const float SmoothFactor = 0.2f;  
            const float shrink = 1f;           
            const float heightMultiplier = 1.2f; 

            int index = 0;
            foreach (var face in faces)
            {
                var box = face.BoundingBox;
                if (box is null)
                {
                    index++;
                    continue;
                }

                float left = box.Left;
                float top = box.Top;
                float right = box.Right;
                float bottom = box.Bottom;

                if (isFront)
                {
                    float mlWidth = ih;
                    float mirroredLeft = mlWidth - right;
                    float mirroredRight = mlWidth - left;

                    left = mirroredLeft;
                    right = mirroredRight;
                }

                float vx = offsetX + left * scale;
                float vy = offsetY + top * scale;
                float vw = (right - left) * scale;
                float vh = (bottom - top) * scale;

                // Mantém o centro, ajustando shrink + heightMultiplier
                float cx = vx + vw / 2f;
                float cy = vy + vh / 2f;

                vw *= shrink;
                vh *= shrink * heightMultiplier; // altura controlada aqui

                vx = cx - vw / 2f;
                vy = cy - vh / 2f;

                var rect = new RectF(vx, vy, vw, vh);

                // Suavização apenas para a primeira face (principal)
                if (index == 0 && _lastRect is RectF prev)
                {
                    float Lerp(float a, float b) => a + (b - a) * SmoothFactor;

                    rect = new RectF(
                        Lerp(prev.X, rect.X),
                        Lerp(prev.Y, rect.Y),
                        Lerp(prev.Width, rect.Width),
                        Lerp(prev.Height, rect.Height));
                }

                if (index == 0)
                    _lastRect = rect;

                float radius = MathF.Min(rect.Width, rect.Height) * 0.08f;

                canvas.DrawRoundedRectangle(rect, radius);

                index++;
            }

            canvas.RestoreState();
        }
    }
}
#endif
