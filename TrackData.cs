using System;
using System.Collections.Generic;

namespace User.PluginMiniSectors
{
    internal readonly struct RangeLabel
    {
        public readonly double Start; // inclusive
        public readonly double End;   // inclusive
        public readonly string Label;

        public RangeLabel(double start, double end, string label)
        {
            Start = start;
            End = end;
            Label = label;
        }

        public bool Contains(double x) => x >= Start && x <= End;
    }

    internal static class TrackData
    {
        /// <summary>
        /// Determines whether the session best should be updated for a sector.
        /// </summary>
        /// <param name="sectorTime">The completed sector time in seconds</param>
        /// <param name="currentBest">The current session best (-1 if unset)</param>
        /// <param name="isLapValid">Whether the lap is valid (no cuts, etc.)</param>
        /// <returns>True if the sector time should become the new session best</returns>
        public static bool ShouldUpdateSessionBest(double sectorTime, double currentBest, bool isLapValid)
        {
            if (!isLapValid) return false;
            if (sectorTime <= 0) return false;
            return currentBest < 0 || sectorTime < currentBest;
        }

        /// <summary>
        /// Mapping: TrackId -> ordered list of TrackPositionPercent ranges -> corner/section label.
        /// Ordering matters: first match wins.
        /// </summary>
        public static readonly Dictionary<string, RangeLabel[]> TrackTurnMap =
            new Dictionary<string, RangeLabel[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["nurburgring_24h"] = new[]
                {
                    new RangeLabel(0.016, 0.031, "Yokohama-S"),
                    new RangeLabel(0.039, 0.048, "Valvoline-Kurve"),
                    new RangeLabel(0.048, 0.0550, "Ford-Kurve"),
                    new RangeLabel(0.067, 0.079, "Dunlop-Kehre"),
                    new RangeLabel(0.085, 0.100, "Michael-Schumacher-S"),
                    new RangeLabel(0.107, 0.116, "Michelin-Kurve"),
                    new RangeLabel(0.116, 0.125, "Warsteiner/Bilstein-Kurve"),
                    new RangeLabel(0.133, 0.144, "ADVAN-Bogen"),
                    new RangeLabel(0.151, 0.163, "NGK-Schikane"),
                    new RangeLabel(0.163, 0.172, "T13"),
                    new RangeLabel(0.172, 0.179, "Sabine-Schmitz-Kurve"),
                    new RangeLabel(0.181, 0.186, "Hatzenbach Bogen"),
                    new RangeLabel(0.186, 0.221, "Hatzenbach"),
                    new RangeLabel(0.221, 0.232, "Hoheichen"),
                    new RangeLabel(0.232, 0.266, "Quiddelbacher Höhe"),
                    new RangeLabel(0.266, 0.281, "Flugplatz"),
                    new RangeLabel(0.281, 0.295, "Kottenborn"),
                    new RangeLabel(0.300, 0.315, "Schwedenkreuz Kurve"),
                    new RangeLabel(0.315, 0.325, "Aremberg"),
                    new RangeLabel(0.327, 0.364, "Fuchsröhre"),
                    new RangeLabel(0.364, 0.379, "Adenauer Forst"),
                    new RangeLabel(0.388, 0.398, "Rebel Tree"),
                    new RangeLabel(0.398, 0.408, "Metzgesfeld I"),
                    new RangeLabel(0.408, 0.414, "Metzgesfeld II"),
                    new RangeLabel(0.419, 0.427, "Kallenhard"),
                    new RangeLabel(0.431, 0.441, "Spiegelkurve/Piff-Paff"),
                    new RangeLabel(0.441, 0.453, "Miss-hit-miss"),
                    new RangeLabel(0.455, 0.466, "Wehrseifen"),
                    new RangeLabel(0.471, 0.478, "Breidscheid"),
                    new RangeLabel(0.478, 0.486, "Breidscheid Bridge"),
                    new RangeLabel(0.486, 0.492, "Ex-Mühle"),
                    new RangeLabel(0.500, 0.514, "Lauda Links"),
                    new RangeLabel(0.514, 0.525, "Bergwerk"),
                    new RangeLabel(0.529, 0.562, "Kesselchen"),
                    new RangeLabel(0.570, 0.588, "Klostertal"),
                    new RangeLabel(0.588, 0.603, "Mutkurve/Courage Corner"),
                    new RangeLabel(0.617, 0.632, "Steilstrecke"),
                    new RangeLabel(0.637, 0.647, "Carraciola-Karussell Kurve"),
                    new RangeLabel(0.678, 0.686, "Hohe Acht"),
                    new RangeLabel(0.686, 0.697, "Hedgwigshöhe"),
                    new RangeLabel(0.697, 0.707, "Wippermann"),
                    new RangeLabel(0.707, 0.726, "Eschbach"),
                    new RangeLabel(0.726, 0.742, "Brünnchen/YouTube Corner"),
                    new RangeLabel(0.744, 0.753, "Eiskurve"),
                    new RangeLabel(0.758, 0.777, "Pflanzgarten I"),
                    new RangeLabel(0.777, 0.794, "Pflanzgarten II"),
                    new RangeLabel(0.794, 0.813, "Stefan-Bellof-S"),
                    new RangeLabel(0.813, 0.832, "Schwalbenschwanz"),
                    new RangeLabel(0.832, 0.8405, "Kleine Karussell"),
                    new RangeLabel(0.848, 0.868, "Galgenkopf"),
                    new RangeLabel(0.868, 0.936, "Döttinger Höhe"),
                    new RangeLabel(0.936, 0.957, "Antoniusbuche"),
                    new RangeLabel(0.959, 0.973, "Tiergarten"),
                    new RangeLabel(0.973, 0.983, "Hohenrain-Schikane"),
                    new RangeLabel(0.983, 0.991, "T13"),
                },

                ["nurburgring"] = new[]
                {
                    new RangeLabel(0.090, 0.135, "Haug-Haken"),
                    new RangeLabel(0.100, 0.242, "Mercedes Arena"),
                    new RangeLabel(0.290, 0.335, "Valvoline-Kurve"),
                    new RangeLabel(0.335, 0.370, "Ford-Kurve"),
                    new RangeLabel(0.440, 0.498, "Dunlop-Kehre"),
                    new RangeLabel(0.525, 0.602, "Michael-Schumacher-S"),
                    new RangeLabel(0.630, 0.670, "Michelin-Kurve"),
                    new RangeLabel(0.680, 0.725, "Warsteiner-Kurve"),
                    new RangeLabel(0.775, 0.810, "ADVAN-Bogen"),
                    new RangeLabel(0.850, 0.908, "NGK-Schikane"),
                    new RangeLabel(0.925, 0.965, "Coca-Cola Kurve"),
                },

                ["misano"] = new[]
                {
                    new RangeLabel(0.040, 0.140, "Variante del Parco"),
                    new RangeLabel(0.170, 0.250, "Curva del Rio"),
                    new RangeLabel(0.360, 0.420, "Curva Quercia"),
                    new RangeLabel(0.470, 0.532, "Curva Tramonto"),
                    new RangeLabel(0.645, 0.785, "Curvone"),
                    new RangeLabel(0.785, 0.828, "Curva Del Carro"),
                    new RangeLabel(0.900, 0.945, "Curva Misano"),
                },

                ["Kyalami"] = new[]
                {
                    new RangeLabel(0.034, 0.070, "The Kink"),
                    new RangeLabel(0.110, 0.165, "Crowthorne"),
                    new RangeLabel(0.170, 0.210, "Jukskei Sweep"),
                    new RangeLabel(0.210, 0.245, "Barbeque"),
                    new RangeLabel(0.325, 0.395, "Sunset"),
                    new RangeLabel(0.412, 0.452, "Clubhouse Bend"),
                    new RangeLabel(0.480, 0.570, "The Esses"),
                    new RangeLabel(0.610, 0.665, "Leeukop"),
                    new RangeLabel(0.680, 0.780, "Mineshaft"),
                    new RangeLabel(0.800, 0.840, "The Crocodiles"),
                    new RangeLabel(0.865, 0.900, "Cheeta"),
                    new RangeLabel(0.915, 0.960, "Ingwe"),
                },

                ["Hungaroring"] = new[]
                {
                    new RangeLabel(0.115, 0.165, "T1"),
                    new RangeLabel(0.175, 0.200, "T1a"),
                    new RangeLabel(0.220, 0.285, "T2"),
                    new RangeLabel(0.290, 0.325, "T3"),
                    new RangeLabel(0.390, 0.435, "T4 - Mansell"),
                    new RangeLabel(0.445, 0.512, "T5"),
                    new RangeLabel(0.530, 0.552, "T6"),
                    new RangeLabel(0.552, 0.570, "T7"),
                    new RangeLabel(0.580, 0.605, "T8"),
                    new RangeLabel(0.612, 0.645, "T9"),
                    new RangeLabel(0.655, 0.688, "T10"),
                    new RangeLabel(0.700, 0.738, "T11 - Alesi"),
                    new RangeLabel(0.785, 0.830, "T12"),
                    new RangeLabel(0.830, 0.850, "T12a"),
                    new RangeLabel(0.850, 0.895, "T13"),
                    new RangeLabel(0.905, 0.965, "T14"),
                },

                ["donington"] = new[]
                {
                    new RangeLabel(0.085, 0.135, "Redgate"),
                    new RangeLabel(0.165, 0.200, "Hollywood"),
                    new RangeLabel(0.220, 0.250, "Craner Curves"),
                    new RangeLabel(0.280, 0.310, "Old Hairpin"),
                    new RangeLabel(0.325, 0.355, "Starkey's Bridge"),
                    new RangeLabel(0.400, 0.425, "Schwantz Curve"),
                    new RangeLabel(0.435, 0.480, "McLeans"),
                    new RangeLabel(0.510, 0.585, "Coppice"),
                    new RangeLabel(0.620, 0.690, "Starkey's Straight"),
                    new RangeLabel(0.700, 0.750, "Fogarty Esses"),
                    new RangeLabel(0.810, 0.870, "Melbourne Hairpin"),
                    new RangeLabel(0.920, 0.965, "Goddarts"),
                    new RangeLabel(0.975, 0.999, "WheatCroft Straight"),
                },

                ["Imola"] = new[]
                {
                    new RangeLabel(0.115, 0.195, "Tamburello"),
                    new RangeLabel(0.252, 0.302, "Villeneuve"),
                    new RangeLabel(0.325, 0.370, "Tosa"),
                    new RangeLabel(0.448, 0.508, "Piratella"),
                    new RangeLabel(0.540, 0.598, "Acque Minerali"),
                    new RangeLabel(0.655, 0.705, "Variante Alta"),
                    new RangeLabel(0.818, 0.887, "Rivazza"),
                },

                ["Laguna_Seca"] = new[]
                {
                    new RangeLabel(0.100, 0.160, "T2 - Andretti Hairpin"),
                    new RangeLabel(0.185, 0.243, "T3"),
                    new RangeLabel(0.258, 0.312, "T4"),
                    new RangeLabel(0.390, 0.470, "T5"),
                    new RangeLabel(0.520, 0.555, "T6"),
                    new RangeLabel(0.570, 0.650, "Rahal Straight"),
                    new RangeLabel(0.650, 0.718, "The Corkscrew"),
                    new RangeLabel(0.740, 0.785, "T9"),
                    new RangeLabel(0.805, 0.862, "T10"),
                    new RangeLabel(0.880, 0.930, "T11"),
                },

                ["monza"] = new[]
                {
                    new RangeLabel(0.135, 0.175, "Prima Variante"),
                    new RangeLabel(0.225, 0.300, "Curva Biassono"),
                    new RangeLabel(0.340, 0.382, "Seconda Variante"),
                    new RangeLabel(0.415, 0.450, "1° Curva di Lesmo"),
                    new RangeLabel(0.475, 0.510, "2° Curva di Lesmo"),
                    new RangeLabel(0.540, 0.620, "Curva del Serraglio"),
                    new RangeLabel(0.660, 0.725, "Variante Ascari"),
                    new RangeLabel(0.870, 0.955, "Curva Parabolica"),
                },

                ["oulton_park"] = new[]
                {
                    new RangeLabel(0.040, 0.075, "Old Hall Corner"),
                    new RangeLabel(0.130, 0.150, "The Avenue"),
                    new RangeLabel(0.160, 0.200, "Cascades"),
                    new RangeLabel(0.210, 0.250, "Lakeside"),
                    new RangeLabel(0.300, 0.350, "Island Bend"),
                    new RangeLabel(0.365, 0.405, "Shell Oils Corner"),
                    new RangeLabel(0.460, 0.500, "Britten's"),
                    new RangeLabel(0.520, 0.550, "Hilltop"),
                    new RangeLabel(0.595, 0.625, "Hislop's"),
                    new RangeLabel(0.630, 0.660, "Knickerbrook"),
                    new RangeLabel(0.680, 0.720, "Clay Hill"),
                    new RangeLabel(0.730, 0.765, "Watter Tower"),
                    new RangeLabel(0.780, 0.825, "Druids Corner"),
                    new RangeLabel(0.900, 0.940, "Lodge Corner"),
                    new RangeLabel(0.955, 0.985, "Deer Leap"),
                },

                ["Paul_Ricard"] = new[]
                {
                    new RangeLabel(0.085, 0.145, "\"S\" de la Verrerie"),
                    new RangeLabel(0.217, 0.235, "Virage de L'Hôtel"),
                    new RangeLabel(0.252, 0.270, "Virage du Camp"),
                    new RangeLabel(0.252, 0.312, "Virage de la Sainte-Beaume"),
                    new RangeLabel(0.380, 0.620, "Ligne Droit du Mistral"),
                    new RangeLabel(0.640, 0.690, "Courbe de Signes"),
                    new RangeLabel(0.712, 0.780, "Double Droite du Beausset"),
                    new RangeLabel(0.800, 0.840, "Virage de Bendor"),
                    new RangeLabel(0.840, 0.890, "Courbe du Garlaban"),
                    new RangeLabel(0.890, 0.930, "Virage de la Tour"),
                    new RangeLabel(0.930, 0.955, "Virage du Pont"),
                },

                ["Silverstone"] = new[]
                {
                    new RangeLabel(0.0245, 0.0455, "Copse"),
                    new RangeLabel(0.115, 0.210, "Maggots & Becketts"),
                    new RangeLabel(0.210, 0.238, "Chapel Curve"),
                    new RangeLabel(0.250, 0.340, "Hangar Straight"),
                    new RangeLabel(0.350, 0.385, "Stowe"),
                    new RangeLabel(0.400, 0.440, "Vale"),
                    new RangeLabel(0.445, 0.500, "Club"),
                    new RangeLabel(0.515, 0.560, "Hamilton Straight"),
                    new RangeLabel(0.570, 0.595, "Abbey"),
                    new RangeLabel(0.610, 0.635, "Farm Curve"),
                    new RangeLabel(0.650, 0.680, "Village"),
                    new RangeLabel(0.680, 0.705, "The Loop"),
                    new RangeLabel(0.717, 0.738, "Aintree"),
                    new RangeLabel(0.760, 0.810, "Wellington Straight"),
                    new RangeLabel(0.825, 0.865, "Brooklands"),
                    new RangeLabel(0.870, 0.910, "Luffield"),
                    new RangeLabel(0.930, 0.960, "Woodcote"),
                },

                ["snetterton"] = new[]
                {
                    new RangeLabel(0.062, 0.110, "Riches"),
                    new RangeLabel(0.145, 0.182, "Montreal (Scary Tree)"),
                    new RangeLabel(0.215, 0.265, "Palmer"),
                    new RangeLabel(0.335, 0.370, "Agostini"),
                    new RangeLabel(0.405, 0.425, "Hamilton"),
                    new RangeLabel(0.450, 0.490, "Oggies"),
                    new RangeLabel(0.500, 0.540, "Williams"),
                    new RangeLabel(0.560, 0.650, "Bentley Straight"),
                    new RangeLabel(0.690, 0.740, "Nelson"),
                    new RangeLabel(0.750, 0.785, "Bomb Hole"),
                    new RangeLabel(0.820, 0.890, "Coram"),
                    new RangeLabel(0.890, 0.905, "Murrays"),
                },

                ["Spa"] = new[]
                {
                    new RangeLabel(0.035, 0.058, "La Source"),
                    new RangeLabel(0.130, 0.152, "Eau Rouge"),
                    new RangeLabel(0.152, 0.185, "Raidillon"),
                    new RangeLabel(0.205, 0.233, "Kemmel"),
                    new RangeLabel(0.233, 0.300, "Kemmel Straight"),
                    new RangeLabel(0.318, 0.360, "Les Combes"),
                    new RangeLabel(0.365, 0.385, "Malmedy"),
                    new RangeLabel(0.410, 0.450, "Bruxelles"),
                    new RangeLabel(0.525, 0.595, "Double Gauche"),
                    new RangeLabel(0.620, 0.672, "Les Fagnes"),
                    new RangeLabel(0.690, 0.715, "Campus"),
                    new RangeLabel(0.715, 0.745, "Stavelot"),
                    new RangeLabel(0.770, 0.810, "Courbe Paul Frere"),
                    new RangeLabel(0.865, 0.900, "Blanchimont"),
                    new RangeLabel(0.935, 0.975, "Chicane"),
                },

                ["Suzuka"] = new[]
                {
                    new RangeLabel(0.050, 0.114, "First Corner"),
                    new RangeLabel(0.128, 0.200, "Snake"),
                    new RangeLabel(0.200, 0.235, "Anti-Banked Curve"),
                    new RangeLabel(0.235, 0.310, "Dunlop"),
                    new RangeLabel(0.330, 0.358, "Degner 1"),
                    new RangeLabel(0.358, 0.382, "Degner 2"),
                    new RangeLabel(0.432, 0.463, "Hairpin"),
                    new RangeLabel(0.490, 0.570, "200R"),
                    new RangeLabel(0.588, 0.665, "Spoon Curve"),
                    new RangeLabel(0.710, 0.770, "Backstretch"),
                    new RangeLabel(0.785, 0.832, "130R"),
                    new RangeLabel(0.860, 0.895, "Cassio Triangle"),
                    new RangeLabel(0.895, 0.930, "Last Curve"),
                },

                ["brands_hatch"] = new[]
                {
                    new RangeLabel(0.010, 0.095, "Paddock Hill Bend"),
                    new RangeLabel(0.095, 0.135, "Pilgrim's Rise"),
                    new RangeLabel(0.135, 0.170, "Druids"),
                    new RangeLabel(0.190, 0.235, "Graham Hill Bend"),
                    new RangeLabel(0.235, 0.270, "Cooper Straight"),
                    new RangeLabel(0.280, 0.350, "Surtees"),
                    new RangeLabel(0.418, 0.480, "Pilgrim's Drop"),
                    new RangeLabel(0.480, 0.550, "Hawthorn Bend"),
                    new RangeLabel(0.550, 0.580, "Derek Minter Straight"),
                    new RangeLabel(0.580, 0.630, "Westfield Bend"),
                    new RangeLabel(0.650, 0.675, "Dingle Dell"),
                    new RangeLabel(0.650, 0.720, "Sheene's"),
                    new RangeLabel(0.735, 0.787, "Stirling's"),
                    new RangeLabel(0.787, 0.855, "Clearways"),
                    new RangeLabel(0.855, 0.940, "Clark Curve"),
                    new RangeLabel(0.955, 0.999, "Brabham Straight"),
                },

                ["mount_panorama"] = new[]
                {
                    new RangeLabel(0.001, 0.030, "Pit Straight"),
                    new RangeLabel(0.042, 0.070, "Hell Corner"),
                    new RangeLabel(0.100, 0.210, "Mountain Straight"),
                    new RangeLabel(0.220, 0.265, "Quarry Bend"),
                    new RangeLabel(0.300, 0.332, "The Cutting"),
                    new RangeLabel(0.332, 0.348, "Griffin's Mouth"),
                    new RangeLabel(0.348, 0.375, "Reid Park"),
                    new RangeLabel(0.380, 0.470, "Sullman Park"),
                    new RangeLabel(0.475, 0.500, "McPhillamy Park"),
                    new RangeLabel(0.500, 0.525, "Skyline"),
                    new RangeLabel(0.525, 0.555, "The Esses"),
                    new RangeLabel(0.560, 0.590, "The Dipper"),
                    new RangeLabel(0.610, 0.640, "Forest's Elbow"),
                    new RangeLabel(0.700, 0.820, "Conrod Straight"),
                    new RangeLabel(0.850, 0.918, "The Chase"),
                    new RangeLabel(0.960, 0.989, "Murray's Corner"),
                },

                ["Barcelona"] = new[]
                {
                    new RangeLabel(0.155, 0.220, "Elf"),
                    new RangeLabel(0.230, 0.320, "Renault"),
                    new RangeLabel(0.355, 0.420, "Repsol"),
                    new RangeLabel(0.440, 0.478, "Seat"),
                    new RangeLabel(0.535, 0.583, "Würth"),
                    new RangeLabel(0.600, 0.655, "Campsa"),
                    new RangeLabel(0.725, 0.770, "La Caixa"),
                    new RangeLabel(0.795, 0.840, "Banc Sabadell"),
                    new RangeLabel(0.850, 0.875, "Europcar"),
                    new RangeLabel(0.885, 0.908, "Chicane RACC"),
                    new RangeLabel(0.915, 0.960, "New Holland"),
                },

                ["Zandvoort"] = new[]
                {
                    new RangeLabel(0.060, 0.108, "Tarzanbocht"),
                    new RangeLabel(0.140, 0.175, "Gerlachbocht"),
                    new RangeLabel(0.180, 0.220, "Hugenholtzbocht"),
                    new RangeLabel(0.240, 0.280, "Hunzerug"),
                    new RangeLabel(0.280, 0.370, "Rob Slotemakerbocht"),
                    new RangeLabel(0.380, 0.440, "Sheivlak"),
                    new RangeLabel(0.470, 0.510, "Mastersbocht"),
                    new RangeLabel(0.525, 0.565, "Bocht 9"),
                    new RangeLabel(0.580, 0.620, "Bocht 10"),
                    new RangeLabel(0.710, 0.755, "Hans Ernst Bocht"),
                    new RangeLabel(0.805, 0.845, "Kumho"),
                    new RangeLabel(0.855, 0.920, "Arie Luyendijk Bocht"),
                },

                ["Zolder"] = new[]
                {
                    new RangeLabel(0.040, 0.100, "Earste"),
                    new RangeLabel(0.138, 0.190, "Sterrenwachtbocht"),
                    new RangeLabel(0.190, 0.239, "Kanaalbocht"),
                    new RangeLabel(0.260, 0.312, "Lucien Bianchibocht"),
                    new RangeLabel(0.420, 0.455, "Kleine Chicane"),
                    new RangeLabel(0.460, 0.500, "Sacramentshelling"),
                    new RangeLabel(0.505, 0.545, "Butte"),
                    new RangeLabel(0.555, 0.596, "Villeneuve Chicane"),
                    new RangeLabel(0.627, 0.670, "Terlamenbocht"),
                    new RangeLabel(0.730, 0.775, "Bolderbergbocht"),
                    new RangeLabel(0.775, 0.805, "Jochen Rindtbocht"),
                    new RangeLabel(0.880, 0.942, "Jacky Ickxbocht"),
                },

                ["watkins_glen"] = new[]
                {
                    new RangeLabel(0.050, 0.080, "The 90"),
                    new RangeLabel(0.111, 0.260, "Esses"),
                    new RangeLabel(0.270, 0.310, "Back Straight"),
                    new RangeLabel(0.330, 0.395, "Inner Loop"),
                    new RangeLabel(0.400, 0.460, "Outer Loop"),
                    new RangeLabel(0.480, 0.542, "Chute"),
                    new RangeLabel(0.575, 0.640, "Toe"),
                    new RangeLabel(0.640, 0.700, "\"The Boot\""),
                    new RangeLabel(0.710, 0.750, "Heel"),
                },

                ["cota"] = new[]
                {
                    new RangeLabel(0.100, 0.132, "T1"),
                    new RangeLabel(0.150, 0.184, "T2"),
                    new RangeLabel(0.200, 0.220, "T3"),
                    new RangeLabel(0.220, 0.236, "T4"),
                    new RangeLabel(0.236, 0.255, "T5"),
                    new RangeLabel(0.255, 0.298, "T6"),
                    new RangeLabel(0.305, 0.325, "T7"),
                    new RangeLabel(0.328, 0.352, "T8"),
                    new RangeLabel(0.352, 0.370, "T9"),
                    new RangeLabel(0.385, 0.405, "T10"),
                    new RangeLabel(0.445, 0.478, "T11"),
                    new RangeLabel(0.673, 0.700, "T12"),
                    new RangeLabel(0.718, 0.739, "T13"),
                    new RangeLabel(0.739, 0.760, "T14"),
                    new RangeLabel(0.760, 0.790, "T15"),
                    new RangeLabel(0.810, 0.825, "T16"),
                    new RangeLabel(0.825, 0.840, "T17"),
                    new RangeLabel(0.840, 0.890, "T18"),
                    new RangeLabel(0.900, 0.925, "T19"),
                    new RangeLabel(0.950, 0.980, "T20"),
                },
            };
    }
}
