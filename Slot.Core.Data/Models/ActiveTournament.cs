using System;
using Slot.Model;
using Slot.Model.Entity;

namespace Slot.Core.Data.Models
{
    public class ActiveTournament
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int OperatorId { get; set; }

        public int MinHands { get; set; }

        public bool IsAllMembers { get; set; }

        public bool IsGlobal { get; set; }

        public ActiveTournament()
        {
        }

        public ActiveTournament(Tournament tournament, bool isGlobal)
        {
            this.Id = tournament.Id;
            this.OperatorId = tournament.OperatorId;
            this.StartTime = tournament.StartTime;
            this.EndTime = tournament.EndTime;
            this.MinHands = tournament.MinHands;
            this.IsAllMembers = (tournament.Flags & (int)TournamentFlag.AllMembers) != 0;
            this.IsGlobal = isGlobal;
        }
    }
}