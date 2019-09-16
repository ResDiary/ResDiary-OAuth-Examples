# EPOSServiceConsumer

This solution contains a series of Unit tests that should help you better understand the EPOS API implementation.
You will require the following information to pass through to environment variables that can be edited in the [App.config](https://github.com/ResDiary/ResDiary-OAuth-Examples/blob/RD-17292/C%23%20Examples/EposServiceConsumer/UnitTests/App.config) file that belongs to the Unit Tests of the solution.

| Variable Names | Description |
| --- | --- |
| `EPOS_CONSUMER_KEY` | A key that is a standard part of oAuth authorization|
| `EPOS_CONSUMER_SECRET`| The first secret for the restaurant you are trying to access.Varies per restaurant  |
|`EPOS_SERVICE_OAUTH_URI` | https://app.rdbranch.com/OAuth/V10a |
|`EPOS_SERVICE_BASE_URI` |http://app.rdbranch.com |
|`EPOS_API_SANDBOX_SECOND_SECRET`|The second secret for the restaurant you are trying to access.Varies per restaurant|
|`EPOS_API_SANDBOX_RESTAURANT_ID`|The Id of the restaurant you are trying to access.|
|`EPOS_API_SANDBOX_SEGMENT_ID`|The id of the service that is currently active for your restaurants|
|`EPOS_API_SANDBOX_SEGMENT_NAME`|The name of the service that is currently active in your restaurant |
|`EPOS_API_SANDBOX_RESTAURANT_AREA_ID`| The area id of an area in your current active segement|
|`EPOS_API_SANDBOX_TABLE_1_ID`|A table id for your active area. Please take not of the number of covers of this table.|
|`EPOS_API_SANDBOX_TABLE_6_ID`|A table id for your active area.|
|`EPOS_API_SANDBOX_TABLE_7_ID`|A table id for your active area. Please take not of the number of covers of this table.|
|`EPOS_API_SANDBOX_API_TEST_CHANNEL_ID`|The id for one of your channels. ONLINE channel by defaults is 3|
|`EPOS_API_SANDBOX_FOREVER_PROMOTION_ID`|The id for one of your active promotions.|

<br/>
<br/>

|Variables for Testing EPOS API WITH BUMPING |Description|
| --- | --- |
|`EPOS_API_SANDBOX_WITH_BUMPING_CONSUMER_SECOND_SECRET`|he second secret for the restaurant you are trying to access.Varies per restaurant|
|`EPOS_API_SANDBOX_WITH_BUMPING_RESTAURANT_ID`|The Id of the restaurant you are trying to access.|
|`EPOS_API_SANDBOX_WITH_BUMPING_SEGMENT_ID`|The id of the service that is currently active for your restaurants|
|`EPOS_API_SANDBOX_WITH_BUMPING_SEGMENT_NAME`|The name of the service that is currently active in your restaurant||
|`EPOS_API_SANDBOX_WITH_BUMPING_RESTAURANT_AREA_ID`|The area id of an area in your current active segement|
|`EPOS_API_SANDBOX_WITH_BUMPING_TABLE_1_ID`|A table id for your active area. Please take not of the number of covers of this table.|
|`EPOS_API_SANDBOX_WITH_BUMPING_TABLE_6_ID`|A table id for your active area.|
|`EPOS_API_SANDBOX_WITH_BUMPING_TABLE_7_ID`|A table id for your active area. Please take not of the number of covers of this table.||
|`EPOS_API_SANDBOX_WITH_BUMPING_API_TEST_CHANNEL_ID`|The id for one of your channels. ONLINE channel by defaults is 3|
|`EPOS_API_SANDBOX_WITH_BUMPING_FOREVER_PROMOTION_ID`|The id for one of your active promotions.|
<br/>
<br/>

| Additional Variables for Unit Testing      |Description|
| --- | --- |
|`Restaurant_Name`| The name of your restaurant as it appears under Settings->Details->MicrositeName|
|`Table_Join_Covers`|The sum of covers for table ids 1+7|
 
 
 
 # Steps on how to retrieve the variables
 
 ## Authorization
 
 The Consumer Key and Consumer Secret identify which API account is making the request, and the Second Secret identifies the restaurant you are accessing. This makes sure that you aren't able to make requests to restaurants you don't have access to.

The Consumer Key and Consumer Secret are standard parts of OAuth, but the Second Secret and Scope are specific to our implementation. 

The above variables should be provided to you by default and are all required for gaining access to the rest of the API.

### Step 1:  Get a Request Token

END POINT:https://app.rdbranch.com/OAuth/V10a?second_secret={{RDEposSecondSecret}}&scope=http://app.restaurantdiary.com/WebServices/Epos/v1

Authorization Parameters:  `EPOS_CONSUMER_KEY` and  `EPOS_CONSUMER_SECRET`

Making this API request will result in generating a request token that will be used in Step 2 to create an Access Token.

### Step 2: Exchange your Request Token for an Access Token.
END POINT:https://app.rdbranch.com/OAuth/V10a

Authorization Parameters:`EPOS_CONSUMER_KEY` , `EPOS_CONSUMER_SECRET` , `REQUEST_TOKEN` (from step 1) and `TOKEN_SECRET`

The access token that gets generated here, will be used to make any other API calls.


### Step 3: Use your Access Token to make your API request




## The Remaining Variables
`EPOS_API_SANDBOX_RESTAURANT_ID`: Your restaurant ID should be known to you and it can also be found when logging on to your diary (and it can also be retrieved in the response body of the Get-Restaurant API CALL). It is part of the URL and it is the number attached at the end of it : 
<br/>
Example:
![Image](https://i.ibb.co/ws629fs/image.png)

 
`EPOS_API_SANDBOX_SEGMENT_ID`  : This can retrieved in the response body of the [Get-Restaurant API CALL](https://login.rdbranch.com/Admin/ApiAccount/EposDocumentation#GET-WebServices-Epos-v1-Restaurant-restaurantId)
`EPOS_API_SANDBOX_SEGMENT_NAME` : This can retrieved in the response body of the [Get-Restaurant API CALL](https://login.rdbranch.com/Admin/ApiAccount/EposDocumentation#GET-WebServices-Epos-v1-Restaurant-restaurantId)
`EPOS_API_SANDBOX_RESTAURANT_AREA_ID` : This can retrieved in the response body of the  [Get-Restaurant API CALL](https://login.rdbranch.com/Admin/ApiAccount/EposDocumentation#GET-WebServices-Epos-v1-Restaurant-restaurantId)
`EPOS_API_SANDBOX_TABLE_6_ID` : This can retrieved in the response body of the [Get-Restaurant API CALL](https://login.rdbranch.com/Admin/ApiAccount/EposDocumentation#GET-WebServices-Epos-v1-Restaurant-restaurantId)
`EPOS_API_SANDBOX_TABLE_7_ID` : This can retrieved in the response body of the  [Get-Restaurant API CALL](https://login.rdbranch.com/Admin/ApiAccount/EposDocumentation#GET-WebServices-Epos-v1-Restaurant-restaurantId)
`EPOS_API_SANDBOX_API_TEST_CHANNEL_ID`: This can retrieved in the response body of the  [Get-Restaurant API CALL](https://login.rdbranch.com/Admin/ApiAccount/EposDocumentation#GET-WebServices-Epos-v1-Restaurant-restaurantId)
`EPOS_API_SANDBOX_FOREVER_PROMOTION_ID`:This can be retrieved in the response body of the [Get Booking API CALL](https://login.resdiary.com/Admin/ApiAccount/EposDocumentation#GET-WebServices-Epos-v1-Restaurant-restaurantId-Booking-bookingId) for a booking that has a promotion attached to it.
