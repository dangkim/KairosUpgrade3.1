namespace Slot.BackOffice.Data.Queries
{
    public interface IOperatorQuery
    {
        int? OperatorId { get; set; }

        string OperatorTag { get; set; }

        int?[] OperatorIds { get; set; }
    }
}
