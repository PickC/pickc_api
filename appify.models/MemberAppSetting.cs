using appify.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace appify.models
{
    public partial class MemberAppSetting
    {
        public MemberAppSetting() { }


        public Int64 UserID { get; set; }
        public string AppName { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string AppName1 { get; set; }
        public string AppName2 { get; set; }
        public string AppLogo { get; set; }
        public string AppIcon { get; set; }
        public string AppIconTransparent { get; set; }
        public string AndroidBundleID { get; set; }
        public string AppleBuldleID { get; set; }
        public string AppleAppID { get; set; }
        public string AndroidAppURL { get; set; }
        public string AppleAppURL { get; set; }
        public string FireBaseProjectID { get; set; }
        public string Website { get; set; }
        public string Keywords { get; set; }
        public Int16 DeploymentStatusAndroid { get; set; }
        public Int16 DeploymentStatusApple { get; set; }

        public string? MobileLink { get; set; }
        public string? TabLink { get; set; }
        public string? ImageLink { get; set; }
        public string? KYCLink { get; set; }
        public string? CompanyDescription { get; set; }
        public string? PlaystoreDescription { get; set; }
        public string? AppstoreWords { get; set; }
        public string? Subtitle { get; set; }
        public bool? IsEmailSent { get; set; }
        public string? Comments { get; set; }
        public string? OnboardedBy { get; set; }

        public Int16 CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int16 ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }


    public partial class MemberAppSettingLite
    {
        public MemberAppSettingLite() { }


        public Int64 UserID { get; set; }
        public string AppName { get; set; }
        public string AppName1 { get; set; }
        public string AppName2 { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string PlayStoreID { get; set; }
        public string AppStoreID { get; set; }
        public string AppIcon { get; set; }

    }
    public partial class  MemberAppStatus
    {
        public short DeploymentStatusAndroid { get; set; }
        public short DeploymentStatusApple { get; set; }
        public string AndroidAppURL { get; set; }
        public string AppleAppURL { get; set; }
    }

    public partial class MemberAppSettingSave
    {
        public long VendorID { get; set; }
        public string AppName { get; set; }
        public string AndroidBundleID { get; set; }
        public string AppleBuldleID { get; set; }
        public string AppleAppID { get; set; }
        public string AppLogo { get; set; }
        public string FireBaseProjectID { get; set; }
        public string AppIcon { get; set; }
    }

    public partial class MemberAppSettingUpdate
    {
        public long VendorID { get; set; }
        public string AppName { get; set; }
        public short DeploymentStatusAndroid { get; set; }
        public short DeploymentStatusApple { get; set; }
        public string AndroidAppURL { get; set; }
        public string AppleAppURL { get; set; }
    }
}


