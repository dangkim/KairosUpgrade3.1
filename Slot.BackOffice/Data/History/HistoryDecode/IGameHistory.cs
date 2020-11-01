using Slot.Model.Entity;

namespace Slot.BackOffice.Data.History.HistoryDecode
{
    public interface IGameHistory
    {
        string ViewNavigation { get; }
        void AmendSpinHistory(MemberHistoryResult model, GameHistory history);
    }
}
