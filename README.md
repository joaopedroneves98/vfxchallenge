# VFX Challenge

## How to run the application
Run 
``
docker-compose up
``
in the root folder of the project to create an instance of the SQL Server database and populate it with some data.

Run the application using one of the defined configurations like 'http'.

Use Swagger, Postman or the HTTP file provided in the root folder to test the API.

## Limitations and Improvements
* Use internal codes to better describe exceptions and return appropriate messages
* Implement the message producer using KafkaFlow
* Create a wrapper for the AlphaVantage client to be able to mock it in testing
* Create Integration Tests

## Third Party libraries used

* https://github.com/kirlut/AlphaVantage.Net
* https://github.com/fluentassertions/fluentassertions
* https://github.com/FluentValidation/FluentValidation
* https://github.com/devlooped/moq
* TODO: https://github.com/Farfetch/kafkaflow
