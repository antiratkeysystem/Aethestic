using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Server.Helper.Bulider;

public static class BuildStats
{
	public class BuildInfo
	{
		public string BuildName { get; set; }

		public string Group { get; set; }

		public string ProcessName { get; set; }

		public int Users { get; set; }

		public string DateCreated { get; set; }

		public string Path { get; set; }
	}

	private static readonly string _file;

	private static readonly List<BuildInfo> _builds;

	public static IReadOnlyList<BuildInfo> Builds => _builds.AsReadOnly();

	static BuildStats()
	{
		_file = Path.Combine("local", "Builds.json");
		try
		{
			if (File.Exists(_file))
			{
				_builds = JsonConvert.DeserializeObject<List<BuildInfo>>(File.ReadAllText(_file)) ?? new List<BuildInfo>();
			}
			else
			{
				_builds = new List<BuildInfo>();
			}
		}
		catch
		{
			_builds = new List<BuildInfo>();
		}
	}

	public static void AddBuild(string buildName, string group, string processName, string path)
	{
		if (!string.IsNullOrWhiteSpace(group))
		{
			BuildInfo buildInfo = _builds.Find((BuildInfo b) => b.BuildName == buildName && b.Group == group);
			if (buildInfo != null)
			{
				buildInfo.ProcessName = processName;
				buildInfo.Path = path;
			}
			else
			{
				_builds.Add(new BuildInfo
				{
					BuildName = buildName,
					Group = group,
					ProcessName = processName,
					Users = 0,
					DateCreated = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
					Path = path
				});
			}
			Save();
		}
	}

	public static void IncrementUsers(string group)
	{
		BuildInfo buildInfo = _builds.Find((BuildInfo x) => x.Group == group);
		if (buildInfo != null)
		{
			buildInfo.Users++;
			Save();
		}
	}

	public static void RemoveBuild(string group)
	{
		_builds.RemoveAll((BuildInfo b) => b.Group == group);
		Save();
	}

	public static void ClearAll()
	{
		_builds.Clear();
		Save();
	}

	private static void Save()
	{
		try
		{
			Directory.CreateDirectory("local");
			File.WriteAllText(_file, JsonConvert.SerializeObject(_builds, Formatting.Indented));
		}
		catch
		{
		}
	}
}
