using Microsoft.Maui.Graphics;

public sealed class ShiftRingDrawable : IDrawable
{
    // Ângulo do ponteiro em graus no dial (0° = 12h, 90° = 3h, etc.)
    public float HandAngleDeg { get; set; }

    // Gap em graus entre a barra e os marcadores
    public float MarkerGapDeg { get; set; } = 7f;

    // Cores base dos marcadores (antes de qualquer estado)
    public Color BaseStartMarkerColor { get; } = Color.FromArgb("#4D9C24"); // verde (entrada)
    public Color BaseEndMarkerColor { get; } = Color.FromArgb("#962020"); // vermelho (saída)

    // Marcador de início
    public float StartMarkerAngleDeg { get; set; }
    public float StartMarkerSweepDeg { get; set; } = 5f;
    public Color StartMarkerColor { get; set; }

    // Marcador de fim
    public float EndMarkerAngleDeg { get; set; }
    public float EndMarkerSweepDeg { get; set; } = 5f;
    public Color EndMarkerColor { get; set; }

    // Barra de progresso (entre entrada e horário atual ou saída)
    public bool ShowProgress { get; set; }
    public Color ProgressColor { get; set; } = Color.FromArgb("#1e2d69");
    public float ProgressStartAngleDeg { get; set; }
    public float ProgressEndAngleDeg { get; set; }

    // Estado de expediente encerrado (ponteiro + barra + marcadores cinza)
    public bool ShiftCompleted { get; set; }
    public Color CompletedColor { get; set; } = Color.FromArgb("#c8c8c8");
    public bool ShowOvertime { get; set; } = false;
    public float OvertimeStartAngleDeg { get; set; }
    public float OvertimeEndAngleDeg { get; set; }
    public Color OvertimeColor { get; set; }

    // Aparência geral
    public float Stroke { get; set; } = 8f;
    public float PointerSize { get; set; } = 10f;
    public Color TrackColor { get; set; } = Color.FromArgb("#F5F8FB");
    public Color HandColor { get; set; } = Color.FromArgb("#1e2d69");

    public ShiftRingDrawable()
    {
        StartMarkerColor = BaseStartMarkerColor;
        EndMarkerColor = BaseEndMarkerColor;
    }

    public void Draw(ICanvas canvas, RectF rect)
    {
        float radius = MathF.Min(rect.Width, rect.Height) / 2f - Stroke / 2f;
        float cx = rect.Width / 2f;
        float cy = rect.Height / 2f;

        canvas.Antialias = true;

        // Trilha base do relógio
        canvas.StrokeSize = Stroke;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeColor = TrackColor;
        canvas.DrawCircle(cx, cy, radius);

        float x1 = cx - radius;
        float y1 = cy - radius;
        float x2 = cx + radius;
        float y2 = cy + radius;

        // Cores dos marcadores: se expediente encerrado, tudo cinza
        Color startColor = ShiftCompleted ? CompletedColor : StartMarkerColor;
        Color endColor = ShiftCompleted ? CompletedColor : EndMarkerColor;

        // Marcadores de início e fim
        DrawMarker(canvas, x1, y1, x2, y2,
            StartMarkerAngleDeg, StartMarkerSweepDeg, startColor);

        DrawMarker(canvas, x1, y1, x2, y2,
            EndMarkerAngleDeg, EndMarkerSweepDeg, endColor);

        // Barra de progresso
        if (ShowProgress)
        {
            DrawDialArc(canvas, x1, y1, x2, y2,
                ProgressStartAngleDeg,
                ProgressEndAngleDeg,
                ProgressColor);
        }

        // Barra EXTRA (tardia) com alpha reduzido
        if (ShowOvertime)
        {
            DrawDialArc(canvas, x1, y1, x2, y2,
                OvertimeStartAngleDeg,
                OvertimeEndAngleDeg,
                OvertimeColor);
        }

        // Ponteiro (cinza se expediente encerrado)
        var pointerColor = ShiftCompleted ? CompletedColor : HandColor;

        canvas.SaveState();
        canvas.Translate(cx, cy);
        canvas.Rotate(HandAngleDeg - 90f); // 0° dial = 12h → -90° no canvas
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

    private void DrawMarker(
        ICanvas canvas,
        float x1, float y1, float x2, float y2,
        float angleDeg,
        float sweepDeg,
        Color color)
    {
        float centerDial = NormalizeAngle(angleDeg);
        float centerCanvas = DialToCanvas(centerDial);

        float startCanvas = centerCanvas - sweepDeg / 2f;
        float endCanvas = centerCanvas + sweepDeg / 2f;

        canvas.StrokeColor = color;
        canvas.StrokeSize = Stroke;
        canvas.StrokeLineCap = LineCap.Butt;

        var path = new PathF();
        path.AddArc(x1, y1, x2, y2, startCanvas, endCanvas, false);
        canvas.DrawPath(path);
    }

    // Converte ângulo de "relógio" (0° = 12h, sentido horário)
    // para ângulo de canvas (0° = direita, CCW).
    private float DialToCanvas(float dialDeg)
    {
        float norm = NormalizeAngle(dialDeg);
        return 90f - norm;
    }

    /// <summary>
    /// Desenha um arco de progresso entre dois ângulos de DIAL,
    /// seguindo o sentido horário lógico do relógio, sem usar clockwise = true.
    /// </summary>
    private void DrawDialArc(
        ICanvas canvas,
        float x1, float y1, float x2, float y2,
        float startDialDeg,
        float endDialDeg,
        Color color)
    {
        float startDial = NormalizeAngle(startDialDeg);
        float endDial = NormalizeAngle(endDialDeg);

        // Span em "sentido horário" do dial
        float spanDial = (endDial - startDial + 360f) % 360f;
        if (spanDial < 0.5f)
            return;

        float cx = (x1 + x2) / 2f;
        float cy = (y1 + y2) / 2f;
        float radius = (x2 - x1) / 2f;

        // Comprimento aproximado do arco em pixels
        float arcLength = radius * spanDial * (float)(Math.PI / 180.0);

        // Segmentos pequenos para ficar liso
        const float pixelsPerSegment = 0.5f;
        int steps = (int)(arcLength / pixelsPerSegment);

        if (steps < 64) steps = 64;
        if (steps > 1440) steps = 1440;

        var path = new PathF();

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float dial = NormalizeAngle(startDial + spanDial * t);

            GetPointOnCircle(cx, cy, radius, dial, out float px, out float py);

            if (i == 0)
                path.MoveTo(px, py);
            else
                path.LineTo(px, py);
        }

        canvas.StrokeColor = color;
        canvas.StrokeSize = Stroke;
        canvas.StrokeLineCap = LineCap.Butt;
        canvas.DrawPath(path);
    }

    /// <summary>
    /// Calcula um ponto na circunferência para um ângulo de "relógio" (0° = 12h).
    /// </summary>
    private static void GetPointOnCircle(
        float cx,
        float cy,
        float radius,
        float dialDeg,
        out float x,
        out float y)
    {
        // dialDeg: 0° = topo, cresce horário → converte para ângulo matemático
        float rad = (float)((dialDeg - 90f) * Math.PI / 180.0);

        x = cx + radius * MathF.Cos(rad);
        y = cy + radius * MathF.Sin(rad);
    }

    private static float NormalizeAngle(float angle)
    {
        var a = angle % 360f;
        if (a < 0f) a += 360f;
        return a;
    }
}
