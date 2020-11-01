using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Http.Internal;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model;
using Slot.Model.Entity;

namespace Slot.Simulations {
    internal static class Utilities {
        public static RequestContext<T> CreateRequestContext<T>(this UserGameKey userGameKey, string gameName) {

            var requestContext = new RequestContext<T>("simulation", gameName, PlatformType.None) {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection {},
            };

            var userSession = new UserSession {
                SessionKey = "simulation"
            };
            requestContext.UserSession = userSession;
            return requestContext;
        }
        public static IReadOnlyList<IReadOnlyList<int>> Encoding(string listString) {
            var arr = listString
                .Split(',')
                .Select(int.Parse)
                .ToArray();

            var wheel = new List<List<int>>();
            for (var i = 0; i < 5; ++i) {
                wheel.Add(new List<int> {});
                for (var j = 0; j < 3; ++j)
                    wheel[i].Add(arr[i * 3 + j]);
            }

            return wheel;
        }

        public static Wheel Encoding(string listString, int width, int height) {
            var arr = listString
                .Split(',')
                .Select(int.Parse)
                .ToArray();

            return arr.Encoding(width, height);
        }

        private static Wheel Encoding(this IReadOnlyList<int> arr, int width, int height) {
            var wheel = new Wheel(width, height) { Type = WheelType.Normal };
            for (var i = 0; i < width; ++i)
                for (var j = 0; j < height; ++j)
                    wheel[i].Add(arr[i * height + j]);
            return wheel;
        }

        /// <summary>
        /// Copies the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TSource Copy<TSource>(this TSource source) where TSource : class {
            using(var memoryStream = new MemoryStream()) {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, source);
                memoryStream.Position = 0;
                return binaryFormatter.Deserialize(memoryStream) as TSource;
            }
        }

        public static List<UserGameKey> GenerateUsers(int gid, int numusers, int level) {
            List<UserGameKey> ugk = new List<UserGameKey>();
            for (int i = 1; i < numusers + 1; ++i)
                ugk.Add(new UserGameKey(-i, gid) { Level = level });
            return ugk;
        }

    }
}