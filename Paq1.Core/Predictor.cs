using Paq1.Core.Models;
using System.Collections.Generic;

namespace Paq1.Core
{
    public class Predictor
    {
        private readonly List<IModel> _models;

        public Predictor()
        {
            _models = new List<IModel>
            {
                new NonStationaryPpmModel(8)
            };
        }

        /// <summary>
        /// Predicts the next bit based on previous bits.
        /// </summary>
        /// <returns>A double representing probability of a 0 being next bit.</returns>
        public double Predict()
        {
            int n0 = 1;
            int n1 = 1;

            foreach (var model in _models)
            {
                (n0, n1) = model.Predict(n0, n1);
            }

            return 1.0 * n0 / (n0 + n1);
        }

        /// <summary>
        /// Update models with bit.
        /// </summary>
        /// <param name="bit">LSB of int is used to update models.</param>
        public void Update(int bit)
        {
            foreach (var model in _models)
            {
                model.Update(bit);
            }
        }
    }
}
