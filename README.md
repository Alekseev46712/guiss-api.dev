# AAA Guiss API

All changes must be made to the *develop* branch.

## Prerequisites

* [Docker Desktop](https://docs.docker.com/docker-for-windows/install/) - needed to run local development versions of services that the API depends on.
* [SpecFlow plugin for Visual Studio](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowForVisualStudio) - used for developing the integration tests.

**After installing the SpecFlow plugin, you need to make a change to its settings** to prevent spurious error messages.
The errors are caused by trying to use a code generation tool that is backward-compatible with old versions of SpecFlow, but does not work with the current version.

To fix this:
* Go to Tools --> Options --> SpecFlow --> General.
* In the "Legacy" section, set "EnableSpecFlowSingleFileGenerator CustomTool" to false.

*Note:* This setting occasionally reverts spontaneously to the default for no apparent reason. If you see a "SpecFlow code generation error" message,
reapply the change to the settings and restart Visual Studio.

## Before running the API
Ensure there is a .aws folder in your C:\%USERPROFILE%\ directory which contains the following 2 files to allow the AWS.SDK to function correctly:
 - config
 - credentials

Sample config file:
```
[default]
region = eu-west-1
```

Sample credentials file:
```
[default]
aws_access_key_id = abc
aws_secret_access_key = abc
aws_session_token =abc
aws_account_id = 11111111
aws_account_alias = abc
expires = 2019-06-13 12:26:29+00:00
aws_role = abc-Developer
```

## Running the API

Use Docker to set up the services that the API depends upon. Open a command prompt at the root of the solution and run:
```
docker-compose up -d
```

(The `-d` switch means "detach", i.e. run in the background.)

This will start up the following services on your local machine:

* Amazon DynamoDB emulator on port 8000, with an empty table set up.
  * DynamoDB JavaScript Shell at http://localhost:8000/shell/
* An unofficial [AWS Simple Notification Service and Simple Queue Service emulator](https://github.com/p4tin/goaws) on port 4100.

All data is held in-memory and will be lost when the services are stopped.

To stop the services, run:
```
docker-compose down
```

If you want to clear the database and immediately restart it, use:
```
docker-compose restart
```
Since UserAPI is not in place as of now. So used command line to add a deleted user message in queue.

Message in queue can be hardcoded for testing purpose.
```
aws --endpoint-url http://localhost:4100 sqs send-message --queue-url http://sqs.us-east-1.goaws.com:4100/100010001000/local-queue3 --message-body "{\"Type\":\"Notification\",\"Token\":\"\",\"MessageId\":\"edf9bc2d-b01c-43fc-963e-3f4c1e377e68\",\"TopicArn\":\"arn:aws:sns:us-east-1:100010001000:local-topic1\",\"Subject\":\"\",\"Messag
e\":\"{\\\"Id\\\":\\\"John Smith\\\",\\\"CorrelationId\\\":[\\\"0101\\\"]}\",\"Timestamp\":\"2019-07-16T09:12:37Z\",\"SignatureVersion\":\"1\",\"Signature\":\"Waoj9Y7ua/IFgRxseAJcuxFWE4
JJAsSTdVvJhDqqTuLHbq4ymm/iv5nI/RGjFVQaBkrslOwpxblGnEmXETka3jWRL+9Otgx8v34UqZahtW4jWCSX7Nc3xqoSqCmHipQlLA99rcnGXJO++NJwRzjVjHWClL9BCU0wMixgkGRLY3joLDMYpb6giQRQDalQlbKD+wTwmmmQ7V9kooqjzVquxOI+MT3p6ZBbaUaCb0UW94
tFR/sivPf7uAf6O4fp5HP4gG8LplPM4Km/yRvi/QsW9gmZ3RASW9PFDAlGd3MnwKQmeNoOv8/e1HgIpXe8ykUKtWpc1geKmDPrDsFrSHn3iw==\",\"SigningCertURL\":\"http://sqs.goaws.com:4100/SimpleNotificationService/edf9bc2d-b01c-43fc-963
e-3f4c1e377e68.pem\",\"UnsubscribeURL\":\"http://sqs.goaws.com:4100/?Action=Unsubscribe\\u0026SubscriptionArn=arn:aws:sns:us-east-1:100010001000:local-topic1:92692e8a-6b2e-4c8d-b263-1138fad3e6b2\",\"Subscribe
URL\":\"\",\"MessageAttributes\":{\"sender\":{\"Type\":\"StringValue\",\"Value\":\"aaa_guiss_api\"},\"typeName\":{\"Type\":\"StringValue\",\"Value\":\"Refinitiv.Aaa.Guiss.MessageTypes.GuissUserDeletedNotification, Refinitiv.Aaa.Guiss.MessageTypes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\"}}}"
```

## Integration Tests

There are two integration test projects:

* `Refinitiv.Aaa.GuissApi.IntegrationTests` - covering the repository classes only, to ensure that they work correctly with DynamoDB.
* `EndToEndTests` - examining the API's responses to actual HTTP requests.
  * The API is hosted inside the end-to-end tests project, and so the tests are able to start and stop it automatically.

Before running these tests, it is necessary to run `docker-compose up -d` as described above.

The integration tests are marked with an NUnit test category of "Integration" so that they can be run separately from the pure unit tests.

* To run unit tests only: `dotnet test --filter TestCategory!=Integration`
* To run integration tests only: `dotnet test --filter TestCategory=Integration`

### SpecFlow

The end-to-end tests are written using [SpecFlow](https://specflow.org), which expresses the test scenarios in natural English such as:
```
Scenario: When the client attempts to create a group with a duplicate name, then the group is not created.
	Given a group named 'test' exists
	When the client attempts to create a group named 'test'
	Then a BadRequest status code is returned
```

SpecFlow is the .NET port of Cucumber, a behaviour-driven development framework that was originally developed for the Ruby language.

The test scenarios can be found in the `.feature` files. During the build process, the feature files are converted automatically into NUnit tests
(`*.feature.cs`).

The `Steps` folder contains that actual C# code that implements the tests. Each method is annotated with an attribute that SpecFlow matches
against the corresponding English text in the feature file.

The classes in the `Context` folder define data structures for passing information from one step to the next. SpecFlow creates
new instances of these classes for each scenario.

### Testing a deployed instance of the Guiss API

Use the Terraform scripts under `terraform/environments/manual` to deploy the appropriate API version to the integration testing environment.

In addition to the Guiss API and database, the integration test environment has an SQS queue which is subscribed to the Guiss SNS topic. The tests will read from this queue to ensure that the appropriate notifications are being sent.

The Terraform output gives the URLs of the API and lambda name, e.g.

```
template_api_base_url = https://gastl8rb2b.execute-api.us-east-1.amazonaws.com/manual
template_lambda_name = a206104-aaa-template-api-feature-PENTITLE-XXXX
```
The topic and queue are set as lambda environment variables, e.g.

```
AppSettings__Sns__Topics__Guiss = a206104_aaa_api_template_feature-PENTITLE-XXXX
AppSettings__Sqs__QueueName = a206104-aaa-api-feature-PENTITLE-XXXX.fifo
```

Reconfigure the SpecFlow tests to use the deployed instance by editing the `testsettings.json` configuration file in the end-to-end tests project:

* Delete the `SNS.ServiceURL` and `Sqs.ServiceUrl` settings.
* Set the topic name for Guiss under the `Sns.Topics` setting to the lambda environment variable value.
* Set `Sqs.QueueName` equal to the lambda environment variable value.
* Set `GuissApi.BaseURL` equal to the `template_api_base_url` output from Terraform.

Before running the tests, log in with `cloud-tool-fr` so that you have a valid token for accessing the message queue.

When testing is finished, delete the integration test environment using `terraform destroy`. This saves money and also ensures that future integration tests will start from a clean state.

#### Example configuration for integration testing of a deployed API

```json
{
  "AWS": {
    "Profile": "tr-fr-preprod",
    "Region": "us-east-1"
  },
  "Sns": {
    "ServiceUrl": "",
    "Topics": {
      "Guiss": "a206104_aaa_api_template_feature-PENTITLE-XXXX",
    }
  },
  "Sqs": {
    "ServiceUrl": "",
    "QueueName": "a206104-aaa-api-feature-PENTITLE-XXXX.fifo"
  },
  "GuissApi": {
    "BaseURL": "https://gastl8rb2b.execute-api.us-east-1.amazonaws.com/manual"
  }
}
```

#### Configuration for local integration testing

Setting `GuissApi.BaseURL` to an empty string causes the tests to start up their own local instance of the Guiss API.

The `SNS.ServiceURL` and `Sqs.ServiceUrl` settings redirect requests to the local emulator instead of the real cloud.

```json
{
  "AWS": {
    "Profile": "aaa-sdlc-preprod",
    "Region": "us-east-1"
  },
  "Sns": {
    "ServiceUrl": "http://localhost:4100",
    "Topics": {
      "Guiss": "a206104_aaa_api_template"
    }
  },
  "Sqs": {
    "ServiceUrl": "http://localhost:4100",
    "QueueName": "a206104-aaa-api.fifo"
  },
  "GuissApi": {
    "BaseURL": ""
  }
}
```

