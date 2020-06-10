﻿using System;
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
    /// TextInsertWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TextInsertWindow : Window
    {
        private static TextInsertWindow textInsertWin;
        private TextListObject txtObj;
        private int insertPosition;
        private int dataPosition;
        private TextInsertWindow()
        {
            InitializeComponent();
        }
        public static TextInsertWindow GetTextInsertWin(TextListObject txtObj)
        {
            if(textInsertWin is null)
            {
                textInsertWin = new TextInsertWindow();
            }
            textInsertWin.txtObj = txtObj;
            textInsertWin.insertPosition = App.TaggingJobWin.corpusListSt.Children.IndexOf(txtObj);
            textInsertWin.dataPosition = App.TaggingJobWin.TextList.IndexOf(txtObj);
            return textInsertWin;
            
        }

        private void prevInsertBtn_Click(object sender, RoutedEventArgs e)
        {
            TextListObject inputTxtObj = new TextListObject(txtObj.SentenceName, insertTextTxtBox.Text);
            App.TaggingJobWin.corpusListSt.Children.Insert(insertPosition, inputTxtObj);
            App.TaggingJobWin.TextList.Insert(insertPosition, inputTxtObj);
            App.TaggingJobWin.TextChangeEvent(inputTxtObj);
            this.Hide();
        }

        private void nextInsertBtn_Click(object sender, RoutedEventArgs e)
        {
            TextListObject inputTxtObj = new TextListObject(txtObj.SentenceName, insertTextTxtBox.Text);
            App.TaggingJobWin.corpusListSt.Children.Insert(insertPosition+1, inputTxtObj);
            App.TaggingJobWin.TextList.Insert(insertPosition+1, inputTxtObj);
            App.TaggingJobWin.TextChangeEvent(inputTxtObj);
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private bool ConditionCheck()
        {
            return true;
        }
    }
}