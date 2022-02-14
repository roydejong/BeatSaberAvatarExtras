using BeatSaberAvatarExtras.Patches;
using Zenject;

namespace BeatSaberAvatarExtras.Installers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BsaeAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AvatarVisualControllerPatcher>().AsSingle();
            Container.BindInterfacesAndSelfTo<AvatarRandomizerPatcher>().AsSingle();
        }
    }
}
