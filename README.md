# C# Logging
An abstracted logging platform for .NET. It can help you collect reliable logs for your application regardless of its size or complexity with minimal performance implications.

## Nuget Packages:

* Chinchilla.Logging - core library with Console and Trace based outputs
* Chinchilla.Logging.Azure - an Azure based package to store outputs in Azure Log Analytics and/or to allow configuration settings to be set via the Azure portal
* Chinchilla.Logging.Azure.ApplicationInsights - an Azure based package to add Telemetry automatically as a result of logging to ApplicationInsights
* Chinchilla.Logging.Azure.Storage - an Azure based package to store outputs in Azure Table Storage
* Chinchilla.Logging.Sql - a SqlServer based package to store outputs in SqlServer.
* Chinchilla.Logging.Serilog - a Serilog based package to store outputs using Serilog.

## The CorrelationId and ICorrelationIdHelper

A `CorrelationId` is an identifier used to group together logs. This might be all logs as a result of calling a webform or MVC controller action. To use it add the following to your `global.asax.cs`, replacing `NinjectDependencyResolver` with your dependency resolver of choice.

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			NinjectDependencyResolver.Current.Resolve<ICorrelationIdHelper>().SetCorrelationId(Guid.NewGuid());
		}

There are three built-in ICorrelationIdHelper implementations, `CorrelationIdHelper`, `WebCorrelationIdHelper` and `NullCorrelationIdHelper`

`WebCorrelationIdHelper` is for use on a website.
`CorrelationIdHelper` is for use in a console app, windows service, winform or other such application not managed by IIS.
`NullCorrelationIdHelper` is for use in unit and integration tests. It always returns `Guid.Empty`

## ILogger Implementations

### TraceLogger
### ConsoleLogger
### SqlLogger
### SerilogLogger
### TableStorageLogger
### LogAnalyticsLogger
### MultiLogger - for those odd times you need to use multiple technology stacks for logging.
