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

namespace CorpusTagging
{
    /// <summary>
    /// TextChangeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TextChangeWindow : Window
    {
        private static TextChangeWindow textChangeWin;
        private TextListObject txtObj;
        private TextChangeWindow()
        {
            InitializeComponent();
        }
        public static TextChangeWindow GetTextChangeWin(TextListObject txtObj)
        {
            if(textChangeWin is null)
            {
                textChangeWin = new TextChangeWindow();
            }
            textChangeWin.txtObj = txtObj;
            textChangeWin.changeTextTxtBox.Text = txtObj.RealText;
            return textChangeWin;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void changeTextBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeFinishAct();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                ChangeFinishAct();
            }
            else if(e.Key == Key.Escape)
            {
                this.Hide();
            }
        }

        private void ChangeFinishAct()
        {
            txtObj.RealText = changeTextTxtBox.Text;
            this.Hide();
            App.TaggingJobWin.TextChangeEvent(txtObj);
        }
    }
}
