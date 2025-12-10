using System.Threading.Tasks;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
