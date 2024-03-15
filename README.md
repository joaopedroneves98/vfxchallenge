# VFX Challenge

## How to run the application
Run 
``
docker-compose up
``
in the root folder of the project to create an instance of the SQL Server database and populate it with some data.

Run the application using the defined configuration 'http'.

Use Swagger, Postman or the HTTP file provided [here](https://github.com/joaopedroneves98/vfxchallenge/blob/master/VFXChallenge.Api/VFXChallenge.Api.http).

## Limitations and Improvements
* Use internal codes to better describe exceptions and return appropriate messages
* Implement the message producer using KafkaFlow
* Create a wrapper for the AlphaVantage client to be able to mock it in testing
* Create Integration Tests
* Remove the API key exposure
* Validation testing
* Use separate entities for EntityFramework instead of Domain Classes to remove public setters used in the PUT flow.

## Third Party libraries used

* https://github.com/kirlut/AlphaVantage.Net
* https://github.com/fluentassertions/fluentassertions
* https://github.com/FluentValidation/FluentValidation
* https://github.com/devlooped/moq
* TODO: https://github.com/Farfetch/kafkaflow
