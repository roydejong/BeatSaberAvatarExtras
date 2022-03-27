﻿using System.Collections.Generic;
using BeatSaberAvatarExtras.Assets;
using BeatSaberAvatarExtras.Models;
using BeatSaberAvatarExtras.Networking;
using IPA.Utilities;
using SiraUtil.Affinity;
using UnityEngine;

namespace BeatSaberAvatarExtras.Patches.App
{
    /// <summary>
    /// Patches the avatar visuals to allow our extras to be shown.
    /// Affects both the local player and other players in multiplayer.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AvatarVisualControllerPatcher : IAffinity
    {
        private static Dictionary<string, Material> _originalMaterialsCache = new();

        #region Patches

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
        /// Prefix for native parts:
        /// - Restores base game materials to undo previously applied special effects
        /// - Applies special color effects (e.g. rainbow) to those parts
        /// </summary>
        [AffinityPatch(typeof(AvatarVisualController), nameof(AvatarVisualController.UpdateAvatarColors))]
        [AffinityPostfix]
        // ReSharper disable once InconsistentNaming
        public void PrefixUpdateAvatarColors(AvatarVisualController __instance) =>
            ApplyColorsPatch(__instance, true, false);

        /// <summary>
        /// Postfix for custom parts, and to change native parts:
        /// - Fixes glasses and facial hair not getting their colors set properly
        /// - Attaches rainbow material to parts that have the "magic" color
        /// </summary>
        [AffinityPatch(typeof(AvatarVisualController), nameof(AvatarVisualController.UpdateAvatarColors))]
        [AffinityPostfix]
        // ReSharper disable once InconsistentNaming
        public void PostfixUpdateAvatarColors(AvatarVisualController __instance) =>
            ApplyColorsPatch(__instance, true, true);

        private void ApplyColorsPatch(AvatarVisualController visualController, bool includeNativeParts,
            bool includeCustomParts)
        {
            var avatarData = visualController.GetField<AvatarData, AvatarVisualController>("_avatarData");

            if (avatarData == null)
                return;

            if (includeNativeParts)
            {
                var headTopMesh = GetPartMesh(visualController, "_headTopMeshFilter");
                var leftHandMesh = GetPartMesh(visualController, "_leftHandsHairMeshFilter");
                var rightHandMesh = GetPartMesh(visualController, "_rightHandsHairMeshFilter");
                var bodyMesh = GetPartMesh(visualController, "_bodyMeshFilter");
                
                ApplyBasePartColor(headTopMesh, new()
                {
                    avatarData.headTopPrimaryColor,
                    avatarData.headTopSecondaryColor
                });
                ApplyBasePartColor(leftHandMesh, new() {avatarData.handsColor});
                ApplyBasePartColor(rightHandMesh, new() {avatarData.handsColor});
                ApplyBasePartColor(bodyMesh, new()
                {
                    avatarData.clothesPrimaryColor,
                    avatarData.clothesSecondaryColor, avatarData.clothesDetailColor
                });
            }

            if (includeCustomParts)
            {
                var glassesMesh = GetPartMesh(visualController, "_glassesMeshFilter");
                var facialHairMesh = GetPartMesh(visualController, "_facialHairMeshFilter");
                
                ApplyCustomPartColor(glassesMesh, avatarData.glassesColor);
                ApplyCustomPartColor(facialHairMesh, avatarData.facialHairColor);
            }
        }

        #endregion

        #region Logic

        private static void ApplyBasePartColor(MeshRenderer mesh, List<Color> applicableColors)
        {
            if (!_originalMaterialsCache.ContainsKey(mesh.name))
                _originalMaterialsCache[mesh.name] = mesh.material;
            
            if (ApplySpecialOption(mesh, SpecialColorOption.DetectNonDefaultOptionMagically(applicableColors)))
                return;

            // Game code will set the color on the default material via the material/shader properties
            // ...we just need to restore that original material on prefix if we're not doing anything special
            RestoreNativeMaterial(mesh);
        }

        private static void ApplyCustomPartColor(MeshRenderer mesh, Color targetColor)
        {
            if (ApplySpecialOption(mesh, SpecialColorOption.DetectNonDefaultOptionMagically(targetColor)))
                return;

            ApplyFlatColor(mesh, targetColor);
        }

        private static void RestoreNativeMaterial(MeshRenderer mesh)
        {
            if (_originalMaterialsCache.TryGetValue(mesh.name, out var originalMat))
                if (mesh.material.name != originalMat.name)
                    mesh.material = originalMat;
        }

        private static void ApplyFlatColor(MeshRenderer mesh, Color color)
        {
            if (mesh.material.name != MaterialFactory.FlatColorMaterial.name)
                mesh.material = MaterialFactory.FlatColorMaterial;
            mesh.material.color = color;
        }

        private static bool ApplySpecialOption(MeshRenderer mesh, SpecialColorOption? option)
        {
            Plugin.Logger.Error($"ApplySpecialOption - {mesh.name} - {option}");
            
            if (option == SpecialColorOption.Rainbow)
            {
                if (mesh.material.name != MaterialFactory.RainbowMaterial.name)
                {
                    mesh.material = MaterialFactory.RainbowMaterial;
                    mesh.material.color = Magic.MagicRainbowColor;
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Helpers

        private static MeshRenderer GetPartMesh(AvatarVisualController controller, string fieldName) =>
            controller.GetField<MeshFilter, AvatarVisualController>(fieldName).GetComponent<MeshRenderer>();

        #endregion
    }
}