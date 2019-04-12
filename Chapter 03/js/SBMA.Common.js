var SBMA = SBMA || {}
SBMA.Common = SBMA.Common || {};

/**
 * This method popolates membership start date for today.
 * Executes at page load event.
 * @returns {Void}
 */

SBMA.Common.setCurrentDate = function (executionContext) {
    var formContext = executionContext.getFormContext();

    //set membership start date for today
    var membeshipStartDate = new Date();

    formContext.getAttribute('sbma_membershipstartdate').setValue(membeshipStartDate);
}

SBMA.Common.setCurrentDate = function () {
    //Get the current date

    //set membership start date for today
    var renewalDate = Date.now();
    Xrm.Page.getAttribute('sbma_membershipstartdate').setValue(renewalDate);

}

SBMA.Common.setCurrentDate = function (executionContext, fieldName) {
    var formContext = executionContext.getFormContext();

    //set membership start date for today
    var membeshipStartDate = new Date();

    formContext.getAttribute(fieldName).setValue(membeshipStartDate);
}
