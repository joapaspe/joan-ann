using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANN
{
    public interface IAction
    {

        int getNInputNeurons();
        int getNOutputNeurons();

        void doFeedForward();
    }
}
