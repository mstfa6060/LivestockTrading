using Common.Contracts.Queue.Models;

namespace Common.Connectors.File.Channels;

public interface IFileChannel
{
    Task<bool> Deliver(FileCreateModelContract fileData);
}