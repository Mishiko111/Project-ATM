using ATM.Domain.Models;
using ATM.Infrastructure;
using ATM.Services.Interface;

namespace ATM.Services.Repositories;
public  class ATMServices :IATMServices
{
    private readonly string _fileManager;

    public ATMServices(string fileManager)
    {
        _fileManager = fileManager;
    }
    public bool deposit(string email, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be greater than zero.");
        }
        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        Client client = users.OfType<Client>().FirstOrDefault(u => u.Email == email);
        if (client == null)
        {
            throw new ArgumentException("Client not found.");
        }
        client.Balance += amount;
        FileManager.SaveUsersToFile(users, _fileManager);
        return true;
    }

    public bool withdraw(string email, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be greater than zero.");
        }
        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        Client client = users.OfType<Client>().FirstOrDefault(u => u.Email == email);
        if (client == null)
        {
            throw new ArgumentException("Client not found.");
        }
        if (client.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient balance.");
        }
        client.Balance -= amount;
        FileManager.SaveUsersToFile(users, _fileManager);
        return true;
    }


    public bool ProcessLoad(int clientId,bool isApproved)
    {
        List<User> users = FileManager.LoadUsersFromFile(_fileManager);
        Client client = users.OfType<Client>().FirstOrDefault(u => u.Id == clientId);
        if (client == null || client.LoadStatus != "Pending")
        {
            throw new ArgumentException("Invalid load request.");
        }
        if (isApproved)
        {
            client.Balance += client.LoadAmount;
            client.LoadStatus = "Approved";
        }
        else
        {
            client.LoadStatus = "Rejected";
        }
        FileManager.SaveUsersToFile(users, _fileManager);
        return true;
    }


}
