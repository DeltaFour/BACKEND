namespace DeltaFour.Application.Dtos.Responses;

public abstract class BaseListResponse<T>
{
    public List<T> Data { get; set; } = new();
}
