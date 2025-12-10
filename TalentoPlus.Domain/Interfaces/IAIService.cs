using System.Threading.Tasks;

namespace TalentoPlus.Domain.Interfaces;

public interface IAIService
{
    Task<string> ProcessNaturalLanguageQueryAsync(string query);
}
