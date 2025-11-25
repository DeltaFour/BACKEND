using Microsoft.Maui.Controls;

#if ANDROID
using System.Collections.Generic;
using DeltaFour.Maui.Local;
using MLFace = Xamarin.Google.MLKit.Vision.Face.Face;
#endif

namespace DeltaFour.Maui.Pages
{
    public partial class FaceRegisterPage : ContentPage
    {
#if ANDROID
        readonly FaceBoxesDrawable _boxesDrawable = new();
#endif

        public FaceRegisterPage()
        {
            InitializeComponent();

#if ANDROID
            FacesOverlay.Drawable = _boxesDrawable;
            Preview.FacesDetected += OnFacesDetected;
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Preview.IsActive = true;
        }

        protected override void OnDisappearing()
        {
#if ANDROID
            Preview.FacesDetected -= OnFacesDetected;
#endif
            Preview.IsActive = false;
            base.OnDisappearing();
        }

#if ANDROID
        void OnFacesDetected(IList<MLFace> faces, int width, int height, int rotation)
        {
            _boxesDrawable.UpdateFaces(faces, width, height, rotation, isFrontCamera: true);
            FacesOverlay.Invalidate();
        }
#endif
    }
}
