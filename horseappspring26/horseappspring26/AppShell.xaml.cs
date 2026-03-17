namespace horseappspring26
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("horsedetail", typeof(Views.HorseDetailPage));
            Routing.RegisterRoute("horseedit", typeof(Views.HorseEditPage));
        }
    }
}
