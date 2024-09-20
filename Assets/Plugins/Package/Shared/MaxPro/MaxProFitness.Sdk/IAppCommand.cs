using UnityEngine.Scripting;
using MaxProFitness.Sdk;

namespace maxprofitness.shared
{
    [Preserve]
    public interface IAppCommand
    {
        CommandType CommandType { get; }

        bool Deserialize(byte[] data);

        string ToHexData();
    }
}
