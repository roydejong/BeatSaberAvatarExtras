using BeatSaberAvatarExtras.Networking;
using SiraUtil.Affinity;
using Zenject;

namespace BeatSaberAvatarExtras.Patches.App
{
    /// <summary>
    /// Patches the AvatarDataMultiplayerAvatarDataConverter to correctly set glassesId.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MultiplayerAvatarDataPatcher : IAffinity
    {
        [Inject] private readonly AvatarDataModel _avatarDataModel = null!;
        
        /// <summary>
        /// This patches stores our PackedExtrasString in the avatar data before it is sent on the network.
        /// </summary>
        [AffinityPatch(typeof(ConnectedPlayerManager), nameof(ConnectedPlayerManager.SetLocalPlayerAvatar))]
        [AffinityPrefix]
        public void PrefixSetLocalPlayerAvatar(ref MultiplayerAvatarData multiplayerAvatarData)
        {
            var extras = PackedExtrasString.TryFromAvatarData(_avatarDataModel.avatarData);

            if (extras is not null)
            {
                extras.Value.ApplyTo(ref multiplayerAvatarData);
            }
        }
    }
}