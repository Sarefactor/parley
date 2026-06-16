using Microsoft.AspNetCore.Http;
using Parley.Dtos.Schema;
using Parley.Dtos.Validation;

namespace Parley.Configuration.Exceptions;

public class ParleyValidationException : Exception
{
    public ParleyValidationContextDto Context { get; set; }

    private const int ParleyValidationExceptionStatusCode = StatusCodes.Status422UnprocessableEntity;

    private const string ParleyExceptionMessage = $"Validation of the {nameof(AgentSchemaDto)} failed.";

    public ParleyValidationException(ParleyValidationContextDto context)
        : base(ParleyExceptionMessage)
    {
        Context = context;
    }
}