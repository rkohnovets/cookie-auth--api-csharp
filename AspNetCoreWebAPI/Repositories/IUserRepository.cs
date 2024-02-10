using AspNetCoreWebAPI.Models;

namespace AspNetCoreWebAPI.Repositories;

public interface IUserRepository
{
    public User AddUser(UserDTO user);
    public bool IsUsernameAvailable(string username);
    public User? FindByDTO(UserDTO user);
}