<?php

namespace ResDiary\OAuth1;

use OAuth\OAuth1\Service\AbstractService;
use OAuth\OAuth1\Signature\SignatureInterface;
use OAuth\OAuth1\Token\StdOAuth1Token;
use OAuth\Common\Http\Exception\TokenResponseException;
use OAuth\Common\Http\Uri\Uri;
use OAuth\Common\Consumer\CredentialsInterface;
use OAuth\Common\Http\Uri\UriInterface;
use OAuth\Common\Storage\TokenStorageInterface;
use OAuth\Common\Http\Client\ClientInterface;
use OAuth\Common\Exception\Exception;

class ResDiaryOAuthService extends AbstractService
{
    const RESDIARY_SCOPE = "http://app.restaurantdiary.com/WebServices/Epos/v1";

    private $baseOAuthUri;
    private $secondSecret;

    public function __construct(
        CredentialsInterface $credentials,
        ClientInterface $httpClient,
        TokenStorageInterface $storage,
        SignatureInterface $signature,
        UriInterface $baseOAuthUri,
        UriInterface $baseApiUri,
        string $secondSecret
    ) {
        parent::__construct($credentials, $httpClient, $storage, $signature, $baseApiUri);

        $this->baseApiUri = $baseApiUri;
        $this->baseOAuthUri = $baseOAuthUri;
        $this->secondSecret = $secondSecret;
    }

    /**
     * {@inheritdoc}
     */
    public function getRequestTokenEndpoint()
    {
        return $this->baseOAuthUri;
    }

    /**
     * {@inheritdoc}
     */
    public function getAuthorizationEndpoint()
    {
        return $this->baseOAuthUri;
    }

    /**
     * @param string $authorizationEndpoint
     *
     * @throws Exception
     */
    public function setAuthorizationEndpoint($endpoint)
    {
        throw new Exception("Setting the Authorization Endpoint is not supported.");
    }

    /**
     * {@inheritdoc}
     */
    public function getAccessTokenEndpoint()
    {
        return $this->baseOAuthUri;
    }

    /**
     * {@inheritdoc}
     */
    protected function parseRequestTokenResponse($responseBody)
    {
        parse_str($responseBody, $data);

        if (null === $data || !is_array($data)) {
            throw new TokenResponseException('Unable to parse response.');
        }

        return $this->parseAccessTokenResponse($responseBody);
    }

    /**
     * {@inheritdoc}
     */
    protected function parseAccessTokenResponse($responseBody)
    {
        parse_str($responseBody, $data);

        if (null === $data || !is_array($data)) {
            throw new TokenResponseException('Unable to parse response: ' . $responseBody);
        } elseif (isset($data['error'])) {
            throw new TokenResponseException('Error in retrieving token: "' . $data['error'] . '"');
        } elseif (!isset($data["oauth_token"]) || !isset($data["oauth_token_secret"])) {
            throw new TokenResponseException('Invalid response. OAuth Token data not set: ' . $responseBody);
        }

        $token = new StdOAuth1Token();

        $token->setRequestToken($data['oauth_token']);
        $token->setRequestTokenSecret($data['oauth_token_secret']);
        $token->setAccessToken($data['oauth_token']);
        $token->setAccessTokenSecret($data['oauth_token_secret']);

        $token->setEndOfLife(StdOAuth1Token::EOL_NEVER_EXPIRES);
        unset($data['oauth_token'], $data['oauth_token_secret']);
        $token->setExtraParams($data);

        return $token;
    }

    protected function buildAuthorizationHeaderForTokenRequest(array $extraParameters = array())
    {
        return parent::buildAuthorizationHeaderForTokenRequest(
            array(
                'scope' => self::RESDIARY_SCOPE,
                'second_secret' => $this->secondSecret));
    }

    /**
     * Builds the authorization header array.
     *
     * @return array
     */
    protected function getBasicAuthorizationHeaderInfo()
    {
        $headerParameters = parent::getBasicAuthorizationHeaderInfo();

        // Remove the oauth_callback parameter because we don't use a callback
        // OAuth scheme
        unset($headerParameters["oauth_callback"]);

        return $headerParameters;
    }

    /**
     * Overridden so that we can remove the "oauth_verifier" body parameter because
     * it causes our OAuth implementation to fail.
     */
    public function requestAccessToken($token, $verifier, $tokenSecret = null)
    {
        if (is_null($tokenSecret)) {
            $storedRequestToken = $this->storage->retrieveAccessToken($this->service());
            $tokenSecret = $storedRequestToken->getRequestTokenSecret();
        }
        $this->signature->setTokenSecret($tokenSecret);

        $bodyParams = array();

        $authorizationHeader = array(
            'Authorization' => $this->buildAuthorizationHeaderForAPIRequest(
                'POST',
                $this->getAccessTokenEndpoint(),
                $this->storage->retrieveAccessToken($this->service()),
                $bodyParams
            )
        );

        $headers = array_merge($authorizationHeader, $this->getExtraOAuthHeaders());

        $responseBody = $this->httpClient->retrieveResponse($this->getAccessTokenEndpoint(), $bodyParams, $headers);

        $token = $this->parseAccessTokenResponse($responseBody);
        $this->storage->storeAccessToken($this->service(), $token);

        return $token;
    }
}
