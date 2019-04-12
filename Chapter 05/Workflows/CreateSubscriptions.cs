using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Linq;


namespace SBMA.Workflows
{
    [CrmPluginRegistration("CreateSubscriptions", "CreateSubscriptions","","",IsolationModeEnum.Sandbox)]
    public class CreateSubscriptions : CodeActivity 
    {
        [Input("Member")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> InMember { get; set; }

        public override void ExecuteCRMWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext crmWorkflowContext)
        {
            // Retrieve the account number from the input paramenter
            var accountId = this.InMember.Get(context).Id;
            var account = GetAccount(new CrmServiceContext(crmWorkflowContext.OrganizationService), accountId);
            CreateMemberSubscription(crmWorkflowContext.OrganizationService,account);
        }

        /// <summary>
        /// Creating the member subscription
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="member"></param>
        private void CreateMemberSubscription(IOrganizationService organizationService, Account member)
        {
            CrmServiceContext crmServiceContext = new CrmServiceContext(organizationService);
            //Get Membership Type of the member
            sbma_membershiptype membershipType = GetMembershipTypeOfMember(crmServiceContext, member.sbma_MembershipTypeId.Id);

            if (membershipType == null)
                throw new ArgumentNullException("Error in retrieving membership type");

            //Set the Membershipcription Properties
            sbma_membersubscription membersubscription = new sbma_membersubscription
            {
                //Se the entity reference with Member record
                sbma_MemberId = new EntityReference(member.LogicalName, member.AccountId.Value),
                //Set the entity reference with Membership Type record
                sbma_MembershipTypeId = new EntityReference(membershipType.LogicalName,
                                                        membershipType.sbma_membershiptypeId.Value),
                //Set the subscription due date
                sbma_SubscriptionDueDate = DateTime.Now.AddDays(7.0),
                //Set the Subscription status to pending
                sbma_SubscriptionStatus = new OptionSetValue(
                                                        (int)sbma_membersubscription_sbma_SubscriptionStatus.Pending)
            };

            //Calling the organization service to create the new member subscription
            organizationService.Create(membersubscription);
        }

        private Account GetAccount(CrmServiceContext crmServiceContext, Guid memberId)
        {
            var account = (from a in crmServiceContext.CreateQuery<Account>().Where(aa => aa.Id == memberId) select a).FirstOrDefault();
            return account;
        }


        /// <summary>
        /// Get membership type details of the new member
        /// </summary>
        /// <param name="crmServiceContext"></param>
        /// <param name="memberid"></param>
        /// <returns></returns>
        private sbma_membershiptype GetMembershipTypeOfMember(CrmServiceContext crmServiceContext,
                                                                Guid membershipTypeId)
        {
            var membershiptype = (from mt in crmServiceContext.sbma_membershiptypeSet.Where
                                  (m => m.sbma_membershiptypeId == membershipTypeId)
                                  select mt).FirstOrDefault();
            return (sbma_membershiptype)membershiptype;
        }
    }
}
