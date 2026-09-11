using ATM.Domain.Models;
using System.Security.Cryptography.X509Certificates;

namespace ATM.Infrastructure;

public class FIleManager
{
    public static void SaveUsersToFile(List<User> users, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var user in users)
            {
                if (user is Client client)
                {
                    writer.WriteLine($"{client.Name},{client.Password},{client.Email},{client.Id},{client.Role},{client.Balance},{client.LoadStatus},{client.LoadAmount}");

                }
                else if (user is Admin admin)
                {
                    writer.WriteLine($"{admin.Name},{admin.Password},{admin.Email},{admin.Id},{admin.Role}");
                }
            }


        }

    }



    public static List<User> LoadUsersFromFile(string filePath)
    {
        List<User> users = new List<User>();
        if (!File.Exists(filePath))
        {
            return users;
        }
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 5) // Admin
                {
                    string name = parts[0];
                    string password = parts[1];
                    string email = parts[2];
                    int id = int.Parse(parts[3]);
                    string role = parts[4];
                    Admin admin = new Admin(name, password, email, id);
                    users.Add(admin);
                }
                else if (parts.Length == 8) // Client
                {
                    string name = parts[0];
                    string password = parts[1];
                    string email = parts[2];
                    int id = int.Parse(parts[3]);
                    string role = parts[4];
                    decimal balance = decimal.Parse(parts[5]);
                    string loadStatus = parts[6];
                    decimal loadAmount = decimal.Parse(parts[7]);
                    Client client = new Client(name, password, email, id, role, balance, loadStatus, loadAmount);
                    users.Add(client);
                }
            }
        }
        return users;
    }
}
      
        



