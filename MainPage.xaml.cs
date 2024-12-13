namespace LocationTrackerApp
{
    public partial class MainPage : ContentPage
    {
        private readonly LocationService _locationService;
        private DatabaseService _locationDatabase;
        private CancellationTokenSource? _trackingCancellationTokenSource;
        public MainPage()
        {
            InitializeComponent();

            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "locations.db");
            _locationDatabase = new DatabaseService(dbPath);
            _locationService = new LocationService(_locationDatabase);

            _locationService.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, DatabaseService locationData)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MapView.AddHeatMapPoints(locationData.GetLocationsAsync().Result);
            });
        }

        private async void StartTrackingButton_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Start Tracking button clicked.");
            if (_trackingCancellationTokenSource != null)
            {
                Console.WriteLine("Location tracking is already in progress.");
                return;
            }

            _trackingCancellationTokenSource = new CancellationTokenSource();

            try
            {
                Console.WriteLine("Location tracking starting.");
                await _locationService.StartTrackingLocationAsync(_trackingCancellationTokenSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting location tracking: {ex.Message}");
            }
        }

        private void StopTrackingButton_Clicked(object sender, EventArgs e)
        {
            Console.WriteLine("Stop Tracking button clicked.");
            if (_trackingCancellationTokenSource != null)
            {
                Console.WriteLine("Location tracking stopping.");
                _locationService.StopTrackingLocation(_trackingCancellationTokenSource);
                _trackingCancellationTokenSource = null;
            }
        }
    }
}
