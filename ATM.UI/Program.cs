using ATM.Domain.Models;
using ATM.Domain.Services;
using ATM.Infrastructure;
using ATM.Services.Repositories;

namespace ATM.UI;

internal class Program
{                 
    private static readonly string _fileManager = "users.json";
    private static AuthService _authService;
    private static ATMServices _atmServices;
    private static EmailService _emailService;

    static void Main(string[] args)
    {
        _authService = new AuthService(_fileManager);
        _atmServices = new ATMServices(_fileManager);

       
        _emailService = new EmailService("mishikochilachava11@gmail.com", "igii pawr lqpr hrsx");

        AdminLogin();
        Console.WriteLine("Welcome to the ATM System!");
        while (true)
        {
            Console.WriteLine("\nPlease select an option:");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Register();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    Console.WriteLine("Exiting the application.");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void Register()
    {
        Console.WriteLine("Enter your name:");
        string name = Console.ReadLine();
        Console.WriteLine("Enter your password:");
        string password = Console.ReadLine();
        Console.WriteLine("Enter your email:");
        string email = Console.ReadLine();
        try
        {
            _authService.RegisterClinet(name, password, email);
            Console.WriteLine("Registration successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void Login()
    {
        Console.WriteLine("Enter your email:");
        string email = Console.ReadLine();
        Console.WriteLine("Enter your password:");
        string password = Console.ReadLine();
        try
        {
            List<User> users = FileManager.LoadUsersFromFile(_fileManager);
            User user = users.FirstOrDefault(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new ArgumentException("Invalid email or password.");
            }

            Console.Clear(); // ძველი ჩანაწერების წაშლა ლოგინის შემდეგ
            Console.WriteLine($"Login successful! Welcome, {user.Name}.");

            if (user is Admin admin)
            {
                AdminMenu(admin);
            }
            else if (user is Client client)
            {
                ClientMenu(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

   
    static Client GetFreshClient(string email)
    {
        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        return users.OfType<Client>().FirstOrDefault(u => u.Email == email);
    }

    static void ClientMenu(Client client)
    {
        while (true)
        {
            Client fresh = GetFreshClient(client.Email) ?? client;

            Console.WriteLine("\nClient Menu:");
            Console.WriteLine($"Current Balance: {fresh.Balance:C}");
            Console.WriteLine("1. Deposit");
            Console.WriteLine("2. Withdraw");
            Console.WriteLine("3. Check Balance");
            Console.WriteLine("4. Request Loan");
            Console.WriteLine("5. Logout");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Deposit(fresh);
                    break;
                case "2":
                    Withdraw(fresh);
                    break;
                case "3":
                    CheckBalance(fresh);
                    break;
                case "4":
                    RequestLoan(fresh);
                    break;
                case "5":
                    Console.WriteLine("Logging out.");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void Deposit(Client client)
    {
        Console.WriteLine("Enter the amount to deposit:");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            try
            {
                _atmServices.deposit(client.Email, amount);
                Client updated = GetFreshClient(client.Email);
                Console.WriteLine($"Successfully deposited {amount:C}.");
                Console.WriteLine($"New Balance: {updated.Balance:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
        }
    }

    static void Withdraw(Client client)
    {
        Console.WriteLine("Enter the amount to withdraw:");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            try
            {
                _atmServices.withdraw(client.Email, amount);
                Client updated = GetFreshClient(client.Email);
                Console.WriteLine($"Successfully withdrew {amount:C}.");
                Console.WriteLine($"New Balance: {updated.Balance:C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
        }
    }

    static void CheckBalance(Client client)
    {
        Client fresh = GetFreshClient(client.Email) ?? client;
        Console.WriteLine($"Your current balance is: {fresh.Balance:C}");
    }

    static void RequestLoan(Client client)
    {
        Console.WriteLine("Enter the loan amount you want to request:");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return;
        }

        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        Client target = users.OfType<Client>().FirstOrDefault(u => u.Email == client.Email);

        if (target == null)
        {
            Console.WriteLine("Error: client not found.");
            return;
        }

        if (target.LoadStatus == "Pending")
        {
            Console.WriteLine("You already have a pending loan request.");
            return;
        }

        target.LoadAmount = amount;
        target.LoadStatus = "Pending";

        FileManager.SaveUsersToFile(users, _fileManager);
        Console.WriteLine($"Loan request for {amount:C} submitted. Status: Pending admin approval.");
    }

    static void AdminMenu(Admin admin)
    {
        while (true)
        {
            Console.WriteLine("\nAdmin Menu:");
            Console.WriteLine("1. View all users");
            Console.WriteLine("2. View pending loan requests");
            Console.WriteLine("3. Approve / Reject a loan request");
            Console.WriteLine("4. Logout");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ViewAllUsers();
                    break;
                case "2":
                    ViewPendingLoans();
                    break;
                case "3":
                    ReviewLoanRequest();
                    break;
                case "4":
                    Console.WriteLine("Logging out.");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void AdminLogin()
    {
        var user = FileManager.LoadUsersFromFile(_fileManager);
        if (!user.Any(u => u.Role == "Admin"))
        {
            string defaultAdminPassword = BCrypt.Net.BCrypt.HashPassword("admin1");
            Admin admin = new Admin("Admin", defaultAdminPassword, "admin@gmail.com", 99999);
            user.Add(admin);
            FileManager.SaveUsersToFile(user, _fileManager);
        }
    }

    static void ViewAllUsers()
    {
        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        foreach (var user in users)
        {
            if (user is Client client)
            {
                Console.WriteLine($"[Client] {client.Name}, {client.Email}, Balance: {client.Balance:C}");
            }
            else if (user is Admin admin)
            {
                Console.WriteLine($"[Admin] {admin.Name}, {admin.Email}");
            }
        }
    }

    static void ViewPendingLoans()
    {
        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        var pending = users.OfType<Client>().Where(c => c.LoadStatus == "Pending").ToList();

        if (!pending.Any())
        {
            Console.WriteLine("No pending loan requests.");
            return;
        }

        Console.WriteLine("Pending loan requests:");
        foreach (var client in pending)
        {
            Console.WriteLine($"- {client.Name} ({client.Email}): {client.LoadAmount:C}");
        }
    }

    static void ReviewLoanRequest()
    {
        Console.WriteLine("Enter the client's email:");
        string email = Console.ReadLine();

        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        Client client = users.OfType<Client>().FirstOrDefault(c => c.Email == email);

        if (client == null)
        {
            Console.WriteLine("Client not found.");
            return;
        }

        if (client.LoadStatus != "Pending")
        {
            Console.WriteLine("This client has no pending loan request.");
            return;
        }

        Console.WriteLine($"Loan request: {client.LoadAmount:C}. Approve? (y/n)");
        string answer = Console.ReadLine();
        bool isApproved = answer?.Trim().ToLower() == "y";
        decimal requestedAmount = client.LoadAmount;

        try
        {
            _atmServices.ProcessLoad(client.Id, isApproved);

            if (isApproved)
            {
                Client updated = GetFreshClient(client.Email);
                Console.WriteLine($"Loan approved. New balance: {updated.Balance:C}");
            }
            else
            {
                Console.WriteLine("Loan rejected.");
            }

            // ემეილის გაგზავნა კლიენტისთვის დადასტურებით (HTML დიზაინით)
            try
            {
                decimal? balanceForEmail = isApproved
                    ? GetFreshClient(client.Email)?.Balance
                    : null;

                _emailService.SendLoanDecisionEmail(client.Email, client.Name, requestedAmount, isApproved, balanceForEmail);
                Console.WriteLine("Confirmation email sent to client.");
            }
            catch (Exception mailEx)
            {
                Console.WriteLine($"Loan processed, but failed to send email: {mailEx.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}