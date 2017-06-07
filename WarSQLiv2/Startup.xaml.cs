using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;
using WarSQLiv2.UserControls;

namespace WarSQLiv2
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        private readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        public Startup()
        {
            InitializeComponent();
        }

        private void Startup_OnLoaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(FileSizeControl);
            Task.WaitAll();
            Dispatcher.Invoke(SelectedLang);
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                var rnd = new Random();
                                var random = rnd.Next(1, 16);
                                var img = new Image();
                                var src = new BitmapImage();
                                src.BeginInit();
                                var address = Directory.GetCurrentDirectory() + @"\Scanner\Loading\" + random + ".png";
                                src.UriSource = new Uri(address, UriKind.Absolute);
                                src.CacheOption = BitmapCacheOption.OnLoad;
                                src.EndInit();
                                img.Source = src;
                                img.Stretch = Stretch.Uniform;
                                Clipboard.SetImage(src);
                                txtStatus.Paste();
                                txtSingleIPOctet1.Focus();
                            });
            }
            catch (Exception)
            {
                throw;
            }

            if (!IsRunningAsAdministrator())
            {
                _languageControl.FindLang();
                _languageControl.SelectedLanguage =
                            new ResourceManager("WarSQLiv2.Language." + _languageControl.LoadedLang,
                                Assembly.GetExecutingAssembly());
                MessageBox.Show(_languageControl.SelectedLanguage.GetString("MessageRunAs"), "WarSQLiv2.1",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }
        }

        private void MenuFileNewSession_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Application.ResourceAssembly.Location);
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }
        private void MenuFileRestart_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
        private void MenuFileExit_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void menuLanguageEnglish_Click(object sender, RoutedEventArgs e)
        {
            _languageControl.SetLanguage = "English";
            _languageControl.SetLang();
        }
        private void menuLanguageTurkish_Click(object sender, RoutedEventArgs e)
        {
            _languageControl.SetLanguage = "Turkish";
            _languageControl.SetLang();
        }
        private void menuAboutAbout_Click(object sender, RoutedEventArgs e)
        {
            var about = new UserControls.Help.FrmAbout();
            about.ShowDialog();
        }


        private void txtSingleIPOctet1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtSingleIPOctet2.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtSingleIPOctet2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtSingleIPOctet3.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtSingleIPOctet3_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtSingleIPOctet4.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtSingleIPOctet4_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtSinglePort.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtSinglePort_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSingleIPOctet1.Text) && !string.IsNullOrEmpty(txtSingleIPOctet2.Text) && !string.IsNullOrEmpty(txtSingleIPOctet3.Text) && !string.IsNullOrEmpty(txtSingleIPOctet4.Text))
            {
                if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
                {
                    txtSinglePort.Focus();
                }
            }
        }
        private void txtSingleIPOctet1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtSingleIPOctet1.Text.Length == 3)
                {
                    txtSingleIPOctet2.Focus();
                }

                if (!string.IsNullOrEmpty(txtSingleIPOctet1.Text))
                {
                    var send = Convert.ToInt32(txtSingleIPOctet1.Text);
                    var okt1 = send;
                    if (okt1 > 255)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                        txtSingleIPOctet1.Clear();
                        txtSingleIPOctet1.Focus();
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
                txtSingleIPOctet1.Clear();
                txtSingleIPOctet1.Focus();
            }
        }
        private void txtSingleIPOctet2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtSingleIPOctet2.Text.Length == 3)
                {
                    txtSingleIPOctet3.Focus();
                }

                if (!string.IsNullOrEmpty(txtSingleIPOctet2.Text))
                {
                    var send = Convert.ToInt32(txtSingleIPOctet2.Text);
                    var okt1 = send;
                    if (okt1 > 255)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                        txtSingleIPOctet2.Clear();
                        txtSingleIPOctet2.Focus();
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
                txtSingleIPOctet2.Clear();
                txtSingleIPOctet2.Focus();
            }
        }
        private void txtSingleIPOctet3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtSingleIPOctet3.Text.Length == 3)
                {
                    txtSingleIPOctet4.Focus();
                }

                if (!string.IsNullOrEmpty(txtSingleIPOctet3.Text))
                {
                    var send = Convert.ToInt32(txtSingleIPOctet3.Text);
                    var okt1 = send;
                    if (okt1 > 255)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                        txtSingleIPOctet3.Clear();
                        txtSingleIPOctet3.Focus();
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
                txtSingleIPOctet3.Clear();
                txtSingleIPOctet3.Focus();
            }
        }
        private void txtSingleIPOctet4_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtSingleIPOctet4.Text.Length == 3)
                {
                    txtSinglePort.Focus();
                }

                if (!string.IsNullOrEmpty(txtSingleIPOctet4.Text))
                {
                    var send = Convert.ToInt32(txtSingleIPOctet4.Text);
                    var okt1 = send;
                    if (okt1 > 255)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                        txtSingleIPOctet4.Clear();
                        txtSingleIPOctet4.Focus();
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
                txtSingleIPOctet4.Clear();
                txtSingleIPOctet4.Focus();
            }
        }
        private void txtStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtStatus.ScrollToEnd();
        }

        private void lstFoundedAddress_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (lstFoundedAddress.Items.Count > 0)
                {
                    var split = lstFoundedAddress.SelectedItem.ToString().Split(':');
                    var dialog = MessageBox.Show($"{split[0]}" + _languageControl.SelectedLanguage.GetString("RightMenu2"), "", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    switch (dialog)
                    {
                        case MessageBoxResult.OK:
                            lstFoundedAddress.Items.Remove(lstFoundedAddress.SelectedItem);
                            txtStatus.AppendText(
                                $"{Environment.NewLine}{split[0]}{_languageControl.SelectedLanguage.GetString("RightMenu4")}{split[1]}{_languageControl.SelectedLanguage.GetString("RightMenu5")}");
                            break;
                    }
                    if (lstFoundedAddress.Items.Count > 0)
                    {
                        btnContinue.IsEnabled = true;
                    }
                    else
                    {
                        btnContinue.IsEnabled = false;
                    }
                }
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }

        private void btnAddSingleIp_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSingleIPOctet1.Text) && !string.IsNullOrEmpty(txtSingleIPOctet2.Text) &&
                !string.IsNullOrEmpty(txtSingleIPOctet3.Text) && !string.IsNullOrEmpty(txtSingleIPOctet4.Text) &&
                !string.IsNullOrEmpty(txtSinglePort.Text))
            {
                lstFoundedAddress.Items.Add(
                    $"{txtSingleIPOctet1.Text}.{txtSingleIPOctet2.Text}.{txtSingleIPOctet3.Text}.{txtSingleIPOctet4.Text}:{txtSinglePort.Text}");
                FormGeneralControl();
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action)delegate
                    {
                        txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ScannerTextIP2")}");
                    });
            }
        }
        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = Directory.GetCurrentDirectory() + @"\Scanner\FoundServer\SqlServerList.txt";
                if (!File.Exists(fileName))
                {
                    var createSqlServerSingleText = new StreamWriter(fileName);
                    foreach (var t in lstFoundedAddress.Items)
                    {
                        createSqlServerSingleText.WriteLine(t.ToString());
                    }
                    createSqlServerSingleText.Flush();
                    createSqlServerSingleText.Close();
                }
                else
                {
                    File.Delete(fileName);
                    var createSqlServerSingleText = new StreamWriter(fileName);
                    foreach (var t in lstFoundedAddress.Items)
                    {
                        createSqlServerSingleText.WriteLine(t.ToString());
                    }
                    createSqlServerSingleText.Flush();
                    createSqlServerSingleText.Close();
                }
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }

            this.Hide();
            var showAttackPanel = new WarSQLiAttack();
            showAttackPanel.Show();
        }
        private void BtnSelect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                var selectDialog = new OpenFileDialog
                                {
                                    Title = _languageControl.SelectedLanguage.GetString("FileSelectTarget1"),
                                    Filter = @"(*.txt)|*.txt",
                                    Multiselect = false,
                                    InitialDirectory = Directory.GetCurrentDirectory() + @"Scanner\FoundServer\",
                                    FileName = _languageControl.SelectedLanguage.GetString("FileSelectTarget2")
                                };
                                selectDialog.ShowDialog();
                                txtImportTarget.Text = selectDialog.FileName;
                                if ((string)txtImportTarget.Text == _languageControl.SelectedLanguage.GetString("FileSelectTarget2"))
                                {
                                    txtImportTarget.Text = "";
                                }

                                if (!string.IsNullOrEmpty(txtImportTarget.Text))
                                {
                                    var fileName = txtImportTarget.Text;
                                    var targetText = File.ReadLines(fileName);
                                    var targetFile = targetText as string[] ?? targetText.ToArray();
                                    var passCount = targetFile.Count();
                                    for (var i = 0; i < passCount; i++)
                                    {
                                        lstFoundedAddress.Items.Add(targetFile[i]);
                                    }
                                }
                                FormGeneralControl();
                            });
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }

        private void FileSizeControl()
        {
            var fileControl = new FileCreationControl();
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)(() => { fileControl.FileSizeControl(); }));
            }
            catch (Exception)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                txtStatus.AppendText(fileControl.Exception);
                            });
            }
        }
        private void SelectedLang()
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action)delegate
                    {
                        _languageControl.FindLang();
                        _languageControl.SelectedLanguage =
                            new ResourceManager("WarSQLiv2.Language." + _languageControl.LoadedLang,
                                Assembly.GetExecutingAssembly());
                        menuFile.Header = _languageControl.SelectedLanguage.GetString("MenuFile");
                        menuFileNewSession.Header = _languageControl.SelectedLanguage.GetString("MenuFileNew");
                        menuFileRestart.Header = _languageControl.SelectedLanguage.GetString("MenuFileRestart");
                        menuFileExit.Header = _languageControl.SelectedLanguage.GetString("MenuFileClose");
                        menuLanguage.Header = _languageControl.SelectedLanguage.GetString("MenuLanguage");
                        menuLanguageEnglish.Header = _languageControl.SelectedLanguage.GetString("MenuLanguageEnglish");
                        menuLanguageTurkish.Header = _languageControl.SelectedLanguage.GetString("MenuLanguageTurkish");
                        menuAbout.Header = _languageControl.SelectedLanguage.GetString("MenuAbout");
                        menuAboutAbout.Header = _languageControl.SelectedLanguage.GetString("MenuAbout");
                        lblStatus.Content = _languageControl.SelectedLanguage.GetString("GroupBoxUserStatus");
                        lblSingleIp.Content = _languageControl.SelectedLanguage.GetString("LabelSingleIp");
                        lblTargetFile.Content = _languageControl.SelectedLanguage.GetString("LabelTargetFile");
                        btnSelect.Content = _languageControl.SelectedLanguage.GetString("ButtonSelectImport");
                        btnAddSingleIp.Content = _languageControl.SelectedLanguage.GetString("ButtonAdd");
                        btnContinue.Content = _languageControl.SelectedLanguage.GetString("ButtonCont");
                    });
            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action)delegate
                    {
                        txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message,
                            _languageControl.SelectedLanguage.GetString("GeneralError1"),
                            _languageControl.SelectedLanguage.GetString("GeneralError2")));
                    });
            }
        }
        private void FormGeneralControl()
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action)delegate
                    {
                        if (lstFoundedAddress.Items.Count > 0)
                        {
                            try
                            {
                                var parse = lstFoundedAddress.Items[0].ToString().Split(':');
                                btnContinue.IsEnabled = true;
                            }
                            catch (Exception exp)
                            {
                                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message,
                                    _languageControl.SelectedLanguage.GetString("GeneralError1"),
                                    _languageControl.SelectedLanguage.GetString("GeneralError2")));
                            }
                        }
                    });

            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action)delegate
                    {
                        txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message,
                            _languageControl.SelectedLanguage.GetString("GeneralError1"),
                            _languageControl.SelectedLanguage.GetString("GeneralError2")));
                    });
            }
            finally
            {
                //Dispatcher.BeginInvoke(DispatcherPriority.Send,
                //    (Action) delegate
                //    {
                //        txtStatus.AppendText(_isBtnSingleIpAddClick == true
                //            ? $"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageIpAdd")}"
                //            : $"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ScanStarted2")}");
                //    });
            }
        }
        public static bool IsRunningAsAdministrator()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            var windowsPrincipal = new WindowsPrincipal(windowsIdentity);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        
    }
}
