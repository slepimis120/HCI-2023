using HCI_Tim_15_2023.Model;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;
using Loc = Microsoft.Maps.MapControl.WPF.Location;
using Location = HCI_Tim_15_2023.Model.Location;

namespace HCI_Tim_15_2023.GUI.Pregledi;

public partial class AgentMonthlySoldTravelViewPage : Page
{
    private ListBoxItem selectedTravel = null;
    private List<Travel> travels = new List<Travel>();
    private List<int> bought = new List<int>();

    private int minPrice = 0;
    private int maxPrice = 99999999;
    private int minDistance = 0;
    private int maxDistance = 99999999;
    private int minLocations = 0;
    private int maxLocations = 99999999;
    private int minSold = 0;
    private int maxSold = 99999999;
    private int month = 6;
    private int year = 2023;

    public AgentMonthlySoldTravelViewPage()
    {
        InitializeComponent();

        LoadTravels();
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
        var travelsDB = collection.Find(filter).ToList();

        return travelsDB;
    }

    private List<BoughtTravel> GetBoughtTravelsFromDB()
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "reservations";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<BoughtTravel>(collectionName);
        var filter = Builders<BoughtTravel>.Filter.Empty;
        var travelsDB = collection.Find(filter).ToList();

        return travelsDB;
    }

    private void LoadTravels()
    {
        travels.Clear();
        bought.Clear();

        foreach (Travel travel in GetTravelsFromDB())
        {
            if (travel.name.Contains(txtSearch.Text)
                && travel.Cost() >= minPrice && travel.Cost() <= maxPrice
                && travel.Distance() >= minDistance && travel.Distance() <= maxDistance
                && travel.locations.Count >= minLocations && travel.locations.Count <= maxLocations)
            {
                int b = 0;
                foreach (BoughtTravel boughtTravel in GetBoughtTravelsFromDB())
                {
                    if (travel.id == boughtTravel.travel.id
                        && boughtTravel.time.Month == month && boughtTravel.time.Year == year)
                        b++;
                }
                if (b >= minSold && b <= maxSold)
                {
                    travels.Add(travel);
                    bought.Add(b);
                }
            }
        }

        UpdateTravels();
    }

    private void UpdateTravels()
    {
        TravelList.Items.Clear();
        ClearSelectedTravel();
        ClearMap();

        List<ListBoxItem> travelItems = new List<ListBoxItem>();

        foreach (Travel travel in travels)
        {
            TextBlock textBlock1 = new TextBlock();
            TextBlock textBlock2 = new TextBlock();
            TextBlock textBlock3 = new TextBlock();
            TextBlock textBlock4 = new TextBlock();
            TextBlock textBlock5 = new TextBlock();
            textBlock1.FontSize = 20;
            textBlock2.FontSize = 15;
            textBlock3.FontSize = 15;
            textBlock4.FontSize = 15;
            textBlock5.FontSize = 15;
            textBlock1.Text = travel.name;
            textBlock2.Text = "Distance: " + travel.Distance() + "m";
            textBlock3.Text = "Locations: " + travel.locations.Count;
            textBlock4.Text = "Price: " + travel.Cost();
            textBlock5.Text = "Sold: " + bought[travels.IndexOf(travel)];

            Grid grid = new Grid();
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = new GridLength(2, GridUnitType.Star);
            RowDefinition gridRow2 = new RowDefinition();
            gridRow2.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition gridRow3 = new RowDefinition();
            gridRow3.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition gridRow4 = new RowDefinition();
            gridRow4.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition gridRow5 = new RowDefinition();
            gridRow4.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(gridRow1);
            grid.RowDefinitions.Add(gridRow2);
            grid.RowDefinitions.Add(gridRow3);
            grid.RowDefinitions.Add(gridRow4);
            grid.RowDefinitions.Add(gridRow5);
            Grid.SetRow(textBlock1, 0);
            Grid.SetRow(textBlock2, 1);
            Grid.SetRow(textBlock3, 2);
            Grid.SetRow(textBlock4, 3);
            Grid.SetRow(textBlock5, 4);

            grid.Children.Add(textBlock1);
            grid.Children.Add(textBlock2);
            grid.Children.Add(textBlock3);
            grid.Children.Add(textBlock4);
            grid.Children.Add(textBlock5);

            ListBoxItem item = new ListBoxItem();
            item.Content = grid;
            item.Selected += Travel_Selected;
            item.Margin = new Thickness(2);
            item.BorderThickness = new Thickness(2);
            item.BorderBrush = Brushes.Blue;
            item.SetValue(BackgroundProperty, Brushes.LightSkyBlue);
            TravelList.Items.Add(item);
        }
    }

    private void Travel_Selected(object sender, System.Windows.RoutedEventArgs e)
    {
        if (selectedTravel != null)
        {
            selectedTravel.BorderThickness = new Thickness(2);
            selectedTravel.BorderBrush = Brushes.Blue;
            selectedTravel.SetValue(BackgroundProperty, Brushes.LightSkyBlue);
        }

        selectedTravel = sender as ListBoxItem;
        selectedTravel.BorderBrush = Brushes.DarkOrange;
        selectedTravel.BorderThickness = new Thickness(4);
        selectedTravel.SetValue(BackgroundProperty, Brushes.Orange);
        selectedTravel.IsSelected = false;

        UpdateMap();
        UpdateSelectedTravel();
    }

    private void UpdateMap()
    {
        ClearMap();

        Travel travel = travels[TravelList.Items.IndexOf(selectedTravel)];
        LocationCollection locs = new LocationCollection();

        MapLayer mapLayer = new MapLayer();
        mapLayer.Name = "MapPinLayer";

        foreach (Location location in travel.locations)
        {
            Loc loc = new Loc(location.lat, location.lon);
            Pushpin pushpin = new Pushpin();
            pushpin.Background = new SolidColorBrush(Colors.Blue);
            pushpin.Location = loc;
            mapLayer.Children.Add(pushpin);

            locs.Add(loc);
        }

        MapPolyline routeLine = new MapPolyline()
        {
            Locations = locs,
            Stroke = new SolidColorBrush(Colors.Orange),
            StrokeThickness = 5
        };

        myMap.Children.Add(routeLine);
        myMap.Children.Add(mapLayer);
    }

    private void ClearMap()
    {
        myMap.Children.Clear();
    }

    private void UpdateSelectedTravel()
    {
        LocationList.Items.Clear();

        Travel travel = travels[TravelList.Items.IndexOf(selectedTravel)];
        txtName.Text = "Name: " + travel.name;
        txtDistance.Text = "Distance: " + travel.Distance() + "m";
        txtPrice.Text = "Price: " + travel.Cost();
        txtLocations.Text = "Locations: " + travel.locations.Count;
        txtSold.Text = "Sold: " + bought[TravelList.Items.IndexOf(selectedTravel)];

        foreach (Location location in travel.locations)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = 20;
            if (location is Restaurant)
                textBlock.Text = "Restaurant: ";
            if (location is Attraction)
                textBlock.Text = "Attraction: ";
            if (location is Accomodation)
                textBlock.Text = "Accomodation: ";
            textBlock.Text += location.name;

            LocationList.Items.Add(textBlock);
        }
    }

    private void ClearSelectedTravel()
    {
        selectedTravel = null;

        txtName.Text = "Name:";
        txtDistance.Text = "Distance:";
        txtPrice.Text = "Price:";
        txtLocations.Text = "Locations:";
        txtSold.Text = "Sold:";

        LocationList.Items.Clear();
    }

    private void Back(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new AgentHomePage());
    }

    private void Search(object sender, TextChangedEventArgs e)
    {
        LoadTravels();
    }

    private void Filter(object sender, RoutedEventArgs e)
    {
        if (FilterDialog.IsVisible)
            FilterDialog.Visibility = Visibility.Hidden;
        else
            FilterDialog.Visibility = Visibility.Visible;
    }

    private void Apply(object sender, RoutedEventArgs e)
    {
        FilterDialog.Visibility = Visibility.Hidden;

        if (!Int32.TryParse(txtMinPrice.Text, out minPrice))
            minPrice = 0;
        if (!Int32.TryParse(txtMaxPrice.Text, out maxPrice))
            maxPrice = 99999999;
        if (!Int32.TryParse(txtMinDistance.Text, out minDistance))
            minDistance = 0;
        if (!Int32.TryParse(txtMaxDistance.Text, out maxDistance))
            maxDistance = 99999999;
        if (!Int32.TryParse(txtMinLocations.Text, out minLocations))
            minLocations = 0;
        if (!Int32.TryParse(txtMaxLocations.Text, out maxLocations))
            maxLocations = 99999999;
        if (!Int32.TryParse(txtMinSold.Text, out minSold))
            minSold = 0;
        if (!Int32.TryParse(txtMaxSold.Text, out maxSold))
            maxSold = 99999999;

        LoadTravels();
    }

    private void Left(object sender, RoutedEventArgs e)
    {
        month--;
        if(month < 1)
        {
            month = 12;
            year--;
        }

        UpdateMonth();
    }

    private void Right(object sender, RoutedEventArgs e)
    {
        month++;
        if (month > 12)
        {
            month = 1;
            year++;
        }

        UpdateMonth();
    }

    private void UpdateMonth()
    {
        string text = "";
        switch(month)
        {
            case 1:
                text = "January";
                break;
            case 2:
                text = "February";
                break;
            case 3:
                text = "March";
                break;
            case 4:
                text = "April";
                break;
            case 5:
                text = "May";
                break;
            case 6:
                text = "June";
                break;
            case 7:
                text = "July";
                break;
            case 8:
                text = "August";
                break;
            case 9:
                text = "September";
                break;
            case 10:
                text = "October";
                break;
            case 11:
                text = "November";
                break;
            case 12:
                text = "December";
                break;
        }
        text += " " + year;

        btnMonth.Content = text;
        LoadTravels();
    }
}
