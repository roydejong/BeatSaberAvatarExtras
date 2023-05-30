using System;
using BeatSaberAvatarExtras.Assets;
using BeatSaberAvatarExtras.Networking;
using BeatSaberAvatarExtras.UI;
using DataModels.PlayerAvatar;
using IPA.Utilities;
using SiraUtil.Affinity;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;
using BeatSaberAvatarExtras.Utils;

namespace BeatSaberAvatarExtras.Patches.Menu
{
    /// <summary>
    /// Patches the edit avatar view to include extra selection options.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EditAvatarViewControllerPatcher : IInitializable, IAffinity
    {
        [Inject] private readonly EditAvatarViewController _editAvatarViewController = null!;
        [Inject] private readonly AvatarPartsModel _avatarPartsModel = null!;
        [Inject] private readonly AvatarDataModel _avatarDataModel = null!;

        private PackedExtrasString _extras;
        private CustomAvatarOptionField? _glassesPicker;
        private CustomAvatarOptionField? _facialHairPicker;

        public void Initialize()
        {
            _extras = PackedExtrasString.TryFromAvatarData(_avatarDataModel.avatarData) ?? new PackedExtrasString();
            
            // Create new fields
            _glassesPicker = CreateCustomField("Glasses", 1);
            _facialHairPicker = CreateCustomField("FacialHair", 2);
        }

        [AffinityPatch(typeof(EditAvatarViewController), "DidActivate")]
        [AffinityPostfix]
        public void PostfixDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                // Set icons
                _glassesPicker!.Icon!.sprite = Sprites.Glasses;
                _facialHairPicker!.Icon!.sprite = Sprites.Mustache;
                
                // Move edit panel up a bit so the layout is less cramped
                var editPanel = _editAvatarViewController.transform.Find("EditPanel");
                (editPanel.transform as RectTransform)!.position += new Vector3(0, .18f, 0);

                // Move random panel down a bit so it doesn't overlap
                var randomPanel = _editAvatarViewController.transform.Find("RandomizePanel");
                (randomPanel.transform as RectTransform)!.position += new Vector3(0, -.28f, 0);

                // Initialize custom fields data/bindings
                InvokeSetupValuePicker<AvatarMeshPartSO>
                (
                    _avatarPartsModel.glassesCollection,
                    _glassesPicker!.ValueController!,
                    delegate(string s)
                    {
                        _extras.GlassesId = s;
                        _extras.ApplyTo(_avatarDataModel.avatarData);
                    },
                    EditAvatarViewController.AvatarEditPart.GlassesModel
                );
                InvokeSetupColorButton
                (
                    _glassesPicker.PrimaryColorController!.button,
                    delegate(Color color)
                    {
                        _avatarDataModel.avatarData.glassesColor = color;
                    },
                    () => _avatarDataModel.avatarData.glassesColor,
                    EditAvatarViewController.AvatarEditPart.GlassesColor
                );

                InvokeSetupValuePicker<AvatarMeshPartSO>
                (
                    _avatarPartsModel.facialHairCollection,
                    _facialHairPicker!.ValueController!,
                    delegate(string s)
                    {
                        _extras.FacialHairId = s;
                        _extras.ApplyTo(_avatarDataModel.avatarData);
                    },
                    EditAvatarViewController.AvatarEditPart.FacialHairModel
                );
                InvokeSetupColorButton
                (
                    _facialHairPicker.PrimaryColorController!.button,
                    delegate(Color color)
                    {
                        _avatarDataModel.avatarData.facialHairColor = color;
                    },
                    () => _avatarDataModel.avatarData.facialHairColor,
                    EditAvatarViewController.AvatarEditPart.FacialHairColor
                );

                InvokeRefreshUi();
            }
        }
        
        /// <summary>
        /// This patch adds glasses and facial hair meshes to the randomization action.
        /// </summary>
        [AffinityPatch(typeof(AvatarRandomizer), nameof(AvatarRandomizer.RandomizeModels))]
        [AffinityPostfix]
        public void PostfixRandomizeModels(AvatarData avatarData, AvatarPartsModel avatarPartsModel)
        {
            // Randomize extras and update avatar data
            _extras.GlassesId = CoinFlip() ? avatarPartsModel.glassesCollection.GetRandom().id : null;
            _extras.FacialHairId = CoinFlip() ? avatarPartsModel.facialHairCollection.GetRandom().id : null;
            _extras.ApplyTo(avatarData);
        }

        private static bool CoinFlip() => Random.Range(0, 2) == 0;

        [AffinityPatch(typeof(EditAvatarViewController), "RefreshUi")]
        [AffinityPostfix]
        public void PostfixRefreshUi()
        {
            _extras = PackedExtrasString.TryFromAvatarData(_avatarDataModel.avatarData) ?? new PackedExtrasString();
            
            if (_glassesPicker != null)
            {
                _glassesPicker.ValueController!.SetValue(
                    _avatarPartsModel.glassesCollection.GetIndexById(_extras.GlassesId));
                _glassesPicker.PrimaryColorController!.SetColor(_avatarDataModel.avatarData.glassesColor);
            }

            if (_facialHairPicker != null)
            {
                _facialHairPicker.ValueController!.SetValue(
                    _avatarPartsModel.facialHairCollection.GetIndexById(_extras.FacialHairId));
                _facialHairPicker.PrimaryColorController!.SetColor(_avatarDataModel.avatarData.facialHairColor);
            }
        }

        private CustomAvatarOptionField CreateCustomField(string name, int offsetPosition) =>
            CustomAvatarOptionField.Create(_editAvatarViewController.transform.Find("EditPanel"), name, offsetPosition);
        
        private void InvokeRefreshUi() =>
            _editAvatarViewController.InvokeMethod<object, EditAvatarViewController>("RefreshUi");
        
        private void InvokeSetupColorButton(Button button,
            Action<Color> colorSetter,
            Func<Color> currentColor,
            EditAvatarViewController.AvatarEditPart avatarEditPart,
            int uvSegment = 0) =>
            _editAvatarViewController.InvokeMethod<object, EditAvatarViewController>("SetupColorButton", 
                button, colorSetter, currentColor, avatarEditPart, uvSegment);
        
        private void InvokeSetupValuePicker<T>(
            AvatarPartCollection<T> partCollection,
            NamedIntListController valuePicker,
            Action<string> setIdAction,
            EditAvatarViewController.AvatarEditPart avatarEditPart)
            where T : UnityEngine.Object, IAvatarPart =>
            _editAvatarViewController.InvokeGenericMethod<object, EditAvatarViewController, T>("SetupValuePicker", 
                partCollection, valuePicker, setIdAction, avatarEditPart);
    }
}