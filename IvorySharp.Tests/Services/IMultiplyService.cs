namespace IvorySharp.Tests.Services
{
    public interface IMultiplyService
    {
        int Multiply(int left, int right);
    }

    public class MultiplyService : IMultiplyService
    {
        public int Multiply(int left, int right)
        {
            return left * right;
        }
    }
}