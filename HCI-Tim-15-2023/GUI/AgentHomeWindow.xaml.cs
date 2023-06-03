using System.Windows;

namespace HCI_Tim_15_2023.GUI;

public partial class AgentHomeWindow : Window
{
    public AgentHomeWindow()
    {
        InitializeComponent();
    }

    private void OpenRestaurantCRUD(object sender, RoutedEventArgs e)
    {
        RestaurantCrudWindow restaurantCrudWindow = new RestaurantCrudWindow();
        restaurantCrudWindow.Show();
        Close();
    }

    private void LogOut(object sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }
}