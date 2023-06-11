using HCI_Tim_15_2023.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HCI_Tim_15_2023.GUI.Pregledi;

public partial class ClientTravelViewPage : Page
{
    ListBoxItem selectedTravel = null;
    List<Travel> travels = new List<Travel>();

    public ClientTravelViewPage()
    {
        InitializeComponent();

        LoadTravels();
        UpdateTravels();
    }

    private void LoadTravels()
    {
        List<Location> locations = new List<Location>();
        Travel travel1 = new Travel("Travel1", "Ilija", locations);
        Travel travel2 = new Travel("Travel2", "Relja", locations);
        Travel travel3 = new Travel("Travel3", "Sime", locations);
        travels.Add(travel1);
        travels.Add(travel2);
        travels.Add(travel3);
    }

    private void UpdateTravels()
    {
        List<ListBoxItem> travelItems = new List<ListBoxItem>();

        foreach(Travel travel in travels)
        {
            TextBlock textBlock1 = new TextBlock();
            TextBlock textBlock2 = new TextBlock();
            TextBlock textBlock3 = new TextBlock();
            textBlock1.Text = travel.name;
            textBlock2.Text = "Distance: 750m";
            textBlock3.Text = "Locations: " + travel.locations.Count;

            Grid grid = new Grid();
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition gridRow2 = new RowDefinition();
            gridRow2.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition gridRow3 = new RowDefinition();
            gridRow3.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(gridRow1);
            grid.RowDefinitions.Add(gridRow2);
            grid.RowDefinitions.Add(gridRow3);
            Grid.SetRow(textBlock1, 0);
            Grid.SetRow(textBlock2, 1);
            Grid.SetRow(textBlock3, 2);

            grid.Children.Add(textBlock1);
            grid.Children.Add(textBlock2);
            grid.Children.Add(textBlock3);

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
    }
}
