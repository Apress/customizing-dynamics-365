using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SBMA.ServiceProcessor.DataAccessLayer
{
    public class SubscriptionDataAccess
    {
        /// <summary>
        /// Get members renewing in 30 days from today
        /// </summary>
        /// <param name="crmServiceContext"></param>
        /// <returns></returns>
        public List<Account> GetMembersExpiringIn30Days(CrmServiceContext crmServiceContext)
        {
            var accountList = from a in crmServiceContext.AccountSet
                              where a.sbma_MembershipRenewalDate.Value == DateTime.Today.AddDays(30.0)
                              select new SBMA.ServiceProcessor.Account
                              {
                                  AccountId = a.AccountId,
                                  AccountNumber = a.AccountNumber,
                                  sbma_MembershipTypeId = a.sbma_MembershipTypeId
                              };
            return accountList.ToList<Account>();
        }

        /// <summary>
        /// Check for unpaid subscriptions
        /// </summary>
        /// <param name="crmServiceContext"></param>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public List<sbma_membersubscription> GetUnpaidSubscriptions(CrmServiceContext crmServiceContext, Guid accountNumber)
        {
            var subscriptionList = from s in crmServiceContext.sbma_membersubscriptionSet
                                   where s.sbma_MemberId.Id == accountNumber &&
                                         s.sbma_SubscriptionStatus.Value == (int)sbma_membersubscription_sbma_SubscriptionStatus.Pending
                                   select new sbma_membersubscription
                                   {
                                       sbma_membersubscriptionId = s.sbma_membersubscriptionId,
                                       sbma_MemberId = s.sbma_MemberId
                                   };
            return subscriptionList.ToList();
        }

        /// <summary>
        /// Create the member subscription
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="membersubscription"></param>
        public void CreateRenewalMemberSubscription(IOrganizationService organizationService, sbma_membersubscription membersubscription)
        {
            organizationService.Create(membersubscription);
        }

        /// <summary>
        /// Get membership type details of the new member
        /// </summary>
        /// <param name="crmServiceContext"></param>
        /// <param name="memberid"></param>
        /// <returns></returns>
        public sbma_membershiptype GetMembershipTypeOfMember(CrmServiceContext crmServiceContext,
                                                                Guid membershipTypeId)
        {
            var membershiptype = (from mt in crmServiceContext.sbma_membershiptypeSet.Where
                                  (m => m.sbma_membershiptypeId == membershipTypeId)
                                  select mt).FirstOrDefault();
            return (sbma_membershiptype)membershiptype;
        }
    }
}
