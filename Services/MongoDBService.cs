using EmployeeAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmployeeAPI.Services
{
    public class MongoDBService
    {

        private readonly IMongoCollection<Employee> _listCollection;
        private readonly IMongoCollection<User> _userlistCollection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _listCollection = database.GetCollection<Employee>(mongoDBSettings.Value.CollectionName);
            _userlistCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionUsers);
        }

        public async Task<List<Employee>> GetAsync() =>
        await _listCollection.Find(_ => true).ToListAsync();

        public async Task<Employee?> GetAsync(string id) =>
            await _listCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Employee newEmployee) =>
            await _listCollection.InsertOneAsync(newEmployee);

        public async Task UpdateAsync(string id, Employee updatedEmployee) =>
            await _listCollection.ReplaceOneAsync(x => x.Id == id, updatedEmployee);

        public async Task RemoveAsync(string id) =>
            await _listCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<User?> GetUserAsync(string username) =>
            await _userlistCollection.Find(x => x.username == username).FirstOrDefaultAsync();

        public async Task UpdateUserAsync(string id, User updated) =>
            await _userlistCollection.ReplaceOneAsync(x => x.Id == id, updated);
        public async Task<List<User>> GetUsersAsync() =>
        await _userlistCollection.Find(_ => true).ToListAsync();

        public async Task CreateUserAsync(User newUser) =>
            await _userlistCollection.InsertOneAsync(newUser);

    }
}
