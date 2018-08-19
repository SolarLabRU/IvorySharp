namespace IvorySharp.Aspects.Components
{
    internal static class ComponentsExtensions
    {
        public static IComponentProvider<TComponent> ToProvider<TComponent>(this TComponent component, bool freeze = true)
            where TComponent : IComponent
        {
            var provider = new InstanceComponentProvider<TComponent>(component);
            if (freeze)
                provider.Freeze();

            return provider;
        }
    }
}