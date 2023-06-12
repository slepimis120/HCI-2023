using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using HCI_Tim_15_2023.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace HCI_Tim_15_2023.GUI.CRUD
{
    public class CostConverter : IValueConverter
    {
        public CostConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Travel travel)
            {
                return travel.Cost();
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class LocationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Travel travel)
            {
                return string.Join("; ", travel.locations.Select(location => location.name));
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public partial class TravelCrudPage : Page
    {
        public Travel SelectedTravel { get; set; }
        public bool IsReadOnly { get; set; }

        public TravelCrudPage()
        {
            InitializeComponent();
            IsReadOnly = true;
            var travels = GetTravelsFromDB();
            travelDataGrid.ItemsSource = travels;
            DataContext = this;
        }

        private void TravelDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsReadOnly = true;
            SelectedTravel = (Travel) travelDataGrid.SelectedItem;

            if (SelectedTravel != null)
            {
                costTextBox.Text = SelectedTravel.Cost().ToString();
                DataContext = null;
                DataContext = this;
            }
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
            TravelCreateDialog dialog = new TravelCreateDialog(null);

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                Travel createdTravel = dialog.CreatedTravel;
                string connectionString = "mongodb://localhost:27017";
                string databaseName = "hci";
                string collectionName = "travels";

                var client = new MongoClient(connectionString);

                var database = client.GetDatabase(databaseName);
                var collection = database.GetCollection<Travel>(collectionName);
                createdTravel.id = GenerateUniqueID(collection);
                collection.InsertOne(createdTravel);
                var travelssFromDb = GetTravelsFromDB();
                travelDataGrid.ItemsSource = travelssFromDb;
                MessageBox.Show("Travel added successfully!");
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


        private List<Travel> GetFilteredTravelsFromDB(int minCost, int maxCost, int minLoc, int maxLoc)
        {
            var travels = GetTravelsFromDB();

            var filteredTravels = travels.Where(travel =>
                travel.Cost() >= minCost &&
                travel.Cost() <= maxCost &&
                travel.locations.Count >= minLoc &&
                travel.locations.Count <= maxLoc
            ).ToList();

            return filteredTravels;
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
            if (travelDataGrid.SelectedItem == null)
            {
                return;
            }

            TravelCreateDialog dialog = new TravelCreateDialog(SelectedTravel);

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                Travel createdTravel = dialog.CreatedTravel;
                string connectionString = "mongodb://localhost:27017";
                string databaseName = "hci";
                string collectionName = "travels";

                var client = new MongoClient(connectionString);

                var database = client.GetDatabase(databaseName);
                var collection = database.GetCollection<Travel>(collectionName);

                FilterDefinition<Travel> filter = Builders<Travel>.Filter.Eq(r => r.id, createdTravel.id);

                UpdateDefinition<Travel> update = Builders<Travel>.Update
                    .Set(r => r.name, createdTravel.name)
                    .Set(r => r.locations, createdTravel.locations);


                var result1 = collection.UpdateOne(filter, update);
                if (result1.ModifiedCount > 0)
                {
                    // Update was successful
                    MessageBox.Show("Travel updated successfully!");
                }
                else
                {
                    // Update did not find a matching document
                    MessageBox.Show("No travel found with the given ID.");
                }

                var travelssFromDb = GetTravelsFromDB();
                travelDataGrid.ItemsSource = travelssFromDb;
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTravel != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this travel?",
                    "Confirmation", MessageBoxButton.YesNo);
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

        private void FilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            int minCostValue;
            int maxCostValue;
            int minLocValue;
            int maxLocValue;
            if (!int.TryParse(minCost.Text, out minCostValue))
            {
                MessageBox.Show("Invalid min cost value.");
                return;
            }

            if (!int.TryParse(maxCost.Text, out maxCostValue))
            {
                MessageBox.Show("Invalid max cost value.");
                return;
            }

            if (!int.TryParse(minLoc.Text, out minLocValue))
            {
                MessageBox.Show("Invalid min location value.");
                return;
            }

            if (!int.TryParse(maxLoc.Text, out maxLocValue))
            {
                MessageBox.Show("Invalid max location value.");
                return;
            }

            var filteredTravelsFromDb = GetFilteredTravelsFromDB(minCostValue, maxCostValue, minLocValue, maxLocValue);
            if (filteredTravelsFromDb.Count != 0)
            {
                travelDataGrid.ItemsSource = filteredTravelsFromDb;
            }
        }
    }
}