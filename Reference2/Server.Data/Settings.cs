namespace Server.Data;

public class Settings
{
	public string[] Ports;

	public bool Start;

	public int second = 35;

	public string WebHook;

	public bool WebHookNewConnect;

	public bool WebHookConnect;

	public bool AutoStealer;

	public int Style;

	public string linkMiner = "http://%IP%/ack";

	public string TelegramBotToken;

	public string TelegramChatID;

	public bool TelegramNewConnect;

	public bool TelegramConnect;

	public bool Sounds;

	public int SoundTypeStart;

	public int SoundTypeConnect;

	public string CustomSoundPathStart = "";

	public string CustomSoundPathConnect = "";

	public int SoundVolume = 100;

	public bool EnableSoundOnStart = true;

	public bool EnableSoundOnConnect = true;

	public bool DiscordRPC;

	public bool DarkTheme;

	public bool RainbowTheme;

	public bool SpeedUPTheme;

	public bool Notificator;

	public string FormText = "✦ LiberiumRAT ✦ | ✨ Version: 3.1 [RECODE] ✨";

	public int FormTextAnimationType;

	public int FormTextAnimationSpeed = 80;

	public bool Background;

	public string BackgroundPath;

	public int BackgroundBlur;

	public int BackgroundOpacity = 100;

	public string[] BackgroundSlots = new string[10];

	public bool AutoNote;

	public bool AutoFRPC;

	public Settings()
	{
		BackgroundSlots = new string[10];
	}
}
