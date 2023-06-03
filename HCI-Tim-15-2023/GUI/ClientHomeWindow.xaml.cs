using System.Windows;

namespace HCI_Tim_15_2023.GUI;

public partial class ClientHomeWindow : Window
{
    public ClientHomeWindow()
    {
        InitializeComponent();
    }

    private void LogOut(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }
}
