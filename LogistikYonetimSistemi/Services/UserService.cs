using LogistikYonetimSistemi.Models.Users;

namespace LogistikYonetimSistemi.Services;

public class UserService
{
    private readonly List<User> _users = new();

    public void Add(User user) => _users.Add(user);

    public void Remove(string userId) =>
        _users.RemoveAll(u => u.Id == userId);

    public User? FindByUsername(string username) =>
        _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public User? FindById(string id) =>
        _users.FirstOrDefault(u => u.Id == id);

    public IReadOnlyList<User> GetAll() => _users.AsReadOnly();
}
