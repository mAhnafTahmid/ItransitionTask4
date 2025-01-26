using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class UserModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    public DateTime? LastSeen { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string Status { get; set; }

    public UserModel()
    {
    }

    public UserModel(int id, string name, string email, string password, string status, DateTime lastseen)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Status = status;
        LastSeen = lastseen;
    }
}
