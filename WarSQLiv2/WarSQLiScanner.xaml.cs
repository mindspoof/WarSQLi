using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.Scanner;
using WarSQLiv2.UserControls;

namespace WarSQLiv2
{
    public partial class WarSQLiScanner : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        private bool _isBtnSingleIpAddClick = false;
        public WarSQLiScanner()
        {
            InitializeComponent();
        }

        private void txtIPOctet1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtIPOctet2.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtIPOctet2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtIPOctet3.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtIPOctet3_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtIPOctet4.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtIPOctet4_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtIPOctetRange.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtIPOctetRange_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.'))
            {
                txtIPOctetRange.Focus();
            }
            else if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }
        private void txtIPOctet1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtIPOctet1.Text.Length == 3)
                {
                    txtIPOctet2.Focus();
                }

                if (!string.IsNullOrEmpty(txtIPOctet1.Text))
                {
                    var send = Convert.ToInt32(txtIPOctet1.Text);
                    var okt1 = send;
                    if (okt1 > 255)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                        txtIPOctet1.Clear();
                        txtIPOctet1.Focus();
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
                txtIPOctet1.Clear();
                txtIPOctet1.Focus();
            }
        }
        private void txtIPOctet2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtIPOctet2.Text.Length == 3)
                {
                    txtIPOctet3.Focus();
                }

                if (!string.IsNullOrEmpty(txtIPOctet2.Text))
                {
                    var send = Convert.ToInt32(txtIPOctet2.Text);
                    var okt1 = send;
                    if (okt1 > 255)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                        txtIPOctet2.Clear();
                        txtIPOctet2.Focus();
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
                txtIPOctet2.Clear();
                txtIPOctet2.Focus();
            }
        }
        private void txtIPOctet3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtIPOctet3.Text.Length == 3)
                {
                    txtIPOctet4.Focus();
                }

                if (!string.IsNullOrEmpty(txtIPOctet3.Text))
                {
                    var send = Convert.ToInt32(txtIPOctet3.Text);
                    var okt1 = send;
                    if (okt1 > 255)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                        txtIPOctet3.Clear();
                        txtIPOctet3.Focus();
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
                txtIPOctet3.Clear();
                txtIPOctet3.Focus();
            }
        }
        private void txtIPOctet4_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtIPOctet4.Text.Length == 3)
                {
                    txtIPOctetRange.Focus();
                }

                if (!string.IsNullOrEmpty(txtIPOctet4.Text))
                {
                    var send = Convert.ToInt32(txtIPOctet4.Text);
                    var okt1 = send;
                    if (okt1 > 255)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                        txtIPOctet4.Clear();
                        txtIPOctet4.Focus();
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
                txtIPOctet4.Clear();
                txtIPOctet4.Focus();
            }
        }
        private void txtIPOctetRange_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var send = Convert.ToInt32(txtIPOctetRange.Text);
                var octet4 = Convert.ToInt32(txtIPOctet4.Text);
                if (!string.IsNullOrEmpty(txtIPOctetRange.Text) && !string.IsNullOrEmpty(txtIPOctet1.Text) && !string.IsNullOrEmpty(txtIPOctet2.Text) && !string.IsNullOrEmpty(txtIPOctet3.Text) && !string.IsNullOrEmpty(txtIPOctet4.Text))
                {
                    if (send > 255)
                    {
                        txtIPOctetRange.Clear();
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));
                    }
                    else if (send >= octet4)
                    {
                        btnStart.IsEnabled = true;
                    }
                }
                else
                {
                    txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP3"), Environment.NewLine));
                    btnStart.IsEnabled = false;
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
            }
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
        private void txtSinglePort_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSinglePort.Text))
                {
                    var send = Convert.ToInt32(txtSinglePort.Text);
                    var okt1 = send;
                    if (okt1 > 65535)
                    {
                        txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP1"), Environment.NewLine));

                    }
                    btnSingleIpAdd.IsEnabled = true;
                }
            }
            catch (Exception)
            {
                txtStatus.AppendText(string.Format("{0}" + _languageControl.SelectedLanguage.GetString("ScannerTextIP2"), Environment.NewLine));
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(FileSizeControl);
            Task.WaitAll();
            Dispatcher.Invoke(SelectedLang);
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                tglMsSql.IsChecked = true;
                                btnStart.IsEnabled = false;
                                btnContinue.IsEnabled = false;
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
                            });
            }
            catch (Exception)
            {
                throw;
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
                    (Action) delegate
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
                        btnContinue.Content = _languageControl.SelectedLanguage.GetString("ButtonCont");
                        btnStart.Content = _languageControl.SelectedLanguage.GetString("ButtonStart");
                        btnSingleIpAdd.Content = _languageControl.SelectedLanguage.GetString("ButtonAdd");
                        lblDelay.Content = _languageControl.SelectedLanguage.GetString("LabelDelay");
                        lblTimeOut.Content = _languageControl.SelectedLanguage.GetString("LabelTimeOut");
                        lblRange.Content = _languageControl.SelectedLanguage.GetString("LabelRange");
                        lblStatus.Content = _languageControl.SelectedLanguage.GetString("GroupBoxUserStatus");
                        lblSingleIp.Content = _languageControl.SelectedLanguage.GetString("LabelSingleIp");
                    });
            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action) delegate
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
                    (Action) delegate
                    {
                        if (lstFoundedAddress.Items.Count > -1)
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
                        else
                        {
                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ScanStarted2")}");
                        }
                    });

            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action) delegate
                    {
                        txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message,
                            _languageControl.SelectedLanguage.GetString("GeneralError1"),
                            _languageControl.SelectedLanguage.GetString("GeneralError2")));
                    });
            }
            finally
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action) delegate
                    {
                        txtStatus.AppendText(_isBtnSingleIpAddClick == true
                            ? $"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageIpAdd")}"
                            : $"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ScanStarted2")}");
                    });
            }
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.AppendText($"{Environment.NewLine}" + _languageControl.SelectedLanguage.GetString("ScanStarted1"));
            Task.WaitAll();
            Task.Factory.StartNew(() => SqlScan());
        }
        private void btnSingleIpAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isBtnSingleIpAddClick = true;
                if (!string.IsNullOrEmpty(txtSingleIPOctet1.Text) && !string.IsNullOrEmpty(txtSingleIPOctet2.Text) &&
                    !string.IsNullOrEmpty(txtSingleIPOctet3.Text) && !string.IsNullOrEmpty(txtSingleIPOctet4.Text) &&
                    !string.IsNullOrEmpty(txtSinglePort.Text))
                {
                    lstFoundedAddress.Items.Add(
                        $"{txtSingleIPOctet1.Text}.{txtSingleIPOctet2.Text}.{txtSingleIPOctet3.Text}.{txtSingleIPOctet4.Text}:{txtSinglePort.Text}");
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                FormGeneralControl();
            }
            
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
                    if(lstFoundedAddress.Items.Count > 0)
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
        private void txtStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtStatus.ScrollToEnd();
        }

        private void SqlScan()
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                if (tglMsSql.IsChecked == true)
                                {
                                    var ipRange = new IPorRangeAdjust
                                    {
                                        IpOctet1 = txtIPOctet1.Text,
                                        IpOctet2 = txtIPOctet2.Text,
                                        IpOctet3 = txtIPOctet3.Text,
                                        IpOctet4 = txtIPOctet4.Text,
                                        IpRange1 = txtIPOctetRange.Text,
                                        SqlType = "MSSQL",
                                        Delay = Convert.ToInt32(sldDelay.Value),
                                        TimeOut = Convert.ToInt32(sldTimeOut.Value)
                                    };
                                    ipRange.FindIpRange();
                                }
                                else if (tglMySql.IsChecked == true)
                                {
                                    var ipRange = new IPorRangeAdjust
                                    {
                                        IpOctet1 = txtIPOctet1.Text,
                                        IpOctet2 = txtIPOctet2.Text,
                                        IpOctet3 = txtIPOctet3.Text,
                                        IpOctet4 = txtIPOctet4.Text,
                                        IpRange1 = txtIPOctetRange.Text,
                                        SqlType = "MYSQL",
                                        Delay = Convert.ToInt32(sldDelay.Value),
                                        TimeOut = Convert.ToInt32(sldTimeOut.Value)
                                    };
                                    ipRange.FindIpRange();
                                }
                                var addresList = new FoundedSqlAddress();
                                addresList.SqlServerFoundAddressFile();
                                foreach (var t in addresList.AddressList)
                                {
                                    lstFoundedAddress.Items.Add(t);
                                }
                            });
            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (Action) delegate
                    {
                        txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message,
                            _languageControl.SelectedLanguage.GetString("GeneralError1"),
                            _languageControl.SelectedLanguage.GetString("GeneralError2")));
                    });

            }
            finally
            {
                FormGeneralControl();
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
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }

            this.Hide();
            var showAttackPanel = new WarSQLiAttack();
            showAttackPanel.Show();
        }
        private void btnAddSingleIp_Click(object sender, RoutedEventArgs e)
        {
            lstFoundedAddress.Items.Add(
                $"{txtSingleIPOctet1.Text}.{txtSingleIPOctet2.Text}.{txtSingleIPOctet3.Text}.{txtSingleIPOctet4.Text}:{txtSinglePort.Text}");
            FormGeneralControl();
        }
        private void tglMsSql_Checked(object sender, RoutedEventArgs e)
        {
            tglMySql.IsChecked = false;
        }
        private void tglMySql_Checked(object sender, RoutedEventArgs e)
        {
            tglMsSql.IsChecked = false;
        }
    }
}
