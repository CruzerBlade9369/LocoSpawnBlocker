using System;
using System.Reflection;

using HarmonyLib;
using UnityModManagerNet;

using UnityEngine;

namespace LocoSpawnBlocker;

[EnableReloading]
public static class Main
{
	public static bool enabled;
	public static UnityModManager.ModEntry? mod;

	public static Settings settings { get; private set; }

	private static bool Load(UnityModManager.ModEntry modEntry)
	{
		Harmony? harmony = null;

		try
		{
			try
			{
				settings = Settings.Load<Settings>(modEntry);
			}
			catch
			{
				Debug.LogWarning("Unabled to load mod settings. Using defaults instead.");
				settings = new Settings();
			}
			mod = modEntry;

			harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			DebugLog("Attempting patch.");

			modEntry.OnGUI = OnGui;
			modEntry.OnSaveGUI = OnSaveGui;
		}
		catch (Exception ex)
		{
			modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
			harmony?.UnpatchAll(modEntry.Info.Id);
			return false;
		}

		return true;
	}

	static void OnGui(UnityModManager.ModEntry modEntry)
	{
		settings.Draw(modEntry);
	}

	static void OnSaveGui(UnityModManager.ModEntry modEntry)
	{
		settings.Save(modEntry);
	}

	public static void DebugLog(string message)
	{
		if (settings.isLoggingEnabled)
			mod?.Logger.Log(message);
	}
}
