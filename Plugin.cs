using BeatSaberAvatarExtras.Assets;
using BeatSaberAvatarExtras.Installers;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Web.SiraSync;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaberAvatarExtras
{
    [Plugin(RuntimeOptions.DynamicInit)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Plugin
    {
        internal static IPALogger Logger { get; private set; } = null!;
        internal static PluginConfig Config { get; private set; } = null!;

        [Init]
        public void Init(IPALogger logger, Zenjector zenjector, IPA.Config.Config config)
        {
            Logger = logger;
            Config = config.Generated<PluginConfig>();
            
            zenjector.UseMetadataBinder<Plugin>();
            zenjector.UseLogger(logger);
            zenjector.UseSiraSync(SiraSyncServiceType.GitHub, "roydejong", "BeatSaberAvatarExtras");
            
            zenjector.Install<BsaeAppInstaller>(Location.App);
            zenjector.Install<BsaeMenuInstaller>(Location.Menu);
        }

        [OnEnable]
        public async void OnEnable()
        {
            Sprites.Initialize();
            
            await BundleLoader.EnsureLoaded();
        }

        [OnDisable]
        public void OnDisable()
        {
            BundleLoader.Unload();
        }
    }
}