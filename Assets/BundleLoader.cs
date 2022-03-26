using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberAvatarExtras.Assets
{
    public static class BundleLoader
    {
        private static AssetBundle? _bundle = null;
        private static GameObject? _rootAsset = null;

        #region Load API

        public static async Task<bool> EnsureLoaded()
        {
            if (_bundle != null)
                return false;

            Plugin.Logger.Info("Loading asset bundle...");

            _bundle = await LoadBundleFromResource();
            _rootAsset = null;

            if (_bundle != null)
            {
                _rootAsset = await LoadRootAssetFromBundle(_bundle);

                if (_rootAsset != null)
                {
                    Plugin.Logger.Info("Loaded asset bundle successfully!");
                    return true;
                }
            }

            Plugin.Logger.Error("Failed to load asset bundle!");
            return false;
        }

        public static void Unload()
        {
            if (_bundle == null)
                return;

            _bundle.Unload(true);
            _bundle = null;
        }

        #endregion

        #region AssetBundle load

        private const string BundlePath = "BeatSaberAvatarExtras.Assets.Bundles.extras.unitypackage";

        private static async Task<AssetBundle?> LoadBundleFromResource()
        {
            var resBytes = ResourceHelpers.GetResource(Assembly.GetExecutingAssembly(), BundlePath);

            if (resBytes is null)
                return null;

            return await LoadBundleFromMemory(resBytes);
        }

        private static async Task<AssetBundle?> LoadBundleFromMemory(byte[] data)
        {
            var tcs = new TaskCompletionSource<AssetBundle?>();
            var request = AssetBundle.LoadFromMemoryAsync(data);
            request.completed += ao => { tcs.TrySetResult(request.assetBundle); };
            return await tcs.Task;
        }

        private static async Task<GameObject?> LoadRootAssetFromBundle(AssetBundle bundle)
        {
            var tcs = new TaskCompletionSource<GameObject?>();
            var request = bundle.LoadAssetWithSubAssetsAsync<GameObject>(RootAssetName);
            request.completed += ao =>
            {
                if (request.asset != null)
                    tcs.TrySetResult((GameObject) request.asset);
                else
                    tcs.TrySetResult(null);
            };
            return await tcs.Task;
        }

        #endregion

        #region Access API
        public static GameObject? GetObject(string objName) => _rootAsset?.transform.Find(objName)?.gameObject;
        public static Material? GetMaterial(string objName) => GetObject(objName)?.GetComponent<MeshRenderer>()?.material;
        #endregion

        private const string RootAssetName = "assets/export.prefab";
    }
}