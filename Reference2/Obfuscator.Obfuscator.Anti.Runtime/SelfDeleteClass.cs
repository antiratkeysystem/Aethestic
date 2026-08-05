using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Obfuscator.Obfuscator.Anti.Runtime;

internal class SelfDeleteClass
{
	public static void Init()
	{
		if (IsSandboxie())
		{
			SelfDelete();
		}
		if (IsDebugger())
		{
			SelfDelete();
		}
		if (IsdnSpyRun())
		{
			SelfDelete();
		}
	}

	private static bool IsSandboxie()
	{
		return IsDetected();
	}

	private static bool IsDebugger()
	{
		return Run();
	}

	private static bool IsdnSpyRun()
	{
		return ValueType();
	}

	private static void SelfDelete()
	{
		Process.Start(new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del \"" + Assembly.GetExecutingAssembly().Location + "\"")
		{
			WindowStyle = ProcessWindowStyle.Hidden
		})?.Dispose();
		Process.GetCurrentProcess().Kill();
	}

	private static bool ValueType()
	{
		return File.Exists(Environment.ExpandEnvironmentVariables("%appdata%") + "\\dnSpy\\dnSpy.xml");
	}

	private static IntPtr GetModuleHandle(string libName)
	{
		foreach (ProcessModule processModule in Process.GetCurrentProcess().Modules)
		{
			if (processModule.ModuleName.ToLower().Contains(libName.ToLower()))
			{
				return processModule.BaseAddress;
			}
		}
		return IntPtr.Zero;
	}

	private static bool IsDetected()
	{
		return GetModuleHandle(Encoding.UTF8.GetString(Convert.FromBase64String("U2JpZURsbC5kbGw="))) != IntPtr.Zero;
	}

	private static bool Run()
	{
		bool result = false;
		if (Debugger.IsAttached || Debugger.IsLogging())
		{
			result = true;
		}
		else
		{
			string[] array = new string[41]
			{
				"codecracker", "x32dbg", "x64dbg", "ollydbg", "ida", "charles", "dnspy", "simpleassembly", "peek", "httpanalyzer",
				"httpdebug", "fiddler", "wireshark", "dbx", "mdbg", "gdb", "windbg", "dbgclr", "kdb", "kgdb",
				"mdb", "processhacker", "scylla_x86", "scylla_x64", "scylla", "idau64", "idau", "idaq", "idaq64", "idaw",
				"idaw64", "idag", "idag64", "ida64", "ida", "ImportREC", "IMMUNITYDEBUGGER", "MegaDumper", "CodeBrowser", "reshacker",
				"cheat engine"
			};
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				if (process == Process.GetCurrentProcess())
				{
					continue;
				}
				for (int j = 0; j < array.Length; j++)
				{
					if (process.ProcessName.ToLower().Contains(array[j]))
					{
						result = true;
					}
					if (process.MainWindowTitle.ToLower().Contains(array[j]))
					{
						result = true;
					}
				}
			}
		}
		return result;
	}
}
