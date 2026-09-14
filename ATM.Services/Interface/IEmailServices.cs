using ATM.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATM.Services.Interface;

internal interface IEmailServices
{
    void SendEmail(string to, string subject, string body);

    EmailService GetEmailService();

    void SendLoanDecisionEmail(string toEmail, string clientName, decimal amount, bool isApproved, decimal? newBalance = null);

    string BuildLoanDecisionHtml(string clientName, decimal amount, bool isApproved, decimal? newBalance);


}
