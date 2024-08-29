using UnityEngine.Scripting;
using MaxProFitness.Sdk;
namespace maxprofitness.login
{
    [Preserve]
    public interface IMaxProCommand
    {
        CommandType CommandType { get; }

        bool Deserialize(byte[] data);

        string ToHexData();
    }
}
