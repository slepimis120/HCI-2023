using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HCI_Tim_15_2023.Model;

public class Travel
{
    public string id { get; set; }
    public string name { get; set; }
    public List<Location> locations { get; set; }
    public Travel(string id, string name, List<Location> locations)
    {
        this.id = id;
        this.name = name;
        this.locations = locations;
    }

    public Travel()
    {
    }

    public int cost() => locations.Sum(location => location.cost);

    public int Distance()
    {
        int distance = 0;
        for (int i = 1; i < this.locations.Count; i++)
        {
            double x1, x2, y1, y2;
            x1 = this.locations[i - 1].lat;
            y1 = this.locations[i - 1].lon;
            x2 = this.locations[i].lat;
            y2 = this.locations[i].lon;
            distance += (int)(Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2)) * 1.41);
        }
        return distance;
    }
}