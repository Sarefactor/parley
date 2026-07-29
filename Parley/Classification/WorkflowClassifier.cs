using Parley.Core.DataAccess.Models.Variables;
using Parley.Core.Enums;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Parley.Classification;

public class WorkflowClassifier : IWorkflowClassifier
{
    public ClassificationContext GetPromptAndSchema(ClassificationOptions options,
                                                    List<WorkflowVariable> classificationVariables)
        => new(GetPrompt(options, classificationVariables),
               "workflowVariables",
               BuildExtractionJsonSchema(classificationVariables));

    private string GetPrompt(ClassificationOptions options,
                             List<WorkflowVariable> classificationVariables)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Here is some text from a user input: {options.Text}");

        if (options.IsWorkflowClassification)
        {
            stringBuilder.AppendLine($"It has been identified as the following:");
            stringBuilder.AppendLine($"Intent: {options.Intent}");
            stringBuilder.AppendLine($"Description: {options.Description}");
        }

        stringBuilder.AppendLine($"You will breakdown the text and identify the properties below.");
        stringBuilder.AppendLine($"Return the properties in JSON format under a list called \"workflowVariables\".");
        stringBuilder.AppendLine($"For each variable you will return its name and the value you identify.");
        stringBuilder.AppendLine($"To assist with DateTime variables the current local time is: {DateTime.UtcNow.ToLocalTime()}");

        for (var i = 0; i < classificationVariables.Count; i++)
        {
            var variable = classificationVariables[i];

            stringBuilder.AppendLine($"== Variable {i + 1} ==");

            if (variable.Type == VariableDataType.Object)
                ObjectVariableInstructions(stringBuilder, variable);
            else
                VariableInstructions(stringBuilder, variable);
        }

        return stringBuilder.ToString();
    }

    private void VariableInstructions(StringBuilder stringBuilder,
                                      ParleyVariable variable)
    {
        stringBuilder.AppendLine($"Name: {variable.Name}");
        stringBuilder.AppendLine($"Description: {variable.Description}");

        if (variable.IsList)
            stringBuilder.AppendLine("The value for this variable will be a list.");

        stringBuilder.AppendLine($"Type: {variable.Type.ToString()}");
        stringBuilder.AppendLine($"Nullable: {variable.Nullable.ToString()}");
    }

    private void ObjectVariableInstructions(StringBuilder stringBuilder,
                                            WorkflowVariable variable)
    {
        stringBuilder.AppendLine($"Name: {variable.Name}");
        stringBuilder.AppendLine($"Description: {variable.Description}");

        if (variable.IsList)
            stringBuilder.AppendLine($"The value for this variable will be a list. Each item will have the following properties. The name of each property will be given alongside a description of its value.");

        if (!variable.IsList)
            stringBuilder.AppendLine($"The value for this variable will be a dictionary, the name will be given along with a description and type for the value:");

        for (var i = 0; i < variable.ObjectVariables.Count; i++)
        {
            var property = variable.ObjectVariables[i];
            stringBuilder.AppendLine($"== Property {i + 1} ==");
            VariableInstructions(stringBuilder, property);
        }
    }

    private JsonElement BuildExtractionJsonSchema(List<WorkflowVariable> classificationVariables)
        => JsonSerializer.SerializeToElement(BuildExtractionObject(classificationVariables));

    private JsonObject BuildExtractionObject(IReadOnlyCollection<ParleyVariable> variables)
    {
        var props = new JsonObject();
        var required = new JsonArray();

        foreach (var variable in variables)
        {
            props[variable.Name] = BuildVariableSchema(variable);
            required.Add(variable.Name);
        }

        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = props,
            ["required"] = required,
            ["additionalProperties"] = false,
        };
    }

    private JsonObject BuildVariableSchema(ParleyVariable variable)
    {
        JsonObject node = variable.Type switch
        {
            VariableDataType.String => new() { ["type"] = "string" },
            VariableDataType.Integer => new() { ["type"] = "integer" },
            VariableDataType.Bool => new() { ["type"] = "boolean" },
            VariableDataType.DateTime => new() { ["type"] = "string", ["format"] = "date-time" },
            VariableDataType.Object => BuildExtractionObject((variable as WorkflowVariable)?.ObjectVariables ?? []),

            _ => throw new NotSupportedException($"Unhandled {nameof(VariableDataType)}: {variable.Type}")
        };

        if (variable.IsList)
            node = new JsonObject { ["type"] = "array", ["items"] = node };

        if (variable.Nullable
            && node["type"] is JsonValue type
            && type.TryGetValue(out string? value))
        {
            node["type"] = new JsonArray(value, "null");
        }

        return node;
    }
}