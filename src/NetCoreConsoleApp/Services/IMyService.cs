using System.Threading.Tasks;

namespace NetCoreConsoleApp.Services
{
    public interface IMyService
    {
        Task PerformLongTaskAsync();
    }
}