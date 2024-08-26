using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    [Preserve]
    public interface IAppCommand
    {
        CommandType CommandType { get; }

        bool Deserialize(byte[] data);

        string ToHexData();
    }
}
