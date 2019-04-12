using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreadTask = System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk;

namespace SBMA.Plugins
{
    [CrmPluginRegistration(MessageNameEnum.Create,
        "account", StageEnum.PostOperation,
        ExecutionModeEnum.Synchronous, "name", "Post-Create Account",
        1000, IsolationModeEnum.Sandbox, Image1Name = "PostImage",
        Image1Type = ImageTypeEnum.PostImage,
        Image1Attributes = "accountid,name,accountnumber,sbma_membershiptypeid")]
    public class MemebershipPlugin : PluginBase
    {
        private readonly string postImageAlias = "PostImage";

        public MemebershipPlugin() : base(typeof(MemebershipPlugin))
        {
        }

        protected override void ExecuteCrmPlugin(LocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
                throw new ArgumentNullException("Local Plugin Context");

            IPluginExecutionContext executionContext = localPluginContext.PluginExecutionContext;
            ITracingService trace = localPluginContext.TracingService;

            //Get the post image
            Account newMembershipEntity = (executionContext.PostEntityImages != null && 
                                           executionContext.PostEntityImages.Contains(this.postImageAlias)) ?
                                           executionContext.PostEntityImages[postImageAlias].ToEntity<Account>() : new Account();

            //Create the membersusbscription
            CreateMemberSubscription(localPluginContext.OrganizationService, newMembershipEntity);

            //Call the action to create the membership subscription payment
            
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
