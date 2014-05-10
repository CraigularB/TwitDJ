using System;
using System.Collections.Generic;
using iTunesLib;
using TweetSharp;
using System.Linq;

namespace TwitDJ
{
    public static class TwitterServer
    {
        static void Main()
        {
            StartServer();
        }

        private static void StartServer()
        {
            var serv = new TwitterService(AuthVars.ConsumerKey, AuthVars.ConsumerSecret);
            serv.AuthenticateWith(AuthVars.TokenPublic, AuthVars.TokenSecret);
            string song = null;
            string artist = null;
            string album = null;
            var iT = new iTunesApp();
            var currPl = (iT.LibrarySource.Playlists.ItemByName["C# Interface"] ??
                         iT.CreatePlaylist("C# Interface")) as IITUserPlaylist;
            if (currPl == null)
            {
                Console.WriteLine("Could not find or create playlist. Exiting");
                throw new Exception("Could not find or create playlist. Exiting");
            }

            TwitterStatus tw = null;
            while (true)
            {
                List<TwitterStatus> tweets = tw != null ? new List<TwitterStatus>(serv.ListTweetsMentioningMe(new ListTweetsMentioningMeOptions { SinceId = tw.Id })) : new List<TwitterStatus>(serv.ListTweetsMentioningMe(new ListTweetsMentioningMeOptions()));
                tweets = tweets.Where(s => s.Text.Substring(s.Text.IndexOf(" ", StringComparison.Ordinal) + 1).Replace("&amp;", "&").StartsWith("DJ:")).ToList();
                if (tweets.Count > 0)
                {
                    tweets.Reverse();
                    foreach (var tweet in tweets)
                    {
                        tw = tweet;
                        IITTrack t = null;
                        var tweetText = tweet.Text;
                        tweetText = tweetText.Substring(tweetText.IndexOf(" ", StringComparison.Ordinal) + 1).Replace("&amp;", "&");
                        if (tweetText.Contains("|"))
                        {
                            tweetText = tweetText.Substring(tweetText.IndexOf("DJ:", StringComparison.Ordinal) + "DJ:".Length);
                            var songParts = tweetText.Split('|');
                            foreach (var part in songParts)
                            {
                                Console.Write(part);
                                Console.Write("||");
                            }
                            Console.WriteLine();
                            song = songParts[1].Trim().ToUpper();
                            artist = songParts[0].Trim().ToUpper();
                            album = songParts.Length == 3 ? songParts[2].Trim().ToUpper() : "";
                            string searchText = song + " " + artist + " " + album;
                            var s = iT.LibraryPlaylist.Search(searchText, ITPlaylistSearchField.ITPlaylistSearchFieldVisible);
                            t = s.Cast<IITTrack>().First(t2 => t2.Name.ToUpper() == song && t2.Artist.ToUpper() == artist && (t2.Album.ToUpper() == album || album == ""));
                            Console.WriteLine(t.Artist + "||" + t.Album + "||" + t.Name);
                        }
                        else if (tweetText.ToUpper().Contains("SHUFFLE") || tweetText.ToUpper().Contains("RANDOM"))
                        {
                            var rnd = new Random(10);
                            var libSize = iT.LibraryPlaylist.Tracks.Count;
                            var idx = rnd.Next(libSize);
                            t = iT.LibraryPlaylist.Tracks.ItemByPlayOrder[idx];
                            song = t.Name;
                            artist = t.Artist;
                            album = t.Album;
                        }
                        var tInPl = currPl.Tracks;
                        if (t != null && (tInPl.ItemByName[song] == null || tInPl.ItemByName[artist] == null))
                        {
                            if (album != "")
                            {
                                if (tInPl.ItemByName[album] == null)
                                    currPl.AddTrack(t);
                            }
                            else
                            {
                                currPl.AddTrack(t);
                            }
                        }
                        Console.WriteLine("\n**********************");
                    }
                    Console.WriteLine("Waiting 30 seconds before checking again");
                }
                else
                {
                    Console.WriteLine("No tweets found, either new or old, waiting 30sec");
                }
                System.Threading.Thread.Sleep(30000);
            }
        }
    }
}
