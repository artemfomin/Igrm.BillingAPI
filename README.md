# Igrm.BillingAPI
ATEA home task challenge - Billing API for imaginary company "XYZ Inc."

"XYZ Inc." focus on selling a variety of goods and services online via their E-Shop platform. 
Currently, there is a need for a new billing API that could process orders.

Each incoming order should contain:
* Order number;
*	User id;
*	Payable amount;
*	Payment gateway (identifier to map appropriate payment gateway);
*	Optional description.

When the billing service processes order, it sends the order to an appropriate payment gateway. If the order is processed successfully by the payment gateway, the billing service creates a receipt and returns it in response.

