using UnityEngine;

namespace BeatSaberAvatarExtras.Networking
{
    /// <summary>
    /// Why send new things over the network when you can just use magic?
    /// </summary>
    public static class Magic
    {
        /// <summary>
        /// If this color code is set for any avatar part, that avatar part will be given the rainbow material.
        /// </summary>
        public static readonly Color MagicRainbowColor = new(0, 1, 0, .5f);
    }
}