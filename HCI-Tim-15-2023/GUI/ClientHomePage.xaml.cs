using HCI_Tim_15_2023.GUI.CRUD;
using HCI_Tim_15_2023.GUI.Pregledi;
using HCI_Tim_15_2023.Model;
using System.Windows;
using System.Windows.Controls;

namespace HCI_Tim_15_2023.GUI;
public partial class ClientHomePage : Page
{
    public User loggedUser;

    public ClientHomePage(User user)
    {
        InitializeComponent();
        this.loggedUser = user;
    }

    private void OpenClientTravelViewPage(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new ClientTravelViewPage(this.loggedUser));
    }

    private void OpenClientBoughtTravelViewPage(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new ClientBoughtTravelViewPage());
    }

    private void OpenClientReservationCreatePage(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new ReservationCreatePage());
    }

    private void LogOut(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new LogInPage());
    }
}
