﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MimeKit;
using WikiLeaks.Models;

namespace WikiLeaks.ViewModels{
    public interface IMainWindowViewModel{
        ICommand NextCommand { get; }
        ICommand PreviousCommand { get; }
        ICommand RefreshCommand { get; }
        ICommand WikileaksCommand { get; }

        InternetAddressList From { get; set; }
        InternetAddressList To { get; set; }
        InternetAddressList Cc { get; set; }
        ObservableCollection<Attachment> Attachments { get; set; }

        string Subject { get; set; }
        DateTimeOffset Date { get; set; }
        int DocumentNo { get; set; }
        string HtmlString { get; set; }
        bool? Validated { get; set; }
    }
}