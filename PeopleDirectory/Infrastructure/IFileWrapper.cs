namespace PeopleDirectory.Infrastructure;

/**
 * This interface provides some flexibility and makes unit testing easier by allowing us to use a mock when running tests
 * instead of hitting our file system
 */
public interface IFileWrapper {
    Task<string> ReadAllTextAsync(string path);
    Task WriteAllTextAsync(string path, string contents);
    bool Exists(string path);
}