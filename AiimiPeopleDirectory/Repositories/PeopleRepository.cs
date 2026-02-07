namespace AiimiPeopleDirectory.Repositories;

using Models;
using Extensions;

public class PeopleRepository
{
    private readonly string _filePath;

    public PeopleRepository(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "people.json");
    }

    public async Task<List<PersonDto>> GetAll()
    {
        if (!File.Exists(_filePath))
            return [];
        
        string json = await File.ReadAllTextAsync(_filePath);

        return JsonExtensions.Deserialize<List<PersonDto>>(json)
               ?? [];
    }

    public async Task<PersonDto?> GetById(int id)
    {
        // We get all people then filter by the requested id. Only done this way for simplicity given the dataset is tiny.
        // In a production system, we would use a database and query against a table (or document for NoSQL) with an indexed
        // id to pull out just the required record
        List<PersonDto> people = await GetAll();
        return people.FirstOrDefault(p => p.Id == id);
    }
}
