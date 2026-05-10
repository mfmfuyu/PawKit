using System.Linq;
using AnimatorAsCode.V1;
using nadena.dev.ndmf;
using PawKit.Editor;
using PawKit.Editor.Builder;
using PawKit.Runtime;
using UnityEditor;
using UnityEngine;

[assembly: ExportsPlugin(typeof(PawKitPlugin))]

namespace PawKit.Editor
{
    public class PawKitPlugin : Plugin<PawKitPlugin>
    {
        private const string SystemName = "PawKit";
        public override string QualifiedName => "net.mfmfuyu.paw-kit";
        public override string DisplayName => "PawKit";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating).Run($"Generate {DisplayName}", Generate);
        }

        private void Generate(BuildContext ctx)
        {
            var gestures = ctx.AvatarRootTransform.GetComponentsInChildren<PawGesture>(true)
                .GroupBy(c => c.gestureType)
                .Select(g => g.First())
                .ToArray();

            if (gestures.Length == 0) return;

            var aac = AacV1.Create(new AacConfiguration
            {
                SystemName = SystemName,
                AnimatorRoot = ctx.AvatarRootTransform,
                DefaultValueRoot = ctx.AvatarRootTransform,
                AssetKey = GUID.Generate().ToString(),
                AssetContainer = ctx.AssetContainer,
                DefaultsProvider = new AacDefaultsProvider()
            });

            var root = new GameObject(SystemName)
            {
                transform = { parent = ctx.AvatarRootTransform }
            };
            var buildContext = new PawBuildContext(root, aac, gestures);

            var animatorBuilder = new PawAnimatorBuilder(buildContext);
            animatorBuilder.Build();
            
            var menuBuilder = new PawMenuBuilder(buildContext);
            menuBuilder.Build();
        }
    }
}