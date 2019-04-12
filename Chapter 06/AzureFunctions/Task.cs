#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
  log.LogInformation("Credit Card Payment request recieved...");
  string cardHoldersName, expiryDate, cardNumber, amount;

  cardHoldersName = req.Query["cardHoldersName"];
  expiryDate = req.Query["expiryDate"];
  cardNumber = req.Query["cardNumber"];
  amount = req.Query["amount"];

  string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
  dynamic data = JsonConvert.DeserializeObject(requestBody);
  cardHoldersName = cardHoldersName ?? data?.cardHoldersName;
  expiryDate = expiryDate ?? data?.expiryDate;
  cardNumber = cardNumber ?? data?.cardNumber;
  amount = amount ?? data?.amount;

  // Payment Gateway Implementation

  if(cardHoldersName == null)
  {
	return new BadRequestObjectResult
	("Please pass a name on the query string or in the request body");
  }
  return (ActionResult)new OkObjectResult
  ($"Payment processed successfully: {cardHoldersName} - {cardNumber}");
}
