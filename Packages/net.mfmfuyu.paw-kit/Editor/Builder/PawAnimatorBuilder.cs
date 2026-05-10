using AnimatorAsCode.V1.VRC;
using PawKit.Runtime;

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

            var idleState = layer.NewState("Idle").TrackingTracks(trackingElement);

            var trackingState = layer.NewState("Tracking").TrackingTracks(trackingElement);
            idleState.TransitionsTo(trackingState).When(_ctx.TrackingParameter.IsTrue());
            trackingState.Exits().When(_ctx.TrackingParameter.IsFalse());

            var av3GestureParameter = handSide.ToAv3Gesture(layer);
            foreach (var gesture in _ctx.Gestures)
            {
                var name = gesture.gestureType.ToString();

                var state = layer.NewState(name)
                    .WithAnimation(gesture.clip)
                    .TrackingAnimates(trackingElement);

                var av3Gesture = gesture.gestureType.ToAv3();

                idleState.TransitionsTo(state)
                    .When(av3GestureParameter.IsEqualTo(av3Gesture));

                state.Exits().When(av3GestureParameter.IsNotEqualTo(av3Gesture));
                state.Exits().When(_ctx.TrackingParameter.IsTrue());
            }
        }
    }
}