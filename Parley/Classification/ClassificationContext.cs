using System.Text.Json;

namespace Parley.Classification;

public sealed record ClassificationContext(string Prompt,
                                           string SchemaName,
                                           JsonElement JsonSchema);