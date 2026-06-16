namespace Parley.Classification;

public interface ITextClassifier
{
    ClassificationContext GetPromptAndSchemaForChoices(string input,
                                                       List<string> choices);
}