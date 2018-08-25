namespace IvorySharp.Components
{
    internal static class ComponentsExtensions
    {
        public static IComponentHolder<TComponent> ToInstanceHolder<TComponent>(this TComponent component, bool freeze = true)
            where TComponent : IComponent
        {
            var provider = new InstanceComponentHolder<TComponent>(component);
            if (freeze)
                provider.Freeze();

            return provider;
        }
    }
}