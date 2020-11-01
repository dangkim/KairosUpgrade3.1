using Slot.Model.Utility;
using System.Collections.Generic;

namespace Slot.Model.Utilities
{
    public static class DataConverter
    {
        private static readonly Dictionary<GameResultType, string> _gameresulttypedesc = new Dictionary<GameResultType, string>()
        {
            { GameResultType.FreeSpinResult,                "FREE SPIN"             },
            { GameResultType.RevealResult,                  "BONUS"                 },
            { GameResultType.BonusFreeSpinResult,           "BONUS"                 },
            { GameResultType.InstantWinResult,              "INSTANT WIN"           },
            { GameResultType.SpinResult,                    "SPIN"                  },
            { GameResultType.GambleResult,                  "DOUBLE UP"             },
            { GameResultType.DoubleUpResult,                "DOUBLE UP"             },
            { GameResultType.MultiModeResult,               "FREE SPIN"             },
            { GameResultType.CollapsingSpinResult,          "COLLAPSE"              },
            { GameResultType.FreeSpinCollapsingSpinResult,  "FREE SPIN - COLLAPSE"  },
            { GameResultType.RespinResult,                  "RESPIN"             }
        };

        private static readonly Dictionary<PlatformType, string> _platformtypedesc = new Dictionary<PlatformType, string>()
        {
            { PlatformType.None,    "None" },
            { PlatformType.Web,     "Web" },
            { PlatformType.WebLD,   "Web" },
            { PlatformType.Desktop, "Desktop" },
            { PlatformType.Mobile,  "Mobile" },
            { PlatformType.Mini,    "Mini" },
        };

        private static readonly Dictionary<PlatformGroup, List<int>> _mapplatformgroup = new Dictionary<PlatformGroup, List<int>>()
        {
            { PlatformGroup.All,    new List<int> { (int)PlatformType.Web, (int)PlatformType.WebLD, (int)PlatformType.Mobile, (int)PlatformType.Desktop, (int)PlatformType.Mini } },
            { PlatformGroup.Web,    new List<int> { (int)PlatformType.Web, (int)PlatformType.WebLD } },
            { PlatformGroup.WebSD,  new List<int> { (int)PlatformType.Web  } },
            { PlatformGroup.WebLD,  new List<int> { (int)PlatformType.WebLD } },
            { PlatformGroup.Mobile, new List<int> { (int)PlatformType.Mobile } },
            { PlatformGroup.Mini,   new List<int> { (int)PlatformType.Mini } },
            { PlatformGroup.Download, new List<int> { (int)PlatformType.Desktop } }
        };

        public static string Description(GameResultType gameResultType)
        {
            string result;
            if (_gameresulttypedesc.TryGetValue(gameResultType, out result)) return result;

            return string.Empty;
        }

        public static string Description(PlatformType platformType)
        {
            string result;
            if (_platformtypedesc.TryGetValue(platformType, out result)) return result;

            return string.Empty;
        }

        public static string ToCommaDelimited(PlatformGroup platformGroup)
        {
            List<int> result;
            if (_mapplatformgroup.TryGetValue(platformGroup, out result)) return result.ToCommaDelimitedString();

            return string.Empty;
        }
    }
}
