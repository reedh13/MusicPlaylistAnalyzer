using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicPlaylistAnalyzer {
    class Program {

        static void Main(string[] args) {
            string data_path = AppendTxt(args[0]);
            string report_path = AppendTxt(args[1]);

            if (ValidArgs(args)) {
                List<Song> Songs = new List<Song>(TxtToList(data_path));
                if (Songs.Count > 0) {
                    GenerateReport(Songs, report_path);
                }
            }
        }

        // Checks the input arguments for validity
        public static bool ValidArgs(string[] args) {
            if (args.Length != 2) {
                Console.WriteLine("ERROR: This program requires exactly two arguments. Follow the template below.\n" +
                    "MusicPlaylistAnalyzer.exe <music_playlist_file_path> <report_file_path>");
                return false;
            } else if (!(File.Exists(args[0]))) {
                Console.WriteLine($"ERROR: Cannot find the file \"{args[0]}.\"");
                return false;
            } else {
                return true;
            }
        }

        // Append ".txt" to the filename if not already present
        public static string AppendTxt(string filename) {
            if (!(filename.EndsWith(".txt"))) {
                filename += ".txt";
            }
            return filename;
        }

        // Reads the TSV text file and returns the data as a list of Song objects
        public static List<Song> TxtToList(string filename) {
            List<Song> SongList = new List<Song>();
            List<Song> Default = new List<Song>();
            int counter = 1;
            int field_count = 0;

            try {
                using (var reader = new StreamReader(@filename)) {
                    while (!reader.EndOfStream) {
                        string line = reader.ReadLine();
                        string[] values = line.Split('\t');

                        foreach (var val in values) {
                            Console.WriteLine(val);
                        }

                        // Skip the header row and count the number of fields
                        if (counter == 1) {
                            field_count = values.Length;
                            counter++;
                            continue;
                        }

                        // Create exception if improper number of values
                        if (field_count != values.Length) {
                            throw new Exception($"Error reading file. Line {counter} (Song: {values[0]}) has {values.Length} fields instead of {field_count} fields.");
                        }

                        // Create exception if improper data types
                        if (!(values[0] is string & values[1] is string & values[2] is string & values[3] is string & IsNumeric(values[4]) & IsNumeric(values[5]) & IsNumeric(values[6]) & IsNumeric(values[7]))) {
                            throw new Exception($"Error reading file. Line {counter} (Song: {values[0]}) contains incorrect type of data.");
                        }

                        // Create a Song object for each row
                        SongList.Add(new Song(
                          values[0],
                          values[1],
                          values[2],
                          values[3],
                          Int32.Parse(values[4]),
                          Int32.Parse(values[5]),
                          Int32.Parse(values[6]),
                          Int32.Parse(values[7])));
                        counter++;
                    }
                }
                return SongList;

            } catch (Exception e) {
                Console.WriteLine($"ERROR: {e.Message}");
            }
            return Default;
        }

        // Run LINQ queries, build the output string, and write the output file
        public static void GenerateReport(List<Song> Songs, string report_path) {
            var Q1 = from song in Songs
                     where song.Plays >= 200
                     select song;

            var Q2 = (from song in Songs
                      where song.Genre == "Alternative"
                      select song).Count();

            var Q3 = (from song in Songs
                      where song.Genre == "Hip-Hop/Rap"
                      select song).Count();

            var Q4 = from song in Songs
                     where song.Album == "Welcome to the Fishbowl"
                     select song;

            var Q5 = from song in Songs
                     where song.Year < 1970
                     select song;

            var Q6 = from song in Songs
                     where song.Name.Length > 85
                     select song;

            var Q7_max = (from song in Songs
                          select song.Time).Max();

            var Q7_song = (from song in Songs
                           where song.Time == Q7_max
                           select song);

            StringBuilder sb = new StringBuilder();

            sb.Append("Music Playlist Report\n\n" +
                      "Songs that have received 200 or more plays:\n");

            foreach (var song in Q1) {
                sb.Append($"Name: {song.Name}, Artist: {song.Artist}, Album: {song.Album}, Genre: {song.Genre}, Size: {song.Size}, Time: {song.Time}, Year: {song.Year}, Plays: {song.Plays}\n");
            }

            sb.Append($"\nNumber of Alternative songs: {Q2}\n");

            sb.Append($"\nNumber of Hip-Hop/Rap songs: {Q3}\n");

            sb.Append("\nSongs from the album Welcome to the Fishbowl:\n");
            foreach (var song in Q4) {
                sb.Append($"Name: {song.Name}, Artist: {song.Artist}, Album: {song.Album}, Genre: {song.Genre}, Size: {song.Size}, Time: {song.Time}, Year: {song.Year}, Plays: {song.Plays}\n");
            }

            sb.Append("\nSongs from before 1970:\n");
            foreach (var song in Q5) {
                sb.Append($"Name: {song.Name}, Artist: {song.Artist}, Album: {song.Album}, Genre: {song.Genre}, Size: {song.Size}, Time: {song.Time}, Year: {song.Year}, Plays: {song.Plays}\n");
            }

            sb.Append("\nSong names longer than 85 characters:\n");
            foreach (var song in Q6) {
                sb.Append($"Name: {song.Name}, Artist: {song.Artist}, Album: {song.Album}, Genre: {song.Genre}, Size: {song.Size}, Time: {song.Time}, Year: {song.Year}, Plays: {song.Plays}\n");
            }

            sb.Append("\nLongest song:\n");
            foreach (var song in Q7_song) {
                sb.Append($"Name: {song.Name}, Artist: {song.Artist}, Album: {song.Album}, Genre: {song.Genre}, Size: {song.Size}, Time: {song.Time}, Year: {song.Year}, Plays: {song.Plays}\n");
            }

            // Console.WriteLine(sb);

            using (StreamWriter swriter = new StreamWriter(report_path)) {
                swriter.Write(sb.ToString());
            }

            Console.WriteLine($"Success! {report_path} has been created.");
        }

        // Tests if input can be converted to integer
        public static bool IsNumeric(string input) {
            try {
                int.Parse(input);
                return true;
            } catch {
                return false;
            }
        }
    }
}