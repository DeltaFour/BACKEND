using DeltaFour.Maui.Pages;

namespace DeltaFour.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("FaceRegisterPage", typeof(FaceRegisterPage));
            Routing.RegisterRoute("EmployeResumePage", typeof(EmployeResume));
        }
    }
}
