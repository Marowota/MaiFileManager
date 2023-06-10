using MaiFileManager.Pages;

namespace MaiFileManager;

public partial class AppShell : Shell
{
	public AppShell()
    {
        InitializeComponent(); 
        Routing.RegisterRoute("perfact", typeof(PerformingAction));

    }
}
