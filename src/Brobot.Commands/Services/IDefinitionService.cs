using System.Threading.Tasks;

namespace Brobot.Commands.Services
{
    public interface IDefinitionService
    {
        Task<string> GetDefinitions(string word);   
    }
}