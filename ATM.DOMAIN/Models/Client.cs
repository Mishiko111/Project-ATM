using System.Diagnostics;

namespace ATM.Domain.Models;

public class Client : User
{
    public decimal Balance { get; set; }
    public string LoadStatus { get; set; }
    public decimal LoadAmount { get; set; }


    public Client(string name, string password, string email, int id, string role, 
        decimal balance, string loadStatus, decimal loadAmount)
        : base(name, password, email, id, role)
    {
        Balance = balance;
        LoadStatus = loadStatus;
        LoadAmount = loadAmount;
    }


}
