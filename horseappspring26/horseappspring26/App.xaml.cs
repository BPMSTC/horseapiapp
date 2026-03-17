namespace horseappspring26
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;

        public App(AppShell appShell)
        {
            InitializeComponent();
            _appShell = appShell;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(_appShell);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await _appShell.GoToAsync("//horses");
            });

            return window;
        }
    }
}