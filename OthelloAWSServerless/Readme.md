# Amazon DynamoDB Othello Game API Serverless Application Project

This project consists of:
* serverless.template - an AWS CloudFormation Serverless Application Model template file for declaring your Serverless functions and other AWS resources
* Functions.cs - class file containing the C# methods mapped to the Serverless functions declared in the template file
* OthelloServerless.cs - file containing a C# class representing an Othello Game in the DynamoDB table
* aws-lambda-tools-defaults.json - default argument settings for use with Visual Studio and command line deployment tools for AWS

You may also have a test project depending on the options selected.

The generated project contains a Serverless template declaration for a simple web API for managing multiple Othello games with the game data stored in a DynamoDB table. 
The Othello Game Control API functions are hosted as a set of AWS Lambda functions that will be exposed through Amazon API Gateway as HTTP operations.

## steps to follow from Visual Studio:

To deploy Serverless application, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view deployed application open the Stack View window by double-clicking the stack name shown beneath the AWS CloudFormation node in the AWS Explorer tree. The Stack View also displays the root URL to your published application.

## steps to follow from the command line:

deploy application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "OthelloAWSServerless.Tests"
    dotnet test
```

Deploy application
```
    cd "OthelloAWSServerless"
    dotnet lambda deploy-serverless
```

#steps to follow from postman
refer to OthelloServerless.postman_collection.json on how to use the REST API, including request bodies, query string parameters and path parameters
