# ResDiary PHP OAuth Example

This example uses https://github.com/Lusitanian/PHPoAuthLib to make the OAuth requests.

The PHP dependencies are installed using [Composer](https://getcomposer.org/). You will either need to install composer yourself, or you can use the Vagrant VM provided which will automatically setup a test environment with composer installed for you. See the main [README.md](../README.md) file for more information on this.

To run the example, edit the ResDiaryOAuthExample.php file to the API URLs and your credentials:

```php
$deploymentUrl = 'http://uk.rdbranch.com';
$consumerKey = 'Key';
$consumerSecret = 'Secret';
$secondSecret = 'SecondSecret';
$restaurantId = 1234;
```

Make sure that the composer dependencies are installed correctly:

```
ubuntu@ubuntu-xenial:/vagrant/php$ php composer.phar install
```

Then just run the example:

```
ubuntu@ubuntu-xenial:/vagrant/php$ php ResDiaryOAuthExample.php
```

The script should authenticate with the API, and then output the restaurant details for the restaurant you have specified.
