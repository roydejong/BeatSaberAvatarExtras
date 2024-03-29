﻿using BeatSaberAvatarExtras.Patches.App;
using Zenject;

namespace BeatSaberAvatarExtras.Installers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BsaeAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AvatarVisualControllerPatcher>().AsSingle();
            Container.BindInterfacesAndSelfTo<MultiplayerAvatarDataPatcher>().AsSingle();
        }
    }
}
