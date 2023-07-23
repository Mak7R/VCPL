using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VCPLBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow;
            string filename;
            if (e.Args.Length > 0)
            {
                filename = e.Args[0];
                mainWindow = new MainWindow(filename);
            }
            else
            {
                mainWindow = new MainWindow();
            }
            mainWindow.Show();
        }
    }
}