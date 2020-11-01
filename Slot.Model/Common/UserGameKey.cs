using System;

namespace Slot.Model
{
    /// <summary>Represents the key used to identify a specific user playing a specific game.
    /// The user id used need to be able to uniquely identify the same user throughout different login session, so the session key is not a suitable candidate.</summary>
    [Serializable]
    public struct UserGameKey
    {
        private int gameId;

        private int userId;

        private bool isBonus;

        private int gameMode;

        private bool isFreeGame;

        private int campaignId;

        private bool frExpired;

        private decimal frWinLose;

        private string funPlayKey;

        public UserGameKey(int userId, int gameId)
            : this()
        {
            this.userId = userId;
            this.gameId = gameId;
            this.Level = 0;
            this.campaignId = 0;
            this.frExpired = true;
        }

        public UserGameKey(int userId, int gameId, int level)
            : this(userId, gameId)
        {
            this.Level = level;
        }


        public UserGameKey(int userId, int gameId, int gameMode, bool isBonus, bool isFreeGame, string funPlayKey) : this()
        {
            this.userId = userId;
            this.gameId = gameId;
            this.Level = 0;
            this.GameMode = gameMode;
            this.isBonus = isBonus;
            this.isFreeGame = isFreeGame;
            this.FunPlayKey = userId <= 0 ? funPlayKey : string.Empty;
        }

        public UserGameKey(int userId, int gameId, int gameMode, bool isBonus, bool isFreeGame, int campaignId, bool frExpired, decimal frWinLose)
            : this()
        {
            this.userId = userId;
            this.gameId = gameId;
            this.Level = 0;
            this.GameMode = gameMode;
            this.isBonus = isBonus;
            this.isFreeGame = isFreeGame;
            this.campaignId = campaignId;
            this.frExpired = frExpired;
            this.frWinLose = frWinLose;
        }

        public UserGameKey(int userId, int gameId, int gameMode, bool isBonus, bool isFreeGame, int campaignId, bool frExpired, decimal frWinLose, string funPlayKey)
            : this()
        {
            this.userId = userId;
            this.gameId = (int)gameId;
            this.Level = 0;
            this.GameMode = gameMode;
            this.isBonus = isBonus;
            this.isFreeGame = isFreeGame;
            this.campaignId = campaignId;
            this.frExpired = frExpired;
            this.frWinLose = frWinLose;
            this.FunPlayKey = userId <= 0 ? funPlayKey : string.Empty;
        }

        public int GameId
        {
            get { return this.gameId; }
            set { this.gameId = value; }
        }

        /// <summary>Gets a value indicating whether is fun play.</summary>
        public bool IsFunPlay
        {
            get { return this.UserId <= 0; }
        }

        public string FunPlayKey
        {
            get { return this.funPlayKey; }
            set { this.funPlayKey = value; }
        }

        /// <summary>Gets or sets the level.</summary>
        public int Level { get; set; }

        /// <summary>Gets or sets the user id.</summary>
        public int UserId
        {
            get { return this.userId; }
            set { this.userId = value; }
        }

        public bool IsBonusAccount
        {
            get { return this.isBonus; }
            set { this.isBonus = value; }
        }

        public bool IsFreeGame
        {
            get { return isFreeGame; }
            set { isFreeGame = value; }
        }

        public int CampaignId
        {
            get { return campaignId; }
            set { campaignId = value; }
        }

        public bool FRExpired
        {
            get { return frExpired; }
            set { frExpired = value; }
        }

        public decimal FRWinLose
        {
            get { return frWinLose; }
            set { frWinLose = value; }
        }

        public int GameMode
        {
            get { return this.gameMode; }
            set { this.gameMode = value; }
        }

        public override string ToString()
        {
            return string.Format("user:{0}|game:{1}", this.UserId, this.GameId);
        }
    }
}