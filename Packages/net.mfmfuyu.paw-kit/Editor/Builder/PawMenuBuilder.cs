using System;
using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace PawKit.Editor.Builder
{
    public class PawMenuBuilder
    {
        private readonly PawBuildContext _ctx;

        public PawMenuBuilder(PawBuildContext ctx)
        {
            _ctx = ctx;
        }

        public void Build()
        {
            _ctx.Root.AddComponent<ModularAvatarMenuInstaller>();
            var menuItem = _ctx.Root.AddComponent<ModularAvatarMenuItem>();
            menuItem.Control = new VRCExpressionsMenu.Control
            {
                type = VRCExpressionsMenu.Control.ControlType.SubMenu
            };
            menuItem.MenuSource = SubmenuSource.Children;

            var tracking = new GameObject
            {
                transform = { parent = _ctx.Root.transform }
            };
            _ctx.MaAc.EditMenuItem(tracking).Name("Tracking").Toggle(_ctx.TrackingParameter);

            BuildSubMenu(HandSide.Left);
            BuildSubMenu(HandSide.Right);
        }

        private void BuildSubMenu(HandSide handSide)
        {
            var subMenuRoot = new GameObject
            {
                name = handSide.ToString(),
                transform = { parent = _ctx.Root.transform }
            };

            var rootMenuItem = subMenuRoot.AddComponent<ModularAvatarMenuItem>();
            rootMenuItem.Control = new VRCExpressionsMenu.Control
            {
                type = VRCExpressionsMenu.Control.ControlType.SubMenu
            };
            rootMenuItem.MenuSource = SubmenuSource.Children;

            foreach (var gesture in _ctx.Gestures)
            {
                var subMenuItem = new GameObject
                {
                    transform = { parent = subMenuRoot.transform }
                };

                _ctx.MaAc.EditMenuItem(subMenuItem).Name(gesture.name).ToggleSets(
                    handSide == HandSide.Left ? _ctx.OverrideGestureLeftParameter : _ctx.OverrideGestureRightParameter,
                    Array.IndexOf(_ctx.Gestures, gesture) + 1
                );
            }
        }
    }
}