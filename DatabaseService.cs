using SQLite;

namespace LocationTrackerApp
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LocationModel>();
        }

        public Task<int> SaveLocationAsync(LocationModel location)
        {
            return _database.InsertAsync(location);
        }
        public Task<List<LocationModel>> GetLocationsAsync() => _database.Table<LocationModel>().ToListAsync();
    }
}