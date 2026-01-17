using Common.Contracts.Queue.Models;

namespace Common.Connectors.File.Channels;

public class WorkerChannel : IFileChannel
{
    public async Task<bool> Deliver(FileCreateModelContract fileData)
    {
        return await Task.FromResult(false);
    }
}