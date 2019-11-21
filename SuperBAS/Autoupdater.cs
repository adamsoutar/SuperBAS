using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
// TODO: Determine if this stops us from using .NET native
using System.Reflection;
using System.Runtime.InteropServices;

namespace SuperBAS
{
    class Autoupdater
    {
        private struct Asset {
            public string name;
            public string browser_download_url;
        }
        private struct GitHubResponse {
            public Asset[] assets;
            public string tag_name;
        }

        // Set from Program.cs
        public static string thisVersion = "";

        public static void UpdateIfAvailable() {
            //try {
                var latest = GetLatestRelease();
                if (latest.tag_name == thisVersion) {
                    Console.WriteLine($"[info] SuperBAS ({thisVersion}) is up to date");
                    return;
                }

                Console.WriteLine($@"[info] SuperBAS {latest.tag_name} is now available.
       You have {thisVersion}. Would you like to update?");
                Console.Write("[y/n] >");

                var userResp = Console.ReadKey().KeyChar;
                if (userResp != 'y') return;

                var seeking = DetermineDesiredBinary();
                foreach (var asset in latest.assets) {
                    if (asset.name == seeking) {
                        var downloadedTo = DownloadAndReplace(asset.browser_download_url);
                        Console.WriteLine("[info] This version of SuperBAS has been overwritten and will now close.");
                        Console.Write("Press any key...");
                        Environment.Exit(0);
                    }
                }

                Console.WriteLine("[error] Couldn't find a release for your platform. Please seek one here:");
                Console.WriteLine("https://github.com/adamsoutar/SuperBAS/releases");
            /*} catch (Exception ex) {
                Console.WriteLine("[warn] Failed to check for updates");
            }*/
        }

        static string DownloadAndReplace (string newUrl) {
            string mePath = Assembly.GetExecutingAssembly().CodeBase;
            using (var client = new WebClient()) {
                client.DownloadFile(newUrl, mePath);
            }
            return mePath;
        }

        static GitHubResponse GetLatestRelease () {
            string apiUrl = "https://api.github.com/repos/adamsoutar/SuperBAS/releases/latest";

            var jsonText = "";
            using (var wc = new WebClient()) {
                wc.Headers.Add("Accept", "Accept: application/vnd.github.v3+json");
                wc.Headers.Add("User-Agent", "adamsoutar/SuperBAS");
                jsonText = wc.DownloadString(apiUrl);
            }

            return JsonSerializer.Deserialize<GitHubResponse>(jsonText);
        }

        static string DetermineDesiredBinary() {
            var seeking = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                seeking = "SuperBAS-macOS-x64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                seeking = "SuperBAS-linux-x64";
            }
            else
            {
                // Windows
                seeking = "SuperBAS-x";
                if (RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                seeking += "64";
                }
                else seeking += "86";
                seeking += ".exe";
            }
            return seeking;
        }
    }
}
