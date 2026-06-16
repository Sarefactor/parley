using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Nodes;

public class NodePosition
{
    public NodePosition(double x,
                        double y)
    {
        X = x;
        Y = y;
    }

    [JsonInclude]
    [JsonPropertyName("x")]
    public double X { get; private set; }

    [JsonInclude]
    [JsonPropertyName("y")]
    public double Y { get; private set; }
}