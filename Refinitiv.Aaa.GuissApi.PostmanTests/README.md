# Deployment of the Routing API Postman smoke tests

This directory contains the Postman tests which can be used to smoke test the Routing API as part of a CodeBuild deployment.


## CodeBuild Buildspec Commands

Add the following commands after the terraform deployment commands to run the Postman smoke tests:

      - BASE_URL=$(terraform output $BASE_URL_ENVVAR)
      - echo $BASE_URL
      - cd ../../../../Refinitiv.Aaa.Routing.Api.PostmanTests
      - newman run "$POSTMAN_COLLECTION" -e "$POSTMAN_ENVIRONMENT" --env-var "$TARGET_URL=$BASE_URL" --reporters json --reporter-json-export "$TEST_RESULTS"


## CodeBuild Environment Variables
    POSTMAN_ENVIRONMENT	Environment.postman_environment.json
    POSTMAN_COLLECTION	RoutingApiIntegrationTests.postman_collection.json
    TARGET_URL	        TargetHostURL
    TEST_RESULTS	    TestResults.json
    BASE_URL_ENVVAR	    api_base_url

