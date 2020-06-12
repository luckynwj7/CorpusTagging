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
        public List<TextListObject> TextList
        {
            get { return textList; }
        }
        private List<ComboBoxItem> corpusComboList;
        private List<ToggleButton> tagTogBtnList;
        private string saveFilePath;
        private string saveFileText;
        private string selectedFileIndex;
        private int selectTextIndex;
        private int sentenceCount;

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
            startSentenceBtn.Background = Brushes.Yellow;
            startSentenceBtn.IsChecked = false;

            sentenceCount = 0;
        }
        public static TaggingJobWindow GetTaggingJobWin(string saveFilePath)
        {
            // 생성자의 역할을 대신함
            if(taggingJobWin is null)
            {
                taggingJobWin = new TaggingJobWindow(saveFilePath);
            }
            taggingJobWin.selectTextIndex = 0;
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
            this.corpusListSt.Children.Clear();
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
            textContent = textContent.Replace(".", StringResources.TempSentSpliter); // 점을 임시 문자열로 바꿈
            textContent = textContent.Replace(StringResources.SentSpliter,"."); // 문장 분리 문자열을 점으로 바꿈

            textContent = textContent.Replace("\r", " ");
            textContent = textContent.Replace("\n", " "); // 개행문자는 제외함
            string[] sentSplitText = textContent.Split('.'); // 문자열 나눔
            foreach(string splitSent in sentSplitText)
            {
                if(splitSent == "")
                {
                    // 아무 내용이 없으면 건너뜀
                    continue;
                }
                string resultSent = splitSent.Replace(StringResources.TempSentSpliter, ".");
                string[] dotSplitText = resultSent.Split(' ');
                string corpusName = "sent_" + sentenceCount.ToString();
                foreach (string text in dotSplitText)
                {
                    if (text != "" && text != " ")
                    {
                        // 값이 존재하는 경우에만 받아옴
                        TextListObject textObj = new TextListObject(corpusName, text);
                        textObj.Text = text + " (O)";
                        textObj.PreviewMouseLeftButtonDown += textObjPreviewMouseLeftButtonDown;
                        taggingJobWin.corpusListSt.Children.Add(textObj);
                        textList.Add(textObj); // 리스트에도 저장함
                    }
                }
                ComboBoxItem comboItem = new ComboBoxItem();
                comboItem.Content = corpusName;
                corpusComboList.Add(comboItem);
                corpusListCombo.SelectedItem = comboItem;
                SaveToCsvFile();
                sentenceCount++;
            }
        }

        private void textObjPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextListObject txtObj = sender as TextListObject;
            selectTextIndex = corpusListSt.Children.IndexOf(txtObj);
            ChangeSelectTextObj(selectTextIndex);
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
                    result += tagText + "\n";
                }
                else
                {
                    result += "O\n";
                }
                
            }
            result = result.Remove(result.Length - 1); // 마지막 문장 없애기
            try
            {
                System.IO.File.WriteAllText(saveFilePath, result, Encoding.GetEncoding("euc-kr"));
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("csv파일이 열려 있습니다. 닫고 실행해주세요.");
            }
            
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
            csvContent = csvContent.Replace("\n", ",");
            if(csvContent[0] == ',')
            {
                csvContent = csvContent.Remove(0, 1); // 첫 컴마는 없애기
            }
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
                        sentenceCount = corpusComboList.Count;
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
                    txtObj.PreviewMouseLeftButtonDown += textObjPreviewMouseLeftButtonDown;
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
            selectTextIndex = 0;
            ChangeSelectTextObj(selectTextIndex);
        }

        private void ChangeSelectTextObj(int index)
        {
            if(index >= corpusListSt.Children.Count)
            {
                // 오류 방지
                return;
            }
            selectTextIndex = index;
            foreach (TextListObject txtObj in corpusListSt.Children)
            {
                txtObj.Foreground = Brushes.Black;
            }
            (corpusListSt.Children[index] as TextListObject).Foreground = Brushes.Red;

        }
        private int ModulateBringIntoView(int index, int flagPlusMinus)
        {
            int maxStChild = corpusListSt.Children.Count-1;
            if (flagPlusMinus < 0)
            {
                index -= 4;
            }
            else if(flagPlusMinus > 0)
            {
                index += 4;
            }
            else
            {
                return 0;
            }
            
            if (index < 0)
            {
                index = 0;
            }
            else if (index > maxStChild)
            {
                index = maxStChild;
            }

            return index;
        }

        private void prevTextBtn_Click(object sender, RoutedEventArgs e)
        {
            MovePrevTextFile();
        }
        private void MovePrevTextFile()
        {
            int selectIndex = corpusListCombo.SelectedIndex - 1;
            if (selectIndex < 0)
            {
                selectIndex = corpusComboList.Count - 1;
            }
            ComboSelectChange(selectIndex);
        }


        private void nextTextBtn_Click(object sender, RoutedEventArgs e)
        {
            MoveNextTextFile();
        }
        private void MoveNextTextFile()
        {
            int selectIndex = corpusListCombo.SelectedIndex + 1;
            if (selectIndex >= corpusComboList.Count)
            {
                selectIndex = 0;
            }
            ComboSelectChange(selectIndex);
        }

        private void ComboSelectChange(int selectIndex)
        {
            selectedFileIndex = corpusComboList[selectIndex].Content.ToString();
            corpusListCombo.SelectedIndex = selectIndex;
            SelectTextFile();
        }

        private void SelectTextIndexMinus()
        {
            selectTextIndex--;
            if (selectTextIndex < 0)
            {
                selectTextIndex = corpusListSt.Children.Count - 1;
            }
            ChangeSelectTextObj(selectTextIndex);
            (corpusListSt.Children[ModulateBringIntoView(selectTextIndex, -1)] as TextListObject).BringIntoView(); // 해당 위치로 포커스가 가도록 함
        }
        private void SelectTextIndexPlus()
        {
            selectTextIndex++;
            if (selectTextIndex >= corpusListSt.Children.Count)
            {
                selectTextIndex = 0;
            }
            ChangeSelectTextObj(selectTextIndex);
            (corpusListSt.Children[ModulateBringIntoView(selectTextIndex, 1)] as TextListObject).BringIntoView(); // 해당 위치로 포커스가 가도록 함
        }
        private void beginSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            BeginSubmitAct();
        }
        private void BeginSubmitAct()
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
            InsideSubmitAct();
        }

        private void InsideSubmitAct()
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
            OutsideSubmitAct();
        }
        
        private void OutsideSubmitAct()
        {
            TextListObject txtObj = (corpusListSt.Children[selectTextIndex] as TextListObject);
            SubmitAct("O", txtObj);
        }

        private void SubmitAct(string submitMode, TextListObject txtObj)
        {
            if (startSentenceBtn.IsChecked == true)
            {
                SentenceAdd(txtObj);
            }
            InitializeToggleButton();
            txtObj.TagText = submitMode;
            SaveToCsvFile();
            ShowingTextUpdate(txtObj);
            SelectTextIndexPlus();

            // 문장시작 초기화
            startSentenceBtn.Background = Brushes.Yellow;
            startSentenceBtn.IsChecked = false;
        }

        private void SentenceAdd(TextListObject txtObj)
        {
            txtObj.Text = StringResources.SentenceStartFlag + txtObj.Text;
            int txtObjIndex = corpusListSt.Children.IndexOf(txtObj);
            for(int indexNum = txtObjIndex; indexNum < corpusListSt.Children.Count; indexNum++)
            {
                (corpusListSt.Children[indexNum] as TextListObject).SentenceName = "sent_" + sentenceCount;
                corpusListCombo.ItemsSource = taggingJobWin.corpusComboList;
            }
            sentenceCount++;
        }
        
        private void ShowingTextUpdate(TextListObject txtObj)
        {
            bool sentenceStartFlag = false;
            int flagLength = StringResources.SentenceStartFlag.Length;
            if (txtObj.Text.Length >= flagLength && txtObj.Text.Substring(0,flagLength) == StringResources.SentenceStartFlag) // 시작 문장임을 알려줌
            {
                sentenceStartFlag = true;
            }
            if (txtObj.TagText != "")
            {
                txtObj.Text = txtObj.RealText + " (" + txtObj.TagText + ")";
            }
            else
            {
                txtObj.Text = txtObj.RealText;
            }
            if (sentenceStartFlag)
            {
                txtObj.Text = StringResources.SentenceStartFlag + txtObj.Text;
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


        private void jobSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            TextListObject txtObj = (corpusListSt.Children[selectTextIndex] as TextListObject);
            SubmitAct("SAVE" + ToggleStrStatus(), txtObj);
        }

        private void insertNewTextBtn_Click(object sender, RoutedEventArgs e)
        {
            InsertNewTextBtnClick();
        }
        private void InsertNewTextBtnClick()
        {
            App.TextInsertWin = TextInsertWindow.GetTextInsertWin(corpusListSt.Children[selectTextIndex] as TextListObject);
            App.TextInsertWin.Show();
        }

        private void changeTextBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeTextBtnClick();
        }
        private void ChangeTextBtnClick()
        {
            App.TextChangeWin = TextChangeWindow.GetTextChangeWin(corpusListSt.Children[selectTextIndex] as TextListObject);
            App.TextChangeWin.Show();
        }

        public void TextChangeEvent(TextListObject txtObj)
        {
            ShowingTextUpdate(txtObj);
            SaveToCsvFile();
        }
        public void InputTextEvent(TextListObject txtObj)
        {
            txtObj.PreviewMouseLeftButtonDown += textObjPreviewMouseLeftButtonDown;
            TextChangeEvent(txtObj);
            ChangeSelectTextObj(corpusListSt.Children.IndexOf(txtObj));
        }

        private void deleteTextBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteTextBtnClick();
        }
        private void DeleteTextBtnClick()
        {

            corpusListSt.Children.RemoveAt(selectTextIndex);
            textList.RemoveAt(selectTextIndex);
            SaveToCsvFile();
            SelectTextIndexMinus();
            SelectTextIndexPlus();
        }

        private void moveSavePositionBtn_Click(object sender, RoutedEventArgs e)
        {
            int maxCount = textList.Count;
            TextListObject currentTxtObj = corpusListSt.Children[selectTextIndex] as TextListObject;
            for (int txtObjIndex = textList.IndexOf(currentTxtObj) + 1; txtObjIndex != textList.IndexOf(currentTxtObj); txtObjIndex++)
            {
                Console.WriteLine(txtObjIndex);
                if(textList[txtObjIndex].TagText == "SAVE")
                {

                    int innerMaxCount = corpusComboList.Count;
                    for(int comboIndex = corpusListCombo.SelectedIndex; comboIndex != corpusListCombo.SelectedIndex-1;comboIndex++)
                    {
                        if (corpusComboList[comboIndex].Content.ToString() == textList[txtObjIndex].SentenceName)
                        {
                            int resultIndex = comboIndex;
                            ComboSelectChange(resultIndex);
                            resultIndex = corpusListSt.Children.IndexOf(textList[txtObjIndex]);
                            ChangeSelectTextObj(resultIndex);
                            (corpusListSt.Children[resultIndex] as TextListObject).BringIntoView();
                            break;
                        }
                        else if(comboIndex >= innerMaxCount - 1)
                        {
                            comboIndex = -1;
                        }
                    }
                    break;
                }
                else if (txtObjIndex >= maxCount-1)
                {
                    txtObjIndex = -1;
                }
                else if (txtObjIndex == textList.IndexOf(currentTxtObj)-1 && currentTxtObj.TagText!="SAVE")
                {
                    MessageBox.Show("저장해 둔 책갈피가 없습니다.");
                    break;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad1)
            {
                BeginSubmitAct();
            }
            else if (e.Key == Key.NumPad2)
            {
                InsideSubmitAct();
            }
            else if(e.Key == Key.NumPad3)
            {
                OutsideSubmitAct();
            }
            else if (e.Key == Key.Up)
            {
                SelectTextIndexMinus();
            }
            else if (e.Key == Key.Down)
            {
                SelectTextIndexPlus();
            }
            else if(e.Key == Key.Q)
            {
                if (perTogBtn.IsChecked == false)
                {
                    perTogBtn.IsChecked = true;
                }
                else
                {
                    perTogBtn.IsChecked = false;
                }
                
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("perTogBtn");
                }
            }
            else if (e.Key == Key.W)
            {
                if (locTogBtn.IsChecked == false)
                {
                    locTogBtn.IsChecked = true;
                }
                else
                {
                    locTogBtn.IsChecked = false;
                }
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("locTogBtn");
                }
            }
            else if (e.Key == Key.E)
            {
                if (orgTogBtn.IsChecked == false)
                {
                    orgTogBtn.IsChecked = true;
                }
                else
                {
                    orgTogBtn.IsChecked = false;
                }
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("orgTogBtn");
                }
            }
            else if (e.Key == Key.R)
            {
                if (dtTogBtn.IsChecked == false)
                {
                    dtTogBtn.IsChecked = true;
                }
                else
                {
                    dtTogBtn.IsChecked = false;
                }
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("dtTogBtn");
                }
            }
            else if (e.Key == Key.T)
            {
                if (timTogBtn.IsChecked == false)
                {
                    timTogBtn.IsChecked = true;
                }
                else
                {
                    timTogBtn.IsChecked = false;
                }
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("timTogBtn");
                }
            }
            else if (e.Key == Key.A)
            {
                if (qtTogBtn.IsChecked == false)
                {
                    qtTogBtn.IsChecked = true;
                }
                else
                {
                    qtTogBtn.IsChecked = false;
                }
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("qtTogBtn");
                }
            }
            else if (e.Key == Key.S)
            {
                if (fdTogBtn.IsChecked == false)
                {
                    fdTogBtn.IsChecked = true;
                }
                else
                {
                    fdTogBtn.IsChecked = false;
                }
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("fdTogBtn");
                }
            }
            else if (e.Key == Key.D)
            {
                if (sprtTogBtn.IsChecked == false)
                {
                    sprtTogBtn.IsChecked = true;
                }
                else
                {
                    sprtTogBtn.IsChecked = false;
                }
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("sprtTogBtn");
                }
            }
            else if (e.Key == Key.F)
            {
                if (clthTogBtn.IsChecked == false)
                {
                    clthTogBtn.IsChecked = true;
                }
                else
                {
                    clthTogBtn.IsChecked = false;
                }
                if (ToggleCountStatus() > 1)
                {
                    InitializeToggleButton("clthTogBtn");
                }
            }
            else if (e.Key == Key.G)
            {
                if (startSentenceBtn.IsChecked == false)
                {
                    startSentenceBtn.IsChecked = true;
                }
                else
                {
                    startSentenceBtn.IsChecked = false;
                }
                
            }
            else if(e.Key == Key.F1)
            {
                InsertNewTextBtnClick();
            }
            else if (e.Key == Key.F2)
            {
                ChangeTextBtnClick();
            }
            else if (e.Key == Key.F3)
            {
                SplitTextBtnClick();
            }
            else if (e.Key == Key.F4)
            {
                DeleteTextBtnClick();
            }


            else if(e.Key == Key.F11)
            {
                MovePrevTextFile();
            }
            else if(e.Key == Key.F12)
            {
                MoveNextTextFile();
            }

        }

        private void removeTextFileBtn_Click(object sender, RoutedEventArgs e)
        {
            for(int txtIndex=0;txtIndex<textList.Count;txtIndex++)
            {
                if(textList[txtIndex].SentenceName == selectedFileIndex)
                {
                    textList.RemoveAt(txtIndex);
                    txtIndex--;
                }
            }
            int selectIndex = corpusListCombo.SelectedIndex;
            corpusComboList.RemoveAt(selectIndex);
            corpusListCombo.ItemsSource = null;
            corpusListCombo.ItemsSource = corpusComboList;

            if (corpusComboList.Count <= 0)
            {
                selectedFileIndex = "";
            }
            else
            {
                MovePrevTextFile();
            }
        }

        private void splitTextBtn_Click(object sender, RoutedEventArgs e)
        {
            SplitTextBtnClick();
        }
        private void SplitTextBtnClick()
        {
            App.TextSplitWin = TextSplitWindow.GetTextSplitWin(corpusListSt.Children[selectTextIndex] as TextListObject);
            App.TextSplitWin.Show();
        }
    }
}
