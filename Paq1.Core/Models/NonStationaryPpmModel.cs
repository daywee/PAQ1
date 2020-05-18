namespace Paq1.Core.Models
{
    /// <summary>
    /// A NonstationaryPPM model guesses the next bit by finding all
    /// matching contexts of n = 1 to 8 bytes(including the last partial
    /// byte of 0-7 bits) and guessing for each match that the next bit
    /// will be the same with weight n^2/f(age).  The function f(age) decays
    /// the count of 0s or 1s for each context by half whenever there are
    /// more than 2 and the opposite bit is observed.This is an approximation
    /// of the nonstationary model, weight = 1 / (t * variance) where t is the
    /// number of subsequent observations and the variance is tp(1-p) for
    /// t observations and p the probability of a 1 bit given the last t
    /// observations.The aged counts are stored in a hash table of 8M
    /// contexts.
    /// </summary>
    public class NonStationaryPpmModel : IModel
    {
        private readonly PartialNonStationaryPpmModel[] _models;

        public NonStationaryPpmModel(int order)
        {
            _models = new PartialNonStationaryPpmModel[order];

            for (int i = 0; i < order; i++)
            {
                _models[i] = new PartialNonStationaryPpmModel(i);
            }
        }

        public (int, int) Predict(int n0, int n1)
        {
            foreach (var model in _models)
            {
                int weight = (model.Order + 1) * (model.Order + 1);
                (int x0, int x1) = model.Predict(n0, n1);
                n0 += weight * x0;
                n1 += weight * x1;
            }

            return (n0, n1);
        }

        public void Update(int bit)
        {
            foreach (var model in _models)
            {
                model.Update(bit);
            }
        }
    }
}
