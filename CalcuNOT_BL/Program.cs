using System;
using System.Collections.Generic;

namespace CalcuNOT_BL
{
    class Program
    {
        //TODO: run unti tests on Tokenize and SY_Convert
        static void Main(string[] args)
        {
            string input = "1/0";

            List<string> tmp = CalcuNOT.Tokenize(input);
            Queue<string> pfix = CalcuNOT.SY_convert(tmp);
            Print_Q(pfix);
            Console.WriteLine();

            double result = CalcuNOT.PostFix_Evaluate(pfix);
            Console.WriteLine(result);

            double wrong_result = CalcuNOT.WrongAnswer_Out(result);
            Console.WriteLine("The answer is not " + wrong_result.ToString());
            
        }

        public static void Print_Arr(List<string> arr)
        {
            Console.Write("[ ");
            foreach (var item in arr)
                Console.Write("\"" + item + "\"" + ",");
            Console.Write(" ]");
        }

        public static void Print_Q(Queue<string> queue)
        {
            foreach (var item in queue)
                Console.Write(item + " ");
        }

    }
}
