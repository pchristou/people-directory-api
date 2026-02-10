using PeopleDirectory.Models;

namespace PeopleDirectory.Infrastructure;

public class PhysicalFileWrapper : IFileWrapper 
{
    public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);
    
    public async Task WriteAllTextAsync(string path, string contents) 
    {
        // Make sure the directory exists before writing!
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        await File.WriteAllTextAsync(path, contents);
    }

    public bool Exists(string path) => File.Exists(path);
}