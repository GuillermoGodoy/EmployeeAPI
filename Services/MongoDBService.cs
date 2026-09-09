using EmployeeAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EmployeeAPI.Services
{
    public class MongoDBService
    {

        private readonly IMongoCollection<Employee> _listCollection;
        private readonly IMongoCollection<User> _userlistCollection;
        private readonly IMongoCollection<Department> _departmentCollection;
        private readonly IMongoCollection<Position> _positionCollection;
        private readonly IMongoCollection<Device> _deviceCollection;
        private readonly IMongoCollection<PunchType> _punchTypeCollection;
        private readonly IMongoCollection<Punch> _punchCollection;
        private readonly IMongoCollection<Enrollment> _enrollmentCollection;
        private readonly IMongoDatabase _database;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _database = database;
            _listCollection = database.GetCollection<Employee>(mongoDBSettings.Value.CollectionName);
            _userlistCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionUsers);
            _departmentCollection = database.GetCollection<Department>("Departments");
            _positionCollection = database.GetCollection<Position>("Positions");
            _deviceCollection = database.GetCollection<Device>("Devices");
            _punchTypeCollection = database.GetCollection<PunchType>("PunchTypes");
            _punchCollection = database.GetCollection<Punch>("Punches");
            _enrollmentCollection = database.GetCollection<Enrollment>("Enrollments");
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

        public async Task<(List<Employee> Items, long TotalCount)> GetPagedAsync(string? departmentName, string? positionName, int page, int pageSize)
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

            var totalCount = await _listCollection.CountDocumentsAsync(filter);
            var items = await _listCollection.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalCount);
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

        public async Task<Employee?> GetByDniAsync(string dni) =>
            await _listCollection.Find(x => x.Dni == dni).FirstOrDefaultAsync();

        // ----- Devices -----

        public async Task<List<Device>> GetDevicesAsync() =>
            await _deviceCollection.Find(_ => true).ToListAsync();

        public async Task<Device?> GetDeviceAsync(string id) =>
            await _deviceCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Device?> GetDeviceByNameAsync(string name) =>
            await _deviceCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        public async Task CreateDeviceAsync(Device newDevice) =>
            await _deviceCollection.InsertOneAsync(newDevice);

        public async Task UpdateDeviceAsync(string id, Device updatedDevice) =>
            await _deviceCollection.ReplaceOneAsync(x => x.Id == id, updatedDevice);

        public async Task RemoveDeviceAsync(string id) =>
            await _deviceCollection.DeleteOneAsync(x => x.Id == id);

        // ----- Punch types -----

        public async Task<List<PunchType>> GetPunchTypesAsync() =>
            await _punchTypeCollection.Find(_ => true).ToListAsync();

        public async Task<PunchType?> GetPunchTypeByCodeAsync(string code) =>
            await _punchTypeCollection.Find(x => x.Code == code).FirstOrDefaultAsync();

        public async Task CreatePunchTypeAsync(PunchType newPunchType) =>
            await _punchTypeCollection.InsertOneAsync(newPunchType);

        // ----- Enrollments -----

        public async Task<List<Enrollment>> GetEnrollmentsAsync(string? employeeId = null, string? deviceId = null)
        {
            var filter = Builders<Enrollment>.Filter.Empty;

            if (!string.IsNullOrEmpty(employeeId))
            {
                filter &= Builders<Enrollment>.Filter.Eq(e => e.Employee_Id, employeeId);
            }

            if (!string.IsNullOrEmpty(deviceId))
            {
                filter &= Builders<Enrollment>.Filter.Eq(e => e.Device_Id, deviceId);
            }

            return await _enrollmentCollection.Find(filter).ToListAsync();
        }

        public async Task<Enrollment?> GetEnrollmentAsync(string id) =>
            await _enrollmentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        /// <summary>Busca el enrolamiento activo del PIN en el dispositivo o, en su defecto, uno global.</summary>
        public async Task<Enrollment?> GetEnrollmentByPinAsync(string pin, string? deviceId)
        {
            var filter = Builders<Enrollment>.Filter.Eq(e => e.Pin, pin)
                & Builders<Enrollment>.Filter.Eq(e => e.Active, true);

            var scope = Builders<Enrollment>.Filter.Eq(e => e.Device_Id, null);
            if (!string.IsNullOrEmpty(deviceId))
            {
                scope |= Builders<Enrollment>.Filter.Eq(e => e.Device_Id, deviceId);
            }

            return await _enrollmentCollection.Find(filter & scope).FirstOrDefaultAsync();
        }

        public async Task CreateEnrollmentAsync(Enrollment newEnrollment) =>
            await _enrollmentCollection.InsertOneAsync(newEnrollment);

        public async Task RemoveEnrollmentAsync(string id) =>
            await _enrollmentCollection.DeleteOneAsync(x => x.Id == id);

        // ----- Punches -----

        public async Task<List<Punch>> GetPunchesAsync(string? employeeId = null, string? deviceId = null, DateTime? from = null, DateTime? to = null)
        {
            var filter = Builders<Punch>.Filter.Empty;

            if (!string.IsNullOrEmpty(employeeId))
            {
                filter &= Builders<Punch>.Filter.Eq(p => p.Employee_Id, employeeId);
            }

            if (!string.IsNullOrEmpty(deviceId))
            {
                filter &= Builders<Punch>.Filter.Eq(p => p.Device_Id, deviceId);
            }

            if (from.HasValue)
            {
                filter &= Builders<Punch>.Filter.Gte(p => p.Punch_Dtm, from.Value.ToUniversalTime());
            }

            if (to.HasValue)
            {
                filter &= Builders<Punch>.Filter.Lte(p => p.Punch_Dtm, to.Value.ToUniversalTime());
            }

            return await _punchCollection.Find(filter).SortBy(p => p.Punch_Dtm).ToListAsync();
        }

        public async Task<Punch?> GetPunchAsync(string id) =>
            await _punchCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Punch?> GetDuplicatePunchAsync(string employeeId, string deviceId, DateTime punchDtm) =>
            await _punchCollection
                .Find(x => x.Employee_Id == employeeId && x.Device_Id == deviceId && x.Punch_Dtm == punchDtm)
                .FirstOrDefaultAsync();

        public async Task CreatePunchAsync(Punch newPunch) =>
            await _punchCollection.InsertOneAsync(newPunch);

        public async Task UpdatePunchAsync(string id, Punch updatedPunch) =>
            await _punchCollection.ReplaceOneAsync(x => x.Id == id, updatedPunch);

        public async Task CreateTimekeeperIndexesAsync()
        {
            await _punchCollection.Indexes.CreateOneAsync(new CreateIndexModel<Punch>(
                Builders<Punch>.IndexKeys
                    .Ascending(p => p.Employee_Id)
                    .Ascending(p => p.Device_Id)
                    .Ascending(p => p.Punch_Dtm),
                new CreateIndexOptions { Unique = true }));

            await _punchTypeCollection.Indexes.CreateOneAsync(new CreateIndexModel<PunchType>(
                Builders<PunchType>.IndexKeys.Ascending(p => p.Code),
                new CreateIndexOptions { Unique = true }));
        }

        public async Task SeedPunchTypesAsync()
        {
            if (await _punchTypeCollection.CountDocumentsAsync(_ => true) > 0)
            {
                return;
            }

            await _punchTypeCollection.InsertManyAsync(new[]
            {
                new PunchType { Code = "IN", Name = "Entrada", Description = "Inicio de jornada" },
                new PunchType { Code = "OUT", Name = "Salida", Description = "Término de jornada" },
                new PunchType { Code = "BREAK_IN", Name = "Inicio de colación" },
                new PunchType { Code = "BREAK_OUT", Name = "Término de colación" }
            });
        }

        public async Task SeedEmployeesAsync()
        {
            const int targetCount = 2000;
            var currentCount = await _listCollection.CountDocumentsAsync(_ => true);

            if (currentCount >= targetCount)
            {
                return;
            }

            var existingEmails = (await _listCollection
                .Find(_ => true)
                .Project(employee => employee.Email)
                .ToListAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var employeesToCreate = new List<Employee>();
            var seedNumber = 1;

            while (currentCount + employeesToCreate.Count < targetCount)
            {
                var email = $"seed.employee.{seedNumber:D4}@example.com";
                seedNumber++;

                if (!existingEmails.Add(email))
                {
                    continue;
                }

                employeesToCreate.Add(new Employee
                {
                    Name = $"Empleado Demo {seedNumber - 1:D4}",
                    Email = email,
                    Dni = $"SEED-{seedNumber - 1:D4}",
                    Department = $"Departamento {(seedNumber - 2) % 10 + 1:D2}",
                    Position = $"Cargo {(seedNumber - 2) % 8 + 1:D2}"
                });
            }

            await _listCollection.InsertManyAsync(employeesToCreate);
        }

        public async Task PingAsync() =>
            await _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
    }
}
