namespace Paq1.Core.Models
{
    public interface IModel
    {
        (int, int) Predict(int n0, int n1);
        void Update(int bit);
    }
}
