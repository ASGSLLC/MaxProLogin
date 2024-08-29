using UnityEngine.Scripting;
using MaxProFitness.Sdk;

namespace maxprofitness.login
{
    [Preserve]
    public interface IAppCommand
    {
        CommandType CommandType { get; }

        bool Deserialize(byte[] data);

        string ToHexData();
    }
}
