using System;
using System.IO;
using System.Threading;
using NAudio.Wave;
using Server.Data;

namespace Server.Helper;

public static class SoundPlayer
{
	private static bool startupSoundPlayed = false;

	private static readonly object lockObject = new object();

	private static string GetStartSoundPath(Settings s)
	{
		if (s == null)
		{
			return null;
		}
		if (!s.EnableSoundOnStart)
		{
			return null;
		}
		switch (s.SoundTypeStart)
		{
		case 0:
			return Path.Combine("Sounds", "start.wav");
		case 1:
			if (string.IsNullOrEmpty(s.CustomSoundPathStart) || !File.Exists(s.CustomSoundPathStart))
			{
				return null;
			}
			return s.CustomSoundPathStart;
		case 2:
			return Path.Combine("Sounds", "Dota", "dotastart.wav");
		default:
			return Path.Combine("Sounds", "start.wav");
		}
	}

	private static string GetConnectSoundPath(Settings s, int clientIndex)
	{
		if (s == null)
		{
			return null;
		}
		if (!s.EnableSoundOnConnect)
		{
			return null;
		}
		switch (s.SoundTypeConnect)
		{
		case 0:
			return Path.Combine("Sounds", "connect.wav");
		case 1:
			if (string.IsNullOrEmpty(s.CustomSoundPathConnect) || !File.Exists(s.CustomSoundPathConnect))
			{
				return null;
			}
			return s.CustomSoundPathConnect;
		case 2:
		{
			string dotaDir = Path.Combine("Sounds", "Dota");
			if (!Directory.Exists(dotaDir))
			{
				return null;
			}
			int idx = Math.Max(1, clientIndex);
			string dotaPath = Path.Combine(dotaDir, "dota" + idx + ".wav");
			if (File.Exists(dotaPath))
			{
				return dotaPath;
			}
			int maxN = 1;
			string[] files = Directory.GetFiles(dotaDir, "dota*.wav");
			for (int i = 0; i < files.Length; i++)
			{
				string name = Path.GetFileNameWithoutExtension(files[i]);
				if (name.Length > 4 && int.TryParse(name.Substring(4), out var n) && n > maxN)
				{
					maxN = n;
				}
			}
			string fallback = Path.Combine(dotaDir, "dota" + ((idx - 1) % maxN + 1) + ".wav");
			if (!File.Exists(fallback))
			{
				return null;
			}
			return fallback;
		}
		default:
			return Path.Combine("Sounds", "connect.wav");
		}
	}

	private static void PlaySoundInternal(string soundPath, float volume)
	{
		if (string.IsNullOrEmpty(soundPath) || !File.Exists(soundPath))
		{
			return;
		}
		try
		{
			using WaveFileReader reader = new WaveFileReader(soundPath);
			using WaveOut waveOut = new WaveOut();
			waveOut.Volume = Math.Max(0f, Math.Min(1f, volume));
			waveOut.Init(reader);
			waveOut.Play();
			while (waveOut.PlaybackState == PlaybackState.Playing)
			{
				Thread.Sleep(50);
			}
		}
		catch (Exception)
		{
		}
	}

	private static void PlaySoundAsync(string soundPath, float volume)
	{
		if (string.IsNullOrEmpty(soundPath) || !File.Exists(soundPath))
		{
			return;
		}
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				using WaveFileReader waveProvider = new WaveFileReader(soundPath);
				using WaveOut waveOut = new WaveOut();
				waveOut.Volume = Math.Max(0f, Math.Min(1f, volume));
				waveOut.Init(waveProvider);
				waveOut.Play();
				while (waveOut.PlaybackState == PlaybackState.Playing)
				{
					Thread.Sleep(50);
				}
			}
			catch (Exception)
			{
			}
		});
	}

	public static void PlayStartupSound()
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				if (Program.form == null || Program.form.settings == null || !Program.form.settings.Sounds)
				{
					lock (lockObject)
					{
						startupSoundPlayed = true;
						return;
					}
				}
				Settings settings = Program.form.settings;
				string startSoundPath = GetStartSoundPath(settings);
				if (!string.IsNullOrEmpty(startSoundPath) && File.Exists(startSoundPath))
				{
					float volume = (float)settings.SoundVolume / 100f;
					PlaySoundInternal(startSoundPath, volume);
				}
				lock (lockObject)
				{
					startupSoundPlayed = true;
				}
			}
			catch
			{
				lock (lockObject)
				{
					startupSoundPlayed = true;
				}
			}
		});
	}

	public static void PlayConnectSound(int clientIndex = 1)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			try
			{
				if (Program.form != null && Program.form.settings != null && Program.form.settings.Sounds)
				{
					Settings settings = Program.form.settings;
					string connectSoundPath = GetConnectSoundPath(settings, clientIndex);
					if (!string.IsNullOrEmpty(connectSoundPath) && File.Exists(connectSoundPath))
					{
						float volume = (float)settings.SoundVolume / 100f;
						PlaySoundAsync(connectSoundPath, volume);
					}
				}
			}
			catch (Exception)
			{
			}
		});
	}
}
