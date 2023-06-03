using System.Windows;
using System.Windows.Controls;

namespace HCI_Tim_15_2023.GUI;
public partial class AgentHomePage : Page
{
    public AgentHomePage()
    {
        InitializeComponent();
    }

    private void OpenRestaurantCRUD(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new RestaurantCrudPage());
    }

    private void LogOut(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new LogInPage());
    }
}
