using nadena.dev.modular_avatar.core;
using PawKit.Runtime;
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
        }
    }
}