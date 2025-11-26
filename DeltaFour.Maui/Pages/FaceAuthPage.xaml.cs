using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeltaFour.Maui.Services;
using System.Diagnostics;
using System.Globalization;




#if ANDROID
using DeltaFour.Maui.Local;
using MLFace = Xamarin.Google.MLKit.Vision.Face.Face;
#endif

namespace DeltaFour.Maui.Pages
{
    [QueryProperty(nameof(PunchType), "punchType")]
    [QueryProperty(nameof(TimeBrtString), "timeBrt")]
    public partial class FaceRegisterPage : ContentPage
    {
        ISession? session;
        IApiService? apiService;
        public string PunchType { get; set; } = "";
        private string _timeBrtString = "";
        private DateTime? _punchTimeBrt;

        public string TimeBrtString
        {
            get => _timeBrtString;
            set
            {
                _timeBrtString = value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    _punchTimeBrt = null;
                    return;
                }

                if (DateTime.TryParseExact(
                        value,
                        "yyyy-MM-dd'T'HH:mm:ss.fff",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var dt))
                {
                    _punchTimeBrt = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
                }
                else
                {
                    _punchTimeBrt = null;
                }
            }
        }
        private bool TryResolveDependencies()
        {
            if (session != null && apiService != null)
                return true;

            var services =
                Handler?.MauiContext?.Services ??
                Application.Current?.Handler?.MauiContext?.Services;

            if (services == null)
                return false;

            session ??= services.GetRequiredService<ISession>();
            apiService ??= services.GetRequiredService<IApiService>();

            return true;
        }
        enum FaceStatus
        {
            NoFace,
            OutOfEllipse,
            TooFar,
            TooClose,
            Perfect
        }

        public static readonly BindableProperty FaceStatusTextProperty =
            BindableProperty.Create(
                nameof(FaceStatusText),
                typeof(string),
                typeof(FaceRegisterPage),
                "Rosto não detectado!");

        const string NoFacePathData =
            "M819-26 528-318l-16 78H352l40-204-72 28v136h-80v-188l96-41L27-818l57-57L876-83l-57 57ZM160-80q-33 0-56.5-23.5T80-160v-120h80v120h120v80H160Zm640-600v-120H680v-80h120q33 0 56.5 23.5T880-800v120h-80Zm-720 0v-120q0-17 6.5-31.5T103-857l57 58v119H80ZM680-80v-80h119l57 56q-10 11-24.5 17.5T800-80H680ZM273-800l-80-80h87v80h-7Zm607 607-80-80v-7h80v87ZM540-580q-33 0-56.5-23.5T460-660q0-33 23.5-56.5T540-740q33 0 56.5 23.5T620-660q0 33-23.5 56.5T540-580Z";

        const string WarningPathData =
            "M80-680v-120q0-33 23.5-56.5T160-880h120v80H160v120H80ZM280-80H160q-33 0-56.5-23.5T80-160v-120h80v120h120v80Zm400 0v-80h120v-120h80v120q0 33-23.5 56.5T800-80H680Zm120-600v-120H680v-80h120q33 0 56.5 23.5T880-800v120h-80ZM540-580q-33 0-56.5-23.5T460-660q0-33 23.5-56.5T540-740q33 0 56.5 23.5T620-660q0 33-23.5 56.5T540-580Zm-28 340H352l40-204-72 28v136h-80v-188l158-68q35-15 51.5-19.5T480-560q21 0 39 11t29 29l40 64q26 42 70.5 69T760-360v80q-66 0-123.5-27.5T540-380l-28 140Z";

        static readonly Geometry NoFaceGeometry =
            (Geometry)new PathGeometryConverter().ConvertFromInvariantString(NoFacePathData);

        static readonly Geometry WarningGeometry =
            (Geometry)new PathGeometryConverter().ConvertFromInvariantString(WarningPathData);

        public string FaceStatusText
        {
            get => (string)GetValue(FaceStatusTextProperty);
            set => SetValue(FaceStatusTextProperty, value);
        }

#if ANDROID
        readonly FaceBoxesDrawable _boxesDrawable = new();
#endif

        FaceStatus _currentStatus = FaceStatus.NoFace;
        CancellationTokenSource? _countdownCts;
        bool _finished;

        public FaceRegisterPage()
        {
            InitializeComponent();

            BindingContext = this;
            _currentStatus = FaceStatus.NoFace;
            FaceStatusText = "Rosto não detectado!";

#if ANDROID
            FacesOverlay.Drawable = _boxesDrawable;
            Preview.FacesDetected += OnFacesDetected;
            StatusIcon.Data = NoFaceGeometry;
#endif

            UpdateIconVisibility();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _finished = false;
            _currentStatus = FaceStatus.NoFace;
            FaceStatusText = "Rosto não detectado!";

            CancelCountdown();

#if ANDROID
            FacesOverlay.Drawable = _boxesDrawable;

            Preview.FacesDetected -= OnFacesDetected;
            Preview.FacesDetected += OnFacesDetected;

            StatusIcon.Data = NoFaceGeometry;
            Preview.IsActive = true; 
#endif

            UpdateIconVisibility();
        }

        protected override void OnDisappearing()
        {
#if ANDROID
            Preview.FacesDetected -= OnFacesDetected;
            CancelCountdown();

            _boxesDrawable.UpdateFaces(Array.Empty<MLFace>(), 0, 0, 0, true);
            FacesOverlay.Invalidate();
            Preview.IsActive = false;
#endif

            base.OnDisappearing();
        }

        void UpdateIconVisibility()
        {
            // Ícone só some enquanto a contagem está ativa
            StatusIcon.IsVisible = _countdownCts == null;
        }
        async Task RegisterPunchAsync(CancellationToken token)
        {
            if (!TryResolveDependencies())
            {
                FaceStatusText = "Erro interno. Tente novamente.";
                return;
            }

#if ANDROID

            FaceStatusText = "Capturando imagem...";
            string imageBase64 = await Preview.CaptureBase64Async(token);
            if (string.IsNullOrEmpty(imageBase64))
            {
                FaceStatusText = "Erro em capturar imagem. Tente novamente.";
                Preview.IsActive = true;
                return;
            }

            FaceStatusText = "Registrando ponto...";
            _finished = true;
          
            double latitude = 0;
            double longitude = 0;
            try
            {
                var location = await Geolocation.Default.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.Medium),
                    token);

                if (location != null)
                {
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                }
                Trace.WriteLine($"Location: {latitude}, {longitude}");
            }
            catch
            {
            }
            DateTime punchTime;
            if (_punchTimeBrt.HasValue)
            {
                punchTime = _punchTimeBrt.Value;
                Trace.WriteLine($"Time BRT (via navigation): {punchTime}");
            }
            else
            {
                punchTime = DateTime.UtcNow;
                Trace.WriteLine($"PunchTime (UTC fallback): {punchTime:o}");
            }
            var shiftType = session?.CurrentUser.ShiftType ?? "DEFAULT";
            Trace.WriteLine($"Testes {shiftType}");
            Trace.WriteLine($"{apiService} API");
            FaceStatusText = "Isso pode demorar um pouco...";
            var ok = await apiService!.PunchInAsync(
                punchTime,
                imageBase64,
                shiftType,
                PunchType,
                latitude,
                longitude,
                token);

            if (ok)
            {
                FaceStatusText = "Ponto registrado com sucesso!";
                Preview.IsActive = false;
                _finished = true;
                await Shell.Current.GoToAsync("//MainTabs/EmployeResumePage");
            }
            else
            {
                FaceStatusText = "Falha ao registrar. Tente novamente.";
                Preview.IsActive = true;
            }
#endif
        }
        void CancelCountdown()
        {
            if (_countdownCts is null)
                return;

            _countdownCts.Cancel();
            _countdownCts.Dispose();
            _countdownCts = null;

            UpdateIconVisibility();
        }

        async Task StartCountdownAsync()
        {
            if (_countdownCts is not null || _finished)
                return;

            _countdownCts = new CancellationTokenSource();
            UpdateIconVisibility();

            var token = _countdownCts.Token;
            const int seconds = 3;
            int remaining = seconds;

            try
            {
                while (remaining > 0)
                {
                    if (_currentStatus != FaceStatus.Perfect || _finished)
                        throw new TaskCanceledException();

                    FaceStatusText = $"Perfeito. Registrando ponto em {remaining}s...";
                    await Task.Delay(1000, token);
                    remaining--;
                }

                await RegisterPunchAsync(token);

            }
            catch (TaskCanceledException)
            {
                // saiu da posição ideal / cancelou
            }
            finally
            {
                CancelCountdown();
            }
        }

#if ANDROID
        void SetStatus(FaceStatus status, string text)
        {
            _currentStatus = status;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // usa binding
                FaceStatusText = text;

                switch (status)
                {
                    case FaceStatus.NoFace:
                        StatusIcon.Data = NoFaceGeometry;
                        break;

                    case FaceStatus.TooFar:
                    case FaceStatus.TooClose:
                    case FaceStatus.OutOfEllipse:
                    case FaceStatus.Perfect:
                        StatusIcon.Data = WarningGeometry;
                        break;
                }

                // visibilidade controlada pela contagem
                UpdateIconVisibility();
            });
        }

        bool IsFaceInsideEllipse(MLFace face, int imgW, int imgH)
        {
            double viewW = Preview.Width;
            double viewH = Preview.Height;

            if (viewW <= 0 || viewH <= 0)
                return true;

            var box = face.BoundingBox;
            if (box == null)
                return false;

            float left = box.Left;
            float top = box.Top;
            float right = box.Right;
            float bottom = box.Bottom;

            const bool isFront = true;
            if (isFront)
            {
                float mlWidth = imgH;
                float mirroredLeft = mlWidth - right;
                float mirroredRight = mlWidth - left;
                left = mirroredLeft;
                right = mirroredRight;
            }

            float scaleX = (float)(viewW / imgH);
            float scaleY = (float)(viewH / imgW);

            double vx = left * scaleX;
            double vy = top * scaleY;
            double vw = (right - left) * scaleX;
            double vh = (bottom - top) * scaleY;

            double cx = vx + vw / 2.0;
            double cy = vy + vh / 2.0;

            double centerX = viewW / 2.0;
            double centerY = viewH / 2.0;

            double radius = Math.Min(viewW, viewH) * 0.55;

            double dx = cx - centerX;
            double dy = cy - centerY;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            double faceRadius = Math.Min(vw, vh) / 2.0;

            return dist + faceRadius * 0.5 <= radius;
        }

        void OnFacesDetected(IList<MLFace> faces, int width, int height, int rotation)
        {
            if (_finished)
                return;

            // desenho das caixas (opcional)
            _boxesDrawable.UpdateFaces(faces, width, height, rotation, isFrontCamera: true);
            FacesOverlay.Invalidate();

            if (faces is null || faces.Count == 0 || width <= 0 || height <= 0)
            {
                if (_currentStatus != FaceStatus.NoFace)
                {
                    CancelCountdown();
                    SetStatus(FaceStatus.NoFace, "Rosto não detectado!");
                }
                return;
            }

            MLFace? bestFace = null;
            float bestArea = 0;

            foreach (var f in faces)
            {
                var box = f.BoundingBox;
                if (box is null) continue;

                float w = box.Width();
                float h = box.Height();
                float area = w * h;
                if (area > bestArea)
                {
                    bestArea = area;
                    bestFace = f;
                }
            }

            if (bestFace is null)
            {
                if (_currentStatus != FaceStatus.NoFace)
                {
                    CancelCountdown();
                    SetStatus(FaceStatus.NoFace, "Rosto não detectado!");
                }
                return;
            }

            if (!IsFaceInsideEllipse(bestFace, width, height))
            {
                if (_currentStatus != FaceStatus.OutOfEllipse)
                {
                    CancelCountdown();
                    SetStatus(FaceStatus.OutOfEllipse, "Mantenha o rosto dentro da área.");
                }
                return;
            }

            var bb = bestFace.BoundingBox;
            var faceHeightRatio = bb.Height() / (double)height;

            const double MinRatio = 0.25;
            const double MaxRatio = 0.55;

            if (faceHeightRatio < MinRatio)
            {
                if (_currentStatus != FaceStatus.TooFar)
                {
                    CancelCountdown();
                    SetStatus(FaceStatus.TooFar, "Aproxime o rosto da área.");
                }
            }
            else if (faceHeightRatio > MaxRatio)
            {
                if (_currentStatus != FaceStatus.TooClose)
                {
                    CancelCountdown();
                    SetStatus(FaceStatus.TooClose, "Afaste um pouco o rosto.");
                }
            }
            else
            {
                if (_currentStatus != FaceStatus.Perfect)
                {
                    SetStatus(FaceStatus.Perfect, "Perfeito. Mantenha o rosto na área...");
                    _ = StartCountdownAsync();
                }
            }
        }
#endif
    }
}
