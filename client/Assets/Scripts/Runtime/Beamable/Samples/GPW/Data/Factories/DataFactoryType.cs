
namespace Beamable.Samples.GPW.Data.Factories
{
    /// <summary>
    /// Type which defines process for loaded data content
    /// </summary>
    public enum DataFactoryType 
    {
        None,
        
        // Basic Setup
        // Randomized on the client
        BasicDataFactory,
        
        // Advanced Setup
        // Stored in a Beamable Microservices MicroStorage Database
        MicroStorage
    }
}