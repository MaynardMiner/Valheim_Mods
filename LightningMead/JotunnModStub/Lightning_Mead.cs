// JotunnModStub
// a Valheim mod skeleton using Jötunn
// 
// File:    JotunnModStub.cs
// Project: JotunnModStub

using BepInEx;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace JotunnModStub
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Lightning_Mead : BaseUnityPlugin
    {
        private AssetBundle Meadbase_light;
        private AssetBundle Mead_light;

        public const string PluginGUID = "com.maynard.lightningmead";
        public const string PluginName = "LightningMead";
        public const string PluginVersion = "0.0.1";

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake()
        {
            // Jotunn comes with MonoMod Detours enabled for hooking Valheim's code
            // https://github.com/MonoMod/MonoMod
            On.FejdStartup.Awake += FejdStartup_Awake;

            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("lightning mead is loading");

            // To learn more about Jotunn's features, go to
            // https://valheim-modding.github.io/Jotunn/tutorials/overview.html

            // Load assets, add the item, and add the fermenter ItemConversion
            LoadAssets();
            CreateBlueprintMead();
            Create_Light_Mead();
            AddConversions();
        }

        private void FejdStartup_Awake(On.FejdStartup.orig_Awake orig, FejdStartup self)
        {
            // This code runs before Valheim's FejdStartup.Awake
            //Jotunn.Logger.LogInfo("FejdStartup is going to awake");

            // Call this method so the original game method is invoked
            orig(self);

            // This code runs after Valheim's FejdStartup.Awake
            //Jotunn.Logger.LogInfo("FejdStartup has awoken");
        }

        private void LoadAssets()
        {
            /// Load embedded assets for mead.
            Jotunn.Logger.LogInfo($"Embedded resources: {string.Join(",", typeof(Lightning_Mead).Assembly.GetManifestResourceNames())}");
            Meadbase_light = AssetUtils.LoadAssetBundleFromResources("meadbase_light", typeof(Lightning_Mead).Assembly);
            Mead_light = AssetUtils.LoadAssetBundleFromResources("mead_light", typeof(Lightning_Mead).Assembly);
        }

        /// This will create the mead base and add it to the game's item list as a CustomItem.
        private void CreateBlueprintMead()
        {
            // Create and add a custom item
            var light_meadbase = Meadbase_light.LoadAsset<GameObject>("MeadBaseLightResist");
            var meadbase = new CustomItem(light_meadbase, fixReference: false,
                new ItemConfig
                {
                    Amount = 1,
                    CraftingStation = "piece_cauldron",
                    MinStationLevel = 1,
                    Name = "$light_meadbase_name", 
                    Description = "$light_meadbase_description",
                    Requirements = new[]
                    {
                        new RequirementConfig
                        {
                            Item = "Thunderstone",
                            Amount = 2,           
                            AmountPerLevel = 0,
                            Recover = false,
                        },
                        new RequirementConfig
                        {
                            Item = "Honey",
                            Amount = 10,           
                            AmountPerLevel = 0,
                            Recover = false,
                        },
                        new RequirementConfig
                        {
                            Item = "Crystal",
                            Amount = 4,        
                            AmountPerLevel = 0,
                            Recover = false,
                        }

                    }
                });
            ItemManager.Instance.AddItem(meadbase);
        }

        // This creates the lightning mead itself. It is technically a non-craftable item as it is converted from fermenter.
        // It needs to be added to the item list before I can actually create the fermenter converstion.
        private void Create_Light_Mead()
        {
            // Create and add a custom item
            var light_mead = Mead_light.LoadAsset<GameObject>("MeadLightResist");
            var mead = new CustomItem(light_mead, fixReference: false,
                new ItemConfig
                {
                    Name = "$light_mead_name",
                    Description = "$light_mead_description",
                    Enabled = false
                });
            ItemManager.Instance.AddItem(mead);
        }


        // This adds the ability to convert lightning mead base to lighning resist mead in the fermenter using
        // Jotunn's CustomItemConversion function.
        private void AddConversions()
        {
            var light_meadbase = Meadbase_light.LoadAsset<GameObject>("MeadBaseLightResist");
            var light_mead = Mead_light.LoadAsset<GameObject>("MeadLightResist");
            // Create a custom recipe with a RecipeConfig
            CustomItemConversion lightmeadRecipe = new CustomItemConversion(new FermenterConversionConfig()
            {
                ProducedItems = 4,
                FromItem = light_meadbase.name,
                ToItem = light_mead.name,
                Station = "fermenter"
            });

            ItemManager.Instance.AddItemConversion(lightmeadRecipe);
        }

    }
}
