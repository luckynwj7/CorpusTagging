﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CorpusTagging
{
    public class TextListObject:TextBlock
    {
        private string sentenceName;
        public string SentenceName
        {
            get { return sentenceName; }
            set { sentenceName = value; }
        }
        private string realText;
        public string RealText
        {
            get { return realText; }
            set { realText = value; }
        }
        private string tagText;
        public string TagText
        {
            get { return tagText; }
            set { tagText = value; }
        }

       

        public TextListObject(string sentenceName, string realText)
        {
            this.sentenceName = sentenceName;
            this.realText = realText;
            this.FontSize = 15;
            this.Margin = new Thickness(5);
        }


    }
}
