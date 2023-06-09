﻿using HCI_Tim_15_2023.GUI.Pregledi;
using HCI_Tim_15_2023.Model;
using System.Windows;
using System.Windows.Controls;
using HCI_Tim_15_2023.GUI.CRUD;

namespace HCI_Tim_15_2023.GUI;
public partial class AgentHomePage : Page
{

    public AgentHomePage()
    {
        InitializeComponent();
    }

    private void OpenRestaurantCRUD(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new RestaurantCrudPage());
    }
    private void OpenAccomodationCRUD(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new AccomodationCrudPage());
    }

    private void OpenAgentMonthlySoldTravelViewPage(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new AgentMonthlySoldTravelViewPage());
    }

    private void OpenAgentSoldIndividualTravelViewPage(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new AgentSoldIndividualTravelViewPage());
    }

    private void LogOut(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new LogInPage());
    }

    private void OpenAttractionCRUD(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new AttractionCrudPage());
    }

    private void OpenLocationCRUD(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new TravelCrudPage());
    }
}
