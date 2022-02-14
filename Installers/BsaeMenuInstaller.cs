using BeatSaberAvatarExtras.Patches;
using Zenject;

namespace BeatSaberAvatarExtras.Installers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BsaeMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EditAvatarViewControllerPatcher>().AsSingle();
        }
    }
}
