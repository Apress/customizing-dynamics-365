(function (SBMA) 
{
 //constants 
  Constants = function () {
	this.CALLING_MODULE = "sbma_MemberPayments.js";
	this.AZURE_BASE_ENDPOINT = 
		"https://sbmacreditcardprocessing.azurewebsites.net/api/";
	this.AZURE_FUNCTION_ENDPOINT = 
	"ProcessCreditCardPayment?code=ISCPrJwBXdpk8bbTI8Y8yBYIbawj9cNPo8Cg/DotanIAnrb9XEepoQ==";
	this.FORM_TYPE_UPDATE = 2;
	this.SUCESS_MESSAGE = "Payment processed successfully.";
	this.FAILURE_MESSAGE = "Payment failed.";

	return this;
  }();

  var formContext = null;

  SBMA.processCCPayment = function (executionContext) {
	formContext = executionContext.getFormContext();

	var formType = formContext.ui.getFormType();
	var fieldValue = formContext.getAttribute("sbma_paymentstatus").getValue();
	alert(fieldValue);

	if (formType === Constants.FORM_TYPE_UPDATE) {
	  if (fieldValue === 646150001) {
	    var ccPayment = {
		  cardHoldersName: 
		    formContext.getAttribute("sbma_nameoncard").getValue(),
		  expiryDate: 
		    formContext.getAttribute("sbma_cardexpirydate").getValue(),
		  cardNumber: 
		    formContext.getAttribute("sbma_ccnumber").getValue(),
		  amount: formContext.getAttribute("sbma_amountdue").getValue()
	  };
	  executeAzureFunction(ccPayment, paymentSuccessHandler, paymentFailureHandler);
	 }
	}
  };

  paymentSuccessHandler = function (response) {
	formContext.ui.setFormNotification(SUCESS_MESSAGE, "INFO", null);
  };

  paymentFailureHandler = function (response) {
	formContext.ui.setFormNotification(FAILURE_MESSAGE, "ERROR", null);
  };

  executeAzureFunction = function (payment, successHandler, failureHandler) {
	var endpoint = Constants.AZURE_BASE_ENDPOINT + Constants.AZURE_FUNCTION_ENDPOINT;
	var req = new XMLHttpRequest();
	req.open("POST", endpoint, true);
	req.setRequestHeader("Accept", "application/json");
	req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
	req.setRequestHeader("OData-MaxVersion", "4.0");
	req.setRequestHeader("OData-Version", "4.0");
	req.onreadystatechange = function () {
		if (this.readyState === 4) {
			req.onreadystatechange = null;

			if (this.status === 200) {
				successHandler(JSON.parse(this.response));
			}
			else {
				failureHandler(JSON.parse(this.response).error);
			}
		}
	};
	req.send(window.JSON.stringify(payment));
};
})(window.AzureServicesLib = window.AzureServicesLib || {});
