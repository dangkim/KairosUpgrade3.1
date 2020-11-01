using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FourGuardians.Configuration;
using Slot.Games.FourGuardians.Engines;
using Slot.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SpinResult = Slot.Games.FourGuardians.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.FourGuardians
{
    public static class SpinsHelper
    {
        public static void DisplayWheelOnOutput(Wheel wheel)
        {
            for (var heightIndex = 0; heightIndex < wheel.Height; heightIndex++)
            {
                for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
                {
                    var willLineBreak = widthIndex == wheel.Width - 1;
                    Debug.Write($"[{wheel[widthIndex][heightIndex]:D2}]{(willLineBreak ? Environment.NewLine : " ")}");
                }
            }
            Debug.WriteLine(string.Empty);
        }

        public static IReadOnlyList<string> GetWildCoordinates(Wheel wheel, int symbol)
        {
            var coordinateList = new List<string>();

            for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
            {
                for (var heightIndex = 0; heightIndex < wheel.Height; heightIndex++)
                {
                    var currentSymbol = wheel[widthIndex][heightIndex];

                    if (currentSymbol == symbol)
                    {
                        coordinateList.Add($"{widthIndex},{heightIndex}");
                    }
                }
            }

            return coordinateList;
        }

        public static string ToFormattedWheelString(this string wheelString)
        {
            return string.Join(',', wheelString.Split('|'));
        }

        public static SpinResult GenerateSpinResult(int level)
        {
            var config = new Configuration();
            var requestContext = new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = 1,
                    Multiplier = 1
                },
                Platform = PlatformType.All
            };

            return MainGameEngine.CreateSpinResult(level, requestContext, config);
        }

        public static SpinResult GenerateWinningSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win == 0)
                spinResult = GenerateSpinResult(level);

            return spinResult;
        }

        public static SpinResult GenerateNonWinningSpinResult(int level)
        {
            var spinResult = GenerateSpinResult(level);

            while (spinResult.Win > 0)
                spinResult = GenerateSpinResult(level);

            return spinResult;
        }
    }
}
