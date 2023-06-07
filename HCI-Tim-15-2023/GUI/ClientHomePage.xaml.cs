using HCI_Tim_15_2023.GUI.Pregledi;
using System.Windows;
using System.Windows.Controls;

namespace HCI_Tim_15_2023.GUI;
public partial class ClientHomePage : Page
{
    public ClientHomePage()
    {
        InitializeComponent();
    }

    private void OpenClientTravelViewPage(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new ClientTravelViewPage());
    }

    private void OpenClientBoughtTravelViewPage(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new ClientBoughtTravelViewPage());
    }

    private void LogOut(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new LogInPage());
    }
}
