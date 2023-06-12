using ZstdSharp.Unsafe;

namespace HCI_Tim_15_2023.Model;

public class Location
{


    public string id { get; set; }
    public double lon { get; set; }
    public double lat { get; set; }
    public string address { get; set; }
    public string name { get; set; }
    public int cost {get;set;}
    public override string ToString()
    {
        return name;
    }
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Location other = (Location)obj;
        return id == other.id && lon == other.lon && lat == other.lat &&
               address == other.address && name == other.name && cost == other.cost;
    }

}