using OWML.Common;

namespace MisfiredJump
{
    public enum RevealVolumeID
    {
        Unknown = -2,
        None = -1,

        // Outward Gazer
        TelescopeGround = 0,
        TelescopeLens = 1,
        TelescopeLowerBasementDoor = 2,
        TelescopeLowerBasementBatteries = 3,
        TelescopeLowerBasementFlooding = 4,
        TelescopeHiddenStone = 5,

        // Lone Drifter
        AsteroidCannon = 100,
        AsteroidCommand = 101,

        // Tanquil Husk
        TranquilHusk = 200,
        TranquilHuskProbes = 201,
        TanquilHuskWorkshop = 202,
        TanquilHuskWorkshopPress = 203,

        // Scalding Abyss
        ScaldingAbyss = 300,
        ScaldingAbyssBeam = 301,
        ScaldingAbyssLavaPod = 302,
        ScaldingAbyssLavaPodInside = 303,

        // Deepspace Objects - Nothing
    }
    public enum RevealInteractID
    {
        Unknown = -2,
        None = -1,

        // Outward Gazer
        TelescopeConsole = 0,
        TelescopeConsoleNoLens = 1,
        TelescopeConsoleLens = 2,
        TelescopeLowerBasementPoweron = 3,
        TelescopeProbeFound = 4,
        TelescopeErnestoFound = 5,

        // Lone Drifter - Nothing

        // Tanquil Husk
        TanquilHuskWorkshopLens = 200,

        // Scalding Abyss
        ScaldingAbyssLavaPodAbort = 300,
        ScaldingAbyssLavaPodDive = 301,
        ScaldingAbyssLavaPodNest = 302,

        // Deepspace Objects - Nothing
    }

    public static class MisfiredJumpFacts
    {
        public static string[] GetRevealVolumeFacts(RevealVolumeID revealVolumeId)
        {
            switch (revealVolumeId)
            {
                // Outward Gazer
                case RevealVolumeID.TelescopeGround:
                    return new string[] { "MISFIREDJUMP_CURIOSITY_TELESCOPE_RUMOR", "MISFIREDJUMP_LOG_TELESCOPE_GROUND" };
                case RevealVolumeID.TelescopeLens: // Volume was buggy so this is no longer used (NH handles it)
                    return new string[] { "MISFIREDJUMP_CURIOSITY_TELESCOPE_RUMOR", "MISFIREDJUMP_LOG_TELESCOPE_LENS", "MISFIREDJUMP_LOG_TELESCOPE_GROUND" };
                case RevealVolumeID.TelescopeLowerBasementDoor:
                    return new string[] { "MISFIREDJUMP_LOG_LOWER_BASEMENT_DOOR" };
                case RevealVolumeID.TelescopeLowerBasementBatteries:
                    return new string[] { "MISFIREDJUMP_LOG_LOWER_BASEMENT_BATTERIES" };
                case RevealVolumeID.TelescopeLowerBasementFlooding:
                    return new string[] { "MISFIREDJUMP_LOG_LOWER_BASEMENT_FLOODING" };
                case RevealVolumeID.TelescopeHiddenStone:
                    return new string[] { "MISFIREDJUMP_LOG_HIDDEN_STONE_RUMOR", "MISFIREDJUMP_LOG_HIDDEN_STONE" };

                // Lone Drifter
                case RevealVolumeID.AsteroidCannon:
                    return new string[] { "MISFIREDJUMP_LOG_ASTEROID_CANNON" };
                case RevealVolumeID.AsteroidCommand:
                    return new string[] { "MISFIREDJUMP_LOG_ASTEROID_COMMAND", "MISFIREDJUMP_LOG_SYMBOLS_CANNON_RUMOR", "MISFIREDJUMP_LOG_SYMBOLS_CANNON" };

                // Tanquil Husk
                case RevealVolumeID.TranquilHusk:
                    return new string[] { "MISFIREDJUMP_LOG_TRANQUIL_HUSK" };
                case RevealVolumeID.TranquilHuskProbes:
                    return new string[] { "MISFIREDJUMP_LOG_TRANQUIL_HUSK_PROBES", "MISFIREDJUMP_LOG_TRANQUIL_HUSK_STONE" };
                case RevealVolumeID.TanquilHuskWorkshop:
                    return new string[] { "MISFIREDJUMP_LOG_TRANQUIL_HUSK_EVACUATED", "MISFIREDJUMP_LOG_WORKSHOP_RUMOR", "MISFIREDJUMP_LOG_WORKSHOP", "MISFIREDJUMP_LOG_WORKSHOP_2" };
                case RevealVolumeID.TanquilHuskWorkshopPress:
                    return new string[] { "MISFIREDJUMP_LOG_WORKSHOP_PRESS" };

                // Scalding Abyss
                case RevealVolumeID.ScaldingAbyss:
                    return new string[] { "MISFIREDJUMP_LOG_SCALDING_ABYSS" };
                case RevealVolumeID.ScaldingAbyssBeam:
                    return new string[] { "MISFIREDJUMP_LOG_SCALDING_ABYSS_BEAM" };
                case RevealVolumeID.ScaldingAbyssLavaPod:
                    return new string[] { "MISFIREDJUMP_LOG_LAVA_POD_RUMOR", "MISFIREDJUMP_LOG_LAVA_POD", "MISFIREDJUMP_LOG_LAVA_POD_2" };
                case RevealVolumeID.ScaldingAbyssLavaPodInside:
                    return new string[] { "MISFIREDJUMP_LOG_LAVA_POD_INSIDE" };

                default: return new string[0];
            }
        }

        public static string[] GetRevealInteractFacts(RevealInteractID revealInteractId)
        {
            switch (revealInteractId)
            {
                // Outward Gazer
                case RevealInteractID.TelescopeConsole:
                    return new string[] { "MISFIREDJUMP_LOG_TELESCOPE_CONSOLE", "MISFIREDJUMP_LOG_TELESCOPE_CONSOLE_2", "MISFIREDJUMP_LOG_SYMBOLS_TELESCOPE_RUMOR", "MISFIREDJUMP_LOG_SYMBOLS_TELESCOPE" };
                case RevealInteractID.TelescopeConsoleNoLens:
                    return new string[] { "MISFIREDJUMP_LOG_TELESCOPE_ORIENTATION_RUMOR", "MISFIREDJUMP_LOG_TELESCOPE_CONSOLE_NOLENS" };
                case RevealInteractID.TelescopeConsoleLens:
                    return new string[] { "MISFIREDJUMP_LOG_TELESCOPE_CONSOLE_LENS" };
                case RevealInteractID.TelescopeLowerBasementPoweron:
                    return new string[] { "MISFIREDJUMP_LOG_TELESCOPE_CONSOLE_POWERON_RUMOR", "MISFIREDJUMP_LOG_LOWER_BASEMENT_POWERON" };
                case RevealInteractID.TelescopeProbeFound:
                    return new string[] { "MISFIREDJUMP_LOG_TELESCOPE_SYMBOL_RUMOR", "MISFIREDJUMP_LOG_PROBE_RUMOR", "MISFIREDJUMP_LOG_PROBE" };
                case RevealInteractID.TelescopeErnestoFound:
                    return new string[] { "MISFIREDJUMP_LOG_TELESCOPE_SYMBOL_RUMOR", "MISFIREDJUMP_LOG_ERNESTO_RUMOR", "MISFIREDJUMP_LOG_ERNESTO" };

                // Tanquil Husk
                case RevealInteractID.TanquilHuskWorkshopLens:
                    return new string[] { "MISFIREDJUMP_LOG_TELESCOPE_LENS_RUMOR", "MISFIREDJUMP_LOG_WORKSHOP_LENS" };

                // Scalding Abyss
                case RevealInteractID.ScaldingAbyssLavaPodAbort:
                    return new string[] { "MISFIREDJUMP_LOG_LAVA_POD_ABORT" };
                case RevealInteractID.ScaldingAbyssLavaPodDive:
                    return new string[] { "MISFIREDJUMP_LOG_LAVA_POD_DIVE" };
                case RevealInteractID.ScaldingAbyssLavaPodNest:
                    return new string[] { "MISFIREDJUMP_LOG_LAVA_POD_NEST", "MISFIREDJUMP_LOG_WORKSHOP_EGG_RUMOR" };

                default: return new string[0];
            }
        }

        public static void RevealFacts(string[] facts)
        {
            if(facts == null)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"{nameof(MisfiredJumpFacts)} was called with a null array", MessageType.Warning);
                return;
            }

            foreach(var factID in facts)
            {
                MisfiredJump.Instance.ModHelper.Console.WriteLine($"Fact {factID} is being revealed!", MessageType.Info);
                Locator.GetShipLogManager().RevealFact(factID);
            }
        }
    }
}
