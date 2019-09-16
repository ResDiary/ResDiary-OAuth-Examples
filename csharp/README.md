# EPOSServiceConsumer

This solution contains a series of Unit tests that should help you better understand the EPOS API implementation.
You will require the following information to pass through to environment variables that can be edited in the [App.config](https://github.com/ResDiary/ResDiary-OAuth-Examples/blob/RD-17292/C%23%20Examples/EposServiceConsumer/UnitTests/App.config) file that belongs to the Unit Tests of the solution.

| Variable Names | Description |
| --- | --- |
| `EPOS_CONSUMER_KEY` | A key that is a standard part of oAuth authorization|
| `EPOS_CONSUMER_SECRET`| The first secret for the restaurant you are trying to access , which is unique to the API account that has been set up for you.  |
|`EPOS_SERVICE_OAUTH_URI` | https://app.rdbranch.com/OAuth/V10a |
|`EPOS_SERVICE_BASE_URI` |http://app.rdbranch.com |
|`EPOS_API_SECOND_SECRET`|The second secret for the restaurant you are trying to access.Varies per restaurant|
|`EPOS_API_RESTAURANT_ID`|The ID of the restaurant you are trying to access.|

| Additional Variables for Unit Testing      |Description|
| --- | --- |
|`Restaurant_Name`| The name of your restaurant as it appears under Settings->Details->MicrositeName|

## Steps on how to retrieve the variables

## Authorization

 The Consumer Key and Consumer Secret identify which API account is making the request, and the Second Secret identifies the restaurant you are accessing. This makes sure that you aren't able to make requests to restaurants you don't have access to.

The Consumer Key and Consumer Secret are standard parts of OAuth, but the Second Secret and Scope are specific to our implementation. 

The above variables should be provided to you by default and are all required for gaining access to the rest of the API.

### Step 1:  Get a Request Token

END POINT:https://app.rdbranch.com/OAuth/V10a?second_secret={{RDEposSecondSecret}}&scope=http://app.restaurantdiary.com/WebServices/Epos/v1

Authorization Parameters: `EPOS_CONSUMER_KEY` and `EPOS_CONSUMER_SECRET`

Making this API request will result in generating a request token that will be used in Step 2 to create an Access Token.

If you look at the  [TempMemoryTokenManager.cs](https://github.com/ResDiary/ResDiary-OAuth-Examples/blob/RD-17292-v2/csharp/Helpers/TempMemoryTokenManager.cs), a method (StoreNewRequestToken) has been created that stores a newly generated unauthorized request token, secret, and optional application-specific parameters for later recall.

### Step 2: Exchange your Request Token for an Access Token.

END POINT: https://app.rdbranch.com/OAuth/V10a

Authorization Parameters: `EPOS_CONSUMER_KEY`, `EPOS_CONSUMER_SECRET`, `REQUEST_TOKEN` (from step 1) and `TOKEN_SECRET`

The access token that gets generated here, will be used to make any other API calls.

If you look at the [TempMemoryTokenManager.cs](https://github.com/ResDiary/ResDiary-OAuth-Examples/blob/RD-17292-v2/csharp/Helpers/TempMemoryTokenManager.cs), a method (ExpireRequestTokenAndStoreNewAccessToken) has been created that deletes the request token and its associated secret and stores a new access token and secret. This is where the exchange happens.

## C# Example

Here is a C# example of the Epos Service Client using the above variables:

```csharp
string serviceBase = "EPOS_SERVICE_BASE_URI";
string consumerKey = "EPOS_CONSUMER_KEY";
string consumerSecret = "EPOS_CONSUMER_SECRET";
string secondSecret = "EPOS_API_SECOND_SECRET";
string oAuthEndpoint = "EPOS_SERVICE_OAUTH_URI";


client = new EposServiceClient(serviceBase, consumerKey, consumerSecret, secondSecret, oAuthEndpoint);
```

Looking at the [EposServiceClient.cs](https://github.com/ResDiary/ResDiary-OAuth-Examples/blob/RD-17292-v2/csharp/Helpers/EposServiceClient.cs) file, you can see how the method InvokeForContentType<T> has been structured to exchange the request token with an access one in order to perform each API call.

### Step 3: Use your Access Token to make your API request

## The Remaining Variables

`EPOS_API_RESTAURANT_ID`: Your restaurant ID should be known to you and it can also be found when logging on to your diary (and it can also be retrieved in the response body of the Get-Restaurant API CALL). It is part of the the number at the end of the URL, before any query string parameters:
Example:
![Image](restaurant-id.png)
