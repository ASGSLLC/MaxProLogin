using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    [Preserve]
    public interface IMaxProCommand
    {
        CommandType CommandType { get; }

        bool Deserialize(byte[] data);

        string ToHexData();
    }
}
