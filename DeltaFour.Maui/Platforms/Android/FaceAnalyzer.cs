#if ANDROID
using System;
using System.Collections.Generic;
using Android.Gms.Tasks;
using Android.Runtime;
using Android.Util;
using AndroidX.Camera.Core;
using Xamarin.Google.MLKit.Vision.Common;
using Xamarin.Google.MLKit.Vision.Face;
using MLFace = Xamarin.Google.MLKit.Vision.Face.Face;
using JavaList = Java.Util.ArrayList;
using Task = Android.Gms.Tasks.Task;
using Runtime = Android.Runtime;
using Size = Android.Util.Size;
namespace DeltaFour.Maui.Platforms.Android.Services
{
    public sealed class FaceAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
    {
        readonly IFaceDetector detector;
        readonly Action<IList<MLFace>, int, int, int> onFacesDetected;

        public FaceAnalyzer(
            IFaceDetector detector,
            Action<IList<MLFace>, int, int, int> onFacesDetected)
        {
            this.detector = detector ?? throw new ArgumentNullException(nameof(detector));
            this.onFacesDetected = onFacesDetected ?? throw new ArgumentNullException(nameof(onFacesDetected));
        }

        [Register(
            "analyze",
            "(Landroidx/camera/core/ImageProxy;)V",
            "GetAnalyze_Landroidx_camera_core_ImageProxy_Handler")]
        public void Analyze(IImageProxy image)
        {
            try
            {
                var mediaImage = image.Image;
                if (mediaImage == null)
                {
                    image.Close();
                    return;
                }

                int rotation = image.ImageInfo?.RotationDegrees ?? 0;
                var inputImage = InputImage.FromMediaImage(mediaImage, rotation);

                var task = detector.Process(inputImage);

                var callback = new FaceTaskCallback(
                    image,
                    onFacesDetected,
                    image.Width,
                    image.Height,
                    rotation);

                task
                    .AddOnSuccessListener(callback)
                    .AddOnFailureListener(callback)
                    .AddOnCompleteListener(callback);
            }
            catch
            {
                image.Close();
            }
        }

        // NOVO REQUISITO DO ANDROIDX: getDefaultTargetResolution
        [Register(
            "getDefaultTargetResolution",
            "()Landroid/util/Size;",
            "GetGetDefaultTargetResolutionHandler")]
        public Size DefaultTargetResolution
        {
            get => new Size(640, 480);
        }

        sealed class FaceTaskCallback :
            Java.Lang.Object,
            IOnSuccessListener,
            IOnFailureListener,
            IOnCompleteListener
        {
            readonly IImageProxy image;
            readonly Action<IList<MLFace>, int, int, int> callback;
            readonly int width;
            readonly int height;
            readonly int rotation;

            public FaceTaskCallback(
                IImageProxy image,
                Action<IList<MLFace>, int, int, int> callback,
                int width,
                int height,
                int rotation)
            {
                this.image = image;
                this.callback = callback;
                this.width = width;
                this.height = height;
                this.rotation = rotation;
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                try
                {
                    // desambiguação do JavaCast
                    var javaList = Runtime.Extensions.JavaCast<JavaList>(result);

                    var faces = new List<MLFace>(javaList.Size());
                    for (int i = 0; i < javaList.Size(); i++)
                    {
                        var f = Runtime.Extensions.JavaCast<MLFace>(javaList.Get(i));
                        faces.Add(f);
                    }

                    callback?.Invoke(faces, width, height, rotation);
                }
                catch
                {
                    // ignora, só não propaga faces
                }
            }

            public void OnFailure(Java.Lang.Exception e)
            {
                System.Diagnostics.Trace.WriteLine($"Face detection failed: {e.Message}");
            }

            public void OnComplete(Task task)
            {
                image.Close();
            }
        }
    }
}
#endif
