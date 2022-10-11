# Gunslinger Templating Engine - CLI Version

The Gunslinger templating engine is built on top of the [Handlebars](https://github.com/Handlebars-Net/Handlebars.Net) templating system, which is an enhancement of the Logic-less [Mustache](https://mustache.github.io/) templating system. 

This is the OpenAPI version of Gunslinger, which means it is designed to generate Client SDKs from OpenAPI 3.0.1 schemas.

I've continued in the tradition of naming this tool after a style of facial hair.

![Gunslinger Beard](https://user-images.githubusercontent.com/1778167/169899048-b905a62e-ad65-418b-a64e-7778f2364505.png)

The primary idea is that a json config file and some templates can provide everything you need to generate massive portions of your project in 
a very flexible way.

This version is designed to be installed as a command line tool on the host machine which is probably going to be a developer machine, but could
potentially be a build server.

Use the wiki to see [documentation](https://github.com/donrolling/Gunslinger.Templates/wiki) and explanation of different elements.

# CLI Tool

## Project Setup

These three lines were needed in the csproj:
```
<PackAsTool>true</PackAsTool>
<ToolCommandName>gs</ToolCommandName>
<PackageOutputPath>./nupkg</PackageOutputPath>
```
## Building and deploying

Terminal command must be run in the terminal from the location of the csproj.
I think install works differently if using a non-local nupkg. More on that when I figure it out.

- build solution
- create a nupkg in the PackageOutputPath location 
	`dotnet pack`
- install the tool globally
	`dotnet tool install -g --add-source ./nupkg GunslingerCLI`

**Update the tool**
`dotnet tool update -g --add-source ./nupkg GunslingerCLI`

**Uninstall the tool**
`dotnet tool uninstall GunslingerCLI -g`

## Using Docker for testing

Build from solution root:
`docker build -t drolling/gunslinger-api-tests:latest -f TestApiProject/Dockerfile .`

Push:
`docker push drolling/gunslinger-api-tests:latest`

Run:
`docker run -p 8080:80 -d drolling/gunslinger-api-tests:latest`

[navigate to](http://localhost:8080/swagger/v1/swagger.json)

## Test the Swagger API

[dotnet tool install documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install)

