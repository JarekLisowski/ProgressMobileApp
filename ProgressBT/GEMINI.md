# About
This is an Android application that reads a text data from REST API and sends it to printer connected via Bluetooth.

# Requirements
1. Application must be executed from web browser Google Chrome. 
To do this, dedicated intent must be created and registered. User will click on a link that should navigate to this application. 
In the link there will be parameter that must be handled and used in API request to get the data to print.
User should see the information that data is ready for print and he must press the button to sent data  to printer.

# Development plan
1. Implement BluetoothHandler responsible for connection with Bluettoth. Status: done.
2. Create API client (class name: APIClient) that sends the request to REST API using POST method and receive data. Data should be stored in program buffer. Status: done. 
3. Create INTENT and handling that gets the parameter from url. In the handler should use API client implemented in point 2 to call the API. Status: in progress.
4. User should be asked if data can be sent to printer via Bluetooth.
5. Send data from buffer to printer.

# Additional info
1. Don't implement any unit tests.
2. Be aware that we are working on Windows and using backslash in path. When you want to change any path in configuration ALWAYS uses double backslash : "\\" instead of "\".

#API
1. Address.
{BASE_ADDRES} - is http or https address of the REST API server.
1.1.  Invoice: {BASE_ADDRES}/api/print/invoice/{id} where id is id of the document to print.
1.2.  Payment: {BASE_ADDRES}/api/print/payment/{id} where id is id of the document to print.
2. Result.
Every request result is a string in JSON format:
{
info: string,
lines:	string[]
}
