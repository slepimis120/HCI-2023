using HCI_Tim_15_2023.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
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
using Newtonsoft.Json;
namespace HCI_Tim_15_2023.GUI.CRUD;
public partial class AccomodationCrudPage : Page
{
    public Accomodation SelectedAccomodation { get; set; }

    public bool IsReadOnly { get; set; }
    public AccomodationCrudPage()
    {
        InitializeComponent();
        IsReadOnly = true;
        var accomodations = GetAccomodationsFromDB();
        accomodadtionDataGrid.ItemsSource = accomodations;
        DataContext = this;
    }
    private void AccomodationDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        IsReadOnly = true;
        SelectedAccomodation = (Accomodation)accomodadtionDataGrid.SelectedItem;
    
        DataContext = null;
        DataContext = this;
    }

    private List<Accomodation> GetFilteredAccomodationsFromDB(string searchTerm)
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "accomodations";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Accomodation>(collectionName);

        var filterBuilder = Builders<Accomodation>.Filter;
        var filter = filterBuilder.Or(
            filterBuilder.Regex(r => r.name, new BsonRegularExpression(searchTerm, "i")),
            filterBuilder.Regex(r => r.address, new BsonRegularExpression(searchTerm, "i"))
        );

        var filteredAccomodations = collection.Find(filter).ToList();

        return filteredAccomodations;
    }
    private List<Accomodation> GetAccomodationsFromDB()
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "accomodations";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Accomodation>(collectionName);
        var filter = Builders<Accomodation>.Filter.Empty;
        var accomodations = collection.Find(filter).ToList();

        return accomodations;
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
    public class GeocodingResult
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }

    private async Task<bool> SetCoordinatesFromAddress(Accomodation accomodation)
    {
        string apiUrl = "https://nominatim.openstreetmap.org/search?format=json&street=" +
                        Uri.EscapeDataString(accomodation.address) + "+&city=" + "Belgrade";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", "My-User-Agent");
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<List<GeocodingResult>>(responseBody);
                if (results.Count > 0)
                {
                    var firstResult = results[0];
                    double latitude = double.Parse(firstResult.lat, CultureInfo.InvariantCulture);
                    double longitude = double.Parse(firstResult.lon, CultureInfo.InvariantCulture);


                    accomodation.lat = latitude;
                    accomodation.lon = longitude;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }


    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        CreateLocationDialog dialog = new CreateLocationDialog();

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "hci";
            string collectionName = "accomodations";

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<Accomodation>(collectionName);
            string name = dialog.NameTextBox.Text;
            string address = dialog.AddressTextBox.Text;
            if (name == "" || address == "")
            {
                MessageBox.Show("Input fields can't be empty.");
                return;
            }
            int cost;
            if (!int.TryParse(dialog.CostTextBox.Text, out cost))
            {
                MessageBox.Show("Invalid cost value. Please enter a valid integer.");
                return;
            }

            Accomodation newAccomodation = new Accomodation
            {
                name = name,
                address = address,
                cost = cost,
                id = GenerateUniqueID(collection)
            };

            bool isAddressValid = await SetCoordinatesFromAddress(newAccomodation);

            if (isAddressValid)
            {
                collection.InsertOne(newAccomodation);
                var accomodationsFromDb = GetAccomodationsFromDB();
                accomodadtionDataGrid.ItemsSource = accomodationsFromDb;
                MessageBox.Show("Accomodation added successfully!");
            }
            else
            {
                MessageBox.Show("Invalid address. Accomodation not added.");
            }
        }
    }



    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchTerm = searchTextBox.Text;

        var filteredAccomodationsFromDb = GetFilteredAccomodationsFromDB(searchTerm);
        if (filteredAccomodationsFromDb.Count != 0)
        {
            accomodadtionDataGrid.ItemsSource = filteredAccomodationsFromDb;
        }
        
    }private string GenerateUniqueID(IMongoCollection<Accomodation> collection)
    {
        string newId;
        bool idExists;

        do
        {
            int randomNumber = new Random().Next(100000, 999999);
            newId = randomNumber.ToString();

            var idFilter = Builders<Accomodation>.Filter.Eq(r => r.id, newId);
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
        }else
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
        string newAddress = addressTextBox.Text;
        int newCost;
        string accomodationId = SelectedAccomodation.id;
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

        FilterDefinition<Accomodation> filter = Builders<Accomodation>.Filter.Eq(r => r.id, accomodationId);


        UpdateDefinition<Accomodation> update = Builders<Accomodation>.Update
            .Set(r => r.name, newName)
            .Set(r => r.address, newAddress)
            .Set(r => r.cost, newCost);

        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "accomodations";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Accomodation>(collectionName);
        var result = collection.UpdateOne(filter, update);

        if (result.ModifiedCount > 0)
        {
            // Update was successful
            MessageBox.Show("Accomodation updated successfully!");
        }
        else
        {
            // Update did not find a matching document
            MessageBox.Show("No accomodation found with the given ID.");
        }
    }
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedAccomodation != null)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this accomodation?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                string connectionString = "mongodb://localhost:27017";
                string databaseName = "hci";
                string collectionName = "accomodations";

                var client = new MongoClient(connectionString);

                var database = client.GetDatabase(databaseName);
                var collection = database.GetCollection<Accomodation>(collectionName);
                var filter = Builders<Accomodation>.Filter.Eq(r => r.id, SelectedAccomodation.id);

                var resultDelete = collection.DeleteOne(filter);

                if (resultDelete.DeletedCount > 0)
                {
                    MessageBox.Show("Accomodation deleted successfully!");

                    var accomodationsFromDb = GetAccomodationsFromDB();
                    accomodadtionDataGrid.ItemsSource = accomodationsFromDb;
                }
                else
                {
                    MessageBox.Show("No accomodation found with the given ID.");
                }
            }
        }
    }


}
