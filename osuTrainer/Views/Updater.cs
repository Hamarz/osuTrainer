using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace osuTrainer
{
    public static class Updater
    {
        public static async Task<Version> Check()
        {
            var github = new GitHubClient(new ProductHeaderValue("osuTrainer"));
            IReadOnlyList<Release> releases = await github.Release.GetAll("condone", "osuTrainer");
            return new Version(releases[0].Name);
        }
    }
}