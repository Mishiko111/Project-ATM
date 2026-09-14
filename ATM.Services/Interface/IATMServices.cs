using System;
using System.Collections.Generic;
using System.Text;

namespace ATM.Services.Interface;

internal interface IATMServices
{
    bool deposit(string email, decimal amount);
    bool withdraw(string email, decimal amount);

    bool ProcessLoad(int clientId, bool isApproved);

}
