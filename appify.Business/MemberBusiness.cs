using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;

namespace appify.Business
{
    public class MemberBusiness : IMemberBusiness
    {
        private IMemberRepository repository;
        public MemberBusiness(IMemberRepository memberRepository)
        {
            this.repository = memberRepository;
        }

        public List<Member> GetAllMembers()
        {
            return repository.GetAllMembers();
        }

        public Member GetMember(long userID)
        {
            return (Member) repository.GetMember(userID);
        }

        public Member IsMemberExist(string emailID, string mobileNo, short memberType, Int64 parentID)
        {
            return repository.IsMemberExist(emailID, mobileNo, memberType, parentID);
        }

        public Member IsCustomerExist(string emailID, string mobileNo, short memberType, Int64 parentID) => repository.IsCustomerExist(emailID,mobileNo, memberType, parentID);

        public CheckOTPSent GetOTPSent(string mobileNo)
        {
            return repository.GetOTPSent(mobileNo);
        }
        public Member RegisterMember(Member member)
        {
            return repository.RegisterMember(member);
        }
        public bool RegisterMobileOTP(RegisterOTP item)
        {
            return repository.RegisterMobileOTP(item);
        }
        public bool CheckMemberDeliveryStatus(long userID)
        {
            return repository.CheckMemberDeliveryStatus(userID);
        }
        public bool UpdateWelcomeEmail(long userID,bool IsWelcomeEmail)
        {
            return repository.UpdateWelcomeEmail(userID,IsWelcomeEmail);
        }
        public bool RemoveMember(long userID)
        {
            return repository.RemoveMember(userID);
        }

        public bool ResetPassword(long userID, string password)
        {
            return repository.ResetPassword(userID, password);
        }


        public bool CheckMemberOnlinePaymentStatus(long userID) { 
            return repository.CheckMemberOnlinePaymentStatus(userID);
        
        }


        public bool CheckMemberDeliveryStatus(long userID) {
            return repository.CheckMemberDeliveryStatus(userID);
        }

        public Member MemberLogIn(string emailID, string mobileNo, string password, Int64 parentID) { 
            return repository.MemberLogIn(emailID,mobileNo,password,parentID);
        }
        public object MemberLogOut(long userID) { 
            return repository.MemberLogOut(userID);
        }

        public MemberDashboardLite MemberDashboard(long userID, DateTime dateFrom, DateTime dateTo)
        {
            return repository.MemberDashboard(userID, dateFrom, dateTo);
        }

        public List<Member> GetAllVendors(int pageNo, int rows)
        {
            throw new NotImplementedException();
        }

        public bool RemoveMemberByMobileNo(string mobileNo, string password) { 
            return repository.RemoveMemberByMobileNo(mobileNo,password);
        }

        public Int32 MemberOrderCount(long userID) { 
            return repository.MemberOrderCount(userID);
        }
        public Int32 VendorOrderCount(long userID)
        {
            return repository.VendorOrderCount(userID);
        }
        public MemberBanner memberBannerAdd(MemberBanner memberBanner)
        {
            return repository.memberBannerAdd(memberBanner);
        }

        public bool memberBannerRemove(long BannerID)
        {
            return repository.memberBannerRemove(BannerID);
        }

        public MemberBanner memberBannerGet(long MemberID)
        {
            return repository.memberBannerGet(MemberID);
        }

        public List<MemberBanner> memberBannerList()
        {
            return repository.memberBannerList();
        }
        public List<MemberBanner> memberBannerListByVendor(long VendorID)
        {
            return repository.memberBannerListByVendor(VendorID);
        }
        public MemberSMSSetting memberSMSSettingGet(long VendorID)
        {
            return repository.memberSMSSettingGet(VendorID);
        }
        public MemberAppLinks getAppLinks(long VendorID)
        {
            return repository.getAppLinks(VendorID);
        }
    }
}