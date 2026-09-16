using ATM.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATM.Services.Interface;

internal interface IAuthServices
{
    bool IsValidEmail(string email);

    Client RegisterClient(string name, string password, string email, decimal balance = 0);

   
    User Login(string email, string password);


}
