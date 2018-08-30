namespace IvorySharp.Tests.Assets
{
    public class BoundaryState
    {
        public BoundaryState(BoundaryType boundaryType)
        {
            BoundaryType = boundaryType;
        }

        public BoundaryType BoundaryType { get; }

        public override bool Equals(object obj)
        {
            if (obj is BoundaryState bs)
            {
                return bs.BoundaryType == BoundaryType;
            }

            return false;
        }       
        
        public override int GetHashCode()
        {
            return (int) BoundaryType;
        }
    }
}