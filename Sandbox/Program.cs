using System;
using FF1Lib;
using RomUtilities;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Sandbox
{
	class Program
	{
		public static readonly List<int> EarlyCrown =
			Enumerable.Range(1, 6).Concat( // Coneria
			Enumerable.Range(10, 3)).Concat( // ToF east side
			Enumerable.Range(13, 4)).Concat( // Elfland
			Enumerable.Range(17, 3)).Concat( // Northwest Castle
			Enumerable.Range(30, 3)).Concat( // Marsh Cave
			Enumerable.Range(35, 8)) // Dwarf Cave
			.ToList();
		public static readonly List<int> ToFR = Enumerable.Range(248, 7).ToList();
		//public static readonly List<int> ToFR = Enumerable.Range(100, 7).ToList();

		public const byte CHAR_FI = 0x00;
		public const byte CHAR_TH = 0x01;
		public const byte CHAR_BB = 0x02;
		public const byte CHAR_RM = 0x03;
		public const byte CHAR_WM = 0x04;
		public const byte CHAR_BM = 0x05;

		public static string[] LocationStrings = new string[] { "<Unused>", "Coneria (Left)", "Coneria (Left)", "Coneria (Left)", "Coneria (Right)", "Coneria (Right)", "Coneria (Right)", "ToF (Upper-Left)", "ToF (Upper-Left)", "ToF (Lower Left)", "ToF (Lower Right)", "ToF (Upper Right)", "ToF (Upper Right)", "Elfland 1", "Elfland 2", "Elfland 3", "Elfland 4", "NorthWest Castle 1", "NorthWest Castle 2", "NorthWest Castle 3", "Marsh Cave 1", "Marsh Cave 2", "Marsh Cave 3", "Marsh Cave 4", "Marsh Cave 5", "Marsh Cave 6", "Marsh Cave 7", "Marsh Cave 8", "Marsh Cave 9", "Marsh Cave 10", "Marsh Cave 11", "Marsh Cave 12", "Marsh Cave 13", "Dwarf Cave 1", "Dwarf Cave 2", "Dwarf Cave 3", "Dwarf Cave 4", "Dwarf Cave 5", "Dwarf Cave 6", "Dwarf Cave 7", "Dwarf Cave 8", "Dwarf Cave 9", "Dwarf Cave 10", "Matoya's Cave 1", "Matoya's Cave 2", "Matoya's Cave 3", "Earth Cave 1", "Earth Cave 2", "Earth Cave 3", "Earth Cave 4", "Earth Cave 5", "Earth Cave 6", "Earth Cave 7", "Earth Cave 8", "Earth Cave 9", "Earth Cave 10", "Earth Cave 11", "Earth Cave 12", "Earth Cave 13", "Earth Cave 14", "Earth Cave 15", "Earth Cave 16", "Earth Cave 17", "Earth Cave 18", "Earth Cave 19", "Earth Cave 20", "Earth Cave 21", "Earth Cave 22", "Earth Cave 23", "Earth Cave 24", "Titan's Tunnel 1", "Titan's Tunnel 2", "Titan's Tunnel 3", "Titan's Tunnel 4", "Gurgu Volcano 1", "Gurgu Volcano 2", "Gurgu Volcano 3", "Gurgu Volcano 4", "Gurgu Volcano 5", "Gurgu Volcano 6", "Gurgu Volcano 7", "Gurgu Volcano 8", "Gurgu Volcano 9", "Gurgu Volcano 10", "Gurgu Volcano 11", "Gurgu Volcano 12", "Gurgu Volcano 13", "Gurgu Volcano 14", "Gurgu Volcano 15", "Gurgu Volcano 16", "Gurgu Volcano 17", "Gurgu Volcano 18", "Gurgu Volcano 19", "Gurgu Volcano 20", "Gurgu Volcano 21", "Gurgu Volcano 22", "Gurgu Volcano 23", "Gurgu Volcano 24", "Gurgu Volcano 25", "Gurgu Volcano 26", "Gurgu Volcano 27", "Gurgu Volcano 28", "Gurgu Volcano 29", "Gurgu Volcano 30", "Gurgu Volcano 31", "Gurgu Volcano 32", "Gurgu Volcano 33", "Ice Cave 1", "Ice Cave 2", "Ice Cave 3", "Ice Cave 4", "Ice Cave 5", "Ice Cave 6", "Ice Cave 7", "Ice Cave 8", "Ice Cave 9", "Ice Cave 10", "Ice Cave 11", "Ice Cave 12", "Ice Cave 13", "Ice Cave 14", "Ice Cave 15", "Ice Cave 16", "Castle of Ordeal 1", "Castle of Ordeal 2", "Castle of Ordeal 3", "Castle of Ordeal 4", "Castle of Ordeal 5", "Castle of Ordeal 6", "Castle of Ordeal 7", "Castle of Ordeal 8", "Castle of Ordeal 9", "Cardia 1", "Cardia 2", "Cardia 3", "Cardia 4", "Cardia 5", "Cardia 6", "Cardia 7", "Cardia 8", "Cardia 9", "Cardia 10", "Cardia 11", "Cardia 12", "Cardia 13", "Not Used 1", "Not Used 2", "Not Used 3", "Not Used 4", "Sea Shrine 1", "Sea Shrine 2", "Sea Shrine 3", "Sea Shrine 4", "Sea Shrine 5", "Sea Shrine 6", "Sea Shrine 7", "Sea Shrine 8", "Sea Shrine 9", "Sea Shrine 10", "Sea Shrine 11", "Sea Shrine 12", "Sea Shrine 13", "Sea Shrine 14", "Sea Shrine 15", "Sea Shrine 16", "Sea Shrine 17", "Sea Shrine 18", "Sea Shrine 19", "Sea Shrine 20", "Sea Shrine 21", "Sea Shrine 22", "Sea Shrine 23", "Sea Shrine 24", "Sea Shrine 25", "Sea Shrine 26", "Sea Shrine 27", "Sea Shrine 28", "Sea Shrine 29", "Sea Shrine 30", "Sea Shrine 31", "Sea Shrine 32", "Waterfall 1", "Waterfall 2", "Waterfall 3", "Waterfall 4", "Waterfall 5", "Waterfall 6", "Not Used 5", "Not Used 6", "Not Used 7", "Not Used 8", "Not Used 9", "Not Used 10", "Not Used 11", "Not Used 12", "Not Used 13", "Mirage Tower 1", "Mirage Tower 2", "Mirage Tower 3", "Mirage Tower 4", "Mirage Tower 5", "Mirage Tower 6", "Mirage Tower 7", "Mirage Tower 8", "Mirage Tower 9", "Mirage Tower 10", "Mirage Tower 11", "Mirage Tower 12", "Mirage Tower 13", "Mirage Tower 14", "Mirage Tower 15", "Mirage Tower 16", "Mirage Tower 17", "Mirage Tower 18", "Sky Palace 1", "Sky Palace 2", "Sky Palace 3", "Sky Palace 4", "Sky Palace 5", "Sky Palace 6", "Sky Palace 7", "Sky Palace 8", "Sky Palace 9", "Sky Palace 10", "Sky Palace 11", "Sky Palace 12", "Sky Palace 13", "Sky Palace 14", "Sky Palace 15", "Sky Palace 16", "Sky Palace 17", "Sky Palace 18", "Sky Palace 19", "Sky Palace 20", "Sky Palace 21", "Sky Palace 22", "Sky Palace 23", "Sky Palace 24", "Sky Palace 25", "Sky Palace 26", "Sky Palace 27", "Sky Palace 28", "Sky Palace 29", "Sky Palace 30", "Sky Palace 31", "Sky Palace 32", "Sky Palace 33", "Sky Palace 34", "ToF Revisited 1", "ToF Revisited 2", "ToF Revisited 3", "ToF Revisited 4", "ToF Revisited 5", "ToF Revisited 6", "ToF Revisited 7", "<unused>" };

		static void Main(string[] args)
		{
			string ROMPATH = @"Final Fantasy (U) [!].nes";
			int TreasureOffset = 0x03100;
			int TreasureSize = 1;
			int TreasureCount = 256;
			int seeds = 5000000;

			int[] opalChestCount = new int[256];


			#region MyRegion
			//Action<int> act3 = (int seed) =>
			//{
			//	var rom = new FF1Rom(ROMPATH);
			//	Blob rseed = Blob.Random(4);
			//	rom.Randomize(rseed, flags);
			//	var treasureBlob = rom.Get(TreasureOffset, TreasureSize * TreasureCount);
			//	//if (rom[0x031F9] == 0x51)
			//	int[] bracelets = new int[] { 0x4E, 0x4F, 0x50, 0x51 };
			//	for (int addr = 0x31F8; addr < 0x31FF; addr++)
			//	{
			//		if (rom[addr] == bracelets[3])
			//		{
			//			//found opal
			//			for (int addr2 = 0x31F8; addr2 < 0x31FF; addr2++)
			//			{
			//				if (rom[addr2] == bracelets[2])
			//				{
			//					//found gold
			//					for (int addr3 = 0x31F8; addr3 < 0x31FF; addr3++)
			//					{
			//						if (rom[addr3] == bracelets[1])
			//						{
			//							//found silver
			//							Console.WriteLine($"Found evil seed lvl3: {rseed.ToHex()}");
			//							for (int addr4 = 0x31F8; addr4 < 0x31FF; addr4++)
			//							{
			//								if (rom[addr4] == bracelets[0])
			//								{
			//									//found copper
			//									Console.WriteLine($"Found evil seed lvl4 : {rseed.ToHex()}");
			//								}
			//							}
			//						}
			//					}
			//				}
			//			}
			//		}
			//	}
			//};
			//Action<int> act4 = (int seed) =>
			//{
			//	var rom = new FF1Rom(ROMPATH);
			//	Blob rseed = Blob.Random(4);
			//	rom.Randomize(rseed, flags);
			//	var treasureBlob = rom.Get(TreasureOffset, TreasureSize * TreasureCount);
			//	//if (rom[0x031F9] == 0x51)
			//	int[] bracelets = new int[] { 0x4E, 0x4F, 0x50, 0x51 };
			//	foreach (int addr in new int[] { 0x3107, 0x3108 })
			//	{
			//		if (rom[addr] == bracelets[3])
			//		{
			//			//found opal
			//			//Console.WriteLine($"Found god seed lvl1: {rseed.ToHex()}");
			//			foreach (int addr2 in new int[] { 0x3107, 0x3108, 0x3109 })
			//			{
			//				if (rom[addr2] == bracelets[2])
			//				{
			//					//found gold
			//					//Console.WriteLine($"Found god seed lvl2: {rseed.ToHex()}");
			//					foreach (int addr3 in new int[] { 0x3107, 0x3108, 0x3109, 0x312B, 0x312C, 0x312D })
			//					{
			//						if (rom[addr3] == bracelets[1])
			//						{
			//							//found silver
			//							Console.WriteLine($"Found god seed lvl3: {rseed.ToHex()}");
			//							foreach (int addr4 in new int[] { 0x3107, 0x3108, 0x3109, 0x312B, 0x312C, 0x312D })
			//							{
			//								if (rom[addr4] == bracelets[0])
			//								{
			//									//found copper
			//									Console.WriteLine($"Found god seed lvl4 : {rseed.ToHex()}");
			//								}
			//							}
			//						}
			//					}
			//				}
			//			}
			//		}
			//	}
			//};

			//void miah_plando(int seed)
			//{
			//	var rom = new FF1Rom(ROMPATH);
			//	Blob rseed = Blob.Random(4);
			//	rom.Randomize(rseed, flags);
			//	var treasureBlob = rom.Get(TreasureOffset, TreasureSize * TreasureCount);
			//	//if (rom[0x031F9] == 0x51)
			//	int[] desired_items = new int[] { 0x3F, 0x3C };
			//	int[] bracelets = new int[] { 0x4E, 0x4F, 0x50, 0x51 };
			//	foreach (int addr in new int[] { 0x3107, 0x3108, 0x3109 })
			//	{
			//		if (rom[addr] == desired_items[0])
			//		{
			//			//found 1
			//			//Console.WriteLine($"Found one item in ToF: {rseed.ToHex()}");
			//			foreach (int addr2 in new int[] { 0x3107, 0x3108, 0x3109 })
			//			{
			//				if (rom[addr2] == desired_items[1])
			//				{
			//					int wm = 0;
			//					if (rom[0x3A0AE] == 0x04) wm++;
			//					if (rom[0x3A0BE] == 0x04) wm++;
			//					if (rom[0x3A0CE] == 0x04) wm++;
			//					if (rom[0x3A0DE] == 0x04) wm++;

			//					//found both items
			//					Console.WriteLine($"Found both items in ToF: {rseed.ToHex()}, {wm} WM(s)");
			//					if (wm == 4)
			//					{
			//						Console.WriteLine($"DING DING DING Found both items in ToF with 4 WMs: {rseed.ToHex()}");
			//					}
			//					//if (rom[0x3A0AE] == 0x04 && rom[0x3A0BE] == 0x04)
			//					//{
			//					//	Console.WriteLine($"Found both items in ToF with 2 WMs: {rseed.ToHex()}");
			//					//	if (rom[0x3A0CE] == 0x04 && rom[0x3A0DE] == 0x04)
			//					//	{
			//					//		Console.WriteLine($"DING DING DING Found both items in ToF with 4 WMs: {rseed.ToHex()}");
			//					//	}
			//					//}
			//					//foreach (int addr3 in new int[] { 0x3107, 0x3108, 0x3109, 0x312B, 0x312C, 0x312D })
			//					//{
			//					//	if (rom[addr3] == bracelets[1])
			//					//	{
			//					//		//found silver
			//					//		Console.WriteLine($"Found god seed lvl3: {rseed.ToHex()}");
			//					//		foreach (int addr4 in new int[] { 0x3107, 0x3108, 0x3109, 0x312B, 0x312C, 0x312D })
			//					//		{
			//					//			if (rom[addr4] == bracelets[0])
			//					//			{
			//					//				//found copper
			//					//				Console.WriteLine($"Found god seed lvl4 : {rseed.ToHex()}");
			//					//			}
			//					//		}
			//					//	}
			//					//}
			//				}
			//			}
			//		}
			//	}
			//}



			#endregion



			Flags flags;
			flags = Flags.DecodeFlagsText("PAK!H0BPnEHAHJ!fPASeUoYAGYU");
			DateTime start = DateTime.Now;
			int seedsGenerated = 0;

			void drawUpdate()
			{
				Console.Write($"\rProcessed {seedsGenerated} seeds.   {(seedsGenerated / ((DateTime.Now - start).TotalSeconds)):F2} seeds/sec.");
			}
			string numOrDash(int i)
			{
				if (i == 0)
				{
					return "-";
				}
				return i.ToString();
			}
			var outfile = System.IO.File.AppendText("falconic_plando9.txt");
			outfile.AutoFlush = true;

			Console.Out.WriteLine(Flags.EncodeFlagsText(flags));
			void falcomic_plando3(int seed)
			{

				var rom = new FF1Rom(ROMPATH);
				Blob rseed = Blob.Random(4);
				rom.Randomize(rseed, flags);
				var treasureBlob = rom.Get(TreasureOffset, TreasureSize * TreasureCount);
				var chests = new List<int> { 0x31F8, 0x31F9, 0x31FA, 0x31FB, 0x31FC };
				int[] weapons = new int[] { 0x33, 0x3B, 0x42, 0x43 };
				int p = 0;
				foreach (int chest in chests)
				{
					if (weapons.Contains(rom[chest]))
					{
						p++;
					}
				}
				if (p > 2)
				{

					lock (outfile)
					{
						outfile.WriteLine($"Found {p} weapons in ToFR: {rseed.ToHex()}");
					}
					Console.WriteLine($"Found {p} weapons in ToFR: {rseed.ToHex()}");
				}
			}
			void falcomic_plando4(int seed)
			{

				var rom = new FF1Rom(ROMPATH);
				Blob rseed = Blob.Random(4);
				rom.Randomize(rseed, flags);
				var treasureBlob = rom.Get(TreasureOffset, TreasureSize * TreasureCount);
				int p = 0, q = 0, r = 0;
				//if (rom[0x3197] == 0x0E) //Cube in sharknado
				//{
				//	p++;
				//}
				int cube = 0x0E;
				int[] sharknadoFloorChests = new int[] { 0x3197, 0x3199, 0x319A, 0x319B, 0x319C, 0x319D, 0x319E };
				foreach (int chest in sharknadoFloorChests)
				{
					if (rom[chest] == cube)
					{
						p++;
						break;
					}
				}

				//if (rom[0x31DF] == 0x05) //Key in bane sword chest
				//{
				//	p++;
				//}
				int key = 0x05;
				int[] keyChests = new int[] { 0x31DF, /*0x31EA, 0x31EB, 0x31EC, 0x31ED, */0x31EE, 0x31EF, 0x31F0, 0x31F1, 0x31F2, 0x31F3, 0x31F4, 0x31F5, 0x31F6, 0x31F7, 0x31E0, 0x31E1, 0x31E2, 0x31E3, 0x31E4, 0x31E5, 0x31E6, 0x31E7, 0x31E8, 0x31E9 };
				foreach (int chest in keyChests)
				{
					if (rom[chest] == key)
					{
						q++;
						break;
					}
				}
				//Lute in TFC

				if (rom[0x31A5] == 0x01)
				{
					r++;
				}

				if (((p + q + r) > 1) && (r > 0))
				{

					lock (outfile)
					{
						outfile.WriteLine($"Found {p + q + r} of 3 conditions ({numOrDash(p)}{numOrDash(q)}{numOrDash(r)}): {rseed.ToHex()}");
					}
					Console.WriteLine($"\rFound {p + q + r} of 3 conditions ({numOrDash(p)}{numOrDash(q)}{numOrDash(r)}): {rseed.ToHex()}                                              ");

				}
				seedsGenerated++;
				drawUpdate();
			}
			void falcomic_plando(int seed)
			{
				flags = Flags.DecodeFlagsText("PAK!H0BPnEHAHJ!fPASeUoYAGYU");
				var rom = new FF1Rom(ROMPATH);
				Blob rseed = Blob.Random(4);
				rom.Randomize(rseed, flags);
				int p = 0;
				//int locChaosHP = 0x30F10;
				//int locChaosDef = 0x30F15;
				//int locChaosEvade = 0x30F14;
				//int locChaosHits = 0x30F16;

				//int chaosHP = BitConverter.ToUInt16(rom.Get(0, 0x40000), locChaosHP);
				//int chaosDef = rom[locChaosDef];
				//int chaosEvade = rom[locChaosEvade];
				//int chaosHits = rom[locChaosHits];

				//if (chaosHP > 5499) p++;
				//if (chaosDef >= 180 && chaosDef <= 240) p++;
				//if (chaosEvade >= 180 && chaosEvade <= 240) p++;
				//if (chaosHits >= 4) p++;

				//if (rom[0x3A0DE] == 0x00) p++;

				var chars = new byte[] { rom[0x3A0AE], rom[0x3A0BE], rom[0x3A0CE], rom[0x3A0DE] };

				if (rom[0x3A0AE] == CHAR_BM) p++;
				if (rom[0x3A0BE] == CHAR_BM) p++;
				if (rom[0x3A0CE] == CHAR_BM) p++;
				if (rom[0x3A0DE] == CHAR_BM) p++;

				if (p == 4)
				{
					lock (outfile)
					{
						outfile.WriteLine($"Found target seed: {rseed.ToHex()}");
					}
					Console.WriteLine($"\rFound target seed: {rseed.ToHex()}                                      ");
				}
				seedsGenerated++;
				//Console.Clear();

				drawUpdate();
			}


			System.Threading.Tasks.Parallel.For(0, seeds, falcomic_plando);



			//Console.Read();
			//flags = Flags.DecodeFlagsText("HACPHJBFYAAADA!fHPoYIB");
			//string ROMPATH = @"D:\games\emulators\NES Favorites\Final Fantasy (U) [!].nes";
			//int seeds = 500;
			//List<double> times = new List<double>();
			//DateTime totalstart = DateTime.Now;
			//for (int i = 0; i < seeds; i++)
			//{
			//	DateTime start = DateTime.Now;
			//	var rom = new FF1Rom(ROMPATH);
			//	Blob rseed = Blob.Random(4);
			//	rom.Randomize(rseed, flags);
			//	times.Add((DateTime.Now - start).TotalSeconds);
			//	Console.Clear();
			//	Console.WriteLine($"Generated {i} seeds.");
			//}
			//Console.WriteLine($"Generated {seeds} seeds in {(totalstart - DateTime.Now).TotalSeconds} seconds.");
			//Console.WriteLine($"Fastest: {times.Min()}, Slowest: {times.Max()}, Average: {times.Average()}");
			//var outfile = System.IO.File.CreateText("d:\\seedtimes_stable.txt");
			//foreach (double d in times)
			//{
			//	outfile.WriteLine($"{d}");
			//}
			outfile.Close();
			Console.WriteLine($"Searched {seeds} seeds.  Process terminated.");
			Console.ReadKey();
		}
	}
}
