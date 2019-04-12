namespace SBMA
{
    export class CommonJs
    {
        public setCurrentDate(executionContext: Xrm.Page.EventContext, fieldName: string)
        {
            var formContext = executionContext.getFormContext();

            //Get Current Date
            var currentDate = new Date();

            //Set the current date to the field
            formContext.getAttribute(fieldName).setValue(currentDate);
        }
    }
}