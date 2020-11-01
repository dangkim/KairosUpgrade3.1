using Slot.Model.Entity;
using System;

namespace Slot.Core.Modules.Infrastructure.Models
{
    [Serializable]
    public class UserSession
    {
        public bool IsFunPlay { get; set; }
        public string SessionKey { get; set; }
        public int UserId { get; set; }
        public string ExtraInfo { get; set; }
        public User User { get; set; }
    }
}
