using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MisfiredJump
{
    public class MisfiredJump : ModBehaviour
    {
        public static MisfiredJump Instance;
        public INewHorizons NewHorizons;

        int _azimuthCode;
        int _altitudeCode;
        int _azimuthSecondaryCode;
        int _altitudeSecondaryCode;
        Vector2Int[] _podObstacles;

        public void Awake()
        {
            Instance = this;

            InitTelescopeCodes();
            InitPodObstacles();
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(MisfiredJump)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);

            new Harmony("Echatsum.MisfiredJump").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
        }

        public void Update(){
            
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }

        private void InitTelescopeCodes()
        {
            // TODO(?): Minor difference for different players

            _azimuthCode = 354420;
            _altitudeCode = 31214;
            _azimuthSecondaryCode = 102522;
            _altitudeSecondaryCode = 12345;
        }
        private void InitPodObstacles_Old()
        {
            // obstacles
            Vector2Int[] obstacles = new Vector2Int[]
            {
                new Vector2Int(10, 9),
                new Vector2Int(10, 11),
                new Vector2Int(4, 6),
                new Vector2Int(4, 7),
                new Vector2Int(4, 8),
                new Vector2Int(5, 8),
                new Vector2Int(6, 6),
                new Vector2Int(6, 7),
                new Vector2Int(6, 8),
            };

            // border (no pacman mode yet)
            var mapBorder = new List<Vector2Int>();
            var mapDimensions = GetPodMapDimensions();
            for (int x = 0; x <= mapDimensions[0]; x++) // row
            {
                mapBorder.Add(new Vector2Int(x, 0));
                mapBorder.Add(new Vector2Int(x, mapDimensions[1]));
            }
            for (int y = 1; y <= mapDimensions[1] - 1; y++) // column
            {
                mapBorder.Add(new Vector2Int(0, y));
                mapBorder.Add(new Vector2Int(mapDimensions[0], y));
            }

            mapBorder.AddRange(obstacles);

            _podObstacles = mapBorder.ToArray();
        }
        private void InitPodObstacles()
        {
            string map =
              // 012345678901234567890
                "XXXXXXXXXXXXXXXXXXXXX" + // 20
                "X  X       X      X X" + // 19
                "X  X XX          XX X" + // 18
                "X  X     XX XX      X" + // 17
                "X       XX  X       X" + // 16
                "X      XX         X X" + // 15
                "X   X  X      XX    X" + // 14
                "XX              X   X" + // 13
                "XXX        X    X   X" + // 12
                "X XX  X   XX        X" + // 11
                "X  X  X   S      XX X" + // 10
                "X         X XX   X  X" + // 9
                "X   XXX   XXXX      X" + // 8
                "X  XXNX             X" + // 7
                "X   X XX   X    X   X" + // 6
                "X      X  XXX   X   X" + // 5
                "X          X    X   X" + // 4
                "X            X  XX  X" + // 3
                "X  XXXX    XX       X" + // 2
                "XXXX            XXXXX" + // 1
                "XXXXXXXXXXXXXXXXXXXXX";  // 0

            _podObstacles = MapParser(map, GetPodMapDimensions());
        }

        private Vector2Int[] MapParser(string map, Vector2Int mapDimensions)
        {
            List<Vector2Int> list = new List<Vector2Int>();

            // Note that mapDimensions are included therefore <= in the forloops
            for(int x=0; x<=mapDimensions.x; x++)
            {
                for(int y=0; y<=mapDimensions.y; y++)
                {
                    int index = x + y * (mapDimensions.x+1);
                    if (map[index] == 'X')
                    {
                        int trueY = mapDimensions.y - y;
                        list.Add(new Vector2Int(x, trueY));
                    }
                }
            }

            return list.ToArray();
        }


        // Telescope config
        public int GetAzimuthCode() => _azimuthCode;
        public int GetAltitudeCode() => _altitudeCode;
        public int GetAzimuthSecondaryCode() => _azimuthSecondaryCode;
        public int GetAltitudeSecondaryCode() => _altitudeSecondaryCode;
        public float GetTelescopeFloodingTime() => 4f;
        public float GetTelescopeWarpRanges() => 6f;

        // Pod config
        public Vector2Int GetPodMapDimensions() => new Vector2Int(20, 20); // Max value is included
        public Vector2Int GetPodNestPosition() => new Vector2Int(5, 7);
        public Vector2Int GetPodStartPosition() => new Vector2Int(10, 10);
        public Direction GetPodStartFacing() => Direction.East;
        public Vector2Int[] GetPodObstaclePositions() => _podObstacles;

        public float GetPodDangerTemperature()
        {
            var secondsToDanger = 90f; // 1.5 minutes seems good enough a baseline

            var temperatureIncrease = secondsToDanger / GetPodTemperatureUpdateFrequency() * GetPodTemperatureChange()[0];
            return temperatureIncrease;
        }
        public float GetPodMaxTemperature()
        {
            var secondsToAbort = 10f; // 10 seconds should be a good baseline for reaching the abort button

            var temperatureIncrease = secondsToAbort / GetPodTemperatureUpdateFrequency() * GetPodTemperatureChange()[0];
            return GetPodDangerTemperature() + temperatureIncrease;
        }
        public float GetPodTemperatureUpdateFrequency() => 0.5f;
        public Vector3 GetPodTemperatureChange() => new Vector3(1f, -10f, 5f);
    }
}
