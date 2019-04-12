using log4net;
using Microsoft.Xrm.Sdk;
using SBMA.ServiceProcessor.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SBMA.ServiceProcessor.BusinessProcessLayer
{
    public class SubscriptionProcessor
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void CreateRenewalSubscriptionProcess(IOrganizationService organizationService)
        {
            CrmServiceContext crmServiceContext = new CrmServiceContext(organizationService);
            SubscriptionDataAccess subscriptionDataAccess = new SubscriptionDataAccess();

            // Get the list of accounts that subscription is expiring in 30 days
            log.Info("Get members renewing in 30 days...");
            List<Account> accountsList = subscriptionDataAccess.GetMembersExpiringIn30Days(crmServiceContext);

            // Execute only if there are any reocrds, else do nothing
            if(accountsList.Count > 0)
            {
                log.Info("Processing "+accountsList.Count+" members...");
                for (int i = 0; i < accountsList.Count; i++)
                {
                    // For each account check whether there are any unpaid subscriptions for the period. Create only if there are no unpaid subscriptions
                    log.Info("Check for unpaid member susbcriptions for the period");
                    List<sbma_membersubscription> membersubscriptions = subscriptionDataAccess.GetUnpaidSubscriptions(crmServiceContext, accountsList[i].AccountId.Value);
                    if (membersubscriptions.Count == 0)
                    {
                        log.Info("Creating member subscription for member: " + accountsList[i].AccountId.Value);
                        sbma_membershiptype membershiptype = subscriptionDataAccess.GetMembershipTypeOfMember(crmServiceContext, accountsList[i].sbma_MembershipTypeId.Id);
                        //Set the Membershipcription Properties
                        sbma_membersubscription membersubscription = new sbma_membersubscription
                        {
                            //Se the entity reference with Member record
                            sbma_MemberId = new EntityReference(accountsList[i].LogicalName, accountsList[i].AccountId.Value),
                            //Set the entity reference with Membership Type record
                            sbma_MembershipTypeId = new EntityReference(membershiptype.LogicalName,
                                                                    membershiptype.sbma_membershiptypeId.Value),
                            //Set the subscription due date
                            sbma_SubscriptionDueDate = DateTime.Now.AddDays(7.0),
                            //Set the Subscription status to pending
                            sbma_SubscriptionStatus = new OptionSetValue(
                                                                    (int)sbma_membersubscription_sbma_SubscriptionStatus.Pending)
                        };

                        // Create the subscription
                        subscriptionDataAccess.CreateRenewalMemberSubscription(organizationService, membersubscription);
                        log.Info("Subscription created successfully.");
                    }
                }

            }

        }
    }
}
