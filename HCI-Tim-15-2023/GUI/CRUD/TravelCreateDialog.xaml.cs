using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using HCI_Tim_15_2023.Model;
using MongoDB.Driver;

namespace HCI_Tim_15_2023.GUI.CRUD;

public partial class TravelCreateDialog : Window
{
    Point startPoint = new Point();

    public ObservableCollection<Location> Locations { get; set; }

    public ObservableCollection<Location> Locations2 { get; set; }

    public TravelCreateDialog(Travel travel)
    {
        InitializeComponent();
        List<Location> l = new List<Location>();
        if (travel is null)
        {
            l = getLocationsFromDB();
        }
        else
        {
            l = travel.locations;
        }

        this.DataContext = this;


        Locations = new ObservableCollection<Location>(l);
        Locations2 = new ObservableCollection<Location>();
    }
    
    public List<Location> getLocationsFromDB()
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionAccomodations = "accomodations";
        string collectionRestaurants = "attractions";
        string collectionAttractions = "restaurants";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        var combinedLocations = new List<Location>();

        var accomodationsCollection = database.GetCollection<Location>(collectionAccomodations);
        var accomodations = accomodationsCollection.Find(_ => true).ToList();
        combinedLocations.AddRange(accomodations);

        var restaurantsCollection = database.GetCollection<Location>(collectionRestaurants);
        var restaurants = restaurantsCollection.Find(_ => true).ToList();
        combinedLocations.AddRange(restaurants);

        var attractionsCollection = database.GetCollection<Location>(collectionAttractions);
        var attractions = attractionsCollection.Find(_ => true).ToList();
        combinedLocations.AddRange(attractions);

        return combinedLocations;
    }

    private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        startPoint = e.GetPosition(null);
    }

    private void ListView_MouseMove(object sender, MouseEventArgs e)
    {
        Point mousePos = e.GetPosition(null);
        Vector diff = startPoint - mousePos;

        if (e.LeftButton == MouseButtonState.Pressed &&
            (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
             Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
            // Get the dragged ListViewItem
            ListView listView = sender as ListView;
            ListViewItem listViewItem =
                FindAncestor<ListViewItem>((DependencyObject) e.OriginalSource);

            // Find the data behind the ListViewItem
            Location location = (Location) listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

            // Initialize the drag & drop operation
            DataObject dragData = new DataObject("myFormat", location);
            DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
        }
    }

    private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
    {
        do
        {
            if (current is T)
            {
                return (T) current;
            }

            current = VisualTreeHelper.GetParent(current);
        } while (current != null);

        return null;
    }

    private void ListView_DragEnter(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void ListView_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("myFormat"))
        {
            Location location = e.Data.GetData("myFormat") as Location;
            Locations.Remove(location);
            Locations2.Add(location);
        }
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        string name = nameTextBox.Text;
        List<Location> locations = new List<Location>(Locations2);

        // Create the travel object
        Travel createdTravel = new Travel
        {
            name = name,
            locations = locations
        };

        CreatedTravel = createdTravel; 

        DialogResult = true;
    }
    public Travel CreatedTravel { get; private set; }
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}