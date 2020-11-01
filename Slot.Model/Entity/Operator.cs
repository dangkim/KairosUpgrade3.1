using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Serializable]
    [Table("Operator")]
    public class Operator : BaseEntity<int>
    {
        [Column(Order = 1)]
        [MaxLength(16)]
        public string Tag { get; set; }

        [Column(Order = 2)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column(Order = 3)]
        public int AuthenticationProviderId { get; set; }

        [NotMapped]
        public AuthProviderId AuthenticationProviderIdentifier
        {
            get
            {
                AuthProviderId authenticationProviderIdentifier;
                return Enum.TryParse(
                    Convert.ToString(this.AuthenticationProviderId),
                    out authenticationProviderIdentifier)
                           ? authenticationProviderIdentifier
                           : AuthProviderId.Unspecified;
            }
        }

        [Column(Order = 4)]
        public int WalletProviderId { get; set; }

        public virtual WalletProvider WalletProvider { get; set; }

        [Column(Order = 5)]
        public int GameSettingGroupId { get; set; }

        public virtual GameSettingGroup GameSettingGroup { get; set; }

        [Column(Order = 6)]
        public int JackpotSettingGroupId { get; set; }

        //public virtual JackpotSettingGroup JackpotSettingGroup { get; set; }

        public bool FunPlayDemo { get; set; }

        [Column(Order = 7)]
        public int FunPlayCurrencyId { get; set; }

        [Column(Order = 8)]
        public decimal FunPlayInitialBalance { get; set; }

        public virtual Currency FunPlayCurrency { get; set; }

        [MaxLength(255)]
        public string AuthenticateURL { get; set; }

        [MaxLength(255)]
        public string AuthenticateParam { get; set; }

        [MaxLength(255)]
        public string LoginURL { get; set; }

        [MaxLength(16)]
        public string OperatorCode { get; set; }

        [MaxLength(255)]
        public string BackOfficeURL { get; set; }

        public bool EnableRollback { get; set; }

        public bool EnableEndGame { get; set; }

        public bool UseRMB { get; set; }

        [DefaultValue("true")]
        public bool FunPlay { get; set; }

        public string ExtraInfo { get; set; }

        public bool EncodeToken { get; set; }

        [MaxLength(255)]
        public string AuthenticateMemberURL { get; set; }

        [DefaultValue("false")]
        public bool HasDownload { get; set; }

        [DefaultValue("false")]
        public bool UseOtp { get; set; }
    }
}