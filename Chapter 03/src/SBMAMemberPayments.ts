namespace SBMA
{
    const CALLING_MODULE_NAME = "";
    const AZURE_BASE_ENDPOINT = "";
    const AZURE_FUNCTION_ENDPOINT = "";
    const FORM_TYPE_UPDATE = 2;
    const MESSAGE_OPS_SUCCESS = "";
    const MESSAGE_OPS_FAILED = "";
    export class ProcessPayments
    {        

        public processCCPayment(executionContext: Xrm.Page.EventContext)
        {
            var formContext = executionContext.getFormContext();

            var formType = formContext.ui.getFormType();

            if (formType == FORM_TYPE_UPDATE)
            {
                var ccPayment = {
                    CardHolderName: formContext.getAttribute("").getValue(),
                    ExpiryDate: formContext.getAttribute("").getValue()
                };
            }
        }
        handleExportSuccess = function (formContext, response) {
            formContext.ui.setFormNotification(MESSAGE_OPS_SUCCESS, "INFO", null);
        }

        //handle opertation failure
        handleExportFailure = function (formContext,response) {
            formContext.ui.setFormNotification(MESSAGE_OPS_FAILED, "ERROR", null);
        }

        public executeAzureFunction(ccPayment, successHandler, failureHandler)
        {
            //set Azure Function endpoint
            var endpoint = AZURE_BASE_ENDPOINT + AZURE_FUNCTION_ENDPOINT;

            //define request
            var req = new XMLHttpRequest();
            req.open("POST", endpoint, true);
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");

            req.onreadystatechange = function () {
                if (this.readyState == 4) {
                    req.onreadystatechange = null;

                    if (this.status == 200) {
                        successHandler(JSON.parse(this.response));
                    }
                    else {
                        failureHandler(JSON.parse(this.response).error);
                    }
                }
            }
            //send request
            //req.send(window.JSON().stringify(ccPayment));
        }
    }
}