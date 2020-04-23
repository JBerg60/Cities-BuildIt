using ICities;

namespace BuildIt
{
    public class Threader : ThreadingExtensionBase
    {
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            Builder.instance.Next();
        }
    }
}
