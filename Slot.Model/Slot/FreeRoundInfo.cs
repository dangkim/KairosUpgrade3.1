using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Xml.Serialization;
using Slot.Model.Entity;

namespace Slot.Model
{
    [Serializable]
    [XmlRoot(ElementName = "freeround")]
    public class FreeRoundInfo
    {
        public enum FreeGameState
        {
            [Description(@"N")]
            Initial = 1,
            [Description(@"A")]
            Activate = 2,
            [Description(@"C")]
            Cancelled = 3
        }
        public FreeGameState State { get; set; }
        public int Round { get; set; }
        public int Lines { get; set; }

        public decimal Bet { get; set; }
        public int Multiplier { get; set; }
        public bool IsFinished
        {
            get { return Round <= 0 ; }
        }

        public bool IsExpired { get; set; }

        public PopUpMessageContent PopUpMessageContent { get; set; }

        public Templates Template { get; set; }

        public FreeRoundInfo()
        {
            State = FreeRoundInfo.FreeGameState.Initial;
        }
    }

    [Serializable]
    public class PopUpMessageContent
    {
        public string Gameid { get; set; }
        public string GameName { get; set; }
        public int NoFreeSpin { get; set; }
        public string ExpirationDate { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, string> MessageContent { get; set; }

        public static List<string> LanguageCode
        {
            get { return new List<string>() { "EN", "CH", "VN", "TH", "ID", "KH", "KR", "JP" }; }
        }

        public PopUpMessageContent(Entity.FreeRound freeRound, DateTime expiryDateTime, int noOfFreeRound)
        {
            NoFreeSpin = noOfFreeRound;
            //ExpirationDate = freeRound.EndDateUtc.AddHours(8).ToString(CultureInfo.InvariantCulture);
            ExpirationDate = string.Format("{0} (GMT + 8)", expiryDateTime.AddHours(8).ToString(CultureInfo.InvariantCulture));
            CreateLanguageTitle(freeRound.MessageTitle);
            CreateLanguageMessageContent(freeRound.MessageContent);
        }

        private void CreateLanguageTitle(string text)
        {
            string[] titles = text.Split('|');
            this.Title = new Dictionary<string, string>();

            for (int i = 0; i < titles.Length; i++)
            {
                this.Title.Add(LanguageCode[i], titles[i]);
            }
        }

        private void CreateLanguageMessageContent(string text)
        {
            string[] messagecontents = text.Split('|');
            this.MessageContent = new Dictionary<string, string>();
            for (int i = 0; i < messagecontents.Length; i++)
            {
                this.MessageContent.Add(LanguageCode[i], messagecontents[i]);
            }
        }
    }

    [Serializable]
    public class Templates
    {
        public bool IsDefault { get; set; }
        public PopUpMessageSettings PopUpMessage { get; set; }
        public ReminderSettings Reminder { get; set; }
        public FloatingReminderSettings FloatingReminder { get; set; }
    }

    [Serializable]
    public class PopUpMessageSettings
    {
        public string BackgroundUrl { get; set; }
        public string TitleColor { get; set; }
        public string GameNameColor { get; set; }
        public string ContentColor { get; set; }
        public string NoSpinColor { get; set; }
        public string ExpiryDateColor { get; set; }
        public string ButtonTextColor { get; set; }
    }

    [Serializable]
    public class ReminderSettings
    {
        public string BackgroundUrl { get; set; }
        public string ContentTextColor { get; set; }
        public string ButtonTextColor { get; set; }
    }

    [Serializable]
    public class FloatingReminderSettings
    {
        public string BackgroundUrl { get; set; }
        public string GradientTop { get; set; }
        public string GradientBottom { get; set; }
    }
}