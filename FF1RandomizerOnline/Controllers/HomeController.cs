﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FF1RandomizerOnline.Models;
using FF1Lib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using RomUtilities;

namespace FF1RandomizerOnline.Controllers
{
    public class HomeController : Controller
    {
	    private readonly IHostingEnvironment _environment;

	    public HomeController(IHostingEnvironment environment)
	    {
		    _environment = environment;
	    }

	    public override void OnActionExecuting(ActionExecutingContext context)
	    {
		    base.OnActionExecuting(context);

			var betaString = _environment.IsDevelopment() ? " Beta" : "";
			ViewData["Title"] = "FF1 Randomizer Online " + FF1Rom.Version + betaString;
	    }

	    public IActionResult Index()
	    {
		    return View();
	    }

	    [HttpGet]
	    public IActionResult Randomize()
	    {
		    return View(new RandomizeViewModel
		    {
			    File = null,
				Seed = Blob.Random(4).ToHex(),
				Flags = new Flags
				{
					Treasures = true,
					IncentivizeIceCave = true,
					IncentivizeOrdeals = true,
					Shops = true,
					MagicShops = false,
					MagicLevels = true,
					MagicPermissions = false,
					Rng = true,
					EnemyScripts = true,
					EnemySkillsSpells = true,
					EnemyStatusAttacks = true,
					Ordeals = true,

					EarlyRod = true,
					EarlyCanoe = true,
					EarlyOrdeals = true,
					EarlyBridge = true,
					NoPartyShuffle = true,
					SpeedHacks = true,
					IdentifyTreasures = true,
					Dash = true,
					BuyTen = true,

					HouseMPRestoration = true,
					WeaponStats = true,
					ChanceToRun = true,
					SpellBugs = true,
					EnemyStatusAttackBug = true,

					FunEnemyNames = true,
					PaletteSwap = true,
					TeamSteak = true,
					ShuffleLeader = true,
					Music = MusicShuffle.None,

					PriceScaleFactor = 2.0,
					EnemyScaleFactor = 2.0,
					ExpMultiplier = 2.0,
					ExpBonus = 100
				}
			});
	    }

	    [HttpPost]
		[ValidateAntiForgeryToken]
	    public async Task<IActionResult> Randomize(RandomizeViewModel viewModel)
	    {
			// Easier to just early return here and not have to verify viewModel.File != null repeatedly.
			if (!ModelState.IsValid)
		    {
			    return View(viewModel);
		    }

			if (viewModel.File.Length < 256 * 1024 || viewModel.File.Length > (256 + 8) * 1024)
			{
				ModelState.AddModelError("File", "Unexpected file length, FF1 ROM should be close to 256 kB.");
			}

			var rom = await FF1Rom.CreateAsync(viewModel.File.OpenReadStream());
			if (!rom.Validate())
			{
				ModelState.AddModelError("File", "File does not appear to be a valid FF1 NES ROM.");
			}

			if (!ModelState.IsValid)
		    {
			    return View(viewModel);
		    }

			rom.Randomize(Blob.FromHex(viewModel.Seed), viewModel.Flags);

		    var filename = viewModel.File.FileName;
		    var extensionIndex = filename.LastIndexOf('.');
		    var newFilename = extensionIndex == -1 ? filename : filename.Substring(0, extensionIndex);
		    newFilename = $"{newFilename}_{viewModel.Seed}_{FF1Rom.EncodeFlagsText(viewModel.Flags)}.nes";

		    Response.StatusCode = 200;
			Response.ContentLength = rom.TotalLength;
		    Response.ContentType = "application/octet-stream";
			Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{newFilename}\"");
			
			await rom.SaveAsync(Response.Body);
			Response.Body.Close();

			return new EmptyResult();
	    }

		public IActionResult Error()
        {
            return View();
        }
    }
}
