using BeatSaberAvatarExtras.Assets;
using BeatSaberAvatarExtras.UI;
using SiraUtil.Affinity;
using UnityEngine;
using Zenject;

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

        private CustomAvatarOptionField? _glassesPicker;
        private CustomAvatarOptionField? _facialHairPicker;

        public void Initialize()
        {
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
                _editAvatarViewController.SetupValuePicker<AvatarMeshPartSO>
                (
                    _avatarPartsModel.glassesCollection,
                    _glassesPicker!.ValueController,
                    delegate(string s) { _avatarDataModel.avatarData.glassesId = s; },
                    EditAvatarViewController.AvatarEditPart.GlassesModel
                );
                _editAvatarViewController.SetupColorButton
                (
                    _glassesPicker.PrimaryColorController!.button,
                    delegate(Color color) { _avatarDataModel.avatarData.glassesColor = color; },
                    () => _avatarDataModel.avatarData.glassesColor,
                    EditAvatarViewController.AvatarEditPart.GlassesColor
                );

                _editAvatarViewController.SetupValuePicker<AvatarMeshPartSO>
                (
                    _avatarPartsModel.facialHairCollection,
                    _facialHairPicker!.ValueController,
                    delegate(string s) { _avatarDataModel.avatarData.facialHairId = s; },
                    EditAvatarViewController.AvatarEditPart.FacialHairModel
                );
                _editAvatarViewController.SetupColorButton
                (
                    _facialHairPicker.PrimaryColorController!.button,
                    delegate(Color color) { _avatarDataModel.avatarData.facialHairColor = color; },
                    () => _avatarDataModel.avatarData.facialHairColor,
                    EditAvatarViewController.AvatarEditPart.FacialHairColor
                );

                _editAvatarViewController.RefreshUi();
            }
        }

        [AffinityPatch(typeof(EditAvatarViewController), nameof(EditAvatarViewController.RefreshUi))]
        [AffinityPostfix]
        public void PostfixRefreshUi()
        {
            if (_glassesPicker != null)
            {
                _glassesPicker.ValueController!.SetValue(
                    _avatarPartsModel.glassesCollection.GetIndexById(_avatarDataModel.avatarData.glassesId));
                _glassesPicker.PrimaryColorController!.SetColor(_avatarDataModel.avatarData.glassesColor);
            }

            if (_facialHairPicker != null)
            {
                _facialHairPicker.ValueController!.SetValue(
                    _avatarPartsModel.facialHairCollection.GetIndexById(_avatarDataModel.avatarData.facialHairId));
                _facialHairPicker.PrimaryColorController!.SetColor(_avatarDataModel.avatarData.facialHairColor);
            }
        }

        private CustomAvatarOptionField CreateCustomField(string name, int offsetPosition) =>
            CustomAvatarOptionField.Create(_editAvatarViewController.transform.Find("EditPanel"), name, offsetPosition);
    }
}