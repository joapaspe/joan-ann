using System;

public enum ActivationFunction
{
    IDENTITY,
    LOGISTIC,
    SOFTMAX,
    TANH
}


namespace ANN
{
    public class Layer : IAction
    {
        public int nNeurons;
        public double[] weights;
        
        public ActivationFunction act_func;


        /// <summary>
        /// Gets a number of nNeurons, and creates the layer
        /// </summary>
        /// <param name="nNeurons">Number of neurons of the layer</param>
        /// <param name="act">Activation Function</param>
        public Layer(int nNeurons, ActivationFunction act)
        {

            weights = new double[nNeurons];
            this.nNeurons = nNeurons;
            this.act_func = act;

        }

        /// <summary>
        /// Gets a vector and generates the layer
        /// </summary>
        /// <param name="v">Vector with the values of the layer</param>
        /// <param name="act">Activation Function</param>
        public Layer(double[] v, ActivationFunction act)
        {
            this.weights = v;
            this.nNeurons = v.Length;
            this.act_func = act;
        }

        public Layer(Layer L)
        {
            this.nNeurons = L.nNeurons;
            weights = new double[L.nNeurons];
            act_func = L.act_func;

        }

        /// <summary>
        /// Returns the number of neurons of the layer
        /// </summary>
        /// <returns>The number of neurons of the layer</returns>
        int IAction.getNInputNeurons()
        {

            return nNeurons;

        }

        /// <summary>
        /// Returns the number neurons of  the layer. The same as the input
        /// </summary>
        /// <returns></returns>
        int IAction.getNOutputNeurons()
        {
            return nNeurons;

        }




        public override string ToString()
        {

            string s = "";

            for (int i = 0; i < this.nNeurons; i++)
            {
                s += this.weights[i];
                s += " ";
            }

            return s;
        }



        #region ActivationFunction

        /// <summary>
        /// Map function that determines the type of the activation function. 
        /// </summary>
        /// <param name="name">The strings that contains the activation function</param>
        /// <returns>The type of the activation function</returns>
        public static ActivationFunction stringToActivation(string name)
        {

            name = name.Replace(",", "");

            switch (name)
            {
                case "input":
                    return ActivationFunction.IDENTITY;
                case "softmax":
                    return ActivationFunction.SOFTMAX;
                case "tanh":
                    return ActivationFunction.TANH;
                case "output":
                case "logistic":
                    return ActivationFunction.LOGISTIC;
                default:
                    return ActivationFunction.IDENTITY;
            }

        }
        /// <summary>
        /// Applies the activation soft max on the desired layer
        /// </summary>
        public void activationSoftMax()
        {


            double sumExp = 0;


            double maxVal = double.MinValue;

            for (int i = 0; i < this.nNeurons; i++)
            {
                if (weights[i] > maxVal) maxVal = weights[i];

            }

            for (int i = 0; i < this.nNeurons; i++)
            {
                this.weights[i] = Math.Exp(this.weights[i] - maxVal);
                sumExp += this.weights[i];
            }

            for (int i = 0; i < this.nNeurons; i++)
            {
                this.weights[i] /= sumExp;

            }


        }

        /// <summary>
        /// Applies the activation sigmoid function on the desired layer
        /// </summary>
        public void activationLogistic()
        {

            for (int i = 0; i < this.nNeurons; i++)
            {

                this.weights[i] = logistic(this.weights[i]);

            }

        }
        /// <summary>
        /// Applies the activation TanH on the desired layer
        /// </summary>
        public void activationTanH()
        {
            for (int i = 0; i < this.nNeurons; i++)
                this.weights[i] = tanH(this.weights[i]);

        }

        /// <summary>
        /// Method that computes the sigmoid
        /// </summary>
        /// <param name="t">Input value</param>
        /// <returns>The logistic value</returns>
        static double logistic(double t)
        {

            double aux = 1.0 / (1.0 + Math.Exp(-t));
            return aux;
        }

        /// <summary>
        /// Computes de tanH function
        /// </summary>
        /// <param name="t">The input value</param>
        /// <returns>The TanH computed value</returns>
        static double tanH(double t)
        {
            return (2.0 / (Math.Exp(-t) + 1)) - 1;


        }

        /// <summary>
        /// An specific action that computes the activation function on the layer values
        /// </summary>
        public void doFeedForward()
        {

            switch (this.act_func)
            {

                case ActivationFunction.IDENTITY:

                    break;

                case ActivationFunction.LOGISTIC:
                    this.activationLogistic();
                    break;

                case ActivationFunction.SOFTMAX:
                    this.activationSoftMax();
                    break;

                case ActivationFunction.TANH:
                    this.activationTanH();
                    break;

            }



        }

        #endregion


            }
}
