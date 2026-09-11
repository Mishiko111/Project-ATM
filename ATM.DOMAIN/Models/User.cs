namespace ATM.Domain.Models;

  public abstract class User
{
  

    public string Name { get; set; }

    public string Password { get; set; }

    public string Email  { get; set; }

    public int Id { get; set; } 

    public string Role { get; set; }


    protected User(string name, string password, string email, int id, string role)
    {
        Name = name;
        Password = password;
        Email = email;
        Id = id;
        Role = role;
    }

}
