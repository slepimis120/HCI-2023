﻿using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using HCI_Tim_15_2023.GUI.CRUD;
using HCI_Tim_15_2023.Model;
using Microsoft.Maps.MapControl.WPF;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Loc = Microsoft.Maps.MapControl.WPF.Location;
using Location = HCI_Tim_15_2023.Model.Location;

namespace HCI_Tim_15_2023.GUI.Pregledi;

public partial class ClientTravelViewPage : Page
{
    private ListBoxItem selectedTravel = null;
    private List<Travel> travels = new List<Travel>();

    private int minPrice = 0;
    private int maxPrice = 99999999;
    private int minDistance = 0;
    private int maxDistance = 99999999;
    private int minLocations = 0;
    private int maxLocations = 99999999;

    public ClientTravelViewPage()
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

    private void LoadTravels()
    {
        travels.Clear();

        foreach (Travel travel in GetTravelsFromDB())
        {
            if(travel.name.Contains(txtSearch.Text)
                && travel.Cost() >= minPrice && travel.Cost() <= maxPrice
                && travel.Distance() >= minDistance && travel.Distance() <= maxDistance
                && travel.locations.Count >= minLocations && travel.locations.Count <= maxLocations)
                travels.Add(travel);
        }

        UpdateTravels();
    }

    private void UpdateTravels()
    {
        TravelList.Items.Clear();
        ClearSelectedTravel();
        ClearMap();

        List<ListBoxItem> travelItems = new List<ListBoxItem>();

        foreach(Travel travel in travels)
        {
            TextBlock textBlock1 = new TextBlock();
            TextBlock textBlock2 = new TextBlock();
            TextBlock textBlock3 = new TextBlock();
            TextBlock textBlock4 = new TextBlock();
            textBlock1.FontSize = 20;
            textBlock2.FontSize = 15;
            textBlock3.FontSize = 15;
            textBlock4.FontSize = 15;
            textBlock1.Text = travel.name;
            textBlock2.Text = "Distance: " + travel.Distance() + "m";
            textBlock3.Text = "Locations: " + travel.locations.Count;
            textBlock4.Text = "Price: " + travel.Cost();

            Grid grid = new Grid();
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = new GridLength(2, GridUnitType.Star);
            RowDefinition gridRow2 = new RowDefinition();
            gridRow2.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition gridRow3 = new RowDefinition();
            gridRow3.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition gridRow4 = new RowDefinition();
            gridRow4.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(gridRow1);
            grid.RowDefinitions.Add(gridRow2);
            grid.RowDefinitions.Add(gridRow3);
            grid.RowDefinitions.Add(gridRow4);
            Grid.SetRow(textBlock1, 0);
            Grid.SetRow(textBlock2, 1);
            Grid.SetRow(textBlock3, 2);
            Grid.SetRow(textBlock4, 3);

            grid.Children.Add(textBlock1);
            grid.Children.Add(textBlock2);
            grid.Children.Add(textBlock3);
            grid.Children.Add(textBlock4);

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
        if(selectedTravel != null)
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

        btnBook.IsEnabled = true;

        foreach(Location location in travel.locations)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = 20;
            if(location is Restaurant)
                textBlock.Text = "Restaurant: ";
            if(location is Attraction)
                textBlock.Text = "Attraction: ";
            if(location is Accomodation)
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

        btnBook.IsEnabled = false;

        LocationList.Items.Clear();
    }

    private void Back(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ClientHomePage());
    }

    private void Search(object sender, TextChangedEventArgs e)
    {
        LoadTravels();
    }

    private void Filter(object sender, RoutedEventArgs e)
    {
        if(FilterDialog.IsVisible)
            FilterDialog.Visibility = Visibility.Hidden;
        else
            FilterDialog.Visibility = Visibility.Visible;
    }

    private void Apply(object sender, RoutedEventArgs e)
    {
        FilterDialog.Visibility = Visibility.Hidden;

        if(!Int32.TryParse(txtMinPrice.Text, out minPrice))
            minPrice = 0;
        if(!Int32.TryParse(txtMaxPrice.Text, out maxPrice))
            maxPrice = 99999999;
        if(!Int32.TryParse(txtMinDistance.Text, out minDistance))
            minDistance = 0;
        if(!Int32.TryParse(txtMaxDistance.Text, out maxDistance))
            maxDistance = 99999999;
        if(!Int32.TryParse(txtMinLocations.Text, out minLocations))
            minLocations = 0;
        if(!Int32.TryParse(txtMaxLocations.Text, out maxLocations))
            maxLocations = 99999999;

        LoadTravels();
    }

    private void BookTravel(object sender, RoutedEventArgs e)
    {
        Travel travel = travels[TravelList.Items.IndexOf(selectedTravel)];
        NavigationService.Navigate(new ReservationCreatePage(travel));
    }
}
