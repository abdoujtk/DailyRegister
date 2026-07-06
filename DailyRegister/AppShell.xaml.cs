using DailyRegister.Views;

namespace DailyRegister
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("AddEditContact", typeof(AddEditContactPopup));
        }
    }
}
