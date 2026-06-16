using Parley.Core.DataAccess.Models.Variables;

namespace Parley.Classification;

public interface IWorkflowClassifier
{
    ClassificationContext GetPromptAndSchema(ClassificationOptions options,
                                             List<WorkflowVariable> classificationVariables);
}