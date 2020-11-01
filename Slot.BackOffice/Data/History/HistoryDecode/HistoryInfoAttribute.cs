using System;
using static Slot.BackOffice.Data.Enums;

namespace Slot.BackOffice.Data.History.HistoryDecode
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HistoryInfoAttribute : Attribute
    {
        public HistoryInfoAttribute(GameId gameId)
        {
            GameId = gameId;
        }

        public GameId GameId { get; }
    }
}