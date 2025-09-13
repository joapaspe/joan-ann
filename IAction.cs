
using System;

namespace ANN
{
    public interface IAction
    {
        int getNInputNeurons();
        int getNOutputNeurons();
        void doFeedForward();
    }
}
