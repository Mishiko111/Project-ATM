using ATM.Domain.Models;
using System.Reflection.Metadata;

namespace ATM.Services;

public class IdGeneration
{

    private static Random random = new Random();
    public static int GenerateId(List<User>users)
    {
        int id;
        do
        {
            id = random.Next(100000, 999999);
        } while (users.Any(a => a.Id == id));
        return id;


    }
}

