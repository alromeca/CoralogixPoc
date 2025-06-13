using DiagnosticImagingSystem.Controllers;

namespace GenerateData
{
    internal class Program
    {
        private readonly ILogger<SystemMessagesController> _logger = logger;
        private readonly Guid[] _companyIds =
        {
            Guid.Parse("1e72b35a-3f60-4f33-be4b-ec7e3c84ef4a"),
            Guid.Parse("e393350b-b411-4ea1-b046-8fa4fae2435c"),
            Guid.Parse("8e32c7ea-8c24-435d-a239-4ae56c5eac34"),
            Guid.Parse("17a983ab-e858-46d5-8fce-d97be400096a"),
            Guid.Parse("1fc48c3d-29a8-416f-b7da-ae4977df602c")
        };

        static void Main(string[] args)
        {
            
        }
    }
}
