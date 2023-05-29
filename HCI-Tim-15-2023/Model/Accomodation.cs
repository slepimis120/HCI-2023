namespace HCI_Tim_15_2023.Model;

public class Accomodation : Location
{
    public Accomodation()
    {
    }
    public Accomodation(string id, double lon, double lat, string address, string name)
    {
        this.id = id;
        this.lon = lon;
        this.lat = lat;
        this.address = address;
        this.name = name;
    }
}