using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CorpusTagging
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileSelectWindow : Window
    {
        private static FileSelectWindow fileSelectWin;
        public static FileSelectWindow FileSelectWin
        {
            get { 
                    if(fileSelectWin is null)
                    {
                        fileSelectWin = new FileSelectWindow();
                    }
                    WindowStartAct();
                    return fileSelectWin; 
                }
        }
        private FileSelectWindow()
        {
            InitializeComponent();
        }
        private static void WindowStartAct()
        {

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void findSaveCsvFileBtn_Click(object sender, RoutedEventArgs e)
        {
            saveCsvFileTxtBlock.Text = FileDialogHandler.FindFileDialogStr("csv");
        }

        private void createCsvFileBtn_Click(object sender, RoutedEventArgs e)
        {
            App.InputTextWin = InputTextWindow.InputTextWin;
            App.InputTextWin.Show();
        }

        private void currentFileCheckBtn_Click(object sender, RoutedEventArgs e)
        {
            if (saveCsvFileTxtBlock.Text != "")
            {
                App.TaggingJobWin = TaggingJobWindow.GetTaggingJobWin(saveCsvFileTxtBlock.Text); // 선택된 txt파일이 없이 진행
                this.Hide();
                App.TaggingJobWin.Show();
            }
            else
            {
                MessageBox.Show("저장 파일을 선택해주세요.");
            }
        }

    }
}
