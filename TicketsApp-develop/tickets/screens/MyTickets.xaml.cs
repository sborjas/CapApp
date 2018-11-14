﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using tickets.API;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace tickets
{
    public partial class MyTickets : ContentPage
    {
        private Server server = new Server();
        ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();


        public MyTickets()
        {
            InitializeComponent();
            this.BindingContext = this;

            var newTicket = new ToolbarItem
            {
                Icon = "nuevo.png",
                Command = new Command(async (s) => await Navigation.PushAsync(new SendTicket())),
                Order = ToolbarItemOrder.Primary
                
            };

            var settings = new ToolbarItem
            {
                Text = "Ajustes",
                Command = new Command(async (s) => await Navigation.PushAsync(new AppSettingsPage())),
                Order = ToolbarItemOrder.Secondary
            };

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    ToolbarItems.Add(newTicket);
                    break;
                case Device.Android:
                    ToolbarItems.Add(newTicket);
                    ToolbarItems.Add(settings);
                    break;
                case Device.UWP:
                    ToolbarItems.Add(newTicket);
                    ToolbarItems.Add(settings);
                    break;
            }
            //TicketsListView.BeginRefresh();
            //GetTickets();
            TicketsListView.ItemsSource = tickets;
        }

        protected override async void OnAppearing()
        {
            GetTickets();
        }

        async void goToViewTicket(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var ticket = tickets.FirstOrDefault(t => t.ID == ((Ticket)e.SelectedItem).ID);
                if (ticket != null)
                {
                    Debug.WriteLine("Opening messages for ticket with id = " + ticket.ID);
                    ticket.Date = await server.getUpdateDate(ticket.ID);
                    ticket.Image = "";

                    ticket.OpenImage = "";

                    await App.Database.UpdateTicket(ticket);
                    await Navigation.PushAsync(new chatTicket()
                    {
                        BindingContext = ticket.ID
                    });
                    TicketsListView.SelectedItem = null;
                }
                else
                {
                    Debug.WriteLine("Ticket is null");
                }
            }
        }

        private async void TicketsListView_Refreshing(object sender, EventArgs e)
        {

            //TicketsListView.ItemsSource = await GetTickets();
            GetTickets();
            TicketsListView.EndRefresh();
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            //List<Ticket> tickets = await App.Database.GetTicketsAsync(App.Database.GetCurrentUserNotAsync());

            if (!String.IsNullOrWhiteSpace(e.NewTextValue))
            {
                var showTickets = tickets.Where(t => t.Subject.Contains(e.NewTextValue)).ToList();
                TicketsListView.ItemsSource = showTickets;
            }
            else
            {
                TicketsListView.ItemsSource = tickets;
            }
        }

        async void GetTickets()
        {
            List<Ticket> dbtickets = await App.Database.GetTicketsAsync(App.Database.GetCurrentUserNotAsync());
            dbtickets = new List<Ticket>(dbtickets.OrderByDescending(t => DateTime.ParseExact(t.Date, "yyyy-MM-dd HH:mm:ss",
                                                                                              System.Globalization.CultureInfo.InvariantCulture)));
            for (int i = 0; i < dbtickets.Count; i++)
            {
                String updateDate = await server.getUpdateDate(dbtickets[i].ID);
                if (!updateDate.Equals(dbtickets[i].Date))
                {
                    dbtickets[i].Image = "https://cdn.pixabay.com/photo/2015/12/16/17/41/bell-1096280_640.png";
                    dbtickets[i].Date = updateDate;
                    await App.Database.UpdateTicket(dbtickets[i]);
                }


                bool open = await server.getOpenTicket(dbtickets[i].ID);
                Console.WriteLine("Recibiendo del sevidor: "+open.ToString());
                if(!open)
                {
                    dbtickets[i].OpenImage = "https://cdn.pixabay.com/photo/2015/12/08/19/08/castle-1083570_960_720.png";
                    dbtickets[i].Open = open;
                    await App.Database.UpdateTicket(dbtickets[i]);
                }

                var exists = tickets.FirstOrDefault(t => t.ID == dbtickets[i].ID);

                if (exists == null) // if no ticket was found with that id
                {
                    tickets.Add(dbtickets[i]);
                }
                else
                {
                    exists.Image = dbtickets[i].Image;

                    exists.OpenImage = dbtickets[i].OpenImage;


                    if (!updateDate.Equals(exists))
                    {
                        exists.Date = updateDate;
                    }

                }
            }


            tickets = new ObservableCollection<Ticket>(
                tickets.OrderByDescending(t => DateTime.ParseExact(t.Date, "yyyy-MM-dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture)
            ).ToList());
            TicketsListView.ItemsSource = tickets;

            //tickets.OrderByDescending(t => DateTime.ParseExact(t.Date, "yyyy-MM-dd HH:mm:ss",
            //System.Globalization.CultureInfo.InvariantCulture));
            //Date=2018-09-05 17:02:41
            //tickets = from item in tickets orderby item.Date select item;
            //tickets = new ObservableCollection<Ticket>(dbtickets);
            //if (String.IsNullOrWhiteSpace(searchText))
            //    return tickets.OrderByDescending(c => c.Date).ToList();
            //return tickets.Where(c => c.Subject.StartsWith(searchText)).OrderByDescending(c => c.Date).ToList();
        }
    }
}
