﻿using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using CsvHelper;
using HarmonyLib;
using Il2CppSystem.IO;
using Peasmod4.API;
using Peasmod4.API.Components;
using Peasmod4.API.UI.Options;
using Peasmod4.Resources;
using Reactor;
using Reactor.Patches;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EventType = Peasmod4.API.Events.EventType;

namespace Peasmod4;

[HarmonyPatch]
[BepInAutoPlugin(ModId, "Peasmod", "4.0.0")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class PeasmodPlugin : BasePlugin
{
    public const string ModId = "xyz.peasplayer.peasmod4";

    public static CustomToggleOption ShowRolesToDead;

    public static bool DestroyIntro = true;

    public PeasmodPlugin()
    {
        Logger = Log;
        ConfigFile = Config;
        Language.Load();

        LoadModMainOptions();

        ResourceManager.LoadAssets();
        RegisterCustomRoleAttribute.Load();
        RegisterEventListenerAttribute.Load();
    }

    public static ManualLogSource Logger { get; private set; }

    public static ConfigFile ConfigFile { get; private set; }

    public Harmony Harmony { get; } = new(Id);

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    [HarmonyPostfix]
    private static void SwitchSettingsPagesPatch(KeyboardJoystick __instance)
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
        }
    }

    [RegisterEventListener(EventType.GameStart)]
    public static void TestListener(object sender, EventArgs test)
    {
        Logger.LogInfo("HALLO WELLLLLT");
    }

    public static void LoadModMainOptions()
    {
        new CustomHeaderOption(MultiMenu.Main, "option.GameplaySettings.header");
        ShowRolesToDead = new CustomToggleOption(MultiMenu.Main, "option.main.ShowRolesToDead");
    }

    public override void Load()
    {
#if !API
        ReactorVersionShower.TextUpdated += text =>
        {
#if !DEV
            var versionText = $"\n{Utility.StringColor.Red}Peasmod{Utility.StringColor.Reset} v{Version}";
#endif
#if DEV
            var versionText = $"\n{Utility.StringColor.Red}Peasmod{Utility.StringColor.Reset} {Utility.StringColor.Red}Unstable Version!{Utility.StringColor.Reset}";
#endif

            text.text += versionText;
        };
#endif

        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            if (scene.name == "MainMenu") CustomRegionManager.AddCustomRegions();
        }));

        CustomRegionManager.AddRegion("Peaspowered", "https://auhk.fangkuai.fun", 443);

        Harmony.PatchAll();
    }

#if !API
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static void Test(MainMenuManager __instance)
    {
        var logo = GameObject.Find("LOGO-AU");
        logo.GetComponent<SpriteRenderer>().sprite = ResourceManager.PeasmodLogo;
        logo.transform.parent = GameObject.Find("RightPanel").transform;
        logo.transform.localPosition = new Vector3(-0.3f, 2.25f, 4.5f);

        var leftPanel = GameObject.Find("LeftPanel");
        var cachePos = leftPanel.transform.position;
        leftPanel.transform.position = new Vector3(cachePos.x, -0.28f, cachePos.z);
        leftPanel.transform.localScale = new Vector3(1, 1.18f, 1);
        var mainButtons = leftPanel.transform.FindChild("Main Buttons");
        mainButtons.localScale = new Vector3(1, 0.82f, 1);
        cachePos = mainButtons.position;
        mainButtons.position = new Vector3(cachePos.x, -0.6f, cachePos.z);
        leftPanel.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.27f, 0.45f);

        var cacheButton = GameObject.Find("PlayButton");
        cachePos = cacheButton.transform.position;
        cacheButton.transform.position = new Vector3(cachePos.x, cachePos.y + 0.6409f, -0.2789f);

        cacheButton = GameObject.Find("ShopButton");
        cachePos = cacheButton.transform.position;
        cacheButton.transform.position = new Vector3(cachePos.x, cachePos.y + 0.6409f, -0.2789f);

        cacheButton = GameObject.Find("Inventory Button");
        cachePos = cacheButton.transform.position;
        cacheButton.transform.position = new Vector3(cachePos.x, cachePos.y + 0.6409f, -0.2789f);

        var newButton = GameObject.Instantiate(cacheButton, mainButtons.transform);
        newButton.name = "Peasmod Button";
        newButton.transform.position = new Vector3(cachePos.x, cachePos.y - 0.6409f, -0.2789f);

        var text = newButton.transform.FindChild("FontPlacer").FindChild("Text_TMP").gameObject;
        Logger.LogInfo(text == null);
        text.GetComponent<TextTranslatorTMP>().Destroy();
        text.GetComponent<TextMeshPro>().text = "Peasmod";

        var buttonComp = newButton.GetComponent<PassiveButton>();
        buttonComp.OnClick = new Button.ButtonClickedEvent();
        buttonComp.AddOnClickListeners(new Action(() =>
        {
            var creditsController = GameObject
                .Find("MainMenuManager/MainUI/AspectScaler/RightPanel/CreditsSizer/CreditsScreen")
                .GetComponent<CreditsScreenPopUp>().CreditsController;
            creditsController.ClearCredits();
            var csvReader =
                new CsvReader(new StringReader(DestroyableSingleton<ReferenceDataManager>.Instance.Refdata.credits
                    .ToString()));
            creditsController.ClearCredits();
            while (csvReader.Read())
            {
                var creditStruct = new CreditsController.CreditStruct(); //default(CreditsController.CreditStruct);
                var field = csvReader.GetField<string>("Format");
                Logger.LogInfo("1: " + field);
                creditStruct.format = field;
                creditStruct.columns = new string[creditsController.NumberOfContentColumns];
                Logger.LogInfo("2: " + creditStruct.columns);
                for (var i = 1; i <= creditsController.NumberOfContentColumns; i++)
                {
                    var field2 = csvReader.GetField<string>("Column" + i);

                    Logger.LogInfo("3: " + field2);
                    creditStruct.columns[i - 1] = field2;
                }

                creditsController.AddCredit(creditStruct);
                creditsController.AddFormat(field);
            }
            //PeasmodPlugin.Logger.LogInfo("Button pressed: " + tmp3);
        }));
    }
#endif

#if !API
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static void PingPatch(PingTracker __instance)
    {
        var position = __instance.GetComponent<AspectPosition>();
        if (AmongUsClient.Instance.IsGameStarted)
        {
            __instance.text.alignment = TextAlignmentOptions.Top;
            position.Alignment = AspectPosition.EdgeAlignments.Top;
            position.DistanceFromEdge = new Vector3(1.5f, 0.11f, 0);
        }
        else
        {
            position.Alignment = AspectPosition.EdgeAlignments.LeftTop;
            __instance.text.alignment = TextAlignmentOptions.TopLeft;
            position.DistanceFromEdge = new Vector3(0.5f, 0.11f);
        }
#if !DEV

        var pingText = $"\n{Utility.StringColor.Red}Peasmod{Utility.StringColor.Reset} v{Version}";
#endif
#if DEV
        var pingText = $"\n{Utility.StringColor.Red}Peasmod{Utility.StringColor.Reset} {Utility.StringColor.Red}Unstable Version!{Utility.StringColor.Reset}";
#endif
        __instance.text.text += pingText;
    }
#endif
}