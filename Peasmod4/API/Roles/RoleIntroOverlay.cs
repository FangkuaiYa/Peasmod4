using System;
using UnityEngine;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using Peasmod4.Resources;
using Peasmod4.API.Roles;

namespace Peasmod4;

[Harmony]
public class CustomOverlay
{
    public static bool overlayShown = false;
    private static SpriteRenderer roleUnderlay;
    private static TMPro.TextMeshPro infoOverlayRoles;
    private static Sprite colorBG;
    private static SpriteRenderer meetingUnderlay;
    private static SpriteRenderer infoUnderlay;
    private static Scroller scroller;

    public static bool initializeOverlays()
    {
        HudManager hudManager = HudManager.Instance;
        if (hudManager == null) return false;

        if (meetingUnderlay == null)
        {
            meetingUnderlay = UnityEngine.Object.Instantiate(hudManager.FullScreen, hudManager.transform);
            meetingUnderlay.transform.localPosition = new Vector3(0f, 0f, 20f);
            meetingUnderlay.gameObject.SetActive(true);
            meetingUnderlay.enabled = false;
        }

        if (colorBG == null)
        {
            colorBG = ResourceManager.IntroColorBG;
        }

        if (infoOverlayRoles == null)
        {
            infoOverlayRoles = UnityEngine.Object.Instantiate(hudManager.TaskPanel.taskText, hudManager.transform);
            infoOverlayRoles.maxVisibleLines = 28;
            infoOverlayRoles.outlineWidth += 0.02f;
            infoOverlayRoles.autoSizeTextContainer = false;
            infoOverlayRoles.enableWordWrapping = false;
            infoOverlayRoles.alignment = TMPro.TextAlignmentOptions.TopLeft;
            infoOverlayRoles.transform.localPosition = new Vector3(-5f, 1.15f, -900f) + new Vector3(2.5f, 0.0f, 0.0f);
            infoOverlayRoles.color = Palette.White;
            infoOverlayRoles.enabled = false;
        }

        if (infoUnderlay == null)
        {
            infoUnderlay = UnityEngine.Object.Instantiate(meetingUnderlay, hudManager.transform);
            infoUnderlay.transform.localPosition = new Vector3(0f, 0f, -900f);
            infoUnderlay.gameObject.SetActive(true);
            infoUnderlay.enabled = false;
        }

        if (scroller == null)
        {
            scroller = infoUnderlay.gameObject.AddComponent<Scroller>();
            scroller.Inner = infoOverlayRoles.transform;
            scroller.allowY = true;
            scroller.gameObject.SetActive(true);
            scroller.enabled = false;
        }

        return true;
    }

    public static void hideBlackBG()
    {
        if (meetingUnderlay == null) return;
        meetingUnderlay.enabled = false;
    }

    public static void OpenSettings(HudManager __instance)
    {
        settingsBackground = GameObject.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
    }

    private static TMPro.TextMeshPro[] settingsTMPs = new TMPro.TextMeshPro[4];
    private static GameObject settingsBackground;
    public static void CloseSettings()
    {
        foreach (var tmp in settingsTMPs)
            if (tmp) tmp.gameObject.Destroy();

        if (settingsBackground) settingsBackground.Destroy();
    }

    public static string cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }


    public static void showInfoOverlay()
    {
        if (overlayShown) return;

        HudManager hudManager = HudManager.Instance;
        if (ShipStatus.Instance == null || PlayerControl.LocalPlayer == null || hudManager == null || HudManager.Instance.IsIntroDisplayed || (!PlayerControl.LocalPlayer.CanMove && MeetingHud.Instance == null))
            return;

        if (!initializeOverlays()) return;

        CloseSettings();

        if (MapBehaviour.Instance != null)
            MapBehaviour.Instance.Close();

        hudManager.SetHudActive(false);

        overlayShown = true;

        Transform parent;
        if (MeetingHud.Instance != null)
            parent = MeetingHud.Instance.transform;
        else
            parent = hudManager.transform;

        infoUnderlay.transform.parent = parent;
        infoOverlayRoles.transform.parent = parent;

        infoUnderlay.sprite = colorBG;
        infoUnderlay.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        infoUnderlay.transform.localScale = new Vector3(7.5f, 5f, 1f);
        infoUnderlay.enabled = true;

        string rolesText = "";
        var customRole = PlayerControl.LocalPlayer.GetCustomRole();
        if (customRole != null)
        {
            rolesText += $"<size=180%>{Utility.ColorString(customRole.Color, customRole.Name)}</size>" + "\n" +
                $"<size=130%>{Utility.ColorString(customRole.Color, customRole.LongDescription)}</size>" + "\n\n";
        }
        else
        {
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                rolesText += $"<size=180%>{Utility.ColorString(Palette.ImpostorRed, DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Impostor))}</size>" + "\n" +
                    $"<size=130%>{Utility.ColorString(Palette.ImpostorRed, DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.ImpostorBlurb))}</size>" + "\n\n";
            }
            else
            {
                rolesText += $"<size=180%>{Utility.ColorString(Palette.CrewmateBlue, DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Crewmate))}</size>" + "\n" +
                    $"<size=130%>{Utility.ColorString(Palette.CrewmateBlue, DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.CrewmateBlurb))}</size>" + "\n\n";
            }
        }

        infoOverlayRoles.text = rolesText;
        infoOverlayRoles.enabled = true;

        var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        HudManager.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            infoUnderlay.color = Color.Lerp(underlayTransparent, underlayOpaque, t);
            infoOverlayRoles.color = Color.Lerp(Palette.ClearWhite, Palette.White, t);
        })));
    }

    public static void hideInfoOverlay()
    {
        if (!overlayShown) return;

        if (MeetingHud.Instance == null) HudManager.Instance.SetHudActive(true);

        overlayShown = false;
        var underlayTransparent = new Color(0.1f, 0.1f, 0.1f, 0.0f);
        var underlayOpaque = new Color(0.1f, 0.1f, 0.1f, 0.88f);

        if (scroller != null) scroller.enabled = false;

        HudManager.Instance.StartCoroutine(Effects.Lerp(0.2f, new Action<float>(t =>
        {
            if (infoUnderlay != null)
            {
                infoUnderlay.color = Color.Lerp(underlayOpaque, underlayTransparent, t);
                if (t >= 1.0f) infoUnderlay.enabled = false;
            }

            if (infoOverlayRoles != null)
            {
                infoOverlayRoles.color = Color.Lerp(Palette.White, Palette.ClearWhite, t);
                if (t >= 1.0f) infoOverlayRoles.enabled = false;
            }
        })));
    }

    public static void toggleInfoOverlay()
    {
        if (overlayShown)
            hideInfoOverlay();
        else
            showInfoOverlay();
    }

    public static void resetOverlays()
    {
        hideBlackBG();
        hideInfoOverlay();
        UnityEngine.Object.Destroy(meetingUnderlay);
        UnityEngine.Object.Destroy(infoUnderlay);
        UnityEngine.Object.Destroy(infoOverlayRoles);
        UnityEngine.Object.Destroy(roleUnderlay);
        UnityEngine.Object.Destroy(scroller);

        overlayShown = false;
        roleUnderlay = null;
        meetingUnderlay = infoUnderlay = null;
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class CustomOverlayKeybinds
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            ChatController cc = DestroyableSingleton<ChatController>.Instance;
            bool isOpen = cc != null && cc.IsOpenOrOpening;
            if (Input.GetKeyDown(KeyCode.H) && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && !isOpen && !Minigame.Instance && !ExileController.Instance)
            {
                toggleInfoOverlay();
            }
        }
    }
}
