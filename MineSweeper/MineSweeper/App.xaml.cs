using MineSweeper.ViewModels;
using System.Windows;

namespace MineSweeper;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // test
        var appViewModel = new AppViewModel();
        var mainWindow = new MainWindow();
        mainWindow.DataContext = appViewModel;
        mainWindow.Show();

        base.OnStartup(e);
    }
}
