using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Server.Helper;

public class RegistrySeeker
{
	public class RegSeekerMatch
	{
		public string Key { get; set; }

		public RegValueData[] Data { get; set; }

		public bool HasSubKeys { get; set; }

		public override string ToString()
		{
			return $"({Key}:{Data})";
		}
	}

	public class RegValueData
	{
		public string Name { get; set; }

		public RegistryValueKind Kind { get; set; }

		public byte[] Data { get; set; }
	}

	private readonly List<RegSeekerMatch> _matches;

	public RegSeekerMatch[] Matches => _matches?.ToArray();

	public RegistrySeeker()
	{
		_matches = new List<RegSeekerMatch>();
	}

	public void BeginSeeking(string rootKeyName)
	{
		if (!string.IsNullOrEmpty(rootKeyName))
		{
			using (RegistryKey rootKey = GetRootKey(rootKeyName))
			{
				if (rootKey != null && rootKey.Name != rootKeyName)
				{
					string name = rootKeyName.Substring(rootKey.Name.Length + 1);
					using RegistryKey registryKey = rootKey.OpenReadonlySubKeySafe(name);
					if (registryKey != null)
					{
						Seek(registryKey);
					}
					return;
				}
				Seek(rootKey);
				return;
			}
		}
		Seek(null);
	}

	private void Seek(RegistryKey rootKey)
	{
		if (rootKey == null)
		{
			foreach (RegistryKey registryKey in GetRootKeys())
			{
				ProcessKey(registryKey, registryKey.Name);
			}
			return;
		}
		Search(rootKey);
	}

	private void Search(RegistryKey rootKey)
	{
		string[] subKeyNames = rootKey.GetSubKeyNames();
		foreach (string text in subKeyNames)
		{
			ProcessKey(rootKey.OpenReadonlySubKeySafe(text), text);
		}
	}

	private void ProcessKey(RegistryKey key, string keyName)
	{
		if (key != null)
		{
			List<RegValueData> list = new List<RegValueData>();
			string[] valueNames = key.GetValueNames();
			foreach (string name in valueNames)
			{
				RegistryValueKind valueKind = key.GetValueKind(name);
				object value = key.GetValue(name);
				list.Add(RegistryKeyHelper.CreateRegValueData(name, valueKind, value));
			}
			AddMatch(keyName, RegistryKeyHelper.AddDefaultValue(list), key.SubKeyCount);
		}
		else
		{
			AddMatch(keyName, RegistryKeyHelper.GetDefaultValues(), 0);
		}
	}

	private void AddMatch(string key, RegValueData[] values, int subkeycount)
	{
		_matches.Add(new RegSeekerMatch
		{
			Key = key,
			Data = values,
			HasSubKeys = (subkeycount > 0)
		});
	}

	public static RegistryKey GetRootKey(string subkeyFullPath)
	{
		string[] array = subkeyFullPath.Split('\\');
		try
		{
			return array[0] switch
			{
				"HKEY_CLASSES_ROOT" => RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64), 
				"HKEY_CURRENT_USER" => RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), 
				"HKEY_LOCAL_MACHINE" => RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), 
				"HKEY_USERS" => RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64), 
				"HKEY_CURRENT_CONFIG" => RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64), 
				_ => throw new Exception("Invalid rootkey, could not be found."), 
			};
		}
		catch (SystemException)
		{
			throw new Exception("Unable to open root registry key, you do not have the needed permissions.");
		}
		catch (Exception ex2)
		{
			throw ex2;
		}
	}

	public static List<RegistryKey> GetRootKeys()
	{
		List<RegistryKey> list = new List<RegistryKey>();
		try
		{
			list.Add(RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64));
			list.Add(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64));
			list.Add(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64));
			list.Add(RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64));
			list.Add(RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64));
			return list;
		}
		catch (SystemException)
		{
			throw new Exception("Could not open root registry keys, you may not have the needed permission");
		}
		catch (Exception ex2)
		{
			throw ex2;
		}
	}
}
