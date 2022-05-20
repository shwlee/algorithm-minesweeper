using MineSweeper.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
