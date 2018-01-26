using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RomUtilities;

namespace FF1Lib
{
	public partial class FF1Rom : NesRom
	{
		private const int TotalOrbsToInsert = 32;

		public void ShiftEarthOrbDown()
		{
			// The orb rewarding code is inefficient enough there was room to add in giving you a shard as well.
			// When not playing shard hunt this value is unused, but we always run this code because we always
			// shuffle the earth orb orbs down 4 location to make the code simpler throughout FFR.
			// Getting free shards for killing fiends is an interesting semi-incentive and adds an edge to this game.
			// See OF_CE12_ShardRewards.asm
			Put(0x7CE12, Blob.FromHex("A202A000F010A202A001D00AA203A002D004A204A003B93160D00FA901993160188A6D35608D3560E66C1860"));
			Put(0x7CDB9, Blob.FromHex("12CE18CE1ECE24CE")); // Orb handling jump table.

			// Now anyplace that refers to orb_earth in the assembly outside of the above code
			// is going to need updating to the new address. Earth Orb is pretty popular actually.
			List<int> earthOrbPtrsLowBytes = new List<int> {
				0x39483, // Canoe Sage when not using Early Canoe
				0x3950C, // Talk_BlackOrb
				0x39529, // Talk_4Orb (bats in ToF)
				0x39561, // Talk_ifearthvamp
				0x39577, // Talk_ifearthfire
				0x3B8A0, // DrawOrbBox in the main menu
				0x7CE04, // SMMove_4Orbs
			};
			earthOrbPtrsLowBytes.ForEach(address =>
			{
				// It's entirely possible some future mods might touch these addresses so
				// let's put a litle guard here.
				System.Diagnostics.Debug.Assert(Data[address] == 0x35);
				Data[address] = 0x31;
			});

			Data[0x7EF45] = 0x11; // Skip over orbs and shards when printing the item menu
		}

		public void EnableShardHunt(MT19337 rng)
		{
			// Replace unused CANOE string and EarthOrb pointer with whatever we're calling the scavenged item.
			Put(0x2B981, FF1Text.TextToBytes("SHARD  ", false, FF1Text.Delimiter.Null));
			Data[0x2B72A] = 0x81;

			// Now actually print shards when printing items, and print them with quantity
			Data[0x7EF49] = 0x15;
			Data[0x7EF91] = 0x15;

			// Replace the upper two tiles of the unlit orb with an empty and found shard.
			// These are at tile address $76 and $77 respectively.
			Put(0x37760, Blob.FromHex("001C22414141221CFFE3DDBEBEBEDDE3001C3E7F7F7F3E1CFFFFE3CFDFDFFFFF"));

			// Hard code the total number of shards and where we start depending on how many are needed
			int goal = rng.Between(16, 24);
			String hexCount = goal.ToString("X2");
			String ppuLowByte = goal <= 24 ? "63" : "43";

			// Fancy shard drawing code, see 0E_B8D7_DrawShardBox.asm
			Put(0x3B87D, Blob.FromHex($"A9{ppuLowByte}8511A977A00048AD0220A9208D0620A51118692085118D0620900DAD0220A9218D0620A9038D062068A200CC3560D002A976C0{hexCount}D001608D0720C8E8E006D0EB1890C3"));

			// Black Orb Override to jump to the final floor. This allows us to give out some last minute loot and
			// and make the repeated attempts the final battle strategy take a little longer due to some walking.
			Put(0x39502, Blob.FromHex($"AD3560C9{hexCount}300CA0CA209690E67DE67DA51160A51260"));

			Put(0x7CDB3, Blob.FromHex("08CE"));
			Data[0x00D80] = 0x80; // Map edits
			Data[0x02D01] = 0x0F;
			Data[0x02D41] = 0x03;
			Data[0x02D81] = 0x3B;

			// ToFR Map Hacks
			Put(0x1A899, Blob.FromHex("0C00810502BF38000462615D5E5F0402BF3334B0020004638405600402B00235BF2D3430B10303840905B1033035BF2A34B1033330068409083032B10335BF2834B10233B00231300684021004118402083031B00232B10235BF2732313330B104300687050830B10430323133BF2634B10233B10538B00336B00338B10532B10235BF2532313330B1053831B8023AB802"));

			// A little narrative overhaul.
			Put(0x289B2, FF1Text.TextToBytes("The SHARDS coalesce to\nrestore the Black ORB.\n\nBrave Light Warriors....\nDestroy the Evil within!")); // Black Orb Text
			Put(0x28CF8, FF1Text.TextToBytes("Ah, the Light Warriors!\n\nSo you have collected\nthe SHARDS and restored\nthe BLACK ORB."));
			Put(0x28D57, FF1Text.TextToBytes("Thus you've travelled\n2000 years into the past\nto try to stop me?\n\nStep forward then,\nto your peril!"));
			Put(0x28DAF, FF1Text.TextToBytes("Oh, Light Warriors!\nSuch arrogant bravery.\n\nLet us see whom history\nremembers. En Garde!"));

			// This ugliness picks a random FinalFormation
			transformFinalFormation((FinalFormation)rng.Between(0, Enum.GetValues(typeof(FinalFormation)).Length - 1));
		}

		public Item ShardHuntTreasureSelector(Item item)
		{
			// The following pile of trash, plus Gold chests from 20 to 400 inclusive amount to precisely 32 chests.
			List<Item> trash = new List<Item> { Item.Lute, Item.Heal, Item.Pure, Item.SmallKnife,
				Item.WoodenRod, Item.Cloth, Item.WoodenShield, Item.Cap, Item.WoodenHelm, Item.Gloves };

			return (trash.Contains(item) || item >= Item.Gold20 && item <= Item.Gold350) ? Item.Shard : item;
		}
	}
}
