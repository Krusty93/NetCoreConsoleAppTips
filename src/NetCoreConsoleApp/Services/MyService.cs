using System.Threading.Tasks;

namespace NetCoreConsoleApp.Services
{
    public class MyService : IMyService
    {
        public async Task PerformLongTaskAsync()
        {
            await Task.Delay(5000);
        }
    }
}