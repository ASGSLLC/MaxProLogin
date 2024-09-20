using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    [Preserve]
    public interface IMaxProCommand
    {
        CommandType CommandType { get; }

        bool Deserialize(byte[] data);

        string ToHexData();
    }
}
