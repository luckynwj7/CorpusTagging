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
    /// TextSplitWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TextSplitWindow : Window
    {
        private static TextSplitWindow textSplitWin;
        private TextListObject txtObj;
        private TextSplitWindow()
        {
            InitializeComponent();
        }
        public static TextSplitWindow GetTextSplitWin(TextListObject txtObj)
        {
            if(textSplitWin is null)
            {
                textSplitWin = new TextSplitWindow();
            }
            textSplitWin.splitTextTxtBox.Text = txtObj.RealText;
            textSplitWin.splitTextTxtBox.Focus();
            textSplitWin.txtObj = txtObj;
            return textSplitWin;
        }

        private void textSplitBtn_Click(object sender, RoutedEventArgs e)
        {
            TextSplitProcess();
        }
        private void TextSplitProcess()
        {
            int splitIndex = splitTextTxtBox.SelectionStart;
            string prevSplitStr = splitTextTxtBox.Text.Substring(0, splitIndex);
            string nextSplitStr = splitTextTxtBox.Text.Substring(splitIndex);

            // 이전거 변경하기
            txtObj.RealText = prevSplitStr;
            App.TaggingJobWin.TextChangeEvent(txtObj);

            // 다음거 추가하기
            TextListObject inputTxtObj = new TextListObject(txtObj.SentenceName, nextSplitStr);
            inputTxtObj.TagText = "O";
            int insertPosition = App.TaggingJobWin.corpusListSt.Children.IndexOf(txtObj) + 1;
            App.TaggingJobWin.corpusListSt.Children.Insert(insertPosition, inputTxtObj);
            App.TaggingJobWin.TextList.Insert(insertPosition, inputTxtObj);
            App.TaggingJobWin.InputTextEvent(inputTxtObj);
            splitTextTxtBox.Text = "";

            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                TextSplitProcess();
            }
            else if(e.Key == Key.Escape)
            {
                this.Hide();
            }
        }
    }
}
