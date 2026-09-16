using ATM.Domain.Models;
using ATM.Infrastructure;
using ATM.Services;
using ATM.Services.Interface;

namespace ATM.Domain.Services;

public class AuthService : IAuthServices
{
    private readonly string _fileManager;

    public AuthService(string fileManager)
    {
        _fileManager = fileManager;
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public Client RegisterClient(string name, string password, string email, decimal balance = 0)
    {
        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Invalid email format.");
        }

        List<User> users = FileManager.LoadUsersFromFile(_fileManager);

        if (users.Any(u => u.Email == email))
        {
            throw new ArgumentException("Email already exists.");
        }

        int id = IdGeneration.GenerateId(users);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        Client newClient = new Client(
            name: name,
            password: hashedPassword,
            email: email,
            id: id,
            role: "Client",
            balance: balance,
            loadStatus: "None",
            loadAmount: 0
        );

        users.Add(newClient);
        FileManager.SaveUsersToFile(users, _fileManager);

        return newClient;
    }

    public User Login(string email, string password)
    {
        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        User user = users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!isPasswordValid)
        {
            throw new ArgumentException("Invalid password.");
        }
        return user;
    }

    bool IAuthServices.IsValidEmail(string email)
    {
        throw new NotImplementedException();
    }

}