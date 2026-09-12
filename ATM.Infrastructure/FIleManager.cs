using ATM.Domain.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATM.Infrastructure;

public class FileManager
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public static void SaveUsersToFile(List<User> users, string filePath)
    {
        string json = JsonSerializer.Serialize(users, _options);
        File.WriteAllText(filePath, json);
    }

    public static List<User> LoadUsersFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<User>();
        }

        string json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<User>();
        }

        List<User>? users = JsonSerializer.Deserialize<List<User>>(json, _options);
        return users ?? new List<User>();
    }
}