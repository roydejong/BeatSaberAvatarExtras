using BeatSaberAvatarExtras.Assets;
using IPA.Utilities;
using SiraUtil.Affinity;
using UnityEngine;

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

            glassesMesh.gameObject.SetActive(true);
            facialHairMesh.gameObject.SetActive(true);
        }

        /// <summary>
        /// This patch assigns custom materials.
        /// - Fixes glasses and facial hair not getting their colors set properly.
        /// </summary>
        [AffinityPatch(typeof(AvatarVisualController), nameof(AvatarVisualController.UpdateAvatarColors))]
        [AffinityPostfix]
        // ReSharper disable once InconsistentNaming
        public void PostfixUpdateAvatarColors(AvatarVisualController __instance)
        {
            var avatarData = __instance.GetField<AvatarData, AvatarVisualController>("_avatarData");

            if (avatarData == null)
                return;

            // Fix glasses and facial hair not getting their colors set properly
            var glassesMesh = __instance.GetField<MeshFilter, AvatarVisualController>("_glassesMeshFilter")
                .GetComponent<MeshRenderer>();
            var facialHairMesh = __instance.GetField<MeshFilter, AvatarVisualController>("_facialHairMeshFilter")
                .GetComponent<MeshRenderer>();

            if (!glassesMesh.material.name.StartsWith(MaterialFactory.MaterialNamePrefix))
                glassesMesh.material = MaterialFactory.RainbowMaterial;
            glassesMesh.material.color = avatarData.glassesColor;
            
            if (!facialHairMesh.material.name.StartsWith(MaterialFactory.MaterialNamePrefix))
                facialHairMesh.material = MaterialFactory.RainbowMaterial;
            facialHairMesh.material.color = avatarData.facialHairColor;
        }
    }
}