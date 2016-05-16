# C# Logging
An abstracted logging platform for .NET. It can help you collect reliable logs for your application regardless of its size or complexity with minimal performance implications.

## Packages:

* cdmdotnet.Logging - core library with Console and Trace based outputs
* cdmdotnet.Logging.Azure - an azure based package to allow configuration settings to be set via the Azure portal
* cdmdotnet.Logging.Sql - a SqlServer based package to store outputs in SqlServer.

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
