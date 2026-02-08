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

    public async Task<List<UserDto>> GetAll()
    {
        if (!File.Exists(_filePath))
            return [];
        
        string json = await File.ReadAllTextAsync(_filePath);

        return JsonExtensions.Deserialize<List<UserDto>>(json)
               ?? [];
    }

    public async Task<UserDto?> GetById(int id)
    {
        // We get all people then filter by the requested id. Only done this way for simplicity given the dataset is tiny.
        // In a production system, we would use a database and query against a table (or document for NoSQL) with an indexed
        // id to pull out just the required record
        List<UserDto> people = await GetAll();
        return people.FirstOrDefault(p => p.Id == id);
    }
}
