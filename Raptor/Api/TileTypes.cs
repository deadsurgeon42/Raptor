﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raptor.Api
{
	public enum TileTypes : ushort
	{
		Dirt = 0,
		Stone = 1,
		Grass = 2,
		Plants = 3,
		Torches = 4,
		Trees = 5,
		Iron = 6,
		Copper = 7,
		Gold = 8,
		Silver = 9,
		ClosedDoor = 10,
		OpenDoor = 11,
		Heart = 12,
		Bottles = 13,
		Tables = 14,
		Chairs = 15,
		Anvils = 16,
		Furnaces = 17,
		WorkBenches = 18,
		Platforms = 19,
		Saplings = 20,
		Containers = 21,
		Demonite = 22,
		CorruptGrass = 23,
		CorruptPlants = 24,
		Ebonstone = 25,
		DemonAltar = 26,
		Sunflower = 27,
		Pots = 28,
		PiggyBank = 29,
		WoodBlock = 30,
		ShadowOrbs = 31,
		CorruptThorns = 32,
		Candles = 33,
		Chandeliers = 34,
		Jackolanterns = 35,
		Presents = 36,
		Meteorite = 37,
		GrayBrick = 38,
		RedBrick = 39,
		ClayBlock = 40,
		BlueDungeonBrick = 41,
		HangingLanterns = 42,
		GreenDungeonBrick = 43,
		PinkDungeonBrick = 44,
		GoldBrick = 45,
		SilverBrick = 46,
		CopperBrick = 47,
		Spikes = 48,
		WaterCandle = 49,
		Books = 50,
		Cobweb = 51,
		Vines = 52,
		Sand = 53,
		Glass = 54,
		Signs = 55,
		Obsidian = 56,
		Ash = 57,
		Hellstone = 58,
		Mud = 59,
		JungleGrass = 60,
		JunglePlants = 61,
		JungleVines = 62,
		Sapphire = 63,
		Ruby = 64,
		Emerald = 65,
		Topaz = 66,
		Amethyst = 67,
		Diamond = 68,
		JungleThorns = 69,
		MushroomGrass = 70,
		MushroomPlants = 71,
		MushroomTrees = 72,
		Plants2 = 73,
		JunglePlants2 = 74,
		ObsidianBrick = 75,
		HellstoneBrick = 76,
		Hellforge = 77,
		ClayPot = 78,
		Beds = 79,
		Cactus = 80,
		Coral = 81,
		ImmatureHerbs = 82,
		MatureHerbs = 83,
		BloomingHerbs = 84,
		Tombstones = 85,
		Loom = 86,
		Pianos = 87,
		Dressers = 88,
		Benches = 89,
		Bathtubs = 90,
		Banners = 91,
		Lampposts = 92,
		Lamps = 93,
		Kegs = 94,
		ChineseLanterns = 95,
		CookingPots = 96,
		Safes = 97,
		SkullCandles = 98,
		TrashCan = 99,
		Candelabras = 100,
		Bookcases = 101,
		Thrones = 102,
		Bowls = 103,
		GrandfatherClocks = 104,
		Statues = 105,
		Sawmill = 106,
		Cobalt = 107,
		Mythril = 108,
		HallowedGrass = 109,
		HallowedPlants = 110,
		Adamantite = 111,
		Ebonsand = 112,
		HallowedPlants2 = 113,
		TinkerersWorkbench = 114,
		HallowedVines = 115,
		Pearlsand = 116,
		Pearlstone = 117,
		PearlstoneBrick = 118,
		IridescentBrick = 119,
		Mudstone = 120,
		CobaltBrick = 121,
		MythrilBrick = 122,
		Silt = 123,
		WoodenPlank = 124,
		CrystalBall = 125,
		DiscoBall = 126,
		MagicalIceBlock = 127,
		Mannequin = 128,
		Crystals = 129,
		ActiveStoneBlock = 130,
		InactiveStoneBlock = 131,
		Lever = 132,
		AdamantiteForge = 133,
		MythrilAnvil = 134,
		PressurePlates = 135,
		Switches = 136,
		Traps = 137,
		Boulder = 138,
		MusicBoxes = 139,
		DemoniteBrick = 140,
		Explosives = 141,
		InletPump = 142,
		OutletPump = 143,
		Timers = 144,
		CandyCaneBlock = 145,
		GreenCandyCaneBlock = 146,
		SnowBlock = 147,
		SnowBrick = 148,
		HolidayLights = 149,
		AdamantiteBeam = 150,
		SandstoneBrick = 151,
		EbonstoneBrick = 152,
		RedStucco = 153,
		YellowStucco = 154,
		GreenStucco = 155,
		GrayStucco = 156,
		Ebonwood = 157,
		RichMahogany = 158,
		Pearlwood = 159,
		RainbowBrick = 160,
		IceBlock = 161,
		BreakableIce = 162,
		CorruptIce = 163,
		HallowedIce = 164,
		Stalagtite = 165,
		Tin = 166,
		Lead = 167,
		Tungsten = 168,
		Platinum = 169,
		PineTree = 170,
		ChristmasTree = 171,
		OldPlatinumChandelier = 172,
		PlatinumCandelabra = 173,
		PlatinumCandle = 174,
		TinBrick = 175,
		TungstenBrick = 176,
		PlatinumBrick = 177,
		ExposedGems = 178,
		GreenMoss = 179,
		BrownMoss = 180,
		RedMoss = 181,
		BlueMoss = 182,
		PurpleMoss = 183,
		LongMoss = 184,
		SmallPiles = 185,
		LargePiles = 186,
		LargePiles2 = 187,
		CactusBlock = 188,
		Cloud = 189,
		MushroomBlock = 190,
		LivingWood = 191,
		LeafBlock = 192,
		SlimeBlock = 193,
		BoneBlock = 194,
		FleshBlock = 195,
		RainCloud = 196,
		FrozenSlimeBlock = 197,
		Asphalt = 198,
		FleshGrass = 199,
		RedIce = 200,
		FleshWeeds = 201,
		Sunplate = 202,
		Crimstone = 203,
		Crimtane = 204,
		CrimsonVines = 205,
		IceBrick = 206,
		WaterFountain = 207,
		Shadewood = 208,
		Cannon = 209,
		LandMine = 210,
		Chlorophyte = 211,
		SnowballLauncher = 212,
		Rope = 213,
		Chain = 214,
		Campfire = 215,
		Firework = 216,
		Blendomatic = 217,
		MeatGrinder = 218,
		Extractinator = 219,
		Solidifier = 220,
		Palladium = 221,
		Orichalcum = 222,
		Titanium = 223,
		Slush = 224,
		Hive = 225,
		LihzahrdBrick = 226,
		DyePlants = 227,
		DyeVat = 228,
		HoneyBlock = 229,
		CrispyHoneyBlock = 230,
		Larva = 231,
		WoodenSpikes = 232,
		PlantDetritus = 233,
		Crimsand = 234,
		Teleporter = 235,
		LifeFruit = 236,
		LihzahrdAltar = 237,
		PlanteraBulb = 238,
		MetalBars = 239,
		Painting3x3 = 240,
		Painting4x3 = 241,
		Painting6x4 = 242,
		ImbuingStation = 243,
		BubbleMachine = 244,
		Painting2x3 = 245,
		Painting3x2 = 246,
		Autohammer = 247,
		PalladiumColumn = 248,
		BubblegumBlock = 249,
		Titanstone = 250,
		PumpkinBlock = 251,
		HayBlock = 252,
		SpookyWood = 253,
		Pumpkins = 254,
		AmethystGemsparkOff = 255,
		TopazGemsparkOff = 256,
		SapphireGemsparkOff = 257,
		EmeraldGemsparkOff = 258,
		RubyGemsparkOff = 259,
		DiamondGemsparkOff = 260,
		AmberGemsparkOff = 261,
		AmethystGemspark = 262,
		TopazGemspark = 263,
		SapphireGemspark = 264,
		EmeraldGemspark = 265,
		RubyGemspark = 266,
		DiamondGemspark = 267,
		AmberGemspark = 268,
		Womannequin = 269,
		FireflyinaBottle = 270,
		LightningBuginaBottle = 271,
		Cog = 272,
		StoneSlab = 273,
		SandStoneSlab = 274,
		BunnyCage = 275,
		SquirrelCage = 276,
		MallardDuckCage = 277,
		DuckCage = 278,
		BirdCage = 279,
		BlueJay = 280,
		CardinalCage = 281,
		FishBowl = 282,
		HeavyWorkBench = 283,
		CopperPlating = 284,
		SnailCage = 285,
		GlowingSnailCage = 286,
		AmmoBox = 287,
		MonarchButterflyJar = 288,
		PurpleEmperorButterflyJar = 289,
		RedAdmiralButterflyJar = 290,
		UlyssesButterflyJar = 291,
		SulphurButterflyJar = 292,
		TreeNymphButterflyJar = 293,
		ZebraSwallowtailButterflyJar = 294,
		JuliaButterflyJar = 295,
		ScorpionCage = 296,
		BlackScorpionCage = 297,
		FrogCage = 298,
		MouseCage = 299,
		BoneWelder = 300,
		FleshCloningVaat = 301,
		GlassKiln = 302,
		LihzahrdFurnace = 303,
		LivingLoom = 304,
		SkyMill = 305,
		IceMachine = 306,
		SteampunkBoiler = 307,
		HoneyDispenser = 308,
		PenguinCage = 309,
		WormCage = 310,
		DynastyWood = 311,
		RedDynastyShingles = 312,
		BlueDynastyShingles = 313,
		MinecartTrack = 314,
		Unknown315 = 315,
		BlueJellyfishBowl = 316,
		GreenJellyfishBowl = 317,
		PinkJellyfishBowl = 318,
		ShipInABottle = 319,
		SeaweedPlanter = 320,
		BorealWood = 321,
		PalmWood = 322,
		PalmTree = 323,
		BeachPiles = 324,
		TinPlating = 325,
		Waterfall = 326,
		Lavafall = 327,
		Confetti = 328,
		ConfettiBlack = 329,
		CopperCoinPile = 330,
		SilverCoinPile = 331,
		GoldCoinPile = 332,
		PlatinumCoinPile = 333,
		WeaponsRack = 334,
		FireworksBox = 335,
		LivingFire = 336,
		AlphabetStatues = 337,
		FireworkFountain = 338,
		GrasshopperCage = 339,
	}
}
