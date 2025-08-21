# PEIAssesment Project

This project automates and validates business processes in Microsoft Dynamics 365 using C# plugins, Azure Functions, and JavaScript form customizations. It also includes a robust unit testing framework to ensure code quality and reliability.

## C# Plugin Files

### pluginCode/PreValidateCContact.cs

- **Purpose:**
  - Validates that a new or updated contact does not have a duplicate email address (`emailaddress1`).
  - Throws an error if a contact with the same email already exists.
  - The plugin is registered to run in the pre-validation stage of the contact creation process.
- **Usage:**
  - Register as a pre-validation plugin on the `contact` entity.

### pluginCode/PostCreateAccount.cs

- **Purpose:**
  - This plugin creates a default contact when a new account is created.
    - It retrieves the account name from the created account entity and uses it to set the last name of the new contact.
    - The first name of the contact is set to "Default".
    - The contact is associated with the account as its parent customer.
    - If the account does not have a name, it uses "Unknown" as the default name.
    - The plugin is registered to run after the account creation operation.
- **Usage:**

  - Register as a post-operation plugin on the Account entity on create message.

  ### Azure Function Files

### PEIMoveOderDetails.cs

- **Purpose:**
  - Function runs when an HTTP request is received.
    This class defines an Azure Function that processes order details.
    - It retrieves order information from the request body, constructs a payload,
    - sends it to an D365 API, and creates a record in a custom table in Dynamics 365.
    - The function expects a JSON payload with CustomerName, OrderTotal, and OrderDate.
    - - It retrieves an access token for Dynamics 365 and creates a record in a custom table named "Order details azure".
    - If successful, it returns a success message; otherwise, it logs the error and returns a failure message.
    - The function is triggered by an HTTP request.

# PEIPollOrders.cs

- **Purpose**

-This class defines an Azure Function that polls for changes in sales orders.

- It retrieves changes using change tracking, processes new or updated orders,
- and sends the details to an external API. In our case its an HTTP trigered function Azure Function PEIMoveOderDetails.
- The function is triggered by a timer and runs every 5 minutes.
- It uses change tracking to efficiently poll for changes in the sales order entity.
- The function expects the environment variables for CRM connection and API URL to be set.
- It logs the changes and sends the order details to an HTTP Azure function.
- If successful, it logs the success; otherwise, it logs the error.

-

## JavaScript Files

### JavaScriptCode/ContactForm.js

- **Purpose:**
  - ValidatePCMFieldsDetails : Function to validate either Email or Phone field is populated or not. If not prevent save and show an alert to user
  - OnChangePCMethod : Function to set Email or Business Phone as required based on Preferred Method of Contact field value
- **Usage:**
  - Attach to the Contact form in Dynamics 365.

### JavaScriptCode/OpportunityForm.js

- **Purpose:**
  - Onload : Function to calculate the Estimated Revenue based on Opportunity Type
  - OnChangeOpportunityType : Function to calculate the Estimated Revenue based on Opportunity Type
- **Usage:**
  - Attach to the Opportunity form in Dynamics 365.

---

### JavaScriptCode/UnitTest/ContactForm.test.js

**Purpose**

- Contact Form Tests
- These tests are designed to validate the functionality of the Contact Form in a Dynamics 365 environment
- They cover the behavior of the Preferred Contact Method field and the validation of email and phone fields
- when saving the form.
- The tests use a shared harness to initialize the form context and execution context for each test case
- ensuring a consistent environment for testing.
- The tests include scenarios for setting required levels based on the Preferred Contact Method,
- validating that at least one contact method is provided and preventing form submission when both email and phone fields are empty.
- Each test case logs its actions and expectations to the console for easier debugging and understanding of the test flow.

### JavaScriptCode/UnitTest/OpportunityForm.test.js

**Purpose**

- Opportunity Form Tests
- These tests cover the JavaScript functionality of the Opportunity form.
- They validate the behavior of the Opportunity Type field and the Estimated Value field.
- The tests ensure that the Estimated Value field is disabled when the Opportunity Type is set to Fixed
- Price and enabled when set to Variable Price.
- They also check that the Estimated Value is calculated correctly based on the Unit Price, Total Units, and Discount
- when the Opportunity Type is Variable Price.
- Each test case initializes the form context and execution context to provide a consistent environment for testing.
- The tests log their actions and expectations to the console for easier debugging and understanding of the test flow.

### JavaScript/D365TestHarness.js

**Purpose**

- initFormContext
- Initialize a fresh fake Xrm environment.
- buildExecutionContext
- Build a fake execution context
- Created two reusable functions for unit tests creating formContext and executionContext objects and properties.

### Plugin/ Unit Test/ FakeXrm.cs

**purpose**

- This class is used to set up a fake XRM context for unit testin purposes.
- It initializes the context, organization service, tracing service, plugin execution context, and workflow context.
- It uses the FakeXrmEasy library to create a simulated environment that mimics Dynamics 365 CRM behavior.
- The class provides properties to access these components, allowing for easy manipulation and testing of CRM.
- It also includes a method to set up the XRM faked context with specific configurations.
- this is a reusable class for the plugin unit testing. one instance created and Plugin execution properties are available to mock the D365 environment

### Plugin/ Unit Test/ PostCAccountTest.cs

**Purpose**

- Unit test cases for plugin PostCAccount.
- CreateContactWithAccountName [Test Method] - Creating contact with Account name as last name. - Checking the last name of contact created is equal to Account name created - and first name is Default. - Also checking the parentcustomerid of contact created is equal to Account id created.

- CreateContactWithOutAccountName[Test Method] - Creating contact with out Account name. - Checking the last name of contact created is equal to Unknown and first name is Default.

### Plugin/ Unit Test/ PreValidateContactTest.cs

- Unit Test case for plugin PreValidateCContact.
- expectPluginException [Test Method] - Creating a contact with an existing email address. - This test checks if the plugin throws an exception when trying to create a contact with an email address that already exists in the system.
- expectPluginExceptionWhiteSpaceEmailProvided [Test method] - Creating a contact with a whitespace email address. - This test checks if the plugin throws an exception when trying to create a contact with an email address that contains only whitespace characters.
- AllowContact [Test method] - Creating a contact with a valid email address. - This test checks if the plugin allows the creation of a contact with a valid email address. - The email address is checked against existing contacts to ensure it is unique.
