using System;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Core.Modules.Infrastructure.Models;
using System.Collections.Generic;

namespace Slot.Core.Modules.Infrastructure
{
    public interface IGameModule
    {
        /// <summary>
        /// The ID in database.
        /// </summary>
        int GameId { get; }

        /// <summary>
        /// We serialize the specific bonus object to byte array and store to database.
        /// This method use to dserialize to the exactly bonus object
        /// </summary>
        /// <param name="bonus"></param>
        /// <returns></returns>
        Bonus ConvertToBonus(BonusEntity bonus);

        /// <summary>
        /// Take notes that we always deduct the amount from user's wallet. 
        /// So if the current spin is free, must treat it as bonus. Or return 0 for this method
        /// </summary>
        /// <param name="userGameSpinData"></param>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        decimal CalculateTotalBet(UserGameSpinData userGameSpinData, RequestContext<SpinArgs> requestContext);

        /// <summary>
        /// Generate random non-winning wheel
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        IWheel InitialRandomWheel();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level">RTP Level for the current user</param>
        /// <param name="userGameSpinData">Last spin's data</param>
        /// <returns></returns>
        IExtraGameSettings GetExtraSettings(int level, UserGameSpinData userGameSpinData);

        Result<SpinResult, ErrorCode> ExecuteSpin(int level, UserGameSpinData userGameSpinData, RequestContext<SpinArgs> requestContext);

        /// <summary>
        /// Use to create the bonus object when user spin got bonus
        /// </summary>
        /// <param name="spinResult"></param>
        /// <returns></returns>
        Result<Bonus, ErrorCode> CreateBonus(SpinResult spinResult);

        Result<BonusResult, ErrorCode> ExecuteBonus(int level, BonusEntity bonus, RequestContext<BonusArgs> requestContext);
    }
}
