using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Ninject;

namespace battleships
{
	public class Program
	{
        private readonly CommandLineArgs arguments;
        private readonly TextWriter textWriter;
        public Program(CommandLineArgs arguments,TextWriter textWriter)
        {
            this.arguments = arguments;
            this.textWriter = textWriter;
        }

		private static void Main(string[] args)
		{
		    var container = new StandardKernel();
            container.Bind<TextWriter>().ToConstant(Console.Out);
            container.Bind<CommandLineArgs>().To<CommandLineArgs>().WithConstructorArgument(args);
            container.Get<Program>().Run();
		}

	    private void Run()
	    {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var aiPath = arguments.AiPath;
            if (aiPath == null)
            {
                textWriter.WriteLine("Usage: {0} <ai.exe>", Process.GetCurrentProcess().ProcessName);
                return;
            }
	        if (!File.Exists(aiPath))
	        {
	            textWriter.WriteLine("No AI exe-file " + aiPath);
	            return;
	        }
            var aiTesterContainer = new StandardKernel(new AiTesterContainer(aiPath));
            aiTesterContainer.Get<IAiTester>().TestAi();
	    }
	}

    public class CommandLineArgs
    {
        private readonly string[] args;

        public CommandLineArgs(string[] args)
        {
            this.args = args;
        }

        public string AiPath { get { return args.Length == 0 ? null : args[0]; } }
    }
}