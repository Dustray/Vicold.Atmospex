using System.Threading.Tasks;

namespace Vicold.Atmospex.Data;
public interface IDataHubService
{
    void DropFiles(string[] filesPath);
    Task LayerFlipAsync(string layerID, bool isRight);
    Task OrderExcuteAsync(string layerID, string order);
}
