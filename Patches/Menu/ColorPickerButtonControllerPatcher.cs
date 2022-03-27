using BeatSaberAvatarExtras.Assets;
using BeatSaberAvatarExtras.Networking;
using IPA.Utilities;
using SiraUtil.Affinity;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberAvatarExtras.Patches.Menu
{
    public class ColorPickerButtonControllerPatcher : IAffinity
    {
        private static Sprite? _originalSprite = null;
        
        [AffinityPrefix]
        [AffinityPatch(typeof(ColorPickerButtonController), nameof(ColorPickerButtonController.SetColor))]
        public bool PrefixSetColor(Color color, ColorPickerButtonController __instance)
        {
            var colorImage = __instance.GetField<Image, ColorPickerButtonController>("_colorImage");
            
            if (_originalSprite == null)
                _originalSprite = colorImage.sprite;

            if (color == Magic.MagicRainbowColor)
            {
                colorImage.sprite = Sprites.RainbowCircle;
                colorImage.color = Color.white;
                return false;
            }

            colorImage.sprite = _originalSprite;
            return true;
        }
    }
}