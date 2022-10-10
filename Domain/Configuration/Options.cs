using CommandLine;
using CommandLine.Text;

namespace Domain.Configuration
{
	public class Options
	{
		[Option('p', "PathToConfig", Required = false, HelpText = "Use to set config path.")]
		public string ConfigPath { get; set; }

		[Usage(ApplicationAlias = "gs")]
		public static IEnumerable<Example> Examples
		{
			get
			{
				return new List<Example>() {
					new Example("Supply no parameter. Will use terminal directory as root and default filename to gunslinger.json", new Options { }),
					new Example("Supply the filename only. Will use terminal directory as root", new Options { ConfigPath = "gunslinger-test.json" }),
					new Example("Supply the path to the config. Will use default filename to gunslinger.json", new Options { ConfigPath = "c:\\projects\\gunslinger" }),
					new Example("Supply the full path to the config", new Options { ConfigPath = "c:\\projects\\gunslinger\\gunslinger-test.json" })
				};
			}
		}
	}
}