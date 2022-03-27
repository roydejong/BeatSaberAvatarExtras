using System.Collections.Generic;
using System.Linq;
using BeatSaberAvatarExtras.Networking;
using BeatSaberAvatarExtras.Utils;
using UnityEngine;

namespace BeatSaberAvatarExtras.Models
{
    public class SpecialColorOption
    {
        public readonly string Key;
        public readonly string Label;
        public readonly Color? MagicColor;

        public SpecialColorOption(string key, string label, Color? magicColor = null)
        {
            Key = key;
            Label = label;
            MagicColor = magicColor;
        }

        public override string ToString() => Label;

        #region Static options

        public static readonly SpecialColorOption Default
            = new SpecialColorOption("default", "Normal", null);

        public static readonly SpecialColorOption Rainbow =
            new SpecialColorOption("rainbow", "Rainbow Shader", Magic.MagicRainbowColor);

        public static readonly List<SpecialColorOption> AllOptions = new() {Default, Rainbow};

        public static SpecialColorOption DetectOptionMagically(Color c)
        {
            foreach (var option in AllOptions.Where(option => c.ApproximatelyEquals(option.MagicColor)))
                return option;
            return Default;
        }

        public static SpecialColorOption? DetectNonDefaultOptionMagically(Color c)
        {
            return AllOptions.FirstOrDefault(option => c.ApproximatelyEquals(option.MagicColor) && option != Default);
        }

        public static SpecialColorOption? DetectNonDefaultOptionMagically(List<Color> colors)
        {
            return (from c in colors
                from option in AllOptions
                where c.ApproximatelyEquals(option.MagicColor) && option != Default
                select option).FirstOrDefault();
        }

        #endregion
    }
}