<?php

/**
 * Example of retrieving an authentication token of the Twitter service
 *
 * PHP version 5.4
 *
 * @author     David Desberg <david@daviddesberg.com>
 * @author     Pieter Hordijk <info@pieterhordijk.com>
 * @copyright  Copyright (c) 2012 The authors
 * @license    http://www.opensource.org/licenses/mit-license.html  MIT License
 */

use OAuth\Common\Storage\Session;
use OAuth\Common\Consumer\Credentials;
use OAuth\Common\Http\Uri\Uri;
use OAuth\Common\Http\Client\StreamClient;
use OAuth\OAuth1\Signature\Signature;
use ResDiary\OAuth1\ResDiaryOAuthService;

include_once 'vendor/autoload.php';
include_once 'ResDiary/OAuth1/ResDiaryOAuthService.php';

// We need to use a persistent storage to save the token, because oauth1 requires the token secret received before
// the redirect (request token request) in the access token request.
$storage = new Session();

$deploymentUrl = 'http://uk.rdbranch.com';
$consumerKey = 'Key';
$consumerSecret = 'Secret';
$secondSecret = 'SecondSecret';
$restaurantId = 1234;

// Setup the credentials for the requests
$credentials = new Credentials(
    $consumerKey,
    $consumerSecret,
    null
);

$oAuthUri = new Uri("$deploymentUrl/OAuth/V10a");
$baseApiUri = new Uri("$deploymentUrl/WebServices/Epos/v1/");

// Instantiate the twitter service using the credentials, http client and storage mechanism for the token
$httpClient = new StreamClient();

echo "Creating ResDiary Service\n";
$resDiaryService = new ResDiaryOAuthService(
    $credentials,
    $httpClient,
    $storage,
    new Signature($credentials),
    $oAuthUri,
    $baseApiUri,
    $secondSecret);

echo "Requesting Request Token\n";
$token = $resDiaryService->requestRequestToken();

echo "Requesting Access Token\n";
$token = $resDiaryService->requestAccessToken($token->getRequestToken(), null, $token->getRequestTokenSecret());

echo "Received Access Token\n";

echo "Making request for restaurant details\n";
$result = $resDiaryService->request("Restaurant/$restaurantId");

echo print_r($result, true);
