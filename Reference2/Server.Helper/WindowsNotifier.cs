using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Server.Forms;

namespace Server.Helper;

public static class WindowsNotifier
{
	private class NotificationData
	{
		public string Ip { get; set; }

		public string Username { get; set; }

		public string Country { get; set; }

		public string Hwid { get; set; }

		public bool IsNewUser { get; set; }

		public bool IsDarkTheme { get; set; }

		public bool IsDisconnect { get; set; }
	}

	private static bool isInitialized = false;

	private static List<FormNotification> activeNotifications = new List<FormNotification>();

	private static Queue<NotificationData> notificationQueue = new Queue<NotificationData>();

	private static object lockObject = new object();

	private const int NOTIFICATION_SPACING = 10;

	private static bool isShowingNotification = false;

	public static void Initialize()
	{
		if (isInitialized)
		{
			return;
		}
		try
		{
			isInitialized = true;
		}
		catch (Exception)
		{
		}
	}

	private static Point GetNextNotificationPosition(int notificationHeight)
	{
		lock (lockObject)
		{
			Screen primaryScreen = Screen.PrimaryScreen;
			int x = primaryScreen.WorkingArea.Right - 350 - 10;
			int y = primaryScreen.WorkingArea.Bottom - notificationHeight - 10;
			return new Point(x, y);
		}
	}

	private static void RegisterNotification(FormNotification notification)
	{
		lock (lockObject)
		{
			activeNotifications.Add(notification);
			notification.FormClosed += delegate
			{
				UnregisterNotification(notification);
			};
		}
	}

	private static void UnregisterNotification(FormNotification notification)
	{
		lock (lockObject)
		{
			activeNotifications.Remove(notification);
			RepositionNotifications();
			isShowingNotification = false;
			ProcessQueue();
		}
	}

	private static void RepositionNotifications()
	{
		lock (lockObject)
		{
			Screen screen = Screen.PrimaryScreen;
			int x = screen.WorkingArea.Right - 350 - 10;
			int y = screen.WorkingArea.Bottom - 10;
			for (int i = activeNotifications.Count - 1; i >= 0; i--)
			{
				FormNotification notification = activeNotifications[i];
				if (notification != null && !notification.IsDisposed)
				{
					y -= notification.Height;
					try
					{
						if (notification.InvokeRequired)
						{
							notification.Invoke((MethodInvoker)delegate
							{
								notification.Location = new Point(x, y);
							});
						}
						else
						{
							notification.Location = new Point(x, y);
						}
					}
					catch
					{
					}
					y -= 10;
				}
			}
		}
	}

	public static void ShowConnectionNotification(string ip, string username, string country, bool isNewUser)
	{
		ShowConnectionNotification(ip, username, country, "Unknown", isNewUser);
	}

	public static void ShowConnectionNotification(string ip, string username, string country, string hwid, bool isNewUser)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		try
		{
			bool isDarkTheme = true;
			if (Program.form != null && Program.form.settings != null)
			{
				isDarkTheme = Program.form.settings.DarkTheme;
			}
			lock (lockObject)
			{
				notificationQueue.Enqueue(new NotificationData
				{
					Ip = ip,
					Username = username,
					Country = country,
					Hwid = hwid,
					IsNewUser = isNewUser,
					IsDarkTheme = isDarkTheme,
					IsDisconnect = false
				});
				if (!isShowingNotification)
				{
					ProcessQueue();
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static void ShowDisconnectionNotification(string ip, string username, string country, string hwid)
	{
		if (!isInitialized)
		{
			Initialize();
		}
		try
		{
			bool isDarkTheme = true;
			if (Program.form != null && Program.form.settings != null)
			{
				isDarkTheme = Program.form.settings.DarkTheme;
			}
			lock (lockObject)
			{
				notificationQueue.Enqueue(new NotificationData
				{
					Ip = ip,
					Username = username,
					Country = country,
					Hwid = hwid,
					IsNewUser = false,
					IsDarkTheme = isDarkTheme,
					IsDisconnect = true
				});
				if (!isShowingNotification)
				{
					ProcessQueue();
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private static void ProcessQueue()
	{
		lock (lockObject)
		{
			if (notificationQueue.Count == 0 || isShowingNotification)
			{
				return;
			}
			NotificationData data = notificationQueue.Dequeue();
			isShowingNotification = true;
			Thread thread = new Thread((ThreadStart)delegate
			{
				try
				{
					FormNotification notification = new FormNotification(data.Ip, data.Username, data.Country, data.Hwid, data.IsNewUser, data.IsDarkTheme, data.IsDisconnect);
					notification.Load += delegate
					{
						notification.Location = GetNextNotificationPosition(notification.Height);
					};
					RegisterNotification(notification);
					notification.Show();
					Application.Run(notification);
				}
				catch (Exception)
				{
					lock (lockObject)
					{
						isShowingNotification = false;
						ProcessQueue();
					}
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
		}
	}

	public static void Dispose()
	{
		lock (lockObject)
		{
			notificationQueue.Clear();
			isShowingNotification = false;
			FormNotification[] array = activeNotifications.ToArray();
			foreach (FormNotification notification in array)
			{
				try
				{
					if (notification != null && !notification.IsDisposed)
					{
						notification.Close();
					}
				}
				catch
				{
				}
			}
			activeNotifications.Clear();
			isInitialized = false;
		}
	}
}
