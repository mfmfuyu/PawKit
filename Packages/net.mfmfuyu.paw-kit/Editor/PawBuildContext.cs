using AnimatorAsCode.V1;
using AnimatorAsCode.V1.ModularAvatar;
using PawKit.Runtime;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace PawKit.Editor
{
    public class PawBuildContext
    {
        public PawBuildContext(GameObject root, AacFlBase aac, PawGesture[] gestures)
        {
            Root = root;
            Aac = aac;
            MaAc = MaAc.Create(root);

            Gestures = gestures;

            Controller = aac.NewAnimatorController();
            var layer = Controller.NewLayer("Internal");
            MaAc.NewMergeAnimator(Controller, VRCAvatarDescriptor.AnimLayerType.Gesture);

            TrackingParameter = layer.BoolParameter("Tracking");
            OverrideGestureLeftParameter = layer.IntParameter("OverrideGestureLeft");
            MaAc.NewParameter(OverrideGestureLeftParameter);
            OverrideGestureRightParameter = layer.IntParameter("OverrideGestureRight");
            MaAc.NewParameter(OverrideGestureRightParameter);
        }

        public PawGesture[] Gestures { get; }

        public AacFlBase Aac { get; }

        public AacFlController Controller { get; }

        public MaAc MaAc { get; }
        public GameObject Root { get; }

        public AacFlBoolParameter TrackingParameter { get; }
        public AacFlIntParameter OverrideGestureLeftParameter { get; }
        public AacFlIntParameter OverrideGestureRightParameter { get; }
    }
}