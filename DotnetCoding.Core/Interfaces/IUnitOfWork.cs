

namespace DotnetCoding.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IProductQueueRepository ProductQueues { get; }

        Task<int> Save();
    }
}
