using HCI_Tim_15_2023.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDB.Bson;

namespace HCI_Tim_15_2023.GUI;
public partial class RestaurantCrudPage : Page
{
    public Restaurant selectedRestaurant { get; set; }

    public bool IsReadOnly { get; set; }
    public RestaurantCrudPage()
    {
        InitializeComponent();
        IsReadOnly = true;
        var restaurants = GetRestaurantsFromDB();
        restaurantsDataGrid.ItemsSource = restaurants;
        selectedRestaurant = new Restaurant("123", 0.0, 0.0, "Sample Address", "Sample Name", 10);
        DataContext = this;
    }
    private void RestaurantsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        selectedRestaurant = (Restaurant)restaurantsDataGrid.SelectedItem;
    
        DataContext = null;
        DataContext = this;
    }

    private List<Restaurant> GetFilteredRestaurantsFromDB(string searchTerm)
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "restaurants";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Restaurant>(collectionName);

        var filterBuilder = Builders<Restaurant>.Filter;
        var filter = filterBuilder.Or(
            filterBuilder.Regex(r => r.name, new BsonRegularExpression(searchTerm, "i")),
            filterBuilder.Regex(r => r.address, new BsonRegularExpression(searchTerm, "i"))
        );

        var filteredRestaurants = collection.Find(filter).ToList();

        return filteredRestaurants;
    }
    private List<Restaurant> GetRestaurantsFromDB()
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "restaurants";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Restaurant>(collectionName);
        var filter = Builders<Restaurant>.Filter.Empty;
        var restaurants = collection.Find(filter).ToList();

        return restaurants;
    }
    private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        if (textBox.Text == "Search")
        {
            textBox.Text = string.Empty;
            textBox.Foreground = Brushes.Black;
            textBox.Opacity = 1.0;
        }
    }
    
    private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = "Search";
            textBox.Foreground = Brushes.Gray;
            textBox.Opacity = 0.5;
        }
    }


    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchTerm = searchTextBox.Text;

        var filteredRestaurants = GetFilteredRestaurantsFromDB(searchTerm);
        if (filteredRestaurants.Count != 0)
        {
            restaurantsDataGrid.ItemsSource = filteredRestaurants;
        }
        
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsReadOnly)
        {
            editButton.Content = "Cancel";
            IsReadOnly = false;
            DataContext = null;
            DataContext = this;
            confirmButton.IsEnabled = true;
        }else
        {
            editButton.Content = "Edit";
            IsReadOnly = true;
            DataContext = null;
            DataContext = this;
            confirmButton.IsEnabled = false;
        }

    }
}
