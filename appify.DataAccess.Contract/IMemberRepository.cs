using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appify.models;

namespace appify.DataAccess.Contract
{
    public interface IMemberRepository
    {
        public List<Member> GetAllMembers();

        public List<Member> GetAllVendors(int pageNo, int rows);
        public Member GetMember(long userID);
          
        public Member RegisterMember(Member item);
          
        public bool RemoveMember(long userID);
          
        public bool ResetPassword(long userID,string password);
          
        public bool DeleteMember(long userID);
          
        public Member IsMemberExist(string emailID,string mobileNo, short memberType, Int64 parentID);


        public Member MemberLogIn(string emailID, string mobileNo, string password,Int64 parentID);
        public bool MemberLogOut(long userID);

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
