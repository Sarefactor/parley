using Microsoft.Extensions.AI;
using Parley.Classification.Responses;
using System.Text;

namespace Parley.Classification;

public class TextClassifier : ITextClassifier
{
    public ClassificationContext GetPromptAndSchemaForChoices(string input,
                                                              List<string> choices)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("The user has been presented with the following choices:");
        stringBuilder.AppendLine($"Choices: {string.Join(", ", choices)}");
        stringBuilder.AppendLine($"Here is their response: {input}");

        stringBuilder.AppendLine($"Return the following properties in JSON format:");
        stringBuilder.AppendLine($"Choice (string): The choice from the supplied list the user wants/has selected.");
        stringBuilder.AppendLine($"IsValid (bool): True if the user selects a choice, false if their choice is unclear.");

        return new ClassificationContext(stringBuilder.ToString(),
                                         "agentChoiceValidator",
                                         AIJsonUtilities.CreateJsonSchema(typeof(ChoiceResponse)));
    }
}