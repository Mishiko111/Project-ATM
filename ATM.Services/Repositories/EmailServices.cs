using System.Net;
using System.Net.Mail;

namespace ATM.Services.Repositories;

public class EmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword; // App Password, არა ჩვეულებრივი Gmail პაროლი

    public EmailService(string senderEmail, string senderPassword)
    {
        _smtpHost = "smtp.gmail.com";
        _smtpPort = 587;
        _senderEmail = "mishikochilachava11@gmail.com";
        _senderPassword = "igii pawr lqpr hrsx";
    }

    public void SendEmail(string toEmail, string subject, string body, bool isHtml = false)
    {
        using (var client = new SmtpClient(_smtpHost, _smtpPort))
        {
            client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
            client.EnableSsl = true;

            var message = new MailMessage
            {
                From = new MailAddress(_senderEmail, "ATM System"),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(toEmail);

            client.Send(message);
        }
    }

    public void SendLoanDecisionEmail(string toEmail, string clientName, decimal amount, bool isApproved, decimal? newBalance = null)
    {
        string subject = isApproved ? "თქვენი სესხი დამტკიცდა ✅" : "თქვენი სესხი უარყოფილია";
        string htmlBody = BuildLoanDecisionHtml(clientName, amount, isApproved, newBalance);

        SendEmail(toEmail, subject, htmlBody, isHtml: true);
    }

    private string BuildLoanDecisionHtml(string clientName, decimal amount, bool isApproved, decimal? newBalance)
    {
        string statusColor = isApproved ? "#16a34a" : "#dc2626";
        string statusText = isApproved ? "დამტკიცებულია" : "უარყოფილია";
        string statusIcon = isApproved ? "✅" : "❌";

        string balanceRow = isApproved && newBalance.HasValue
            ? $@"
                <tr>
                    <td style=""padding:12px 0;color:#6b7280;font-size:14px;"">განახლებული ბალანსი</td>
                    <td style=""padding:12px 0;text-align:right;font-weight:600;color:#111827;font-size:14px;"">{newBalance:C}</td>
                </tr>"
            : "";

        string message = isApproved
            ? "თქვენი სესხის მოთხოვნა დამტკიცდა და თანხა უკვე დაემატა თქვენს ანგარიშს."
            : "სამწუხაროდ, თქვენი სესხის მოთხოვნა ამ ეტაპზე ვერ დაკმაყოფილდა. დამატებითი ინფორმაციისთვის დაგვიკავშირდით.";

        return $@"
<!DOCTYPE html>
<html lang=""ka"">
<head>
<meta charset=""UTF-8"">
</head>
<body style=""margin:0;padding:0;background-color:#f3f4f6;font-family:Segoe UI, Arial, sans-serif;"">
    <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f3f4f6;padding:32px 0;"">
        <tr>
            <td align=""center"">
                <table role=""presentation"" width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 1px 3px rgba(0,0,0,0.1);"">
                    <tr>
                        <td style=""background-color:#111827;padding:24px 32px;"">
                            <span style=""color:#ffffff;font-size:18px;font-weight:700;letter-spacing:0.5px;"">ATM SYSTEM</span>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding:32px;"">
                            <div style=""font-size:32px;margin-bottom:8px;"">{statusIcon}</div>
                            <h1 style=""margin:0 0 4px 0;font-size:20px;color:#111827;"">გამარჯობა, {clientName}</h1>
                            <p style=""margin:0 0 24px 0;color:#6b7280;font-size:14px;line-height:1.5;"">{message}</p>

                            <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-top:1px solid #e5e7eb;border-bottom:1px solid #e5e7eb;"">
                                <tr>
                                    <td style=""padding:12px 0;color:#6b7280;font-size:14px;"">მოთხოვნილი თანხა</td>
                                    <td style=""padding:12px 0;text-align:right;font-weight:600;color:#111827;font-size:14px;"">{amount:C}</td>
                                </tr>
                                <tr>
                                    <td style=""padding:12px 0;color:#6b7280;font-size:14px;"">სტატუსი</td>
                                    <td style=""padding:12px 0;text-align:right;"">
                                        <span style=""background-color:{statusColor}1A;color:{statusColor};font-weight:600;font-size:12px;padding:4px 10px;border-radius:999px;"">{statusText}</span>
                                    </td>
                                </tr>
                                {balanceRow}
                            </table>

                            <p style=""margin:24px 0 0 0;color:#9ca3af;font-size:12px;line-height:1.5;"">
                                ეს არის ავტომატური შეტყობინება ATM System-იდან. კითხვების შემთხვევაში დაგვიკავშირდით.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""background-color:#f9fafb;padding:16px 32px;text-align:center;"">
                            <span style=""color:#9ca3af;font-size:11px;"">© {DateTime.Now.Year} ATM System. ყველა უფლება დაცულია.</span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}