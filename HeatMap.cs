using Microsoft.Maui.Controls.Maps;

namespace LocationTrackerApp
{
    public class HeatMap : Microsoft.Maui.Controls.Maps.Map
    {
        public void AddHeatMapPoints(IEnumerable<LocationModel> locations)
        {
            foreach (var location in locations)
            {
                var pin = new Pin
                {
                    Label = "Heat Point",
                    Location = new Location(location.Latitude, location.Longitude),
                    Type = PinType.Place
                };
                Pins.Add(pin);
            }
        }
    }
}