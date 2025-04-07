using EmployeeAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmployeeAPI.Services
{
    public class MongoDBService
    {

        private readonly IMongoCollection<Employee> _listCollection;
        private readonly IMongoCollection<User> _userlistCollection;
        private readonly IMongoCollection<Department> _departmentCollection;
        private readonly IMongoCollection<Position> _positionCollection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _listCollection = database.GetCollection<Employee>(mongoDBSettings.Value.CollectionName);
            _userlistCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionUsers);
            _departmentCollection = database.GetCollection<Department>("Departments");
            _positionCollection = database.GetCollection<Position>("Positions");
        }

        public async Task<List<Employee>> GetAsync() =>
        await _listCollection.Find(_ => true).ToListAsync();

        public async Task<List<Employee>> GetAsync(string? departmentName = null, string? positionName = null)
        {
            var filter = Builders<Employee>.Filter.Empty;

            if (!string.IsNullOrEmpty(departmentName))
            {
                filter &= Builders<Employee>.Filter.Eq(e => e.Department, departmentName);
            }

            if (!string.IsNullOrEmpty(positionName))
            {
                filter &= Builders<Employee>.Filter.Eq(e => e.Position, positionName);
            }

            return await _listCollection.Find(filter).ToListAsync();
        }

        public async Task<Employee?> GetAsync(string id) =>
            await _listCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        //Get by Email GetByEmailAsync
        public async Task<Employee?> GetByEmailAsync(string email) =>
            await _listCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
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

        public async Task CreateUniqueIndexOnEmailAsync()
        {
            var indexKeys = Builders<Employee>.IndexKeys.Ascending(e => e.Email);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<Employee>(indexKeys, indexOptions);

            await _listCollection.Indexes.CreateOneAsync(indexModel);
        }

        public async Task<Department?> GetDepartmentByNameAsync(string name) =>
            await _departmentCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        public async Task<Department> CreateDepartmentAsync(string name)
        {
            var newDepartment = new Department { Name = name };
            await _departmentCollection.InsertOneAsync(newDepartment);
            return newDepartment;
        }

        public async Task<List<Department>> GetDepartmentsAsync() =>
            await _departmentCollection.Find(_ => true).ToListAsync();

        public async Task<Position?> GetPositionByNameAndDepartmentAsync(string name, string departmentId) =>
            await _positionCollection.Find(x => x.Name == name && x.DepartmentId == departmentId).FirstOrDefaultAsync();

        public async Task<Position> CreatePositionAsync(string name, string departmentId)
        {
            var newPosition = new Position { Name = name, DepartmentId = departmentId };
            await _positionCollection.InsertOneAsync(newPosition);
            return newPosition;
        }

        public async Task<List<Position>> GetPositionsByDepartmentIdAsync(string departmentId) =>
            await _positionCollection.Find(x => x.DepartmentId == departmentId).ToListAsync();
    }
}
