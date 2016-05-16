using System;

namespace cdmdotnet.Logging
{
	public class LogInformation
	{
		public LogInformation()
		{
			Raised = DateTime.UtcNow;
		}

		public DateTime Raised { get; private set; }

		public string Level { get; set; }

		public string Message { get; set; }

		public string Container { get; set; }

		public string Exception { get; set; }
	}
}