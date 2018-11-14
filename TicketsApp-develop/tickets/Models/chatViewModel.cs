using System;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms;
using MvvmHelpers;
using tickets.API;
using System.Collections.Generic;

namespace tickets.Models
{
    public class chatViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Message> ListMessages { get; }
        public ICommand SendCommand { get; set; }
        private Server server = new Server();
        private string ticketID;
        private chatTicket chatfile;

        public chatViewModel(string ticket)
        {
            this.ticketID = ticket;
            ListMessages = new ObservableRangeCollection<Message>();
            SendCommand = new Command(() =>
            {
                if (!String.IsNullOrWhiteSpace(OutText))
                {
                    var message = new Message
                    {
                        Text = OutText,
                        Files = chatfile.Files,
                        IsTextIn = false,
                        IsAdjIn = false,
                        MessageDateTime = DateTime.Now
                    };
                    
                    
                    sendMessage(message);
                    //ListMessages.Add(message);
                    //OutText = "";
                }
                  
            });
            
        }
        public async void sendMessage(Message message)
        {
            string status = await server.replyTicket(message.Text,message.Files, this.ticketID);
            if(status.Equals("ok"))
            {
                ListMessages.Add(message);
                OutText = "";
            }
            else
            {
                //OutText = this.ticketID;
            }
        }



        public string OutText
        {
            get { return _outText; }
            set { SetProperty(ref _outText, value); }
        }
        string _outText = string.Empty;


    }
    
}
