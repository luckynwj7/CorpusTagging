using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    /// InputTextWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputTextWindow : Window
    {
        private static InputTextWindow inputTextWin;
        public static InputTextWindow InputTextWin
        {
            get
            {
                if (inputTextWin is null)
                {
                    inputTextWin = new InputTextWindow();
                }
                WindowStartAct();
                return inputTextWin;
            }
        }
        private InputTextWindow()
        {
            InitializeComponent();
        }
        private static void WindowStartAct()
        {
            inputTextWin.saveFileNameTxtBox.Text = "";
        }

        private void savePathSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (saveFileNameTxtBox.Text == "")
            {
                MessageBox.Show("파일 이름을 입력해주세요");
                return;
            }
            else if (!FileNameCheck(saveFileNameTxtBox.Text))
            {
                MessageBox.Show("파일 이름에 들어갈 수 없는 문자가 포함되어 있습니다.");
                return;
            }
            while (!FindDirectory())
            {
                // 적당한 결과가 나올 때까지 다이얼로그 창 무한 반복시키기
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private bool FindDirectory()
        {
            // 완벽하게 종료했으면 true, 그렇지 아니하면 false를 반환하여 계속 실행함
            string savePath = FileDialogHandler.FindDirectoryDialogStr();
            if (!(savePath == null || savePath == "")) // 다이얼로그 창 중단하는 경우를 방지
            {
                savePath += ("\\" + saveFileNameTxtBox.Text + ".csv");
                string result = StringResources.CsvHeader;
                FileInfo fileInfo = new FileInfo(savePath);
                if (fileInfo.Exists)
                {
                    MessageBoxResult msgResult = MessageBox.Show("이미 파일이 존재합니다. 덮어 씌우겠습니까?", "경고", MessageBoxButton.YesNoCancel);
                    if (msgResult == MessageBoxResult.Yes)
                    {
                        FinishWindow(savePath, result);
                        return true;
                    }
                    else if(msgResult == MessageBoxResult.No)
                    {
                        return true;
                    }
                    else if(msgResult == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    // 덮어 씌울 파일이 없을 경우
                    FinishWindow(savePath, result);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        private bool FileNameCheck(string fileName)
        {
            // 이름으로 짓지 못하는 문자들을 제외시키기
            List<string> refuseFileNameList = new List<string>()
            {
                "\\", "/", ":", "*", "?", "\"", "<", ">", "|"
            };
            foreach(string limit in refuseFileNameList)
            {
                if (fileName.Contains(limit))
                {
                    return false;
                }
            }
            return true;
        }

        private void FinishWindow(string savePath, string result)
        {
            App.FileSelectWin.saveCsvFileTxtBlock.Text = savePath;
            System.IO.File.WriteAllText(savePath, result, Encoding.GetEncoding("euc-kr"));
            this.Hide();
        }
    }
}
