using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CorpusTagging
{
    /// <summary>
    /// TaggingJobWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TaggingJobWindow : Window
    {
        private static TaggingJobWindow taggingJobWin;
        private List<TextListObject> textList;
        private List<ComboBoxItem> corpusComboList;
        private List<ToggleButton> tagTogBtnList;
        private string saveFilePath;
        private string saveFileText;
        private string selectedFileIndex;
        private int selectTextIndex;

        private TaggingJobWindow(string saveFilePath)
        {
            InitializeComponent();
            tagTogBtnList = new List<ToggleButton>()
            {
                perTogBtn, locTogBtn, orgTogBtn, dtTogBtn, qtTogBtn, timTogBtn, fdTogBtn, sprtTogBtn, clthTogBtn
            };
            foreach (ToggleButton btn in tagTogBtnList)
            {
                btn.Background = Brushes.Yellow;
                btn.IsChecked = false;
            }
        }
        public static TaggingJobWindow GetTaggingJobWin(string saveFilePath)
        {
            // 생성자의 역할을 대신함
            if(taggingJobWin is null)
            {
                taggingJobWin = new TaggingJobWindow(saveFilePath);
            }
            taggingJobWin.saveFilePath = saveFilePath;
            taggingJobWin.saveFileText = File.ReadAllText(saveFilePath, Encoding.GetEncoding("euc-kr"));

            taggingJobWin.textList = new List<TextListObject>();
            taggingJobWin.corpusComboList = new List<ComboBoxItem>();
            taggingJobWin.ReadCsvFile();
            taggingJobWin.corpusListCombo.ItemsSource = taggingJobWin.corpusComboList;
            return taggingJobWin;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void windowExitBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void selectOtherFileBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            App.FileSelectWin.Show();
        }

        private void addTextFileBtn_Click(object sender, RoutedEventArgs e)
        {
            string textFilePath = FileDialogHandler.FindFileDialogStr("txt");
            if (textFilePath == "" || textFilePath is null)
            {
                return;
            }
            //string textContent = File.ReadAllText(textFilePath, Encoding.GetEncoding("euc-kr"));
            string textContent = File.ReadAllText(textFilePath);
            string corpusName = FindFileNameFromPath(textFilePath);
            foreach(TextListObject txtObj in textList)
            {
                if(corpusName == txtObj.SentenceName)
                {
                    MessageBox.Show("이미 추가한 텍스트 파일입니다.");
                    return;
                }
            }
            textContent = textContent.Replace("\r", " ");
            textContent = textContent.Replace("\n", " "); // 개행문자는 제외함
            string[] dotSplitText = textContent.Split('.'); // 컴마로 먼저 나눔
            int indexNum = 0;
            foreach(string splitText in dotSplitText)
            {
                string[] blankSplit = splitText.Split(' '); // 띄어쓰기로 분리함
                foreach (string text in blankSplit)
                {
                    if (text != "" && text != " ")
                    {
                        // 값이 존재하는 경우에만 받아옴
                        TextListObject textObj = new TextListObject(corpusName + "_" + indexNum, text);
                        textObj.Text = text;
                        textObj.PreviewMouseLeftButtonDown += textObjPreviewMouseLeftButtonDown;
                        taggingJobWin.corpusListSt.Children.Add(textObj);
                        textList.Add(textObj); // 리스트에도 저장함
                    }
                }
                ComboBoxItem comboItem = new ComboBoxItem();
                comboItem.Content = corpusName + "_" + indexNum;
                corpusComboList.Add(comboItem);
                corpusListCombo.SelectedItem = comboItem;
                SaveToCsvFile();
            }
            
        }

        private void textObjPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("gg");
        }

        private string FindFileNameFromPath(string filePath)
        {
            string[] path = filePath.Split('\\');
            return (path[path.Length - 1]).Replace(".txt","");
        }

        private void SaveToCsvFile()
        {
            // CSV파일에 저장하는 함수. TextList에 있는 오브젝트에서 관여하기 때문에 매개변수가 필요하지 않음
            string result = StringResources.CsvHeader;
            string sentenceName = null;
            string realText = null;
            string tagText = null;

            foreach (TextListObject txtObj in textList)
            {
                // 파일 이름 받기
                if (sentenceName != txtObj.SentenceName)
                {
                    sentenceName = txtObj.SentenceName;
                    sentenceName = sentenceName.Replace("\"", "\"\"");
                    if (sentenceName.Contains(",") || sentenceName.Contains("\""))
                    {
                        sentenceName = "\"" + sentenceName + "\"";
                    }
                    result += (sentenceName + ",");
                }
                else
                {
                    result += ",";
                }

                // 보여지는 텍스트 받기
                realText = txtObj.RealText;
                realText = realText.Replace("\"", "\"\"");
                if (realText.Contains(",") || realText.Contains("\""))
                {
                    realText = "\"" + realText + "\"";
                }
                result += (realText + ",");

                // 지정된 태그 받기
                if (txtObj.TagText != null)
                {
                    tagText = txtObj.TagText;
                    tagText = tagText.Replace("\"", "\"\"");
                    if (tagText.Contains(",") || tagText.Contains("\""))
                    {
                        tagText = "\"" + tagText + "\"";
                    }
                    result += tagText + ",\n";
                }
                else
                {
                    result += ",\n";
                }
                
            }
            result = result.Remove(result.Length - 2); // 마지막 문장 없애기
            System.IO.File.WriteAllText(saveFilePath, result, Encoding.GetEncoding("euc-kr"));
        }

        private void ReadCsvFile()
        {
            int startCount = StringResources.CsvHeader.Length;
            if(startCount == saveFileText.Length)
            {
                // 빈 파일일 경우에
                return;
            }
            string csvContent = saveFileText.Substring(startCount, saveFileText.Length-startCount);

            csvContent = csvContent.Replace("\r", "");
            csvContent = csvContent.Replace("\n", "");
            csvContent = ChangeExceptionComma(csvContent);
            string[] csvSplitText = csvContent.Split(',');
            int indexNum = 0;
            string sentenceName = null;
            string realText = null;
            string tagText = null;
            string comboBoxItemName = null;
            TextListObject txtObj;
            foreach(string csvText in csvSplitText)
            {
                if (indexNum == 0)
                {
                    sentenceName = csvText.Replace(StringResources.ExceptionArea,",");
                    if (comboBoxItemName != sentenceName && sentenceName != "")
                    {
                        // 콤보 박스에 파일 이름 추가하기
                        ComboBoxItem comboBoxItem = new ComboBoxItem();
                        comboBoxItem.Content = sentenceName;
                        comboBoxItemName = sentenceName;
                        corpusComboList.Add(comboBoxItem);
                        corpusListCombo.SelectedItem = comboBoxItem;
                        selectedFileIndex = sentenceName;
                    }
                    if (sentenceName == "")
                    {
                        sentenceName = comboBoxItemName;
                    }
                }
                else if (indexNum == 1)
                {
                    realText = csvText.Replace(StringResources.ExceptionArea, ",");
                }
                else if(indexNum == 2)
                {
                    tagText = csvText.Replace(StringResources.ExceptionArea, ",");
                    txtObj = new TextListObject(sentenceName, realText);
                    if (txtObj.TagText != "")
                    {
                        txtObj.TagText = tagText;
                    }
                    ShowingTextUpdate(txtObj);

                    textList.Add(txtObj);
;                   sentenceName = null;
                    realText = null;
                    tagText = null;
                    txtObj = null;

                    indexNum = -1;
                }
                indexNum++;
            }

            SelectTextFile();

        }
        private string ChangeExceptionComma(string sentence)
        {
            // 파일 내에 속해있는 컴마를 예외코드로 바꿔주는 함수. 따옴표도 같이 처리해 줌
            bool splitFlag = false;
            for (int charIndex = 0; charIndex < sentence.Length - 1; charIndex++)
            {
                if (sentence[charIndex] == '"')
                {
                    if (splitFlag)
                    {
                        if (sentence[charIndex + 1] == ',')
                        {
                            sentence = sentence.Remove(charIndex, 1);
                            charIndex--;
                            splitFlag = false;
                        }
                        else
                        {
                            // 따옴표가 두 개라면 하나는 없앤다
                            sentence = sentence.Remove(charIndex+1, 1);
                        }
                    }
                    else
                    {
                        sentence = sentence.Remove(charIndex, 1);
                        charIndex--;
                        splitFlag = true;
                    }
                }
                else if (splitFlag && sentence[charIndex] == ',')
                {
                    sentence = sentence.Remove(charIndex, 1);
                    sentence = sentence.Insert(charIndex, StringResources.ExceptionArea);
                    charIndex += StringResources.ExceptionArea.Length-1;
                }
            }
            return sentence;
        }

        private void corpusListCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((sender as ComboBox).SelectedItem as ComboBoxItem) is null)
            {
                return; // 오류 방지
            }
            selectedFileIndex = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
            SelectTextFile();
        }

        private void SelectTextFile()
        {
            corpusListSt.Children.Clear();
            foreach (TextListObject txtObj in textList)
            {
                if (txtObj.SentenceName == selectedFileIndex)
                {
                    corpusListSt.Children.Add(txtObj);
                }
            }
            ChangeSelectTextObj(0);
        }

        private void ChangeSelectTextObj(int index)
        {
            if(index >= corpusListSt.Children.Count)
            {
                // 오류 방지
                return;
            }
            foreach(TextListObject txtObj in corpusListSt.Children)
            {
                txtObj.Foreground = Brushes.Black;
            }
            (corpusListSt.Children[index] as TextListObject).Foreground = Brushes.Red;
            
        }

        private void prevTextBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectTextIndexMinus();
        }


        private void nextTextBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectTextIndexPlus();
        }
        private void SelectTextIndexMinus()
        {
            selectTextIndex--;
            if (selectTextIndex < 0)
            {
                selectTextIndex = corpusListSt.Children.Count - 1;
            }
            ChangeSelectTextObj(selectTextIndex);
        }
        private void SelectTextIndexPlus()
        {
            selectTextIndex++;
            if (selectTextIndex >= corpusListSt.Children.Count)
            {
                selectTextIndex = 0;
            }
            ChangeSelectTextObj(selectTextIndex);
        }
        private void beginSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() != 1)
            {
                MessageBox.Show("태그 상태가 하나가 아니라면 이 옵션을 실행할 수 없습니다.");
                return;
            }
            TextListObject txtObj = (corpusListSt.Children[selectTextIndex] as TextListObject);
            SubmitAct("B-" + ToggleStrStatus(), txtObj);
        }

        private void insdeSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() != 1)
            {
                MessageBox.Show("태그 상태가 하나가 아니라면 이 옵션을 실행할 수 없습니다.");
                return;
            }
            TextListObject txtObj = (corpusListSt.Children[selectTextIndex] as TextListObject);
            SubmitAct("I-" + ToggleStrStatus(), txtObj);
        }

        private void outsideSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 0)
            {
                MessageBox.Show("태그 상태가 입력되어 있는 상태로는 \"해당없음\" 옵션을 실행할 수 없습니다.");
                return;
            }
            TextListObject txtObj = (corpusListSt.Children[selectTextIndex] as TextListObject);
            SubmitAct("0", txtObj);
        }

        private void SubmitAct(string submitMode, TextListObject txtObj)
        {
            InitializeToggleButton();
            txtObj.TagText = submitMode;
            SaveToCsvFile();
            ShowingTextUpdate(txtObj);
            SelectTextIndexPlus();
        }
        
        private void ShowingTextUpdate(TextListObject txtObj)
        {
            if (txtObj.TagText != "")
            {
                txtObj.Text = txtObj.RealText + " (" + txtObj.TagText + ")";
            }
            else
            {
                txtObj.Text = txtObj.RealText;
            }
        }

        private void InitializeToggleButton(string exceptionName = null)
        {
            foreach(ToggleButton btn in tagTogBtnList)
            {
                if (!(exceptionName != null && exceptionName == btn.Name)) //예외정보
                {
                    btn.Background = Brushes.Yellow;
                    btn.IsChecked = false;
                }
            }
        }
        private int ToggleCountStatus()
        {
            int toggleCount = 0;
            foreach(ToggleButton btn in tagTogBtnList)
            {
                if (btn.IsChecked == true) 
                {
                    toggleCount++;
                }
            }
            return toggleCount;
        }
        private string ToggleStrStatus()
        {
            foreach(ToggleButton btn in tagTogBtnList)
            {
                if (btn.IsChecked == true)
                { 
                    if(btn.Name == "perTogBtn")
                    {
                        return "PER";
                    }
                    else if (btn.Name == "locTogBtn")
                    {
                        return "LOC";
                    }
                    else if (btn.Name == "orgTogBtn")
                    {
                        return "ORG";
                    }
                    else if (btn.Name == "dtTogBtn")
                    {
                        return "DT";
                    }
                    else if (btn.Name == "timTogBtn")
                    {
                        return "TIM";
                    }
                    else if (btn.Name == "qtTogBtn")
                    {
                        return "QT";
                    }
                    else if (btn.Name == "fdTogBtn")
                    {
                        return "FD";
                    }
                    else if (btn.Name == "sprtTogBtn")
                    {
                        return "SPRT";
                    }
                    else if (btn.Name == "clthTogBtn")
                    {
                        return "CLTH";
                    }
                }
            }
            return null;
        }

        private void perTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("perTogBtn");
            }
        }

        private void orgTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("orgTogBtn");
            }
        }

        private void dtTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("dtTogBtn");
            }
        }

        private void timTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("timTogBtn");
            }
        }

        private void qtTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("qtTogBtn");
            }
        }

        private void sprtTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("sprtTogBtn");
            }
        }

        private void clthTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("clthTogBtn");
            }
        }

        private void fdTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("fdTogBtn");
            }
        }

        private void locTogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleCountStatus() > 1)
            {
                InitializeToggleButton("locTogBtn");
            }
        }

        private void removeTextFileBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
