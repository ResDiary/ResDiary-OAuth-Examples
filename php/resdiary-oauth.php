<?php

include_once "OAuth-PHP/library/OAuthStore.php";
include_once "OAuth-PHP/library/OAuthRequester.php";

// Test of the OAuthStore2Leg

$key = 'F5dA0Vl5Av5TWcEezRR27HDec5nKYlOw'; // fill with your public key
$secret = 'cNmeSkza9cBR1lUCa3QHKMQayEtIbfHC'; // fill with your secret key
$second_secret = '1UJbVLXv7X4GXuui';
$url = "https://uk.rdbranch.com/OAuth/V10a"; // fill with the url for the oauth service

$options = array('consumer_key' => $key, 'consumer_secret' => $secret, 'oauth_version' => '1.0a');
OAuthStore::instance("2Leg", $options);

$method = "GET";
$params = array('scope' => 'http://app.restaurantdiary.com/WebServices/Epos/v1', 'second_secret' => $second_secret);

try
{
	// Obtain a request object for the request we want to make
	$request = new OAuthRequester($url, $method, $params);

	// Sign the request, perform a curl request and return the results,
	// throws OAuthException2 exception on an error
	// $result is an array of the form: array ('code'=>int, 'headers'=>array(), 'body'=>string)
	$result = $request->doRequest();

	$response = $result['body'];
	var_dump($response);
}
catch(OAuthException2 $e)
{
	echo $e->getTraceAsString();
}

?>
