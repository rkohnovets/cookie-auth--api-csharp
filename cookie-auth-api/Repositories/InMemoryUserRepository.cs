using AspNetCoreWebAPI.Models;

namespace AspNetCoreWebAPI.Repositories;

/// <summary>
/// Наипростейшее хранилище пользователей - в оперативной памяти 
/// (точнее в памяти сервера, который является по сути консольным приложением).
/// 
/// Удобно, так как не нужно разворачивать никакие БД, но, конечно, 
/// при выключении или краше сервака все добавленные пользователи пропадут.
/// </summary>
public class InMemoryUserRepository : IUserRepository
{
    private List<User> Users { get; set; }

    public InMemoryUserRepository()
    {
        Users = new List<User>()
        {
            new User(0, "a", "a"),
            new User(1, "b", "b")
        };
    }

    public User AddUser(UserDTO user)
    {
        if (!IsUsernameAvailable(user.Username))
            throw new Exception("This username is not available");

        User newUser = new User(
            Users.Count,
            user.Password,
            user.Username
        );

        Users.Add(newUser);

        return newUser;
    }

    public bool IsUsernameAvailable(string username)
    {
        return Users.All(u => u.Username != username);
    }

    public User? FindByDTO(UserDTO user)
    {
        return Users.Find(u => u.Username == user.Username && u.Password == user.Password);
    }
}
