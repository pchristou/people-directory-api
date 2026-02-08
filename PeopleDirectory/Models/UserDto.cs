namespace PeopleDirectory.Models;

public class UserDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string JobTitle { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
}