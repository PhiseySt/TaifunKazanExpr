using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using TaifunKazanExpress.WebDrivers;

namespace TaifunKazanExpress
{

    public partial class MainWindow : Window
    {
        #region Methods
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var uri = e.Uri.ToString();
            try
            {
                Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Process.Start("chrome.exe", uri);
            }

        }

        private void StartProcessClick(object sender, RoutedEventArgs e)
        {
            var chromeWorkerActions = new ChromeWorkerActions();
            chromeWorkerActions.OpenLoginPage();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        #endregion
    }
}
