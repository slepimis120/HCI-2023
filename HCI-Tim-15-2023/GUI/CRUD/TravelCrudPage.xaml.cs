using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HCI_Tim_15_2023.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace HCI_Tim_15_2023.GUI.CRUD;

public partial class TravelCrudPage : Page
{
    public Travel SelectedTravel { get; set; }
    public bool IsReadOnly { get; set; }

    public TravelCrudPage()
    {
        InitializeComponent();
    }
        private void TravelDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        IsReadOnly = true;
        SelectedTravel = (Travel) travelDataGrid.SelectedItem;
        DataContext = null;
        DataContext = this;
    }

    private List<Travel> GetFilteredTravelsFromDB(string searchTerm)
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "travels";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Travel>(collectionName);

        var filterBuilder = Builders<Travel>.Filter;
        var filter = filterBuilder.Or(
            filterBuilder.Regex(r => r.name, new BsonRegularExpression(searchTerm, "i"))
        );

        var getFilteredTravelsFromDb = collection.Find(filter).ToList();

        return getFilteredTravelsFromDb;
    }

    private List<Travel> GetTravelsFromDB()
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "travels";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Travel>(collectionName);
        var filter = Builders<Travel>.Filter.Empty;
        var attractions = collection.Find(filter).ToList();

        return attractions;
    }

    private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (TextBox) sender;
        if (textBox.Text == "Search")
        {
            textBox.Text = string.Empty;
            textBox.Foreground = Brushes.Black;
            textBox.Opacity = 1.0;
        }
    }

    private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (TextBox) sender;
        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = "Search";
            textBox.Foreground = Brushes.Gray;
            textBox.Opacity = 0.5;
        }
    }


   

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        TravelCreateDialog dialog = new TravelCreateDialog(SelectedTravel);

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "hci";
            string collectionName = "travels";

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<Travel>(collectionName);
            
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchTerm = searchTextBox.Text;

        var filteredTravelsFromDb = GetFilteredTravelsFromDB(searchTerm);
        if (filteredTravelsFromDb.Count != 0)
        {
            travelDataGrid.ItemsSource = filteredTravelsFromDb;
        }
    }

    private string GenerateUniqueID(IMongoCollection<Travel> collection)
    {
        string newId;
        bool idExists;

        do
        {
            int randomNumber = new Random().Next(100000, 999999);
            newId = randomNumber.ToString();

            var idFilter = Builders<Travel>.Filter.Eq(r => r.id, newId);
            idExists = collection.Find(idFilter).Any();
        } while (idExists);

        return newId;
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
        }
        else
        {
            editButton.Content = "Edit";
            IsReadOnly = true;
            DataContext = null;
            DataContext = this;
            confirmButton.IsEnabled = false;
        }
    }

    private void ConfirmButton_OnClickButton_Click(object sender, RoutedEventArgs e)
    {
        string newName = nameTextBox.Text;
        string newAddress = "";
        int newCost;
        string attractionId = SelectedTravel.id;

        if (!int.TryParse(costTextBox.Text, out newCost))
        {
            MessageBox.Show("Invalid cost value.");
            return;
        }

        if (newName.Length == 0 || newAddress.Length == 0 || newCost <= 0)
        {
            MessageBox.Show("Input fields can't be empty!");
            return;
        }

        FilterDefinition<Attraction> filter = Builders<Attraction>.Filter.Eq(r => r.id, attractionId);

        UpdateDefinition<Attraction> update = Builders<Attraction>.Update
            .Set(r => r.name, newName)
            .Set(r => r.address, newAddress)
            .Set(r => r.cost, newCost);

        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "attractions";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Attraction>(collectionName);
        var result = collection.UpdateOne(filter, update);

        if (result.ModifiedCount > 0)
        {
            // Update was successful
            MessageBox.Show("Attraction updated successfully!");
        }
        else
        {
            // Update did not find a matching document
            MessageBox.Show("No attraction found with the given ID.");
        }
    }
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedTravel != null)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this travel?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                string connectionString = "mongodb://localhost:27017";
                string databaseName = "hci";
                string collectionName = "travels";

                var client = new MongoClient(connectionString);

                var database = client.GetDatabase(databaseName);
                var collection = database.GetCollection<Travel>(collectionName);
                var filter = Builders<Travel>.Filter.Eq(r => r.id, SelectedTravel.id);

                var resultDelete = collection.DeleteOne(filter);

                if (resultDelete.DeletedCount > 0)
                {
                    MessageBox.Show("Travel deleted successfully!");

                    // Refresh the data grid
                    var attractionsFromDb = GetTravelsFromDB();
                    travelDataGrid.ItemsSource = attractionsFromDb;
                }
                else
                {
                    MessageBox.Show("No travel found with the given ID.");
                }
            }
        }
    }
}