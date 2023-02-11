using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Give value 0 to the operations field
            this.operations.Text = "0";

            // Give value 0 to the result field
            this.result.Text = "0";

            // Assign buttons functions
            delOperationsButton.Click += EraseCurrentOperations;
            delAllButton.Click += EraseAll;
            divButton.Click += AddSymbol;
            multButton.Click += AddSymbol;
            minButton.Click += AddSymbol;
            addButton.Click += AddSymbol;
            dotButton.Click += ParseLastIntegerInputToFloat;
            zeroButton.Click += AddNumber;
            oneButton.Click += AddNumber;
            twoButton.Click += AddNumber;
            threeButton.Click += AddNumber;
            fourButton.Click += AddNumber;
            fiveButton.Click += AddNumber;
            sixButton.Click += AddNumber;
            sevenButton.Click += AddNumber;
            eightButton.Click += AddNumber;
            neinButton.Click += AddNumber;
            equalButton.Click += Calculate;
        }

        /// <summary>
        /// Function dedicated to reset the operation line
        /// </summary>
        /// <param name="sender">Object which raise the event calling this function</param>
        /// <param name="e">Event which call this function</param>
        private void EraseCurrentOperations(object sender, RoutedEventArgs e)
        {
            // Give value 0 to the operations field
            this.operations.Text = "0";
        }

        /// <summary>
        /// Function dedicated to reset the operation line and result line
        /// </summary>
        /// <param name="sender">Object which raise the event calling this function</param>
        /// <param name="e">Event which call this function</param>
        private void EraseAll(object sender, RoutedEventArgs e)
        {
            // Give value 0 to the operations field
            this.operations.Text = "0";

            // Give value 0 to the result field
            this.result.Text = "0";
        }

        /// <summary>
        /// Function dedicated to add an operation symbol to the current string operation
        /// </summary>
        /// <param name="sender">Object which raise the event calling this function</param>
        /// <param name="e">Event which call this function</param>
        private void AddSymbol(object sender, RoutedEventArgs e)
        {
            // Get symbol of the current operation in the differents cases
            if (sender == divButton || sender == multButton || sender == addButton || sender == minButton)
            {
                Button sendedButton = sender as Button;
                string selection = sendedButton.Content.ToString();

                // The update cannot proceed if the last char of the operation string is an operation char
                if (!this.operations.Text.EndsWith('/') && !this.operations.Text.EndsWith('*') && !this.operations.Text.EndsWith('+') && !this.operations.Text.EndsWith('-'))
                {
                    // Just update the current operation line
                    this.operations.Text += selection;
                }
            }
        }

        /// <summary>
        /// Function to transform the last number input integer to a float
        /// </summary>
        /// <param name="sender">Object which raise the event calling this function</param>
        /// <param name="e">Event which call this function</param>
        private void ParseLastIntegerInputToFloat(object sender, RoutedEventArgs e)
        {
            // Get the operations string
            string input = this.operations.Text;

            // Parse the current operations string as an array of strings
            string[] inputs = input.Split(new char[] { '+', '-', '/', '*' }, StringSplitOptions.None);

            // If the last input is not null or contain a dot
            if (!string.IsNullOrEmpty(inputs.Last()) && !inputs.Last().Contains('.'))
            {
                this.operations.Text = string.Concat(this.operations.Text, '.');
            }

            // If the last occurence is null
            if (string.IsNullOrEmpty(inputs.Last()))
            {
                this.operations.Text = string.Concat(this.operations.Text, "0.");
            }
        }

        /// <summary>
        /// Function dedicated to add a number to the current string operation
        /// </summary>
        /// <param name="sender">Object which raise the event calling this function</param>
        /// <param name="e">Event which call this function</param>
        private void AddNumber(object sender, RoutedEventArgs e)
        {
            // Get the number of the button in the differents cases
            if (sender == zeroButton || sender == oneButton || sender == twoButton || sender == threeButton || sender == fourButton || sender == fiveButton || sender == sixButton || sender == sevenButton || sender == eightButton || sender == neinButton)
            {
                Button sendedButton = sender as Button;
                string selection = sendedButton.Content.ToString();

                if (this.operations.Text == "0")
                {
                    if (selection != "0")
                    {
                        this.operations.Text = selection;
                    }
                }
                else
                {
                    this.operations.Text += selection;
                }
            }
        }

        /// <summary>
        /// Function dedicated to run the operation of calculation
        /// </summary>
        /// <param name="sender">Object which raise the event calling this function</param>
        /// <param name="e">Event which call this function</param>
        private void Calculate(object sender, RoutedEventArgs e)
        {
            try
            {
                // Change all divisions by a multiplication by inverted number
                string operations = this.operations.Text.Replace("/", "*1/");

                // Change all substraction by an addition of negative number
                operations = operations.Replace("-", "+-1*");

                // Separate all additions blocks and do all multiplications
                string[] additivesBlocks = operations.Split('+');

                // Transform each substrings of the additivesBlocks to a number
                additivesBlocks = additivesBlocks.Select(block =>
                {
                    // Split all elements in the substring which contains the multiply operator
                    if (block.Contains('*'))
                    {
                        string[] multiplicationBlocks = block.Split('*');

                        multiplicationBlocks = multiplicationBlocks
                            .Select(subBlock =>
                            {
                                // Transform all division of one by a number to the associated invert number
                                if (subBlock.Contains('/'))
                                {
                                    string numberToInvert = subBlock.Replace("1/", null);

                                    // Handle a possible division by 0 from user
                                    if(numberToInvert.Equals("0"))
                                    {
                                        throw new Exception("Error : division by 0");
                                    }

                                    double result = Convert.ToDouble(numberToInvert);
                                    result = Math.Pow(result, -1);
                                    subBlock = result.ToString();
                                }

                                return subBlock;
                            })
                            .ToArray();

                        // Transform all subblock into a double subblock
                        double[] allBlocks = multiplicationBlocks.Select(subBlock => Convert.ToDouble(subBlock)).ToArray();

                        // Multiply all blocks into one unique double number
                        block = allBlocks.Aggregate((x, y) => x * y).ToString();
                    }

                    return block;
                })
                    .ToArray();

                // Convert all string blocks into double blocks
                double[] doubleBlocks = additivesBlocks.Select(block => Convert.ToDouble(block)).ToArray();

                // Calculation of the final result
                double result = doubleBlocks.Aggregate((x, y) => x + y);

                // Give value 0 to the operations field
                this.operations.Text = "0";

                // Give value 0 to the result field
                this.result.Text = result.ToString();
            }
            catch(Exception ex)
            {
                // Give value 0 to the operations field
                this.operations.Text = "0";

                // Give value 0 to the result field
                this.result.Text = ex.Message;
            }
        }
    }
}
