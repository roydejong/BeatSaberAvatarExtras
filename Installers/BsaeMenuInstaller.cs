using BeatSaberAvatarExtras.Patches.Menu;
using Zenject;

namespace BeatSaberAvatarExtras.Installers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BsaeMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ColorPickerButtonControllerPatcher>().AsSingle();
            Container.BindInterfacesAndSelfTo<EditAvatarViewControllerPatcher>().AsSingle();
            Container.BindInterfacesAndSelfTo<EditAvatarColorViewControllerPatcher>().AsSingle();
        }
    }
}
