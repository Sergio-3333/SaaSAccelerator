using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Clients
{
    public int InstallationID { get; set; }

    public int LicenseID { get; set; }

    public string LastAccessed { get; set; }

    public string Created { get; set; }

    public string ContactInfoCompany { get; set; }

    public string ContactInfoContact { get; set; }

    public string ContactInfoPhone { get; set; }

    public string ContactInfoEmail { get; set; }

    public int? ContactInfoCountryID { get; set; }

    public int? UsageCounter { get; set; }

    public string ContactInfoOK { get; set; }

    public int? PartnerID { get; set; }

    public string OWADispName { get; set; }

    public string OWAEmail { get; set; }

    public string OWAEWSURL { get; set; }

    public string OWAEWSUID { get; set; }

    public string OWAEWSPWD { get; set; }

    public int? OWAPersonColor { get; set; }

    public string OWAInitials { get; set; }

    public int? OWAHasImage { get; set; }

    public string OWADispLang { get; set; }

    public int? LicenseType { get; set; }

    public string LastTokenRefresh { get; set; }

    public int? UseEWS { get; set; }

    public int? TestMode { get; set; }

    public int? TimeZone { get; set; }

    public string UserDevice { get; set; }

    public int? TradeID { get; set; }

    public int? FirstEmailSent { get; set; }

    public int? ClientTypeID { get; set; }

    public int? SkipConsent { get; set; }

    public int? TrialDays { get; set; }

    public string CampaignGUID { get; set; }

    public string LastLocCheck { get; set; }

    public int? NewsLetterUsageCounter { get; set; }

    public string ContactInfoTitle { get; set; }

    public string ContactInfoWebSite { get; set; }

    public string ContactInfoAddress { get; set; }

    public string ContactInfoLinkedIn { get; set; }

    public int? FlowUsageCounter { get; set; }

    public int? CJMode { get; set; }

    public string InternalNote { get; set; }

    public int? LastProcessedSkipConsent { get; set; }

    public string InstallDateATC { get; set; }

    public Licenses License { get; set; }

}
