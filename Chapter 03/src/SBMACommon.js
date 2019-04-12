var SBMA;
(function (SBMA) {
    var CommonJs = /** @class */ (function () {
        function CommonJs() {
        }
        CommonJs.prototype.setCurrentDate = function (executionContext, fieldName) {
            var formContext = executionContext.getFormContext();
            //Get Current Date
            var currentDate = new Date();
            //Set the current date to the field
            formContext.getAttribute(fieldName).setValue(currentDate);
        };
        return CommonJs;
    }());
    SBMA.CommonJs = CommonJs;
})(SBMA || (SBMA = {}));
//# sourceMappingURL=SBMACommon.js.map