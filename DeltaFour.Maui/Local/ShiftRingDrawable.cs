using Microsoft.Maui.Graphics;
using System;
using System.Diagnostics;
public sealed class ShiftRingDrawable : IDrawable
{
    public float HandAngleDeg { get; set; } = 0f;

    public float Stroke { get; set; } = 8f;
    public float PointerSize { get; set; } = 10f;
    public Color TrackColor { get; set; } = Color.FromArgb("#F5F8FB");
    public Color HandColor { get; set; } = Color.FromArgb("#1e2d69");
    public bool Dimmed { get; set; } = false;

    // NOVO: janela de início (em graus) e cor
    public float StartMarkerAngleDeg { get; set; } = 0f;   // ângulo do início do expediente (0° no topo, sentido horário)
    public float StartMarkerSweepDeg { get; set; } = 5f;   // 10 minutos no aro de 12h = 5°
    public Color StartMarkerColor { get; set; } = Colors.Green;

    static float Norm360(float a)
    {
        a %= 360f;
        if (a < 0) a += 360f;
        return a;
    }

    public void Draw(ICanvas canvas, RectF rect)
    {
        float radius = MathF.Min(rect.Width, rect.Height) / 2f - Stroke / 2f;
        float cx = rect.Width / 2f;
        float cy = rect.Height / 2f;

        canvas.Antialias = true;

        // trilho
        canvas.StrokeSize = Stroke;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeColor = TrackColor;
        canvas.DrawCircle(cx, cy, radius);
        // —— JANELA de início do expediente (START -> START + 10min) ——
        // Converte para o sistema (0° no topo) e desenha o arco da janela.
        float winStart = Norm360((StartMarkerAngleDeg - 90f));
        float winEnd = Norm360(winStart + StartMarkerSweepDeg);
        Trace.WriteLine($"{winStart} --- {winEnd}");

        canvas.StrokeColor = StartMarkerColor;
#if ANDROID || IOS || MACCATALYST || WINDOWS
        canvas.DrawArc(cx - radius, cy - radius, 2 * radius, 2 * radius, winStart, winEnd, true, false);
#endif

        // cor ativa/congelada do ponteiro
        var color = Dimmed ? Colors.Gray : HandColor;

        // “>” na borda (triângulo = ponteiro)
        canvas.SaveState();
        canvas.Translate(cx, cy);
        canvas.Rotate(HandAngleDeg - 90f);
        canvas.FillColor = color;

        float r = radius;
        float backX = r - PointerSize;
        float half = PointerSize * 0.35f;

        var path = new PathF();
        path.MoveTo(r, 0);             // ponta
        path.LineTo(backX, -half);     // base sup.
        path.LineTo(backX, half);      // base inf.
        path.Close();
        canvas.FillPath(path);

        canvas.RestoreState();
    }
}
