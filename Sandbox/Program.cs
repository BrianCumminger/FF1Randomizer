using System;
using FF1Lib;
using RomUtilities;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Sandbox
{
	class Program
	{
		public const string HelpText =	"Final Fantasy Randomizer Statistics Generator\n" +
										"Usage: ffrstag (-f|--flags) <FLAGS> (-i|--iterations) <INT> [(-c|--compact)] [(-s|--summarize)]\n" +
										"\n" +
										"-f|--flags          Flags to generate stats for\n" +
										"-i|--iterations     Number of seeds to process\n" +
										"-c|--compact        Only output stats for 'important' items\n" +
										"-s|--summarize      Combine chest locations into general areas\n" +
										"\n" +
										"Please ensure that a valid ff1.nes rom can be found in the current working directory.\n";
		public static Dictionary<string, int[]> LocationGroups = new Dictionary<string, int[]>
		{
			{"Coneria", new int[] {0x3101, 0x3102, 0x3103, 0x3104, 0x3105, 0x3106} },
			{"ToF", new int[] {0x3107, 0x3108, 0x3109} },
			{"ToF Locked", new int[] {0x310A, 0x310B, 0x310C} },
			{"Elfland", new int[] {0x310D, 0x310E, 0x310F, 0x3110} },
			{"Northwest Castle", new int[] {0x3111, 0x3112, 0x3113} },
			{"Marsh", new int[] {0x3114, 0x3115, 0x3116, 0x3117, 0x3118, 0x3119, 0x311A, 0x311B, 0x311C, 0x311D} },
			{"Marsh Locked", new int[] {0x311E, 0x311F, 0x3120} },
			{"Dwarf", new int[] {0x3121, 0x3122} },
			{"Dwarf Locked", new int[] {0x3123, 0x3124, 0x3125, 0x3126, 0x3127, 0x3128, 0x3129, 0x312A} },
			{"Matoya's Cave", new int[] {0x312B, 0x312C, 0x312D} },
			{"Earth", new int[] {0x312E, 0x312F, 0x3130, 0x3131, 0x3132, 0x3133, 0x3134, 0x3135, 0x3136, 0x3137, 0x3138, 0x3139, 0x313A, 0x313B, 0x313C, 0x313D} },
			{"Earth (Rod Locked)", new int[] {0x313E, 0x313F, 0x3140, 0x3141, 0x3142, 0x3143, 0x3144, 0x3145} },
			{"Titan's Tunnel", new int[] {0x3146, 0x3147, 0x3148, 0x3149} },
			{"Volcano", new int[] {0x314A, 0x314B, 0x314C, 0x314D, 0x314E, 0x314F, 0x3150, 0x3151, 0x3152, 0x3153, 0x3154, 0x3155, 0x3156, 0x3157, 0x3158, 0x3159, 0x315A, 0x315B, 0x315C, 0x315D,  0x315E, 0x315F, 0x3160, 0x3161, 0x3162, 0x3163, 0x3164, 0x3165, 0x3166, 0x3167, 0x3168, 0x3169, 0x316A} },
			{"Ice Cave", new int[] {0x316B, 0x316C, 0x316D, 0x316E, 0x316F, 0x3170, 0x3171, 0x3172, 0x3173, 0x3174, 0x3175, 0x3176, 0x3177, 0x3178, 0x3179, 0x317A} },
			{"Ordeals", new int[] {0x317B, 0x317C, 0x317D, 0x317E, 0x317F, 0x3180, 0x3181, 0x3182, 0x3183} },
			{"Cardia", new int[] {0x3184, 0x3185, 0x3186, 0x3187, 0x3188, 0x3189, 0x318A, 0x318B, 0x318C, 0x318D, 0x318E, 0x318F, 0x3190} },
			{"Sea Shrine", new int[] {0x3195, 0x3196, 0x3197, 0x3198, 0x3199, 0x319A, 0x319B, 0x319C, 0x319D, 0x319E, 0x319F, 0x31A0, 0x31A1, 0x31A2, 0x31A3, 0x31A4, 0x31A5, 0x31A6, 0x31A7, 0x31A8, 0x31A9, 0x31AA, 0x31AB, 0x31AC, 0x31AD, 0x31AE, 0x31AF, 0x31B0, 0x31B1, 0x31B2, 0x31B3, 0x31B4} },
			{"Waterfall", new int[] {0x31B5, 0x31B6, 0x31B7, 0x31B8, 0x31B9, 0x31BA} },
			{"Mirage Tower", new int[] {0x31C4, 0x31C5, 0x31C6, 0x31C7, 0x31C8, 0x31C9, 0x31CA, 0x31CB, 0x31CC, 0x31CD, 0x31CE, 0x31CF, 0x31D0, 0x31D1, 0x31D2, 0x31D3, 0x31D4, 0x31D5} },
			{"Sky Cave", new int[] {0x31D6, 0x31D7, 0x31D8, 0x31D9, 0x31DA, 0x31DB, 0x31DC, 0x31DD, 0x31DE, 0x31DF, 0x31E0, 0x31E1, 0x31E2, 0x31E3, 0x31E4, 0x31E5, 0x31E6, 0x31E7,	0x31E8, 0x31E9, 0x31EA, 0x31EB, 0x31EC, 0x31ED, 0x31EE, 0x31EF, 0x31F0, 0x31F1, 0x31F2, 0x31F3, 0x31F4, 0x31F5, 0x31F6, 0x31F7} },
			{"ToFR", new int[] {0x31F8, 0x31F9, 0x31FA, 0x31FB, 0x31FC, 0x31FD, 0x31FE} },
			{"Princess", new int[] { 0x39620 } },
			{"King", new int[] { 0x395DC } },
			{"Matoya", new int[] { 0x39600} },
			{"Bikke", new int[] { 0x395E8} },
			{"Elf Prince", new int[] { 0x000395f0} },
			{"Astos", new int[] { 0x000395f4} },
			{"Nerrick", new int[] { 0x000395f8} },
			{"Dwarven Smith", new int[] { 0x000395fc} },
			{"Sarda", new int[] { 0x0003960c} },
			{"Canoe Sage", new int[] { 0x0003962c} },
			{"Waterfall Robot", new int[] { 0x0003961c} },
			{"Fairy", new int[] { 0x00039624} },
			{"Lefein NPC", new int[] { 0x000398c4} }
		};
		public static Dictionary<string, int> Locations = new Dictionary<string, int> {
			{"Coneria Left Locked 1", 0x3101 },
			{"Coneria Left Locked 2", 0x3102 },
			{"Coneria Left Locked 3", 0x3103 },
			{"Coneria Right Locked 1", 0x3104 },
			{"Coneria Right Locked 2", 0x3105 },
			{"Coneria Right Locked 3", 0x3106 },
			{"ToF Top-Left 1", 0x3107 },
			{"ToF Top-Left 2", 0x3108 },
			{"ToF Bottom-Left", 0x3109 },
			{"ToF Bottom-Right Locked", 0x310A },
			{"ToF Top-Right Locked 1", 0x310B },
			{"ToF Top-Right Locked 2", 0x310C },
			{"Elfland Locked 1", 0x310D },
			{"Elfland Locked 2", 0x310E },
			{"Elfland Locked 3", 0x310F },
			{"Elfland Locked 4", 0x3110 },
			{"NorthWest Castle Locked 1", 0x3111 },
			{"NorthWest Castle Locked 2", 0x3112 },
			{"NorthWest Castle Locked 3", 0x3113 },
			{"Marsh Bottom Floor 4-1", 0x3114 },
			{"Marsh Bottom Floor 2-1", 0x3115 },
			{"Marsh Bottom Floor 2-2 Top", 0x3116 },
			{"Marsh Bottom Floor 3-3", 0x3117 },
			{"Marsh Top Floor Bottom Room 1", 0x3118 },
			{"Marsh Top Floor Bottom Room 2", 0x3119 },
			{"Marsh Top Floor Top-Left", 0x311A },
			{"Marsh Top Floor Top-Right", 0x311B },
			{"Marsh Bottom Floor 2-3 Wizard Chest", 0x311C },
			{"Marsh Bottom Floor 2-2 Left", 0x311D },
			{"Marsh Bottom Floor Locked Left", 0x311E },
			{"Marsh Bottom Floor Locked Center", 0x311F },
			{"Marsh Bottom Floor Locked Right", 0x3120 },
			{"Dwarf Cave 1", 0x3121 },
			{"Dwarf Cave 2", 0x3122 },
			{"Dwarf Cave Locked 1", 0x3123 },
			{"Dwarf Cave Locked 2", 0x3124 },
			{"Dwarf Cave Locked 3", 0x3125 },
			{"Dwarf Cave Locked 4", 0x3126 },
			{"Dwarf Cave Locked 5", 0x3127 },
			{"Dwarf Cave Locked 6", 0x3128 },
			{"Dwarf Cave Locked 7", 0x3129 },
			{"Dwarf Cave Locked 8", 0x312A },
			{"Matoya's Cave 1", 0x312B },
			{"Matoya's Cave 2", 0x312C },
			{"Matoya's Cave 3", 0x312D },
			{"Earth Cave B1 Bottom-Left 1", 0x312E },
			{"Earth Cave B1 Bottom-Left 2", 0x312F },
			{"Earth Cave B1 Bottom-Right 1", 0x3130 },
			{"Earth Cave B1 Bottom-Right 2", 0x3131 },
			{"Earth Cave B1 Top Room", 0x3132 },
			{"Earth Cave B2 Top Room Top-Left", 0x3133 },
			{"Earth Cave B2 Top Room Top-Right", 0x3134 },
			{"Earth Cave B2 Top Room Left Chest", 0x3135 },
			{"Earth Cave B2 Bottom Room Top", 0x3136 },
			{"Earth Cave B2 Bottom Room Bottom-Left", 0x3137 },
			{"Earth Cave B2 Bottom Room Bottom-Right", 0x3138 },
			{"Earth Cave B3 Fourth Room", 0x3139 },
			{"Earth Cave B3 Third Room", 0x313A },
			{"Earth Cave B3 First Room", 0x313B },
			{"Earth Cave B3 Second Room", 0x313C },
			{"Earth Cave B3 Vampire Chest", 0x313D },
			{"Earth Cave B4 Bottom Room Top", 0x313E },
			{"Earth Cave B4 Bottom Room Right", 0x313F },
			{"Earth Cave B4 Bottom Room Bottom", 0x3140 },
			{"Earth Cave B4 Top Room Left Group 1", 0x3141 },
			{"Earth Cave B4 Top Room Left Group 2", 0x3142 },
			{"Earth Cave B4 Top Room Right Group 1", 0x3143 },
			{"Earth Cave B4 Top Room Right Group 2", 0x3144 },
			{"Earth Cave B4 Top Room Left Group 3", 0x3145 },
			{"Titan's Tunnel 1", 0x3146 },
			{"Titan's Tunnel 2", 0x3147 },
			{"Titan's Tunnel 3", 0x3148 },
			{"Titan's Tunnel 4", 0x3149 },
			{"Gurgu Volcano B2 Armory 1", 0x314A },
			{"Gurgu Volcano B2 Armory 2", 0x314B },
			{"Gurgu Volcano B2 Armory 3", 0x314C },
			{"Gurgu Volcano B2 Armory 4", 0x314D },
			{"Gurgu Volcano B2 Armory 5", 0x314E },
			{"Gurgu Volcano B2 Armory 6", 0x314F },
			{"Gurgu Volcano B2 Armory 7", 0x3150 },
			{"Gurgu Volcano B2 Armory 8", 0x3151 },
			{"Gurgu Volcano B2 Armory 9", 0x3152 },
			{"Gurgu Volcano B2 Armory 10", 0x3153 },
			{"Gurgu Volcano B2 Armory 11", 0x3154 },
			{"Gurgu Volcano B2 Armory 12", 0x3155 },
			{"Gurgu Volcano B2 Top Group Left Chest", 0x3156 },
			{"Gurgu Volcano B2 Top Group Right Chest", 0x3157 },
			{"Gurgu Volcano B2 Upper-Right Maze Left Chest", 0x3158 },
			{"Gurgu Volcano B2 Upper-Right Maze Right Chest", 0x3159 },
			{"Gurgu Volcano B2 Bottom Long Hall", 0x315A },
			{"Gurgu Volcano B2 Middle Lower Maze", 0x315B },
			{"Gurgu Volcano B4 Top-Right Room Right Chest", 0x315C },
			{"Gurgu Volcano B4 Top-Right Room Left Chest", 0x315D },
			{"Gurgu Volcano B4 Upper-Mid Room", 0x315E },
			{"Gurgu Volcano B4 Far-Left Room 1", 0x315F },
			{"Gurgu Volcano B4 Far-Left Room 2", 0x3160 },
			{"Gurgu Volcano B4 Lower-Mid Room Top Chest", 0x3161 },
			{"Gurgu Volcano B4 Lower-Mid Room Right Chest", 0x3162 },
			{"Gurgu Volcano B4 Mid-Right Room Left Chest 1", 0x3163 },
			{"Gurgu Volcano B4 Mid-Right Room Left Chest 2", 0x3164 },
			{"Gurgu Volcano B4 Mid-Right Room Right Chest", 0x3165 },
			{"Gurgu Volcano B4 Agama Room Top Chest", 0x3166 },
			{"Gurgu Volcano B4 Agama Room Bottom Chest", 0x3167 },
			{"Gurgu Volcano B4 Far-Left Room 3", 0x3168 },
			{"Gurgu Volcano B4 Far-Left Room 4", 0x3169 },
			{"Gurgu Volcano B5 Red D Chest", 0x316A },
			{"Ice Cave Exit Floor Left Room Top Chest", 0x316B },
			{"Ice Cave Exit Floor Left Room Bottom Chest", 0x316C },
			{"Ice Cave Exit Floor Right Room Chest 1", 0x316D },
			{"Ice Cave Exit Floor Right Room Chest 2", 0x316E },
			{"Ice Cave Exit Floor Right Room Chest 3", 0x316F },
			{"Ice Cave Eye Floor Right", 0x3170 },
			{"Ice Cave Eye Floor Left", 0x3171 },
			{"Ice Cave Eye Floor Floater Chest", 0x3172 },
			{"Ice Cave Drop Floor Bottom Room 1", 0x3173 },
			{"Ice Cave Drop Floor Bottom Room 2", 0x3174 },
			{"Ice Cave Drop Floor Bottom Room 3", 0x3175 },
			{"Ice Cave Drop Floor Bottom Room 4", 0x3176 },
			{"Ice Cave Drop Floor Bottom Room 5", 0x3177 },
			{"Ice Cave Drop Floor Bottom Room 6", 0x3178 },
			{"Ice Cave Drop Floor Frost D Room Left Chest", 0x3179 },
			{"Ice Cave Drop Floor Frost D Room Right Chest", 0x317A },
			{"Castle of Ordeal Lower-Left 1", 0x317B },
			{"Castle of Ordeal Lower-Left 2", 0x317C },
			{"Castle of Ordeal Lower-Left 3", 0x317D },
			{"Castle of Ordeal Lower-Left 4", 0x317E },
			{"Castle of Ordeal Top-Left 1", 0x317F },
			{"Castle of Ordeal Top-Left 2", 0x3180 },
			{"Castle of Ordeal Top-Left 3", 0x3181 },
			{"Castle of Ordeal Top-Right", 0x3182 },
			{"Castle of Ordeal Incentivized Chest", 0x3183 },
			{"Cardia Forest Island Right Room 1", 0x3184 },
			{"Cardia Forest Island Right Room 2", 0x3185 },
			{"Cardia Forest Island Right Room 3", 0x3186 },
			{"Cardia Forest Island Left Room 1", 0x3187 },
			{"Cardia Forest Island Left Room 2", 0x3188 },
			{"Cardia Swamp Island 1", 0x3189 },
			{"Cardia Swamp Island 2", 0x318A },
			{"Cardia Swamp Island 3", 0x318B },
			{"Cardia Grassy Island Bottom Room 1", 0x318C },
			{"Cardia Grassy Island Bottom Room 2", 0x318D },
			{"Cardia Grassy Island Top Room", 0x318E },
			{"Cardia Forest Island Left Room 3", 0x318F },
			{"Cardia Forest Island Left Room 4", 0x3190 },
			{"Sea Shrine B4 Sharknado Room 1", 0x3195 },
			{"Sea Shrine B4 Sharknado Room 2", 0x3196 },
			{"Sea Shrine B4 Sharknado Room 3", 0x3197 },
			{"Sea Shrine B4 Sharknado Room 4", 0x3198 },
			{"Sea Shrine B4 Lower-Right 1", 0x3199 },
			{"Sea Shrine B4 Lower-Right 2", 0x319A },
			{"Sea Shrine B4 Lower-Left 1", 0x319B },
			{"Sea Shrine B4 Lower-Left 2", 0x319C },
			{"Sea Shrine B4 Lower-Left 3", 0x319D },
			{"Sea Shrine B4 Upper-Right", 0x319E },
			{"Sea Shrine Entrance Left", 0x319F },
			{"Sea Shrine Entrance Right", 0x31A0 },
			{"Sea Shrine Small Pre-Kraken Floor Bottom", 0x31A1 },
			{"Sea Shrine Small Pre-Kraken Floor Top", 0x31A2 },
			{"Sea Shrine B2 Left", 0x31A3 },
			{"Sea Shrine B2 Top", 0x31A4 },
			{"Sea Shrine B2 Reverse-C", 0x31A5 },
			{"Sea Shrine B2 Middle", 0x31A6 },
			{"Sea Shrine B2 Bottom", 0x31A7 },
			{"Sea Shrine Mermaids 1", 0x31A8 },
			{"Sea Shrine Mermaids 2", 0x31A9 },
			{"Sea Shrine Mermaids 3", 0x31AA },
			{"Sea Shrine Mermaids 4", 0x31AB },
			{"Sea Shrine Mermaids 5", 0x31AC },
			{"Sea Shrine Mermaids 6", 0x31AD },
			{"Sea Shrine Mermaids 7", 0x31AE },
			{"Sea Shrine Mermaids 8", 0x31AF },
			{"Sea Shrine Mermaids 9", 0x31B0 },
			{"Sea Shrine Mermaids 10", 0x31B1 },
			{"Sea Shrine Mermaids Slab Room 1", 0x31B2 },
			{"Sea Shrine Mermaids Slab Room 2", 0x31B3 },
			{"Sea Shrine Mermaids Slab Room 3", 0x31B4 },
			{"Waterfall 1", 0x31B5 },
			{"Waterfall 2", 0x31B6 },
			{"Waterfall 3", 0x31B7 },
			{"Waterfall 4", 0x31B8 },
			{"Waterfall 5", 0x31B9 },
			{"Waterfall 6", 0x31BA },
			{"Mirage Tower F1 1", 0x31C4 },
			{"Mirage Tower F1 2", 0x31C5 },
			{"Mirage Tower F1 3", 0x31C6 },
			{"Mirage Tower F1 4", 0x31C7 },
			{"Mirage Tower F1 5", 0x31C8 },
			{"Mirage Tower F1 6", 0x31C9 },
			{"Mirage Tower F1 7", 0x31CA },
			{"Mirage Tower F1 8", 0x31CB },
			{"Mirage Tower F2 Right 1", 0x31CC },
			{"Mirage Tower F2 Right 2", 0x31CD },
			{"Mirage Tower F2 Right 3", 0x31CE },
			{"Mirage Tower F2 Right 4", 0x31CF },
			{"Mirage Tower F2 Right 5", 0x31D0 },
			{"Mirage Tower F2 Left 1", 0x31D1 },
			{"Mirage Tower F2 Left 2", 0x31D2 },
			{"Mirage Tower F2 Left 3", 0x31D3 },
			{"Mirage Tower F2 Left 4", 0x31D4 },
			{"Mirage Tower F2 Left 5", 0x31D5 },
			{"Sky Palace F1 Left 1", 0x31D6 },
			{"Sky Palace F1 Left 2", 0x31D7 },
			{"Sky Palace F1 Left 3", 0x31D8 },
			{"Sky Palace F1 Left 4", 0x31D9 },
			{"Sky Palace F1 Right 1", 0x31DA },
			{"Sky Palace F1 Right 2", 0x31DB },
			{"Sky Palace F1 Right 3", 0x31DC },
			{"Sky Palace F1 Right 4", 0x31DD },
			{"Sky Palace F1 Right 5", 0x31DE },
			{"Sky Palace F1 Bottom", 0x31DF },
			{"Sky Palace F2 Bottom-Right 1", 0x31E0 },
			{"Sky Palace F2 Bottom-Right 2", 0x31E1 },
			{"Sky Palace F2 Right 1", 0x31E2 },
			{"Sky Palace F2 Top-Right", 0x31E3 },
			{"Sky Palace F2 Right 2", 0x31E4 },
			{"Sky Palace F2 Top-Left 1", 0x31E5 },
			{"Sky Palace F2 Top-Left 2", 0x31E6 },
			{"Sky Palace F2 Left 1", 0x31E7 },
			{"Sky Palace F2 Left 2", 0x31E8 },
			{"Sky Palace F2 Bottom-Left", 0x31E9 },
			{"Sky Palace F4 Left 1", 0x31EA },
			{"Sky Palace F4 Left 2", 0x31EB },
			{"Sky Palace F4 Left 3", 0x31EC },
			{"Sky Palace F4 Left 4", 0x31ED },
			{"Sky Palace F4 Right 1", 0x31EE },
			{"Sky Palace F4 Right 2", 0x31EF },
			{"Sky Palace F4 Right 3", 0x31F0 },
			{"Sky Palace F4 Right 4", 0x31F1 },
			{"Sky Palace F4 Right 5", 0x31F2 },
			{"Sky Palace F4 Right 6", 0x31F3 },
			{"Sky Palace F4 Top 1", 0x31F4 },
			{"Sky Palace F4 Top 2", 0x31F5 },
			{"Sky Palace F4 Top 3", 0x31F6 },
			{"Sky Palace F4 Top 4", 0x31F7 },
			{"ToFR Air Floor Lower-Right", 0x31F8 },
			{"ToFR Fire Floor Far Right", 0x31F9 },
			{"ToFR Fire Floor Far Left", 0x31FA },
			{"ToFR Fire Floor Left", 0x31FB },
			{"ToFR Fire Floor Right", 0x31FC },
			{"ToFR Phantom Floor 1", 0x31FD },
			{"ToFR Phantom Floor 2", 0x31FE },
			{"Princess", 0x39620 },
			{"King", 0x395DC },
			{"Matoya", 0x39600},
			{"Bikke", 0x395E8},
			{"Elf Prince", 0x000395f0},
			{"Astos", 0x000395f4},
			{"Nerrick", 0x000395f8},
			{"Dwarven Smith", 0x000395fc},
			{"Sarda", 0x0003960c},
			{"Canoe Sage", 0x0003962c},
			{"Waterfall Robot", 0x0003961c},
			{"Fairy", 0x00039624},
			{"Lefein NPC", 0x000398c4}
		};
		public static Dictionary<string, int> TreasuresCondensed = new Dictionary<string, int>{
			{"Lute", 0x1},
			{"Crown", 0x2},
			{"Crystal", 0x3},
			{"Herb", 0x4},
			{"Key", 0x5},
			{"TNT", 0x6},
			{"Adamant", 0x7},
			{"Slab", 0x8},
			{"Ruby", 0x9},
			{"Rod", 0xA},
			{"Floater", 0xB},
			{"Chime", 0xC},
			{"Tail", 0xD},
			{"Cube", 0xE},
			{"Bottle", 0xF},
			{"Oxyale", 0x10},
			{"Canoe_Low", 0x11},
			{"LightAxe", 0x38},
			{"HealStaff", 0x39},
			{"MageStaff", 0x3A},
			{"Defense", 0x3B},
			{"WizardStaff", 0x3C},
			{"Vorpal", 0x3D},
			{"ThorHammer", 0x3F},
			{"BaneSword", 0x40},
			{"Katana", 0x41},
			{"Xcalber", 0x42},
			{"Masmune", 0x43},
			{"DragonArmor", 0x4D},
			{"OpalBracelet", 0x51},
			{"WhiteShirt", 0x52},
			{"BlackShirt", 0x53},
			{"AegisShield", 0x5A},
			{"HealHelm", 0x62},
			{"Ribbon", 0x63},
			{"ZeusGauntlet", 0x68},
			{"PowerGauntlet", 0x69},
			{"ProRing", 0x6B},
			{"Ship", 0xE0},
			{"Bridge", 0xE8},
			{"Canal", 0xEC},
			{"Canoe", 0xF2}
		};
		public static Dictionary<string, int> TreasuresComplete = new Dictionary<string, int> {
			{"Nothing", 0},
			{"Lute", 1},
			{"Crown", 2},
			{"Crystal", 3},
			{"Herb", 4},
			{"Key", 5},
			{"TNT", 6},
			{"Adamant", 7},
			{"Slab", 8},
			{"Ruby", 9},
			{"Rod", 10},
			{"Floater", 11},
			{"Chime", 12},
			{"Tail", 13},
			{"Cube", 14},
			{"Bottle", 15},
			{"Oxyale", 16},
			{"Canoe_Low", 17},
			{"Tent", 22},
			{"Cabin", 23},
			{"House", 24},
			{"Heal", 25},
			{"Pure", 26},
			{"Soft", 27},
			{"WoodenNunchucks", 28},
			{"SmallKnife", 29},
			{"WoodenStaff", 30},
			{"Rapier", 31},
			{"IronHammer", 32},
			{"ShortSword", 33},
			{"HandAxe", 34},
			{"Scimtar", 35},
			{"IronNunchucks", 36},
			{"LargeKnife", 37},
			{"IronStaff", 38},
			{"Sabre", 39},
			{"LongSword", 40},
			{"GreatAxe", 41},
			{"Falchon", 42},
			{"SilverKnife", 43},
			{"SilverSword", 44},
			{"SilverHammer", 45},
			{"SilverAxe", 46},
			{"FlameSword", 47},
			{"IceSword", 48},
			{"DragonSword", 49},
			{"GiantSword", 50},
			{"SunSword", 51},
			{"CoralSword", 52},
			{"WereSword", 53},
			{"RuneSword", 54},
			{"PowerStaff", 55},
			{"LightAxe", 56},
			{"HealStaff", 57},
			{"MageStaff", 58},
			{"Defense", 59},
			{"WizardStaff", 60},
			{"Vorpal", 61},
			{"CatClaw", 62},
			{"ThorHammer", 63},
			{"BaneSword", 64},
			{"Katana", 65},
			{"Xcalber", 66},
			{"Masmune", 67},
			{"Cloth", 68},
			{"WoodenArmor", 69},
			{"ChainArmor", 70},
			{"IronArmor", 71},
			{"SteelArmor", 72},
			{"SilverArmor", 73},
			{"FlameArmor", 74},
			{"IceArmor", 75},
			{"OpalArmor", 76},
			{"DragonArmor", 77},
			{"CopperBracelet", 78},
			{"SilverBracelet", 79},
			{"GoldBracelet", 80},
			{"OpalBracelet", 81},
			{"WhiteShirt", 82},
			{"BlackShirt", 83},
			{"WoodenShield", 84},
			{"IronShield", 85},
			{"SilverShield", 86},
			{"FlameShield", 87},
			{"IceShield", 88},
			{"OpalShield", 89},
			{"AegisShield", 90},
			{"Buckler", 91},
			{"ProCape", 92},
			{"Cap", 93},
			{"WoodenHelm", 94},
			{"IronHelm", 95},
			{"SilverHelm", 96},
			{"OpalHelm", 97},
			{"HealHelm", 98},
			{"Ribbon", 99},
			{"Gloves", 100},
			{"CopperGauntlet", 101},
			{"IronGauntlet", 102},
			{"SilverGauntlet", 103},
			{"ZeusGauntlet", 104},
			{"PowerGauntlet", 105},
			{"OpalGauntlet", 106},
			{"ProRing", 107},
			{"Gold10G", 108},
			{"Gold20G", 109},
			{"Gold25G", 110},
			{"Gold30G", 111},
			{"Gold55G", 112},
			{"Gold70G", 113},
			{"Gold85G", 114},
			{"Gold110G", 115},
			{"Gold135G", 116},
			{"Gold155G", 117},
			{"Gold160G", 118},
			{"Gold180G", 119},
			{"Gold240G", 120},
			{"Gold255G", 121},
			{"Gold260G", 122},
			{"Gold295G", 123},
			{"Gold300G", 124},
			{"Gold315G", 125},
			{"Gold330G", 126},
			{"Gold350G", 127},
			{"Gold385G", 128},
			{"Gold400G", 129},
			{"Gold450G", 130},
			{"Gold500G", 131},
			{"Gold530G", 132},
			{"Gold575G", 133},
			{"Gold620G", 134},
			{"Gold680G", 135},
			{"Gold750G", 136},
			{"Gold795G", 137},
			{"Gold880G", 138},
			{"Gold1020G", 139},
			{"Gold1250G", 140},
			{"Gold1455G", 141},
			{"Gold1520G", 142},
			{"Gold1760G", 143},
			{"Gold1975G", 144},
			{"Gold2000G", 145},
			{"Gold2750G", 146},
			{"Gold3400G", 147},
			{"Gold4150G", 148},
			{"Gold5000G", 149},
			{"Gold5450G", 150},
			{"Gold6400G", 151},
			{"Gold6720G", 152},
			{"Gold7340G", 153},
			{"Gold7690G", 154},
			{"Gold7900G", 155},
			{"Gold8135G", 156},
			{"Gold9000G", 157},
			{"Gold9300G", 158},
			{"Gold9500G", 159},
			{"Gold9900G", 160},
			{"Gold10000G", 161},
			{"Gold12350G", 162},
			{"Gold13000G", 163},
			{"Gold13450G", 164},
			{"Gold14050G", 165},
			{"Gold14720G", 166},
			{"Gold15000G", 167},
			{"Gold17490G", 168},
			{"Gold18010G", 169},
			{"Gold19990G", 170},
			{"Gold20000G", 171},
			{"Gold20010G", 172},
			{"Gold26000G", 173},
			{"Gold45000G", 174},
			{"Gold65000G", 175},
			{"Ship", 224},
			{"Bridge", 232},
			{"Canal", 236},
			{"Canoe", 242}
		};
		
		static void Main(string[] args)
		{
			string ROMPATH = @"ff1.nes";
			int seeds = 10000;
			Flags flags = Flags.DecodeFlagsText("PACBGAAAAAAAHI!fPAPeZZeAAeP");
			string flagsstring = "PACBGAAAAAAAHI!fPAPeZZeAAeP";
			Dictionary<string, int> Treasures = TreasuresComplete;
			bool flagsset = false;
			bool summarize = false;
			bool compact = false;

			int a = 0;
			while (a < args.Length)
			{
				string paramName = args[a];
				string param = "";

				switch (paramName)
				{
					case "-f":
						param = args[a + 1];
						if (!Regex.IsMatch(param, "[0-9A-Z-a-z!-]+"))
						{
							throw new ArgumentException("Bad flags string");
						}

						flags = Flags.DecodeFlagsText(param);
						flagsstring = param;
						a += 2;
						flagsset = true;
						break;

					case "-i":
					case "--iterations":
						param = args[a + 1];

						seeds = int.Parse(param);
						a += 2;
						break;

					case "-c":
					case "--compact":
						Treasures = TreasuresCondensed;
						compact = true;
						a += 1;
						break;

					case "-s":
					case "--summarize":
						summarize = true;
						a += 1;
						break;

					case "--help":
					case "-h":
						Console.WriteLine(HelpText);
						return;

					default:
						throw new ArgumentException($"Unrecognized parameter", paramName);
				}

			}

			if (!flagsset)
			{
				Console.WriteLine(HelpText);
				return;
			}

			DateTime start = DateTime.Now;
			int seedsGenerated = 0;

			void drawUpdate()
			{
				Console.Write($"\rProcessed {seedsGenerated} seeds.   {(seedsGenerated / ((DateTime.Now - start).TotalSeconds)):F2} seeds/sec.");
			}

			

			Console.Out.WriteLine(Flags.EncodeFlagsText(flags));
			Dictionary<int, Dictionary<int, int>> treasureCounts = new Dictionary<int, Dictionary<int, int>>();
			Dictionary<int, Dictionary<int[], int>> treasureGroupCounts = new Dictionary<int, Dictionary<int[], int>>();
			IEnumerable<int> LocationValues = Locations.Values;
			IEnumerable<int> TreasureValues = Treasures.Values;
			IEnumerable<int[]> LocationGroupValues = LocationGroups.Values;
			foreach (int t in TreasureValues)
			{
				treasureCounts.Add(t, new Dictionary<int, int>());
				treasureGroupCounts.Add(t, new Dictionary<int[], int>());
				foreach (int loc in LocationValues)
				{
					treasureCounts[t].Add(loc, 0);
				}
				foreach (int[] locs in LocationGroupValues)
				{
					treasureGroupCounts[t].Add(locs, 0);
				}
			}

			void get_the_data(int seed)
			{
				var rom = new FF1Rom(ROMPATH);
				Blob rseed = Blob.Random(4);
				//rseed = Blob.FromHex("11111111");
				rom.Randomize(rseed, flags);
				lock (treasureCounts)
				{
					foreach (int i in LocationValues)
					{
						if (TreasureValues.Contains(rom[i]))
						{
							treasureCounts[rom[i]][i]++;
						}
					}
					foreach (int[] locs in LocationGroupValues)
					{
						foreach (int loc in locs)
						{
							if (TreasureValues.Contains(rom[loc]))
							{
								treasureGroupCounts[rom[loc]][locs]++;
							}
						}
					}
					seedsGenerated++;
				}
				drawUpdate();
			}


			System.Threading.Tasks.Parallel.For(0, seeds, get_the_data);

			string fnCompact = "";
			string fnSummarize = "";
			string fnSeparator = "";

			if (compact) { fnCompact = "c"; }
			if (summarize) { fnSummarize = "s"; }
			if (compact | summarize) { fnSeparator = "_"; }

			var outfile = System.IO.File.CreateText($"ffrstag_{flagsstring}_{seeds}{fnSeparator}{fnCompact}{fnSummarize}.csv");
			outfile.AutoFlush = true;
			if (summarize)
			{
				WriteSummarized(outfile, Treasures, treasureGroupCounts);
			}
			else
			{
				WriteUnsummarized(outfile, Treasures, treasureCounts);
			}
			

			outfile.Close();
			Console.WriteLine(" ");
			Console.WriteLine($"Summarized {seeds} seeds.");
			//Console.ReadKey();
		}

		private static void WriteSummarized(StreamWriter outfile, Dictionary<string, int> treasures, Dictionary<int, Dictionary<int[], int>> treasureGroupCounts)
		{
			StringBuilder header = new StringBuilder();
			StringBuilder line = new StringBuilder(2048);
			header.Append(",");
			IEnumerable<string> LocationNames = LocationGroups.Keys;
			IEnumerable<string> TreasureNames = treasures.Keys;
			foreach (string s in TreasureNames)
			{
				header.Append($"{s},");
			}
			header.Remove(header.Length - 1, 1);
			outfile.WriteLine(header);
			foreach (string location in LocationNames)
			{
				line = new StringBuilder(2048);
				line.Append($"{location},");
				foreach (string treasure in TreasureNames)
				{
					line.Append($"{treasureGroupCounts[treasures[treasure]][LocationGroups[location]]},");
				}
				line.Remove(line.Length - 1, 1);
				outfile.WriteLine(line.ToString());
			}
		}

		private static void WriteUnsummarized(StreamWriter outfile, Dictionary<string, int> treasures, Dictionary<int, Dictionary<int, int>> treasureCounts)
		{
			StringBuilder header = new StringBuilder();
			StringBuilder line = new StringBuilder(2048);
			header.Append(",");
			IEnumerable<string> LocationNames = Locations.Keys;
			IEnumerable<string> TreasureNames = treasures.Keys;
			foreach (string s in TreasureNames)
			{
				header.Append($"{s},");
			}
			header.Remove(header.Length - 1, 1);
			outfile.WriteLine(header);
			foreach (string location in LocationNames)
			{
				line = new StringBuilder(2048);
				line.Append($"{location},");
				foreach (string treasure in TreasureNames)
				{
					line.Append($"{treasureCounts[treasures[treasure]][Locations[location]]},");
				}
				line.Remove(line.Length - 1, 1);
				outfile.WriteLine(line.ToString());
			}
		}

	}
}
