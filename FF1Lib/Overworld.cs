﻿using FF1Lib.Sanity;
using RomUtilities;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FF1Lib
{
	public class Overworld
	{

		private const int lut_BtlBackdrops = 0xB300;
		private const int lut_BtlBackdrops_Bank = 0x00;

		private OverworldMap overworldMap;
		private OwMapExchange owMapExchange;
		//private OwMapExchangeData owMapExchangeData;
		private OwLocationData locations;
		private ShipLocations shipLocations;
		private DomainData domains;
		private TileSet owTileSet;

		private Flags flags;
		private FF1Rom rom;
		private MT19337 rng;

		public List<Backdrop> BattleBackdrops { get; private set; }
		public TileSet TileSet { get; private set; }
		public List<List<byte>> DecompressedMap { get => overworldMap.MapBytes; }
		public OwMapExchangeData MapExchangeData { get => owMapExchange?.Data; }
		public OwMapExchange MapExchange { get => owMapExchange; }
		public OverworldMap OverworldMap { get => overworldMap; }
		public OwLocationData Locations { get => locations; }
		public ShipLocations ShipLocations { get => shipLocations; }
		public Overworld(FF1Rom _rom, Flags _flags, Settings _settings, MT19337 _rng)
		{
			flags = _flags;
			rom = _rom;
			rng = _rng;

			locations = new OwLocationData(rom);
			overworldMap = new OverworldMap(rom, flags);
			domains = new DomainData(rom);
			owTileSet = new TileSet(rom, 0xFF);
			domains.LoadTable();
			//owMapExchange = new(rom, );
			//locations = new OwLocationData(_rom);
			shipLocations = OwMapExchange.GetDefaultShipLocations(locations);
			BattleBackdrops = rom.GetFromBank(lut_BtlBackdrops_Bank, lut_BtlBackdrops, 0x80).ToBytes().Select(b => (Backdrop)b).ToList();
			//_owMapExchange = await OwMapExchange.FromFlags(this, _overworldMap, flags, rng);


		}
		public void Write()
		{
			domains.StoreTable();
			locations.StoreData();
			owTileSet.StoreData();
			rom.PutInBank(lut_BtlBackdrops_Bank, lut_BtlBackdrops, BattleBackdrops.Select(b => (byte)b).ToArray());
			//overworldMap.
		}
		public async void LoadMapExchange()
		{
			owMapExchange = await OwMapExchange.FromFlags(rom, overworldMap, flags, rng);
			owMapExchange?.UpdateBridgeSprite();
			if (owMapExchange != null)
			{
				UpdateOverworld(owMapExchange);
				shipLocations = new(locations, owMapExchange.Data.ShipLocations);
			}
			//locations = o
			//shipLocations = owMapExchange?.ShipLocations ?? OwMapExchange.GetDefaultShipLocations(locations);
		}
		public void UpdateOverworld(OwMapExchange owdata)
		{
			if (owdata.Data.DecompressedMapRows != null)
			{
				overworldMap.SwapMap(owdata.Data.DecompressedMapRows);
			}
			else
			{
				overworldMap.SwapMap(owdata.Name + ".ffm");
			}

			locations.GetFrom(owdata.Data);

			owdata.UpdateDomains(domains);
		}
		public void Update(Settings settings)
		{
			shipLocations.UpdateDocks(settings);
		}
		public SCCoords SetShipLocation(int dungeonindex)
		{
			return shipLocations.SetShipLocation(dungeonindex);
		}
		public ShipLocation GetShipLocation(int dungeonindex)
		{
			return shipLocations.GetShipLocation(dungeonindex);
		}
		public void Update(Teleporters teleporters)
		{
			shipLocations.UpdateDocks(flags);

			if ((bool)flags.ShuffleChimeAccess)
			{
				overworldMap.ShuffleChime(rng, (bool)flags.ShuffleChimeIncludeTowns);
			}

			// we just want to upadte palette here, so this should be a teleporter thing
			overworldMap.ShuffleEntrancesAndFloors(rng, teleporters, flags);

			if ((bool)flags.Treasures && (bool)flags.ShuffleObjectiveNPCs && (flags.GameMode != GameModes.DeepDungeon))
			{
				overworldMap.ShuffleObjectiveNPCs(rng);
			}

			if ((bool)flags.IsAirshipFree)
			{
				locations.AirShipLocation = locations.StartingLocation;
			}

			overworldMap.ApplyMapEdits();
		}

	}
}