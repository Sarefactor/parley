namespace Parley.Dtos.Validation;

public class ParleyNodeValidationErrorDto
{
    public Guid NodeId { get; set; }
    public List<ParleyNodeValidationErrorDetailDto> ErrorDetails { get; set; } = [];
}