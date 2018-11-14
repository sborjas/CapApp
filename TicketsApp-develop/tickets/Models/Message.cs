﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;

namespace tickets.Models
{
    public class Message : ObservableObject
    {
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        string _text;
        public List<(string, byte[])> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }

        }
        List<(string, byte[])> _files;
        public DateTime MessageDateTime
        {
            get { return _messageDateTime; }
            set { SetProperty(ref _messageDateTime, value); }
        }

        DateTime _messageDateTime;

        public string TimeDisplay => MessageDateTime.ToLocalTime().ToString();

        public bool IsTextIn
        {
            get { return _isTextIn; }
            set { SetProperty(ref _isTextIn, value); }
        }
        bool _isTextIn;
        public bool IsAdjIn
        {
            get { return _isAdjIn; }
            set { SetProperty(ref _isAdjIn, value); }
        }
        bool _isAdjIn;
    }
}
