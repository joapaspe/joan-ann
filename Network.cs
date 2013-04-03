using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Forms;


namespace ANN
{
    public class Network : IAction
    {

        public Layer input, output;
        public List<IAction> actionList;
        //public List<Connections> connections;

        /// <summary>
        /// Creates a empty network
        /// </summary>
        public Network(){
            actionList = new List<IAction>();
            input = output = null;
        }

        /// <summary>
        /// Load a net from a file
        /// </summary>
        /// <param name="file">File Network route</param>
        public Network(string file): this()
        {

            StreamReader reader = null;
            reader = new StreamReader(file);

            this.readfromFile(reader);
            
        }

        public Network(Network n)
        {
            
            this.actionList = new List<IAction>();
            foreach(IAction ac in n.actionList){
                if (ac.GetType() == typeof(Layer))
                {
                    Layer acLayer = (Layer)ac;
                    Layer aux = new Layer(acLayer);
                    this.actionList.Add(aux);

                    if (acLayer == n.input)
                    {
                        this.input = aux;

                    }
                    else if (acLayer == n.output)
                    {

                        this.output = aux;
                    }
                }
                else
                {
                    this.actionList.Add(ac);
                }

            }

        }

        public static Network fromDescription(string description)
        {
            Network network = new Network();

            string[] desc = description.Split();

            int idx = 0;
            Layer ant = null;

            while (idx < desc.Length)
            {
                int nNeurons = Int32.Parse(desc[idx++]);
                string sAct = desc[idx++];
                ActivationFunction fAct = Layer.stringToActivation(sAct);

                if (ant == null)
                {
                    network.input = new Layer(nNeurons, fAct);
                    ant = network.input;
                    continue;
                }

                Layer newLayer = new Layer(nNeurons, fAct);

                Connections newConnection = new Connections(ant, newLayer);
                network.actionList.Add(newConnection);

                network.actionList.Add(newLayer);

                ant = newLayer;


            }

            network.output = ant;
            return network;

        }

        /// <summary>
        /// Load a net from a streamreader (file)
        /// </summary>
        /// <param name="stream">Stream file reader correctly open</param>
        public Network(StreamReader stream)
        {
            this.readfromFile(stream);

        }

        /// <summary>
        /// Add a layer from a determined vector
        /// </summary>
        /// <param name="v">Vector with the values of the layer</param>
        /// <param name="activate">The activation function</param>
        /// <returns></returns>
        public Layer addLayer(double[] v, ActivationFunction activate)
        {
            Layer layer = new Layer(v, activate);
            
            actionList.Add(layer);

            return layer;
        }

        /// <summary>
        /// Add a layer with a given input neurons
        /// </summary>
        /// <param name="nNeurons">Number of neurons of the function</param>
        /// <param name="activate">Activation Function</param>
        /// <returns></returns>
        public Layer addLayer(int nNeurons, ActivationFunction activate)
        {
            Layer layer = new Layer(nNeurons, activate);
            

            actionList.Add(layer);

            return layer;
        }

        /// <summary>
        /// Parser of the lua format
        /// </summary>
        /// <param name="reader">Correctly open stream file reader</param>
        public void readfromFile(StreamReader reader){

            /* return {
               "256 inputs 1024 tanh 10 softmax",
               matrix.fromString[[273418
                ascii
            */
            string line;

            //Avoid comments and lua lines
            do
            {
                line = reader.ReadLine().Trim();
                
            } while (line.StartsWith("#") || line.StartsWith("return"));


            string description = line;

            description = description.Replace("\"", "");
            
            string[] camps = description.Split();
            int ncamps = camps.Length;
            // Read the Network info
            // Nneurons0 tipe0 .. NneuronsN tipeN

            Layer ant = null;

            //Avoid lua code
            do
            {

                line = reader.ReadLine().Trim();
                

            } while (line.StartsWith("#") || line.StartsWith("matrix"));

            
            for (int i = 0; i < ncamps; i+=2) {
                int nNeurons = int.Parse(camps[i]);
                string type = camps[i + 1];
                
                if (ant == null)
                {
                    // Is the first layer
                    this.input = addLayer(nNeurons, Layer.stringToActivation(type));
                    ant = this.input;
                    continue;
                }
                
                Layer newLayer = new Layer(nNeurons, Layer.stringToActivation(type));

                Connections newConnection = new Connections(ant, newLayer);
                this.actionList.Add(newConnection);
                    
                this.actionList.Add(newLayer);

                newConnection.readWeightsfromStream(reader);

                ant = newLayer;

                //Console.WriteLine(newConnection.ToString());

            }

            if (ant != null) this.output = ant;
        }

        /// <summary>
        /// Returns the output layer. The computed values.
        /// </summary>
        /// <returns>Returns a double vector with the output values</returns>
        public double []getOutput()
        {

            return this.output.weights;
        }


        public int getMaxOutputLabel(){

            double max = double.MinValue;
            int index = -1;
            for (int i = 0; i < this.getNOutputNeurons(); i++)
            {
                if (this.output.weights[i] > max)
                {
                    max = this.output.weights[i];
                    index = i;

                }

            }

            return index;

        }

        /// <summary>
        /// Returns the number of the input neuron of the network
        /// </summary>
        /// <returns>Returns the number of the input neuron of the network</returns>
        public int getNInputNeurons()
        {

            if (this.input == null)
            {
                return 0;
            }
            else return this.input.nNeurons;


        }

        /// <summary>
        /// Returns the number of the neurons on the output layer
        /// </summary>
        /// <returns>Returns the number of the neurons on the output layer</returns>
        public int getNOutputNeurons()           
        {

            if (this.output == null)
            {
                return 0;
            }
            else return this.output.nNeurons;


        }

        /// <summary>
        /// Method that computes the feedforward action along the ActionList
        /// </summary>
        public void doFeedForward()
        {

            foreach (IAction actionItem in this.actionList)
            {
                
                actionItem.doFeedForward();
            }


        }

        /// <summary>
        /// Specific methods that compute the feedforward on a net given a determined input
        /// </summary>
        /// <param name="v">The desired input of the network</param>
        /// <returns>The Output value of the network</returns>
        public double[] computeNetwork(double []v)
        {

            if (v.Length != this.input.nNeurons){

                return null;

            }

            /*
            for (int i = 0; i < this.input.nNeurons; i++)
            {
                

            }*/
            this.input.weights = v;

            this.doFeedForward();

            return this.output.weights;

        }
    }
}
