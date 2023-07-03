using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserRepository
    {
        public List<User> Users { get; set; }
        public UserRepository() => Users = new List<User>() { new User { Id = 0, Username = "a", Password = "a" } };
        public User AddUser(RegisterUser registerUser)
        {
            if(registerUser.Password != registerUser.ConfirmPassword)
                throw new Exception("Password != Confirm Password");

            if (!IsUsernameAvailable(registerUser.Username))
                throw new Exception("This username is not available");

            User newUser = new User
            {
                Id = Users.Count,
                Password = registerUser.Password,
                Username = registerUser.Username
            };

            Users.Add(newUser);
            
            return newUser;
        }
        public bool IsUsernameAvailable(string username)
        {
            return Users.All(u => u.Username != username);
        }

        public User? TryLogin(LoginUser loginUser)
        {
            return Users.Find(u => u.Username == loginUser.Username && u.Password == loginUser.Password);
        }
    }
}
