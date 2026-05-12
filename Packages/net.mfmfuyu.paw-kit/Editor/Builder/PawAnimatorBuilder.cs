using System;
using System.Linq;
using AnimatorAsCode.V1.VRC;

namespace PawKit.Editor.Builder
{
    public class PawAnimatorBuilder
    {
        private readonly PawBuildContext _ctx;

        public PawAnimatorBuilder(PawBuildContext ctx)
        {
            _ctx = ctx;
        }

        public void Build()
        {
            SubBuild(HandSide.Left);
            SubBuild(HandSide.Right);
        }

        private void SubBuild(HandSide handSide)
        {
            var avatarMask = handSide.ToAvatarMask(_ctx.Aac);
            var layer = _ctx.Controller.NewLayer(handSide.ToString()).WithAvatarMask(avatarMask);

            var trackingElement = handSide.ToAv3TrackingElement();
            var overrideParameter = handSide == HandSide.Left
                ? _ctx.OverrideGestureLeftParameter
                : _ctx.OverrideGestureRightParameter;

            var idleState = layer.NewState("Idle").TrackingTracks(trackingElement);

            var trackingState = layer.NewState("Tracking").TrackingTracks(trackingElement);
            idleState.TransitionsTo(trackingState).When(_ctx.TrackingParameter.IsTrue());
            trackingState.Exits().When(_ctx.TrackingParameter.IsFalse());
            trackingState.Exits().When(overrideParameter.IsNotEqualTo(0));

            var av3GestureParameter = handSide.ToAv3Gesture(layer);
            var oppositeHandSide = handSide == HandSide.Left ? HandSide.Right : HandSide.Left;
            var av3OppositeGestureParameter = oppositeHandSide.ToAv3Gesture(layer);

            foreach (var gesture in _ctx.Gestures)
            {
                var name = gesture.gestureType.ToString();
                var idx = Array.IndexOf(_ctx.Gestures, gesture) + 1;

                var state = layer.NewState(name)
                    .WithAnimation(gesture.clip)
                    .TrackingAnimates(trackingElement);

                var av3Gesture = gesture.gestureType.ToAv3();
                var av3OppositeGesture = gesture.oppositeGestureType.ToAv3();

                if (gesture.isCombination)
                    // combination
                    idleState.TransitionsTo(state)
                        .When(av3GestureParameter.IsEqualTo(av3Gesture))
                        .And(av3OppositeGestureParameter.IsEqualTo(av3OppositeGesture))
                        .And(overrideParameter.IsEqualTo(0));
                else
                    // normal
                    idleState.TransitionsTo(state)
                        .When(av3GestureParameter.IsEqualTo(av3Gesture))
                        .And(overrideParameter.IsEqualTo(0));

                // override mode
                idleState.TransitionsTo(state)
                    .When(overrideParameter.IsEqualTo(idx));

                if (gesture.isCombination)
                {
                    state.Exits().When(av3GestureParameter.IsNotEqualTo(av3Gesture))
                        .And(overrideParameter.IsEqualTo(0))
                        .Or()
                        .When(av3OppositeGestureParameter.IsNotEqualTo(av3OppositeGesture))
                        .And(overrideParameter.IsEqualTo(0));
                }
                else
                {
                    state.Exits().When(av3GestureParameter.IsNotEqualTo(av3Gesture))
                        .And(overrideParameter.IsEqualTo(0));

                    var combos = _ctx.Gestures.Where(g => g.isCombination && g.gestureType == gesture.gestureType)
                        .Select(g => g.oppositeGestureType.ToAv3());

                    foreach (var combo in combos)
                        state.Exits().When(av3OppositeGestureParameter.IsEqualTo(combo))
                            .And(overrideParameter.IsEqualTo(0));
                }

                // tracking mode
                state.Exits().When(_ctx.TrackingParameter.IsTrue());
                // override mode
                state.Exits().When(overrideParameter.IsNotEqualTo(0)).And(overrideParameter.IsNotEqualTo(idx));
            }
        }
    }
}