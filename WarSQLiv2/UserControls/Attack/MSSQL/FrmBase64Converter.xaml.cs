using System.Windows;
using WarSQLiv2.Exploitation.Control;
using WarSQLiv2.Exploitation.PostExploitation;

namespace WarSQLiv2.UserControls.Attack.MSSQL
{
    /// <summary>
    /// Interaction logic for FrmBase64Converter.xaml
    /// </summary>
    public partial class FrmBase64Converter : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public FrmBase64Converter()
        {
            InitializeComponent();
        }
        private void BtnConvert_OnClick(object sender, RoutedEventArgs e)
        {
            txtBase64.Text = EncodeBase64.ConvertTextToBase64(txtClearText.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _languageControl.FindLang();
            btnConvert.Content = _languageControl.SelectedLanguage.GetString("ButtonConvertBase64");
            lblInput.Content = _languageControl.SelectedLanguage.GetString("LabelBase64Input");
            lblOutput.Content = _languageControl.SelectedLanguage.GetString("LabelBase64Output");
            Title = _languageControl.SelectedLanguage.GetString("TitleBase64Encoder");
        }
    }
}
