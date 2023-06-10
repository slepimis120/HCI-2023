using System.Collections.Generic;

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


}