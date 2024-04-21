using appify.models;

namespace appify.Business.Contract
{
    public interface IMemberBusiness
    {
        public List<Member> GetAllMembers();

        public List<Member> GetAllVendors(int pageNo,int rows);

        public Member GetMember(long userID);

        public Member RegisterMember(Member member);

        public bool ResetPassword(long userID, string password);

        public bool RemoveMember(long userID);

        public Member IsMemberExist(string emailID,string mobileNo,short memberType,Int64 parentID);

        public Member MemberLogIn(string emailID, string mobileNo, string password, Int64 parentID);
        public object MemberLogOut(long userID);
        public object MemberDashboard(long userID);

        public bool RemoveMemberByMobileNo(string mobileNo, string password);

        public Int32 MemberOrderCount(long userID);

        public bool CheckMemberOnlinePaymentStatus(long userID);

        public MemberBanner memberBannerAdd(MemberBanner memberBanner);
        public bool memberBannerRemove(long MemberID);
        public MemberBanner memberBannerGet(long MemberID);
        public List<MemberBanner> memberBannerList();
        public List<MemberBanner> memberBannerListByVendor(long VendorID);
    }
}