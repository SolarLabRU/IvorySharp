using IvorySharp.Aspects.Configuration;
using IvorySharp.Caching;
using IvorySharp.Extensions.ClassAspectSelection.Aspects.Selection;
using IvorySharp.Extensions.ClassAspectSelection.Aspects.Weaving;

namespace IvorySharp.Extensions.ClassAspectSelection
{
    /// <summary>
    /// Набор расширений для <see cref="AspectsConfiguration"/>.
    /// </summary>
    public static class AspectsConfigurationExtensions
    {
        /// <summary>
        /// Включает расширение получения аспектов с классов.
        /// </summary>
        public static void UseClassAspectSelection(this AspectsConfiguration configuration)
        {
            configuration.ReplaceComponent(s => s.AspectDeclarationCollector)
                .Use(st => new TargetTypeAspectDeclarationCollector(st.AspectSelector));
                    
            configuration.ReplaceComponent(s => s.AspectWeavePredicate)
                .Use(st => new TargetTypeWeavePredicate(st.AspectSelector, 
                    ConcurrentDictionaryCacheFactory.Default));
        }
    }
}