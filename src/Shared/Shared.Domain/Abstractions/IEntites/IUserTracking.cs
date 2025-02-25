namespace Shared.Domain.Abstractions.IEntites
{
    public interface IUserTracking
    {
        Guid CreatedBy { get; set; }
        Guid? ModifiedBy { get; set; }
    }
}
