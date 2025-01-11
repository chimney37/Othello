# Amazon DynamoDB Othello Game API Serverless Application Project

This project consists of:
* serverless.template - an AWS CloudFormation Serverless Application Model template file for declaring your Serverless functions and other AWS resources
* Functions.cs - class file containing the C# methods mapped to the Serverless functions declared in the template file
* OthelloServerless.cs - file containing a C# class representing an Othello Game in the DynamoDB table
* aws-lambda-tools-defaults.json - default argument settings for use with Visual Studio and command line deployment tools for AWS

The generated project contains a AWS cloudformation template for a simple web API for managing multiple Othello games with the game data stored in a DynamoDB table. 
The Othello Game Control API functions are hosted as a set of AWS Lambda functions that will be exposed through Amazon API Gateway as HTTP operations.

## steps to follow from Visual Studio:

To deploy the Serverless application, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view deployed application open the Stack View window by double-clicking the stack name shown beneath the AWS CloudFormation node in the AWS Explorer tree. The Stack View also displays the root URL to your published application.

## steps to follow from the command line:

deploy application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line (Developer Command Prompt for VS 20XX).

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

#steps to follow from postman (from debugging)
refer to OthelloServerless.postman_collection.json on how to use the REST API, including request bodies, query string parameters and path parameters

#steps to follow from OthelloServerlessConsole
this is a console-based skeleton game application that makes use of the REST APIs.

#What is the game design?
Architecture:
    <game client> <---> <API gateway> <---> <Lambda> <---> <DynamoDB>
The game is managed through a list of lambda functions similar to those that we see in the OthelloAdapter project in the top-level solution.

To illustrate how the design works, the game is instantiated by creating a new game by a lambda function. 
The calls to lambda is made through API gateway that supports Representational State Tranfer(REST), which supports a number of HTTP methods and resources.
The lambda function instantiates a a game using OthelloAdapter.
The lambda function then writes to DynamoDB the game representation that stores a base 64 string encoded binary representation of the Othello object, which
contains everything there is to know about a given Othello game, such as the state of the game.

On an action in the client, e.g. a move made in Othello to place an Othello token, the client makes a call through the API Gateway to call lambda function,
passing move coordinate information like (x, y) and game ID. The lambda reads from the DynamoDB using the game ID as a key, and fetches the corresponding binary game representation.
The lambda function then re-creates the game state from the binary information, and uses the OthelloAdapter interface to execute the move through a method.
If the move is valid, the changed game representation is re-written to the DynamoDB.

A typical game loop design that goes like:
<update> ---> <render> ---> <process key input> ---> <process states>

There will be a REST call for an <update> step in the loop, and there will be 1 or more REST calls in the "process states" step.
Specifically to the OthelloServerlessConsole application, the game loop waits on the <process key input>, so the number of REST calls made is the same rate as a human player will make a move.

In other game loops, this may require a different design since the game loop may operate at a much higher frequency.

#What are some design challanges and how to solve it?
* Lambda cold start problem causes slow responses for the first time the APIs are called after a idle time. E.g. a non-VPC function instance is kept warm for 5 minutes at some point, after which the ephemeral container closes. 
The problem is aggravated using .NET Core used in Lambda because at startup the JIT compiler, which is a process to convert machine agnostic assemblies to machine specific ones.
AWS X-ray analytics indicate that GetGamesAsync, a function that gets all the list of games, on a cold start condition, took 8 seconds to execute on a lambda with memory size 256MB.
the problem is 2 fold because of 1) container instance start up time; 2) .NET JIT time. 
 * While not a complete fix, increasing the memory size to 3008 reduced cold start initiation time to 1.6 seconds.
 * Also, the access to DynamoDB from lambda is uncached, which adds to the latency. DAX: is an in-memory cache of DynamoDB that I failed to implement because there are no specific documents that describe how this should be implemented with .NET using lambda.
* API gateway calls cannot exceed 29 seconds of maximum execution time. Previously, a long running AI calculation using AlphaBeta pruning can take a few minutes depending on the depth, and values of alpha and beta.
 * A long running routine like the AI moves using AlphaBeta pruning is capped to complete approximately after X seconds. In the initial version, X is 1 seconds.
* Design for data transfer for this game design wasn't clear in the beginning about what lambda functions should be supported.
 * It was only clear after a prototype that the only thing that matters is that the game updates should only be done through a synchronization of the game representation through a lambda function call, given the data on the cloud.
 There isn't a need to get any other data about the game, and the only other set of functions are for the actions that change the game state, e.g. making new game, making moves.
* Cliffhangers: now there's no lambda function to wipe the DynamoDB clean, and the only way to do this is to wipe it via the AWS console or AWS explorer.