namespace ATM.Domain.Models
{
    public class Admin : User
    {
        public Admin(string name, string password, string email, int id)
            : base(name, password, email, id, "Admin")
        {
        }


    }
}
