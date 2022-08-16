using System;
using System.IO;
using System.Linq;
using OctanGames.Map;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;
using File = UnityEngine.Windows.File;

namespace OctanGames
{
    public static class MapParser
    {
        public static MapData ParseMapData(string directory, string file)
        {
            string path = Path.Combine(directory, file);
            if (!Directory.Exists(directory))
            {
                Debug.Log($"Directory {directory} not found");
            }
            if (!File.Exists(path))
            {
                Debug.Log($"File {file} not found");
                return null;
            }

            MapData mapData;

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        int countChips = int.Parse(streamReader.ReadLine() ?? string.Empty);
                        int countPoints = int.Parse(streamReader.ReadLine() ?? string.Empty);

                        var points = new Vector2Int[countPoints];
                        for (var i = 0; i < points.Length; i++)
                        {
                            string[] coordinates = streamReader.ReadLine()?.SplitString();

                            int x = int.Parse(coordinates[0]);
                            int y = int.Parse(coordinates[1]);

                            points[i] = new Vector2Int(x, y);
                        }

                        int[] startPoints = streamReader.ReadLine()
                            .SplitString()
                            .Select(int.Parse)
                            .ToArray();

                        int[] winPoints = streamReader.ReadLine()
                            .SplitString()
                            .Select(int.Parse)
                            .ToArray();

                        int countConnections = int.Parse(streamReader.ReadLine() ?? string.Empty);

                        var connections = new Connection[countConnections];
                        for (var i = 0; i < connections.Length; i++)
                        {
                            string[] coordinates = coordinates = streamReader.ReadLine()?.SplitString();
                            int startPoint = int.Parse(coordinates[0]);
                            int endPoint = int.Parse(coordinates[1]);
                            connections[i] = new Connection()
                            {
                                StartPointNumber = startPoint,
                                EndPointNumber = endPoint
                            };
                        }

                        mapData = new MapData()
                        {
                            CountChips = countChips,
                            CountPoints = countPoints,
                            Points = points,
                            StartPositions = startPoints,
                            WinPositions = winPoints,
                            CountConnections = countConnections,
                            Connections = connections
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return null;
            }

            return mapData;
        }

        private static string[] SplitString(this string line, char separator = ',')
        {
            return line.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}