namespace Slot.BackOffice.Data.Queries
{
    public interface IMemberQuery
    {
        int? MemberId { get; set; }

        string MemberName { get; set; }
    }
}
