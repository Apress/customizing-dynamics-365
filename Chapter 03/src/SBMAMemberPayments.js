var SBMA;
(function (SBMA) {
    var CALLING_MODULE_NAME = "";
    var AZURE_BASE_ENDPOINT = "";
    var AZURE_FUNCTION_ENDPOINT = "";
    var FORM_TYPE_UPDATE = 2;
    var MESSAGE_OPS_SUCCESS = "";
    var MESSAGE_OPS_FAILED = "";
    var ProcessPayments = /** @class */ (function () {
        function ProcessPayments() {
            this.handleExportSuccess = function (formContext, response) {
                formContext.ui.setFormNotification(MESSAGE_OPS_SUCCESS, "INFO", null);
            };
            //handle opertation failure
            this.handleExportFailure = function (formContext, response) {
                formContext.ui.setFormNotification(MESSAGE_OPS_FAILED, "ERROR", null);
            };
        }
        ProcessPayments.prototype.processCCPayment = function (executionContext) {
            var formContext = executionContext.getFormContext();
            var formType = formContext.ui.getFormType();
            if (formType == FORM_TYPE_UPDATE) {
                var ccPayment = {
                    CardHolderName: formContext.getAttribute("").getValue(),
                    ExpiryDate: formContext.getAttribute("").getValue()
                };
            }
        };
        ProcessPayments.prototype.executeAzureFunction = function (ccPayment, successHandler, failureHandler) {
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
            };
            //send request
            //req.send(window.JSON().stringify(ccPayment));
        };
        return ProcessPayments;
    }());
    SBMA.ProcessPayments = ProcessPayments;
})(SBMA || (SBMA = {}));
//# sourceMappingURL=SBMAMemberPayments.js.map