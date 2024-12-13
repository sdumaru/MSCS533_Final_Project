
using Microsoft.Maui.Devices.Sensors;

namespace LocationTrackerApp
{
    public class LocationService
    {
        private readonly DatabaseService _database;
        public event EventHandler<DatabaseService>? LocationChanged;
        private LocationModel? _lastKnownLocation;

        public LocationService(DatabaseService database)
        {
            _database = database;
        }

        protected virtual void OnLocationChanged(DatabaseService locationData)
        {
            LocationChanged?.Invoke(this, locationData);
        }


        public async Task StartTrackingLocationAsync(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Low, TimeSpan.FromSeconds(30));
                Console.WriteLine("Location tracking started.");
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var location = await Geolocation.GetLocationAsync(request, cancellationTokenSource.Token);

                    if (location != null)
                    {
                        var newlocationData = new LocationModel
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            Timestamp = location.Timestamp.DateTime
                        };

                        if (IsNewLocation(_lastKnownLocation, newlocationData))
                        {
                            await Task.Run(async () =>
                            {
                                Console.WriteLine("Saving location.");
                                await _database.SaveLocationAsync(newlocationData);
                                OnLocationChanged(_database);
                            });

                            _lastKnownLocation = newlocationData;
                        }
                        else
                        {
                            Console.WriteLine("Location unchanged. Not saving.");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Failed to get location.");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Location tracking canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during location tracking: {ex.Message}");
            }
        }

        public void StopTrackingLocation(CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource?.Cancel();
            Console.WriteLine("Location tracking stopped.");
        }

        private bool IsNewLocation(LocationModel? lastLocation, LocationModel newLocation)
        {
            if (lastLocation == null)
                return true;

            const double threshold = 0.0001;

            return Math.Abs(lastLocation.Latitude - newLocation.Latitude) > threshold ||
                Math.Abs(lastLocation.Longitude - newLocation.Longitude) > threshold;
        }
    }
}
