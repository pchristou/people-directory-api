using System.Text.Json;
using PeopleDirectory.Exceptions;
using PeopleDirectory.Infrastructure;

namespace PeopleDirectory.Repositories;

using Models;
using Extensions;
    
public class UserRepository
{
    private readonly IFileWrapper _file;
    private readonly string _filePath;

    public UserRepository(IWebHostEnvironment env, IFileWrapper file) {
        _file = file;
        _filePath = Path.Combine(env.ContentRootPath, "Data", "users.json");
    }

    public async Task<UserDto> CreateUser(UserDto user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));
        
        var users = await GetAll();

        var emailExists = users.Any(u =>
            u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));

        if (emailExists)
            throw new DuplicateEmailException(user.Email);
        
        user.Id = users.Any() ? users.Max(p => p.Id) + 1 : 1;
        users.Add(user);

        var json = JsonSerializer.Serialize(
            users,
            new JsonSerializerOptions { WriteIndented = true });

        await _file.WriteAllTextAsync(_filePath, json);

        return user;
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
        if (!_file.Exists(_filePath))
            return [];
        
        string json = await _file.ReadAllTextAsync(_filePath);

        return JsonExtensions.Deserialize<List<UserDto>>(json)
               ?? [];
    }
}
