using Microsoft.Maui.Graphics;

public sealed class ShiftRingDrawable : IDrawable
{
    public float HandAngleDeg { get; set; } = 0f;

    // Janela do INÍCIO
    public float StartMarkerAngleDeg { get; set; } = 0f;
    public float StartMarkerSweepDeg { get; set; } = 0.1f;
    public Color StartMarkerColor { get; set; } = Colors.Green;

    // Janela do FIM
    public float EndMarkerAngleDeg { get; set; } = 0f;
    public float EndMarkerSweepDeg { get; set; } = 0.1f;
    public Color EndMarkerColor { get; set; } = Color.FromArgb("#962020");

    // Barra de progresso
    public bool ShowProgress { get; set; } = false;
    public Color ProgressColor { get; set; } = Color.FromArgb("#1e2d69");

    // Estado completo (após saída)
    public bool ShiftCompleted { get; set; } = false;
    public Color CompletedColor { get; set; } = Colors.Gray;

    public float Stroke { get; set; } = 8f;
    public float PointerSize { get; set; } = 10f;
    public Color TrackColor { get; set; } = Color.FromArgb("#F5F8FB");
    public Color HandColor { get; set; } = Color.FromArgb("#1e2d69");
    public Color OutsideHoursHandColor { get; set; } = Colors.Gray;
    public bool Dimmed { get; set; } = false;

    public void Draw(ICanvas canvas, RectF rect)
    {
        float radius = MathF.Min(rect.Width, rect.Height) / 2f - Stroke / 2f;
        float cx = rect.Width / 2f;
        float cy = rect.Height / 2f;

        canvas.Antialias = true;

        // 1) Trilho
        canvas.StrokeSize = Stroke;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeColor = TrackColor;
        canvas.DrawCircle(cx, cy, radius);

        float x1 = cx - radius, y1 = cy - radius, x2 = cx + radius, y2 = cy + radius;

        // 2) Se expediente completo, desenhar arco completo cinza
        if (ShiftCompleted)
        {
            canvas.StrokeColor = CompletedColor;
            canvas.StrokeSize = Stroke;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.DrawCircle(cx, cy, radius);
        }
        else
        {
            // 3) Marcadores - Marcador de início fica com cor de progresso quando ativo
            Color startColor = ShowProgress ? ProgressColor : StartMarkerColor;
            DrawMarker(canvas, x1, y1, x2, y2, StartMarkerAngleDeg, StartMarkerSweepDeg, startColor);
            DrawMarker(canvas, x1, y1, x2, y2, EndMarkerAngleDeg, EndMarkerSweepDeg, EndMarkerColor);

            // 4) Barra de progresso (se ativa) - CORREÇÃO FINAL
            if (ShowProgress)
            {
                // CORREÇÃO: Calcular o progresso de forma mais simples e direta
                (float startAngle, float endAngle) = CalculateProgressArc();

                // Só desenhar se houver um arco significativo
                if (Math.Abs(startAngle - endAngle) > 1f)
                {
                    canvas.StrokeColor = ProgressColor;
                    canvas.StrokeSize = Stroke;
                    canvas.StrokeLineCap = LineCap.Round;

                    var progressPath = new PathF();
                    progressPath.AddArc(x1, y1, x2, y2, startAngle, endAngle, false);
                    canvas.DrawPath(progressPath);
                }
            }
        }

        // 5) Ponteiro
        var pointerColor = Dimmed ? OutsideHoursHandColor : HandColor;
        if (ShiftCompleted) pointerColor = CompletedColor;

        canvas.SaveState();
        canvas.Translate(cx, cy);
        canvas.Rotate(HandAngleDeg - 90f);
        canvas.FillColor = pointerColor;

        float r = radius;
        float backX = r - PointerSize;
        float half = PointerSize * 0.35f;

        var pointer = new PathF();
        pointer.MoveTo(r, 0);
        pointer.LineTo(backX, -half);
        pointer.LineTo(backX, half);
        pointer.Close();
        canvas.FillPath(pointer);

        canvas.RestoreState();
    }

    private void DrawMarker(ICanvas canvas, float x1, float y1, float x2, float y2,
                          float angleDeg, float sweepDeg, Color color)
    {
        float center = 90f - angleDeg;
        float startDeg = center - sweepDeg / 2f;
        float endDeg = center + sweepDeg / 2f;

        canvas.StrokeColor = color;
        canvas.StrokeSize = Stroke;
        canvas.StrokeLineCap = LineCap.Round;

        var path = new PathF();
        path.AddArc(x1, y1, x2, y2, startDeg, endDeg, false);
        canvas.DrawPath(path);
    }

    // CORREÇÃO FINAL: Nova lógica mais simples e direta
    private (float startAngle, float endAngle) CalculateProgressArc()
    {
        // Converter ângulos para sistema MAUI
        float startCenter = 90f - StartMarkerAngleDeg;
        float handCenter = 90f - HandAngleDeg;

        // Calcular distância do ponteiro até o fim (com margem de segurança)
        float distanceToEnd = (EndMarkerAngleDeg - HandAngleDeg + 360) % 360;
        float safeDistanceToEnd = 5f; // 5 graus de margem

        // Se o ponteiro está muito perto do fim, limitar o progresso
        if (distanceToEnd < safeDistanceToEnd)
        {
            float limitedHandAngle = (EndMarkerAngleDeg - safeDistanceToEnd + 360) % 360;
            handCenter = 90f - limitedHandAngle;
        }

        return (startCenter, handCenter);
    }
}