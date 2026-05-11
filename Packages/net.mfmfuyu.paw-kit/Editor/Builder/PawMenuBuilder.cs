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
            var sub = new GameObject()
            {
                name = handSide.ToString(),
                transform = { parent = _ctx.Root.transform }
            };
            
            var menuItem = sub.AddComponent<ModularAvatarMenuItem>();
            menuItem.Control = new VRCExpressionsMenu.Control()
            {
                type = VRCExpressionsMenu.Control.ControlType.SubMenu
            };
            menuItem.MenuSource = SubmenuSource.Children;
            
            foreach (var gesture in _ctx.Gestures)
            {
                var a = new GameObject()
                {
                    transform = { parent = sub.transform }
                };

                _ctx.MaAc.EditMenuItem(a).Name(gesture.name).ToggleSets(
                    handSide == HandSide.Left ? _ctx.OverrideGestureLeftParameter : _ctx.OverrideGestureRightParameter,
                    Array.IndexOf(_ctx.Gestures, gesture) + 1
                );
            }
        }
    }
}