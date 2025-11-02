namespace DeltaFour.Maui
{
    public partial class App : Application
    {
        readonly IServiceProvider sp;
        public App(IServiceProvider sp)
        {
            InitializeComponent();
            this.sp = sp;
            MainPage = new NavigationPage(sp.GetRequiredService<MainPage>());
        }
        public void EnterShell()
        {
            MainPage = sp.GetRequiredService<AppShell>();
        }
    }
}
