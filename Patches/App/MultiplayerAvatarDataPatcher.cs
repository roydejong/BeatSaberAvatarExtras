using SiraUtil.Affinity;

namespace BeatSaberAvatarExtras.Patches.App
{
    /// <summary>
    /// Patches the AvatarDataMultiplayerAvatarDataConverter to correctly set glassesId.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MultiplayerAvatarDataPatcher : IAffinity
    {
        /// <summary>
        /// This patches fixes an issue that prevents "glassesId" from being set correctly on MultiplayerAvatarData.
        /// </summary>
        [AffinityPatch(typeof(AvatarDataMultiplayerAvatarDataConverter),
            nameof(AvatarDataMultiplayerAvatarDataConverter.CreateMultiplayerAvatarData))]
        [AffinityPostfix]
        public void PostfixCreateMultiplayerAvatarData(AvatarData avatarData, ref MultiplayerAvatarData __result)
        {
            __result
                .GetType()
                .GetField("glassesId")
                .SetValueDirect(__makeref(__result), avatarData.glassesId);
        }
    }
}