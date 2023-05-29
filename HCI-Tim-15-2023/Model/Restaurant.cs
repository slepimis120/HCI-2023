﻿namespace HCI_Tim_15_2023.Model;

public class Restaurant : Location
{

    public int cost { get; set; }

    public Restaurant()
    {
    }

    public Restaurant(string id, double lon, double lat, string address, string name, int cost)
    {
        this.id = id;
        this.lon = lon;
        this.lat = lat;
        this.address = address;
        this.name = name;
        this.cost = cost;
    }
}