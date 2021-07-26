using System;
using System.Collections.Generic;
using System.Text;

namespace Kveer.Creteus.Console
{
	static class ConsoleExtension
	{
		static void InternalWrite(string value, Action<string> func, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
		{
			var default_fg = System.Console.ForegroundColor;
			var default_bg = System.Console.BackgroundColor;

			if (foregroundColor.HasValue)
				System.Console.ForegroundColor = foregroundColor.Value;
			if (backgroundColor.HasValue)
				System.Console.BackgroundColor = backgroundColor.Value;

			func.Invoke(value);
			System.Console.ForegroundColor = default_fg;
			System.Console.BackgroundColor = default_bg;
		}

		public static void Write(string value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
		{
			InternalWrite(value, System.Console.Write, foregroundColor, backgroundColor);
		}

		public static void WriteLine(string value, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
		{
			InternalWrite(value, System.Console.WriteLine, foregroundColor, backgroundColor);
		}

	}
}
