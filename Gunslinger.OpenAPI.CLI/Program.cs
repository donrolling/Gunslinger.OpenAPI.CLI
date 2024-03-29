﻿using System.Reflection;
using System.Diagnostics;
using CommandLine;
using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectConfiguration;
using System.Text.Json;
using Options = Domain.Configuration.Options;

var configuration = Bootstrapper.GetHostConfiguration(AppDomain.CurrentDomain.BaseDirectory);
using (var scope = configuration.Host.Services.CreateScope())
{
	var version = GetVersion();
	var services = scope.ServiceProvider;
	var logger = services.GetRequiredService<ILogger<Program>>();
	var generationManager = services.GetRequiredService<IGenerationManager>();
	var commandSettingsResult = GetOptions(args, logger);

	if (commandSettingsResult.Failed)
	{
		logger.LogError(commandSettingsResult.Message);
		return;
	}

	var commandSettings = commandSettingsResult.Result;
	var commandSettingsJson = JsonSerializer.Serialize(commandSettings);
	var configurationMessage = $"Version:{version}{Environment.NewLine}Command Settings:{Environment.NewLine}{commandSettingsJson}";
	logger.LogInformation(configurationMessage);

	var result = await generationManager.GenerateAsync(commandSettings);
	if (result.Success)
	{
		logger.LogInformation("Model generation succeeded.");
	}
	else
	{
		logger.LogError("Model generation failed.");
	}

	logger.LogInformation(result.Message);
	// this is required because serilog doesn't flush messages quite fast enough if you don't do this.
	Thread.Sleep(TimeSpan.FromSeconds(1));
}

static string GetVersion()
{
	var assembly = Assembly.GetExecutingAssembly();
	var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
	return fvi.FileVersion;
}

static OperationResult<CommandSettings> GetOptions(string[] args, ILogger logger)
{
	var options = Parser.Default.ParseArguments<Options>(args);
	if (options.Errors.Any())
	{
		var message = string.Join(Environment.NewLine, options.Errors);
		return OperationResult.Fail<CommandSettings>(message);
	}
	var commandLineArgs = Environment.GetCommandLineArgs();
	if (commandLineArgs == null)
	{
		return OperationResult.Fail<CommandSettings>("Command Line Arguments are missing.");
	}
	var terminalRoot = Environment.CurrentDirectory;
	logger.LogInformation($"Current Directory: {terminalRoot}");
	if (string.IsNullOrWhiteSpace(terminalRoot))
	{
		return OperationResult.Fail<CommandSettings>("Command Line Arguments are missing.");
	}
	var commandSettings = new CommandSettings
	{
		ConfigPath = options.Value.ConfigPath,
		RootPath = terminalRoot
	};
	return OperationResult.Ok(commandSettings);
}