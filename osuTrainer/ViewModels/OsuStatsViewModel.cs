﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using osuTrainer.Commands;
using osuTrainer.Properties;
using ServiceStack.Text;

namespace osuTrainer.ViewModels
{
    public class OsuStatsViewModel : ViewModelBase
    {
        public OsuStatsViewModel()
        {
            UpdateContent = "Update";
            LoadSettings();
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
            Match match = Regex.Match(json, @"""user_id"":""(.+?)"".+?""username"":""(.+?)""");
            _userId = Convert.ToInt32(match.Groups[1].Value);
            Username = match.Groups[2].Value;

            json =
                _client.DownloadString(GlobalVars.UserBestApi + ApiKey + "&u=" + _userId + GlobalVars.Mode +
                                       SelectedGameMode);
            var userBest = JsonSerializer.DeserializeFromString<List<UserBest>>(json);
            foreach (UserBest item in userBest)
            {
                UserScores.Add(item.Beatmap_Id);
            }

            json =
                _client.DownloadString(@"http://osustats.ezoweb.de/API/osuTrainer.php?mode=" + SelectedGameMode +
                                       @"&uid=" + _userId);
            var osuStatsBest = JsonSerializer.DeserializeFromString<List<OsuStatsBest>>(json);
            foreach (OsuStatsBest item in osuStatsBest)
            {
                UserScores.Add(item.Beatmap_Id);
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
            string statsjson = "";
            string enabledMods = SelectedModsToString();
            try
            {
                statsjson =
                    _client.DownloadString(@"http://osustats.ezoweb.de/API/osuTrainer.php?mode=" + SelectedGameMode +
                                           @"&pp_value=" + MinPp + @"&mod_only_selected=" +
                                           IsExclusiveCbChecked.ToString().ToLowerInvariant() + @"&mod_string=" +
                                           enabledMods);
            }
            catch (Exception)
            {
                IsWorking = false;
                UpdateContent = "Update";
                return scores;
            }
            if (statsjson.Length < 3)
            {
                IsWorking = false;
                UpdateContent = "Update";
                return scores;
            }
            var osuStatsScores = JsonSerializer.DeserializeFromString<List<OsuStatsScores>>(statsjson);
            osuStatsScores =
                osuStatsScores.GroupBy(e => new {e.Beatmap_Id, e.Enabled_Mods}).Select(g => g.First()).ToList();
            for (int i = 0; i < osuStatsScores.Count; i++)
            {
                if (UserScores.Contains(osuStatsScores[i].Beatmap_Id)) continue;
                if (IsFcOnlyCbChecked) if ((int) osuStatsScores[i].Rank > 3) continue;
                UserScores.Add(osuStatsScores[i].Beatmap_Id);
                double dtmodifier = 1.0;
                if (osuStatsScores[i].Enabled_Mods.HasFlag(GlobalVars.Mods.DT) ||
                    osuStatsScores[i].Enabled_Mods.HasFlag(GlobalVars.Mods.NC))
                {
                    dtmodifier = 1.5;
                }
                scores.Add(new ScoreInfo
                {
                    Mods = (osuStatsScores[i].Enabled_Mods & ~GlobalVars.Mods.Autoplay),
                    BeatmapName = osuStatsScores[i].Beatmap_Title,
                    Version = osuStatsScores[i].Beatmap_Version,
                    BeatmapCreator = osuStatsScores[i].Beatmap_Creator,
                    BeatmapArtist = osuStatsScores[i].Beatmap_Artist,
                    Bpm = Math.Truncate(osuStatsScores[i].Beatmap_Bpm*dtmodifier),
                    Pp = Math.Truncate(osuStatsScores[i].Pp_Value),
                    TotalTime = TimeSpan.FromSeconds(osuStatsScores[i].Beatmap_Total_Length).ToString(@"mm\:ss"),
                    DrainingTime = TimeSpan.FromSeconds(osuStatsScores[i].Beatmap_Hit_Length).ToString(@"mm\:ss"),
                    RankImage = GetRankImageUri(osuStatsScores[i].Rank),
                    BeatmapId = osuStatsScores[i].Beatmap_Id,
                    BeatmapSetId = osuStatsScores[i].Beatmap_SetId,
                    ThumbUrl = GlobalVars.ThumbUrl + osuStatsScores[i].Beatmap_SetId + @"l.jpg",
                });
                ScoresAdded++;
            }
            IsWorking = false;
            UpdateContent = "Update";
            return scores;
        }
    }

    internal class OsuStatsScores
    {
        public string Username { get; set; }

        public int Uid { get; set; }

        public GlobalVars.Mods Enabled_Mods { get; set; }

        public GlobalVars.Rank Rank { get; set; }

        public int Beatmap_Id { get; set; }

        public double Pp_Value { get; set; }

        public int Beatmap_SetId { get; set; }

        public double Beatmap_Diffrating { get; set; }

        public string Beatmap_Creator { get; set; }

        public double Beatmap_Bpm { get; set; }

        public string Beatmap_Title { get; set; }

        public string Beatmap_Artist { get; set; }

        public int Beatmap_Total_Length { get; set; }

        public int Beatmap_Hit_Length { get; set; }

        public string Beatmap_Version { get; set; }
    }

    internal class OsuStatsBest
    {
        public string Username { get; set; }

        public int Uid { get; set; }

        public GlobalVars.Mods Enabled_Mods { get; set; }

        public GlobalVars.Rank Rank { get; set; }

        public int Beatmap_Id { get; set; }

        public double PP_Value { get; set; }

        public int Beatmap_SetId { get; set; }

        public double Beatmap_Diffrating { get; set; }

        public string Beatmap_Creator { get; set; }

        public double Beatmap_BPM { get; set; }

        public string Beatmap_Title { get; set; }

        public string Beatmap_Artist { get; set; }

        public int Beatmap_Total_Length { get; set; }
    }
}