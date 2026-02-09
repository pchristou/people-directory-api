using System.Text.Json;

namespace PeopleDirectory.Repositories;

using Models;
using Extensions;
    
public class UserRepository
{
    private readonly string _filePath;

    public UserRepository(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "users.json");
    }

    public async Task<UserDto> AddAsync(UserDto person)
    {
        if (person is null)
            throw new ArgumentNullException(nameof(person));

        var people = await GetAll();

        person.Id = people.Any() ? people.Max(p => p.Id) + 1 : 1;
        people.Add(person);

        var json = JsonSerializer.Serialize(
            people,
            new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(_filePath, json);

        return person;
    }
    
    public async Task<UserDto?> GetById(int id)
    {
        var people = await GetAll();
        return people.FirstOrDefault(p => p.Id == id);
    }

    public async Task<List<UserDto>> SearchByName(string searchTerm, int limit)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<UserDto>();

        // We get all people then filter by the requested id. Only done this way for simplicity given the dataset is tiny.
        // In a production system, we would use a database and filter against a table (or document for NoSQL) with an indexed
        // id to pull out just the required record
        var people = await GetAll();

        return people
            .Where(p =>
                p.FirstName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.LastName.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))
            .Take(limit)
            .ToList();
    }
    
    private async Task<List<UserDto>> GetAll()
    {
        if (!File.Exists(_filePath))
            return [];
        
        string json = await File.ReadAllTextAsync(_filePath);

        return JsonExtensions.Deserialize<List<UserDto>>(json)
               ?? [];
    }
}
