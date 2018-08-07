namespace IvorySharp.Tests.Assets
{
    public class BoundaryState
    {
        public BoundaryState(BoundaryType boundaryType)
        {
            BoundaryType = boundaryType;
        }

        public BoundaryType BoundaryType { get; }
    }
}