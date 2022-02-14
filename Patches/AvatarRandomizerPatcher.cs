using DataModels.PlayerAvatar;
using SiraUtil.Affinity;

namespace BeatSaberAvatarExtras.Patches
{
    /// <summary>
    /// Patches the avatar "randomize" option to include our extras.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AvatarRandomizerPatcher : IAffinity
    {
        /// <summary>
        /// This patch adds glasses and facial hair meshes to the randomization action.
        /// </summary>
        [AffinityPatch(typeof(AvatarRandomizer), nameof(AvatarRandomizer.RandomizeModels))]
        [AffinityPostfix]
        public void PostfixRandomizeModels(AvatarData avatarData, AvatarPartsModel avatarPartsModel)
        {
            avatarData.glassesId = avatarPartsModel.glassesCollection.GetRandom().id;
            avatarData.facialHairId = avatarPartsModel.facialHairCollection.GetRandom().id;
        }
    }
}