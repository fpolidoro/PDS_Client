using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow _mainWindow;

        //per mantenere un riferimento all'oggetto MainWindow, in modo da poter terminare i server
        //nel caso di chiusura del sistema o log-out
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _mainWindow = new MainWindow();
            _mainWindow.Show();
        }

        //chiude i server quando si esce dall'applicazione
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _mainWindow.OnClose(sender, null);
        }

        //chiude i server quando si fa il log-out
        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            _mainWindow.OnClose(sender, null);
        }

        /*
        [STAThread]
        public static void Main()
        {
        var application = new App();
        application.InitializeComponent();
        application.Run();
        }*/

    }

    
}
