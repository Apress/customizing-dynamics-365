function submitApplication()
{
	var actionName = "sbma_SubmitApplication";
	
	// Define the input parameters
	var inParams = {
		"Topic" : ("#Topic"),
		"FisrtName" : ("#FisrtName"),
		"LastName" : ("#LastName"),
		"CompanyName" : ("#CompanyName"),
		"BuisnessPhone" : ("#BuisnessPhone"),
		"Email" : ("#Email"),
		"AnnualRevenue" : ("#AnnualRevenue")
	};
	
	var actionResponse = callCustomAction(actionName, inpParams);
}

//Call the custom action
callCustomAction = function(actionName, inputParameters)
{
	var results = null;
	var orgUrl = Xrm.Page.context.getClientUrl();
	// Web request
	var request = new XMLHttpRequest();
	request.open("POST", orgUrl + actionName,false);
	request.setRequestHeader("Accept", "application/json");
	request.setRequestHeader("Content-Type","application/json; charset-utf-8");
	request.setRequestHeader("OData-MaxVersion", "4.0");
	request.setRequestHeader("OData-Version", "4.0");
	
	request.onreadystatechange = function () {
	if (this.readyState == 4) {
		request.onreadystatechange = null;
		if (this.status == 200) {
			alert("Application submitted successfully.");

		} else {
			var error = JSON.parse(this.response).error;
			alert(error.message);
		}
	};
	
	request.send(window.JSON.stringify(inputParameters);
	return results;
}
