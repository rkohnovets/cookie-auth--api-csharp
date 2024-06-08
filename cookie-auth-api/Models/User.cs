using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public User(int id, string username, string password)
    {
        Id = id;
        Username = username;
        Password = password;
    }
}

/// <summary>
/// Класс, описывающий, в каком виде должны приходить JSON-объекты 
/// при регистрации или входе пользователя на сайт.
/// </summary>
public class UserDTO
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
