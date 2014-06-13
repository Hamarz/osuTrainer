﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace osuTrainer.Views
{
    /// <summary>
    ///     Interaction logic for NewestRanked.xaml
    /// </summary>
    public partial class Beatmaps : UserControl
    {
        private List<FeedItem> newBeatmaps;
        private DispatcherTimer timer;

        public Beatmaps()
        {
            InitializeComponent();
        }

        private void Beatmaps_OnInitialized(object sender, EventArgs e)
        {
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 1, 0)
            };
            timer.Tick += timer_Tick;
            timer_Tick(sender, e);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            const string url = "https://osu.ppy.sh/feed/ranked/";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            if (feed != null)
            {
                newBeatmaps = feed.Items.Select(item => new FeedItem
                {
                    BeatmapSetId = Regex.Match(item.Links.First().Uri.ToString(), @"\d+").Value,
                    Title = item.Title.Text,
                    Author = item.Authors.First().Email,
                    Link = item.Links.First().Uri.ToString(),
                    ThumbUri =
                        new Uri(GlobalVars.ThumbUrl + Regex.Match(item.Links.First().Uri.ToString(), @"\d+").Value +
                                "l.jpg"),
                    RankedDate = item.PublishDate.LocalDateTime
                }).AsParallel().ToList();
                UpdateBeatmapDisplay();
            }
        }

        private void UpdateBeatmapDisplay()
        {
            BeatmapSp.Children.Clear();

            foreach (FeedItem item in newBeatmaps)
            {
                var grid1 = new Grid();
                grid1.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(120)});
                grid1.ColumnDefinitions.Add(new ColumnDefinition());

                var grid2 = new Grid();
                grid2.RowDefinitions.Add(new RowDefinition {Height = new GridLength(25)});
                grid2.RowDefinitions.Add(new RowDefinition {Height = new GridLength(25)});
                grid2.RowDefinitions.Add(new RowDefinition {Height = new GridLength(25)});

                var text = new TextBlock
                {
                    Text = item.Title
                };
                grid2.Children.Add(text);
                Grid.SetRow(text, 0);

                text = new TextBlock();
                text.Text = "by " + item.Author;
                grid2.Children.Add(text);
                Grid.SetRow(text, 1);

                text = new TextBlock();
                TimeSpan diff = DateTime.Now - item.RankedDate;
                text.Text = "Ranked " + diff.Days + " days " + diff.Hours + " hours " + diff.Minutes + " minutes ago";
                grid2.Children.Add(text);
                Grid.SetRow(text, 2);


                grid1.Children.Add(grid2);
                Grid.SetColumn(grid2, 1);


                var image = new Image
                {
                    Width = 120,
                    Height = 90,
                    Source = new BitmapImage(item.ThumbUri)
                };
                var button = new Button
                {
                    Content = image,
                    Tag = item.Link
                };
                button.Click += ButtonOnClick;
                grid1.Children.Add(button);
                Grid.SetColumn(image, 0);

                BeatmapSp.Children.Add(grid1);
            }
        }

        private void ButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Process.Start((string) (sender as Button).Tag);
        }
    }

    internal class FeedItem
    {
        public string BeatmapSetId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public Uri ThumbUri { get; set; }
        public DateTime RankedDate { get; set; }
    }
}