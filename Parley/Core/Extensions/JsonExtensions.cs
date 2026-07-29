using System.Text.Json.Nodes;

namespace Parley.Core.Extensions;

public static class JsonExtensions
{
    public static bool TryGetNode(this JsonNode root,
                                  bool isTargetNodeAnArray,
                                  out JsonNode? result,
                                  params object?[] path)
    {
        result = null;
        JsonNode? currentNode = root;

        path = path.Where(part => part is not null
                                  && (part is not string text || !string.IsNullOrWhiteSpace(text)))
                   .ToArray();

        for (var i = 0; i < path.Length; i++)
        {
            var segment = path[i];

            switch (segment)
            {
                case string stringProperty when currentNode is JsonObject objT:

                    if (!objT.TryGetPropertyValue(stringProperty, out currentNode))
                        return false;
                    break;

                case int indexProperty when currentNode is JsonArray array:

                    if (isTargetNodeAnArray
                        && i == path.Length - 1)
                    {
                        result = array;
                        return true;
                    }

                    if (indexProperty < 0 || indexProperty >= array.Count)
                        return false;

                    currentNode = array[indexProperty];
                    break;

                default:
                    return false;
            }
        }

        result = currentNode;
        return true;
    }
}