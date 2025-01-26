using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class UserModel
{
    [Key] // Primary key
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    [EmailAddress] // Ensures the field is a valid email
    public required string Email { get; set; }

    public DateTime? LastSeen { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string Status { get; set; }

    public UserModel()
    {
    }

    public UserModel(int id, string name, string email, string password, string status, DateTime lastseen) // Calls the parameterless constructor
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Status = status;
        LastSeen = lastseen;
    }
}
