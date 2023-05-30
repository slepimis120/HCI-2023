using System.Collections.Generic;
using System.Windows;
using HCI_Tim_15_2023.Model;
using MongoDB.Driver;

namespace HCI_Tim_15_2023.GUI;

public partial class RestaurantCrudWindow : Window
{
    public RestaurantCrudWindow()
    {
        InitializeComponent();
        var restaurants = GetRestaurantsFromDB();
        restaurantsDataGrid.ItemsSource = restaurants;
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
}