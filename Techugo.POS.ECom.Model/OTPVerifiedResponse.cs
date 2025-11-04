
using System.Text.Json.Serialization;


namespace Techugo.POS.ECom.Model
{
    public class OTPVerifiedResponse : BaseResponse
    {

        [JsonPropertyName("data")]
        public OTPVerifiedData Data { get; set; }
    }

    public class OTPVerifiedData
    {
        public string BranchID { get; set; }
        public int BrandID { get; set; }
        public string OwnerName { get; set; }
        public string CustomerCode { get; set; }
        public string Branch { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string OTPSentOn { get; set; }
        public string OTP { get; set; }
        public int StateID { get; set; }
        public string Address { get; set; }
        public string Landmark { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string PostalCode { get; set; }
        public string MID { get; set; }
        public string MKey { get; set; }
        public string AppID { get; set; }
        public string ConcProp { get; set; }
        public string CustCNO { get; set; }
        public string POSNo { get; set; }
        public string CustomeCodeSFA { get; set; }
        public string PayerCode { get; set; }
        public string IGSTSale { get; set; }
        public int? TotalLimit { get; set; }
        public int? CustomerLimit { get; set; }
        public bool IsFreezed { get; set; }
        public int MaxCreditDays { get; set; }
        public bool IsCreditEnabled { get; set; }
        public int? BalanceLimit { get; set; }
        public string GSTIN { get; set; }
        public string GSTDocUrl { get; set; }
        public string FSSAILicNo { get; set; }
        public string FSSAILicDocUrl { get; set; }
        public int FreeDeliveryAbove { get; set; }
        public string Status { get; set; }
        public bool ReviewedRequest { get; set; }
        public bool IsReviewed { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string SlotType { get; set; }
        public int? ETA { get; set; }
        public string ETARemark { get; set; }
        public string OnBoardingDate { get; set; }
        public string IsActive { get; set; }
        public string ApplicationSubmittedOn { get; set; }
        public string ApplicationApprovedOn { get; set; }
        public string createdAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
