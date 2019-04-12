using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;


namespace SBMA.Workflows
{
    [CrmPluginRegistration("CreateMemberPayment", "CreateMemberPayment","","",IsolationModeEnum.Sandbox)]
    public class CreateMemberPayment : CodeActivity
    {
        [Input("Member")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> InMember { get; set; }

        [Input("MemberSubscription")]
        [ReferenceTarget("sbma_membersubscription")]
        public InArgument<EntityReference> InMemberSubscription { get; set; }

        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext crmWorkflowContext)
        {
            // Retrieve the account number from the input paramenter
            var accountId = this.InMember.Get(context).Id;

            // Retrieve the membersubscription form the input parameter
            var memberSubscriptionId = this.InMemberSubscription.Get(context).Id;

            CreateMemberPaymentRecord(crmWorkflowContext.OrganizationService, memberSubscriptionId, accountId);
        }

        /// <summary>
        /// Create member payment record
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="memberSubscriptionId"></param>
        /// <param name="accountId"></param>
        private void CreateMemberPaymentRecord(IOrganizationService organizationService, Guid memberSubscriptionId, Guid accountId)
        {
            CrmServiceContext crmServiceContext = new CrmServiceContext(organizationService);
            sbma_membersubscription membersubscriptionlocal = GetSubscription(crmServiceContext, memberSubscriptionId);
            Account membershipLocal = GetMember(crmServiceContext, accountId);

            sbma_memberpayment memberpayment = new sbma_memberpayment()
            {
                sbma_AmountDue = new Money(GetDuePayment(crmServiceContext, membersubscriptionlocal)),
                sbma_PaymentDue = membersubscriptionlocal.sbma_SubscriptionDueDate,
                sbma_PaymentMethod = new OptionSetValue((membershipLocal.sbma_PaymentMethod.Value == (int)Account_sbma_PaymentMethod.CreditCard) ?
                                                            (int)Account_sbma_PaymentMethod.CreditCard : (int)Account_sbma_PaymentMethod.DirectDebit),
                sbma_MemberSubscriptionId = new EntityReference(membersubscriptionlocal.LogicalName, membersubscriptionlocal.Id)
            };
            organizationService.Create(memberpayment);

        }

        /// <summary>
        /// Get Subscription fee
        /// </summary>
        /// <param name="crmServiceContext"></param>
        /// <param name="membersubscription"></param>
        /// <returns></returns>
        private decimal GetDuePayment(CrmServiceContext crmServiceContext, sbma_membersubscription membersubscription)
        {
            var membershipType = (from mt in crmServiceContext.sbma_membershiptypeSet
                                  where mt.sbma_membershiptypeId == membersubscription.sbma_MembershipTypeId.Id
                                  select mt).FirstOrDefault();

            return membershipType.sbma_SubscriptionFee.Value;
        }

        /// <summary>
        /// Get member details
        /// </summary>
        /// <param name="crmServiceContext"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private Account GetMember(CrmServiceContext crmServiceContext, Guid memberId)
        {
            var membership = (from m in crmServiceContext.AccountSet.Where(a => a.AccountId == memberId) select m).FirstOrDefault();
            return membership;
        }

        /// <summary>
        /// Get Subscription Details
        /// </summary>
        /// <param name="serviceContext"></param>
        /// <param name="memberSubscriptionId"></param>
        /// <returns></returns>
        private sbma_membersubscription GetSubscription(CrmServiceContext serviceContext, Guid memberSubscriptionId)
        {
            var memberSubscription = (from sm in serviceContext.sbma_membersubscriptionSet
                                      where sm.sbma_membersubscriptionId == memberSubscriptionId
                                      select sm).FirstOrDefault();
            return memberSubscription;
        }

        private int GetMemberPaymentType(CrmServiceContext crmServiceContext, Guid accountId)
        {
            Account account = (from m in crmServiceContext.AccountSet.Where(aid => aid.AccountId == accountId) select m).FirstOrDefault();
            return account.sbma_PaymentMethod.Value;
        }
    }
}
