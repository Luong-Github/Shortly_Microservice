namespace Shared.Domain.Abstractions.IEntites
{
    public interface ISortDelete
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedDate { get; set; }
        public void Undo()
        {
            IsDeleted = false;
            DeletedDate = null;
        }
    }
}
