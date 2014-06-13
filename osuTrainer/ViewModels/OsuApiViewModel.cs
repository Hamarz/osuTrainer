﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using osuTrainer.Commands;
using osuTrainer.Properties;
using ServiceStack.Text;

namespace osuTrainer.ViewModels
{
    public class OsuApiViewModel : ViewModelBase
    {
        private int[] ctbIds;
        private double curUserPp;
        private int curUserPpRank;
        private int[] maniaIds;
        private int[] standardIds;
        private int[] taikoIds;

        public OsuApiViewModel()
        {
            UpdateContent = "Update";
            Scores = new ObservableCollection<ScoreInfo>();
            LoadSettings();
            LoadUserIds();
            _client = new CustomWebClient();
            UpdateCommand = new UpdateCommand(this);
            OpenBeatmapLinkCommand = new OpenBeatmapLinkCommand(this);
            CopyLinkCommand = new CopyLinkCommand(this);
            DownloadCommand = new DownloadCommand(this);
            DownloadOdCommand = new DownloadOdCommand(this);
            DownloadBcCommand = new DownloadBcCommand(this);
        }

        private void LoadSettings()
        {
            Username = Settings.Default.Username;
            ApiKey = Settings.Default.ApiKey;
            MinPp = Settings.Default.MinPp;
            SelectedGameMode = Settings.Default.GameMode;
            IsDoubletimeCbChecked = Settings.Default.IsDoubletimeCbChecked;
            IsHardrockCbChecked = Settings.Default.IsHardrockCbChecked;
            IsHiddenCbChecked = Settings.Default.IsHiddenCbChecked;
            IsFlashlightCbChecked = Settings.Default.IsFlashlightCbChecked;
            IsFcOnlyCbChecked = Settings.Default.IsFcOnlyCbChecked;
            IsExclusiveCbChecked = Settings.Default.IsExclusiveCbChecked;
        }

        private void LoadUserIds()
        {
            var formatter = new BinaryFormatter();
            standardIds =
                (int[])
                    formatter.Deserialize(
                        Application.GetResourceStream(new Uri("Resources/standard", UriKind.Relative)).Stream);
            taikoIds =
                (int[])
                    formatter.Deserialize(
                        Application.GetResourceStream(new Uri("Resources/taiko", UriKind.Relative)).Stream);
            ctbIds =
                (int[])
                    formatter.Deserialize(
                        Application.GetResourceStream(new Uri("Resources/ctb", UriKind.Relative)).Stream);
            maniaIds =
                (int[])
                    formatter.Deserialize(
                        Application.GetResourceStream(new Uri("Resources/mania", UriKind.Relative)).Stream);
        }

        private bool GetUserBest()
        {
            UserScores = new List<int>();
            string json =
                _client.DownloadString(GlobalVars.UserApi + ApiKey + "&u=" + Username + GlobalVars.Mode +
                                       SelectedGameMode);
            if (json.Length < 33)
            {
                return false;
            }
            //TODO: fix error if there are null values
            Match match = Regex.Match(json,
                @"""user_id"":""(.+?)"".+?""username"":""(.+?)"".+?""pp_rank"":""(.+?)"".+?""pp_raw"":""(.+?)""");
            _userId = Convert.ToInt32(match.Groups[1].Value);
            Username = match.Groups[2].Value;
            curUserPpRank = Convert.ToInt32(match.Groups[3].Value);
            curUserPp = Convert.ToDouble(match.Groups[4].Value, CultureInfo.InvariantCulture);

            json =
                _client.DownloadString(GlobalVars.UserBestApi + ApiKey + "&u=" + _userId + GlobalVars.Mode +
                                       SelectedGameMode);
            var userBest = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
            foreach (UserBest item in userBest)
            {
                UserScores.Add(item.Beatmap_Id);
            }

            json = "";
            try
            {
                json =
                    _client.DownloadString(@"http://osustats.ezoweb.de/API/osuTrainer.php?mode=" + SelectedGameMode +
                                           @"&uid=" + _userId);
            }
            catch (Exception)
            {
            }

            var osuStatsBest = JsonSerializer.DeserializeFromString<List<OsuStatsBest>>(json);
            if (osuStatsBest != null)
            {
                foreach (OsuStatsBest item in osuStatsBest)
                {
                    UserScores.Add(item.Beatmap_Id);
                }
            }
            return true;
        }

        protected override ObservableCollection<ScoreInfo> GetScores()
        {
            IsWorking = true;
            UpdateContent = "Updating";
            var scores = new ObservableCollection<ScoreInfo>();
            if (!GetUserBest())
            {
                IsWorking = false;
                UpdateContent = "Update";
                MessageBox.Show("Wrong API key or username.");
                return scores;
            }
            string json = "";
            GlobalVars.Mods mods = SelectedModsToEnum();
            int[] userids;
            int startid;
            switch (SelectedGameMode)
            {
                case 0:
                    userids = standardIds;
                    startid = curUserPp < 200
                        ? 12843
                        : curUserPpRank < 5001
                            ? curUserPpRank - 2
                            : FindStartingUser(curUserPp, userids);
                    break;

                case 1:
                    userids = taikoIds;
                    startid = curUserPp < 200
                        ? 6806
                        : curUserPpRank < 5001
                            ? curUserPpRank - 2
                            : FindStartingUser(curUserPp, userids);
                    break;

                case 2:
                    userids = ctbIds;
                    startid = curUserPp < 200
                        ? 7638
                        : curUserPpRank < 5001
                            ? curUserPpRank - 2
                            : FindStartingUser(curUserPp, userids);
                    break;

                case 3:
                    userids = maniaIds;
                    startid = curUserPp < 200
                        ? 7569
                        : curUserPpRank < 5001
                            ? curUserPpRank - 2
                            : FindStartingUser(curUserPp, userids);
                    break;
                default:
                    IsWorking = false;
                    UpdateContent = "Update";
                    return scores;
            }

            Stopwatch sw = Stopwatch.StartNew();
            var maxDuration = new TimeSpan(0, 0, 10);
            while (sw.Elapsed < maxDuration)
            {
                if (startid < 0) return scores;
                json =
                    _client.DownloadString(GlobalVars.UserBestApi + ApiKey + "&u=" + userids[startid] + GlobalVars.Mode +
                                           SelectedGameMode);
                startid -= 1;
                var userBestList = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
                for (int j = 0; j < userBestList.Count; j++)
                {
                    if (userBestList[j].PP < MinPp) break;
                    if (IsFcOnlyCbChecked) if ((int) userBestList[j].Rank > 3) continue;
                    userBestList[j].Enabled_Mods &=
                        ~(GlobalVars.Mods.NV | GlobalVars.Mods.Perfect | GlobalVars.Mods.SD | GlobalVars.Mods.SpunOut);
                    if ((!IsExclusiveCbChecked || userBestList[j].Enabled_Mods != mods) &&
                        (IsExclusiveCbChecked || !userBestList[j].Enabled_Mods.HasFlag(mods))) continue;
                    if (UserScores.Contains(userBestList[j].Beatmap_Id)) continue;
                    var beatmap =
                        JsonSerializer.DeserializeFromString<List<Beatmap>>(
                            _client.DownloadString(GlobalVars.BeatmapApi + ApiKey + "&b=" + userBestList[j].Beatmap_Id));
                    double dtmodifier = 1.0;
                    if (userBestList[j].Enabled_Mods.HasFlag(GlobalVars.Mods.DT) ||
                        userBestList[j].Enabled_Mods.HasFlag(GlobalVars.Mods.NC))
                    {
                        dtmodifier = 1.5;
                    }
                    UserScores.Add(beatmap.First().Beatmap_Id);
                    scores.Add(new ScoreInfo
                    {
                        Accuracy = Math.Round(
                            GetAccuracy(userBestList[j].Count50, userBestList[j].Count100, userBestList[j].Count300,
                                userBestList[j].CountMiss, userBestList[j].CountKatu, userBestList[j].CountGeki), 2),
                        BeatmapTitle = beatmap.First().Title,
                        Version = beatmap.First().Version,
                        BeatmapCreator = beatmap.First().Creator,
                        BeatmapArtist = beatmap.First().Artist,
                        Mods = userBestList[j].Enabled_Mods,
                        Bpm = (int) Math.Truncate(beatmap.First().Bpm*dtmodifier),
                        Difficultyrating = Math.Round(beatmap.First().Difficultyrating, 2),
                        Pp = (int) Math.Truncate(userBestList[j].PP),
                        RankImage = GetRankImageUri(userBestList[j].Rank),
                        BeatmapId = beatmap.First().Beatmap_Id,
                        BeatmapSetId = beatmap.First().BeatmapSet_Id,
                        ThumbUrl = GlobalVars.ThumbUrl + beatmap.First().BeatmapSet_Id + @"l.jpg",
                        TotalTime = TimeSpan.FromSeconds(beatmap.First().Total_Length).ToString(@"mm\:ss"),
                        DrainingTime = TimeSpan.FromSeconds(beatmap.First().Hit_Length).ToString(@"mm\:ss"),
                        ApprovedDate = beatmap.First().Approved_Date.ToShortDateString(),
                        DiffApproach = beatmap.First().Diff_Approach,
                        DiffDrain = beatmap.First().Diff_Drain,
                        DiffOverall = beatmap.First().Diff_Overall,
                        DiffSize = beatmap.First().Diff_Size,
                        Mode = beatmap.First().Mode.ToString()
                    });
                    ScoresAdded++;
                }
            }
            IsWorking = false;
            UpdateContent = "Update";
            return scores;
        }

        private int FindStartingUser(double targetpp, int[] ids)
        {
            int low = 0;
            int high = ids.Length - 1;
            int midpoint = 0;
            int iterations = 0;
            while (low < high && iterations < 7)
            {
                midpoint = low + (high - low)/2;
                double midUserPp = GetUserPp(ids[midpoint]);
                if (targetpp > midUserPp)
                {
                    high = midpoint - 1;
                }
                else if (midUserPp - targetpp < 100)
                {
                    return midpoint;
                }
                else
                {
                    low = midpoint - 1;
                }
                iterations++;
            }
            return midpoint;
        }

        private double GetUserPp(int userId)
        {
            string json =
                _client.DownloadString(GlobalVars.UserApi + ApiKey + "&u=" + userId + GlobalVars.Mode + SelectedGameMode);
            Match match = Regex.Match(json, @"pp_raw"":""(.+?)""");
            return Convert.ToDouble(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }
    }

    internal class OsuApiScores : IComparable<OsuApiScores>
    {
        public int Beatmap_Id { get; set; }

        public int Score { get; set; }

        public int MaxCombo { get; set; }

        public int Count50 { get; set; }

        public int Count100 { get; set; }

        public int Count300 { get; set; }

        public int CountMiss { get; set; }

        public int CountKatu { get; set; }

        public int CountGeki { get; set; }

        public int Perfect { get; set; }

        public GlobalVars.Mods Enabled_Mods { get; set; }

        public int User_Id { get; set; }

        public DateTime Date { get; set; }

        public GlobalVars.Rank Rank { get; set; }

        public double Pp { get; set; }

        public int CompareTo(OsuApiScores other)
        {
            return Beatmap_Id.CompareTo(other.Beatmap_Id);
        }
    }
}