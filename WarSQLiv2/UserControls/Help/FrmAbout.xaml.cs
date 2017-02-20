using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
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
using WarSQLiv2.Exploitation.Control;

namespace WarSQLiv2.UserControls.Help
{
    /// <summary>
    /// Interaction logic for FrmAbout.xaml
    /// </summary>
    public partial class FrmAbout : Window
    {
        private readonly LanguageControl _languageControl = new LanguageControl();
        public FrmAbout()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate
            {
                _languageControl.FindLang();
                _languageControl.SelectedLanguage = new ResourceManager("WarSQLiv2.Language." + _languageControl.LoadedLang,
                                Assembly.GetExecutingAssembly());
                Title = _languageControl.SelectedLanguage.GetString("TitleAbout");
                lblVersion.Content = "Application Name: " + Application.ResourceAssembly.ToString();
                lblVersion.Content += Environment.NewLine + "Description: " + ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute), false)).Description;
                lblVersion.Content += Environment.NewLine + "Company: " + ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company;
                lblVersion.Content += Environment.NewLine + "Copyright: " + ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright;
                txtDescription.AppendText(string.Format(Environment.NewLine + "Bu program SQL Sunuculara şifre denemeleri yapan ve bu şifre denemelerinin başarılı olması halinde sistemi exploit edebilen bir araçtır.{0}", Environment.NewLine));
                txtDescription.AppendText($"Bu araç SQL veritabanı güvenliği denetimleri için geliştirilmiştir. Kötüye kullanılması durumunda geliştiricinin herhangi bir yasal yükümlülüğü bulunmamaktadır. Programı kullanan herkes bu şartı kabul etmiş sayılır.{Environment.NewLine}");
                txtDescription.AppendText($"Katkılarından dolayı Kriptondan yardıma koşan süpermene ve aşağıda adları yazılı saz arkadaşlarına teşekkür ederim.{Environment.NewLine}");
                txtDescription.AppendText($"- Hamza Şamlıoğlu{Environment.NewLine}");
                txtDescription.AppendText($"- Betül Erdem{Environment.NewLine}");
                txtDescription.AppendText($"- Muhammet Dilmaç{Environment.NewLine}");
                txtDescription.AppendText($"- Tolga Sezer{Environment.NewLine}");
            });
        }
    }
}
