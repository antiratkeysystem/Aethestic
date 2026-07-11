using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Intelix.Helper
{
	// Token: 0x02000064 RID: 100
	public static class RegistryParser
	{
		// Token: 0x0600016F RID: 367 RVA: 0x00011EA0 File Offset: 0x000100A0
		public static List<string> ParseKey(RegistryKey key)
		{
			List<string> list = new List<string>();
			bool flag = key == null;
			List<string> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				string[] valueNames = key.GetValueNames();
				string[] array = valueNames;
				int i = 0;
				while (i < array.Length)
				{
					string text = array[i];
					object value = key.GetValue(text);
					string str;
					switch (key.GetValueKind(text))
					{
					case RegistryValueKind.String:
					case RegistryValueKind.ExpandString:
						str = (((value != null) ? value.ToString() : null) ?? "null");
						break;
					case RegistryValueKind.Binary:
					{
						byte[] array2 = value as byte[];
						str = ((array2 == null) ? "null" : BitConverter.ToString(array2).Replace("-", ""));
						break;
					}
					case RegistryValueKind.DWord:
					case RegistryValueKind.QWord:
						str = value.ToString();
						break;
					case (RegistryValueKind)5:
					case (RegistryValueKind)6:
					case (RegistryValueKind)8:
					case (RegistryValueKind)9:
					case (RegistryValueKind)10:
						goto IL_F9;
					case RegistryValueKind.MultiString:
					{
						string[] array3 = value as string[];
						str = ((array3 == null) ? "null" : string.Join(", ", array3));
						break;
					}
					default:
						goto IL_F9;
					}
					IL_114:
					list.Add(text + ": " + str);
					i++;
					continue;
					IL_F9:
					str = (((value != null) ? value.ToString() : null) ?? "null");
					goto IL_114;
				}
				result = list;
			}
			return result;
		}
	}
}
