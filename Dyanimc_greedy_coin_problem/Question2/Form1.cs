using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Question2
{
    public partial class Form1 : Form
    {
        public Form1 ()
        {
            InitializeComponent();
        }

        #region Varibles
        /*data holders for Dynamic method*/
        int[,] optimal;
        string[,] optCoin;
        List<string> allPossibilities = new List<string>();
        int dynAmount;
        int[] denominations;

        /*data holders for greedy method*/
        Dictionary<int, int> greedyCoins = new Dictionary<int, int>();
        int[] coins = new int[] { 1, 2, 5, 10, 25, 50, 100 };
        int coinAmount = 72;

        bool run = false;


        #endregion


        #region Dynamic Methods

        /*Display all possible Solutions of dynamic method in to textbox*/
        public void displayAllPossibilities ()
        {
            if(allPossibilities.Count > 0)
            {

                int i = 1;
                foreach(string s in allPossibilities)
                {
                    SetText2(i + ") " + s + "\n");
                    i++;
                }
                SetText2("\n");
            }
            else
            {
                SetText2("No change for dynAmount=" + dynAmount + ", Denominations=" + denomiStr() + "\n");
            }

        }

        /*returns the optimal SOlution of the all found solutions by dynamic method*/
        public String DynamicOptimal ()
        {
            int i = optCoin.GetLength(0);
            int j = optCoin.GetLength(1);
            String str = optCoin[i - 1, j - 1].Trim();

            return str;
        }

        /*build teh denominations into a string*/
        public string denomiStr ()
        {
            StringBuilder sb = new StringBuilder();
            foreach(int i in denominations)
            {
                sb.Append(i + " ");
            }
            return sb.ToString();
        }

        /*method to find all the possible solutions recursively*/
        private void dynamicAllPossiblitiesRecursive (string tmpstr, int sIndex, int remaining)
        {
            for(int i = sIndex; i < denominations.Length; i++)
            {
                int temp = remaining - denominations[i];
                String tempStr = tmpstr + "" + denominations[i] + ",";
                if(temp < 0)
                {
                    break;
                }
                if(temp == 0)
                {
                    allPossibilities.Add(tempStr);
                    break;
                }
                else
                {

                    dynamicAllPossiblitiesRecursive(tempStr, i, temp);
                }
            }
        }

        /*get the optimal solutions from all possibilities*/
        public void findOptimalChange (int dynAmount, int[] denominations)
        {

            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < optimal.GetLength(1); i++)
            {
                optimal[0, i] = i;
                optCoin[0, i] = sb.ToString();
                sb.Append(denominations[0] + " ");
            }


            for(int i = 1; i < denominations.Length; i++)
            {
                for(int j = 0; j < dynAmount + 1; j++)
                {
                    int value = j;
                    int targetWithPrevDenomiation = optimal[i - 1, j];
                    int ix = (value) - denominations[i];
                    if(ix >= 0 && (denominations[i] <= value))
                    {
                        int x2 = denominations[i] + optimal[i, ix];
                        if(x2 <= dynAmount && (1 + optimal[i, ix] < targetWithPrevDenomiation))
                        {
                            String temp = optCoin[i, ix] + denominations[i] + " ";
                            optCoin[i, j] = temp;
                            optimal[i, j] = 1 + optimal[i, ix];
                        }
                        else
                        {
                            optCoin[i, j] = optCoin[i - 1, j] + " ";
                            optimal[i, j] = targetWithPrevDenomiation;
                        }
                    }
                    else
                    {
                        optCoin[i, j] = optCoin[i - 1, j];
                        optimal[i, j] = targetWithPrevDenomiation;
                    }
                }
            }

        }
        /*method to call the recursive function*/
        public void findAllPossibleCombinations (int dynAmount, int[] denominations)
        {
            string tempStr = "";
            dynamicAllPossiblitiesRecursive(tempStr, 0, dynAmount);
        }

        /*initialization of data for Dynamic Solution*/
        public void solutionCoins (int dynAmountx, int[] denominationsx)
        {
            dynAmount = dynAmountx;
            denominations = denominationsx;
            optCoin = new String[denominations.Length, dynAmount + 1];
            optimal = new int[denominations.Length, dynAmount + 1];
        }

        #endregion


        #region Greedy Methods

        /*greedy method to get the minimum number of coins for given coinAmount*/
        private void calcGreedy (int coinAmount)
        {

            
            int[] coinArrayx = coins.ToArray();
            Array.Reverse(coinArrayx);
            int change = coinAmount;

            foreach(int coinv in coinArrayx)
            {
                int noCoins = change / coinv;

                if(noCoins == 0)
                {
                    continue;
                }
                change = change % coinv;
                greedyCoins.Add(coinv, noCoins);

                if(change == 0)
                {
                    break;
                }
            }
            foreach(KeyValuePair<int, int> coinv in greedyCoins)
            {
                SetText3(coinv.Value + "  Coins of  denomination  " + coinv.Key + "\n");
            }
        }

        #endregion




        /*method to be used in dynamic functions thread */
        private void dynmaicStart ()
        {
            solutionCoins(coinAmount, coins);
            findAllPossibleCombinations(coinAmount, coins);
            displayAllPossibilities();
            findOptimalChange(coinAmount, coins);
            SetText4(DynamicOptimal());
        }

        /*method to be used in greedy functions thread */
        private void greedyStart ()
        {
            calcGreedy(coinAmount);
        }

        private void button1_Click (object sender, EventArgs e)
        {

            try
            {
                List<int> tempc = new List<int>();
                string[] temp = textBox5.Text.Split(' ');
                for(int i = 0; i < temp.Length; i++)
                {
                    tempc.Add(Convert.ToInt32(temp[i]));
                }
                coins = tempc.ToArray();
                Array.Sort(coins);

            }
            catch(Exception et)
            {
                MessageBox.Show("wrong input!");

            }

            if(run == false)
            {
                coinAmount = Convert.ToInt32(textBox1.Text);
                Thread dynamic = new Thread(dynmaicStart);
                dynamic.Start();
                Thread greedy = new Thread(greedyStart);
                greedy.Start();
                run = true;
            }
            else
            {
                Form1 frm = new Form1();
                this.Hide();
                frm.Show();
            }

        }

        #region GUI methods

        delegate void SetTextCallback (string text);


        private void SetText3 (string text)
        {

            if(this.textBox3.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText3);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox3.AppendText(text);

            }
        }

        private void SetText2 (string text)
        {

            if(this.textBox2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox2.AppendText(text);

            }
        }

        private void SetText4 (string text)
        {

            if(this.textBox4.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText4);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox4.AppendText(text);

            }
        }

    }

        #endregion


}
