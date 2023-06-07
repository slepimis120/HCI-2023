using System.Windows;
using System.Windows.Controls;

namespace HCI_Tim_15_2023.GUI;
public partial class LogInPage : Page
{
    public LogInPage()
    {
        InitializeComponent();
    }

    private void LoginAgent(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new AgentHomePage());
    }

    private void LoginClient(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new ClientHomePage());
    }

    private void RegisterClient(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new RegisterPage());
    }
}
