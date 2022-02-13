using IPA.Utilities;
using SiraUtil.Affinity;
using UnityEngine;

namespace BeatSaberAvatarExtras.Core
{
    public class AvatarVisualControllerPatcher : IAffinity
    {
        /// <summary>
        /// This patch will ensure "glasses" and "facial hair" game objects are made active.
        /// This will allow us to see our and other player's glasses/facial hair options everywhere.
        /// </summary>
        [AffinityPatch(typeof(AvatarVisualController), nameof(AvatarVisualController.UpdateAvatarVisual))]
        [AffinityPostfix]
        public void PostfixUpdateAvatarVisual(AvatarData avatarData, AvatarVisualController __instance)
        {
            var glassesMesh = __instance.GetField<MeshFilter, AvatarVisualController>("_glassesMeshFilter");
            var facialHairMesh = __instance.GetField<MeshFilter, AvatarVisualController>("_facialHairMeshFilter");
            
            glassesMesh.gameObject.SetActive(true);
            facialHairMesh.gameObject.SetActive(true);
        }
    }
}