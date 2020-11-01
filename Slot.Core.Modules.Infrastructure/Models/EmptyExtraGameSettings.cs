namespace Slot.Core.Modules.Infrastructure.Models
{
    public class EmptyExtraGameSettings : IExtraGameSettings
    {
        public static readonly IExtraGameSettings Instance = new EmptyExtraGameSettings();
    }
}
