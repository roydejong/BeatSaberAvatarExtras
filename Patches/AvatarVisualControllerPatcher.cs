using IPA.Utilities;
using SiraUtil.Affinity;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace BeatSaberAvatarExtras.Patches
{
    /// <summary>
    /// Patches the avatar visuals to allow our extras to be shown.
    /// Affects both the local player and other players in multiplayer.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AvatarVisualControllerPatcher : IAffinity
    {
        /// <summary>
        /// This patch will ensure "glasses" and "facial hair" game objects are made active.
        /// This will allow us to see our and other player's glasses/facial hair options everywhere.
        /// </summary>
        [AffinityPatch(typeof(AvatarVisualController), nameof(AvatarVisualController.UpdateAvatarVisual))]
        [AffinityPostfix]
        // ReSharper disable once InconsistentNaming
        public void PostfixUpdateAvatarVisual(AvatarData avatarData, AvatarVisualController __instance)
        {
            var glassesMesh = __instance.GetField<MeshFilter, AvatarVisualController>("_glassesMeshFilter");
            var facialHairMesh = __instance.GetField<MeshFilter, AvatarVisualController>("_facialHairMeshFilter");
            var eyesSprite = __instance.GetField<SpriteRenderer, AvatarVisualController>("_eyesSprite");
            
            glassesMesh.gameObject.SetActive(true);
            facialHairMesh.gameObject.SetActive(true);
        }
        
        [AffinityPatch(typeof(AvatarVisualController), nameof(AvatarVisualController.UpdateAvatarColors))]
        [AffinityPrefix]
        // ReSharper disable once InconsistentNaming
        public void PrefixUpdateAvatarColors(AvatarVisualController __instance)
        {
            var avatarData = __instance.GetField<AvatarData, AvatarVisualController>("_avatarData");
            
            // TODO Fix colors...
        }
    }
}