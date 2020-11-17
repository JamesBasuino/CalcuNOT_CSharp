using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CalcuNOT_BL
{
    public class CalcuNOT
    {
        //TODO: documentation and Unit Tests
        public static List<string> Tokenize(string expr)
        {
            string operators = "-+/*()";
            List<string> ret_lst = new List<string>();
            string tmp = "";

            for (int i = 0; i < expr.Length; i++)
            {
                if (operators.Contains(expr[i])) {
                    //if previous token was a operand, push onto list and clear
                    if (tmp.Length > 0)
                    {
                        ret_lst.Add(tmp);
                        tmp = "";
                    }
                    ret_lst.Add(expr[i].ToString());
                } else {
                    //Build up operand
                    tmp += expr[i];
                }
            }

            //If anything left
            if (tmp.Length > 0)
                ret_lst.Add(tmp);

            return ret_lst;
        }

        public static Queue<string> SY_convert(List<string> expr_tokens)
        {
            //All left associative, so dont have to check for different associtivty with same precedence
            Dictionary<string, int> precedence_table = new Dictionary<string, int>()
            {
                {"*", 3 },
                {"/", 3 },
                {"+", 2 },
                {"-", 2 },
            };

            Queue<string> output_queue = new Queue<string>();
            Stack<string> operator_stack = new Stack<string>();

            foreach (var token in expr_tokens) {
                bool isNumber = Double.TryParse(token, out _);

                if (isNumber) {
                    output_queue.Enqueue(token);
                }
                else if (precedence_table.ContainsKey(token)) {
                    if (operator_stack.Count == 0) {
                        operator_stack.Push(token);
                    } else {
                        while ((operator_stack.Count > 0) && (operator_stack.Peek() != "(") && (precedence_table[operator_stack.Peek()] >= precedence_table[token])) //realies on short circuit (fix!!)
                            output_queue.Enqueue(operator_stack.Pop());
                        operator_stack.Push(token);
                    }
                }
                else if (token == "(") {
                    operator_stack.Push(token);
                }
                else if (token == ")") {
                    try {
                        while (operator_stack.Peek() != "(")
                            output_queue.Enqueue(operator_stack.Pop());

                        if (operator_stack.Peek() == "(")
                            operator_stack.Pop();
                    }
                    catch (InvalidOperationException e) {
                        throw new InvalidOperationException("Missmatching Parenthesis: Extra ) ?");
                    }
                } else {
                    throw new ArgumentException("Invalid Token: " + token);
                }
            }

            while (operator_stack.Count > 0)
            {
                if (operator_stack.Peek() == "(" || operator_stack.Peek() == ")")
                    throw new InvalidOperationException("Missmatching Parenthesis");
                output_queue.Enqueue(operator_stack.Pop());
            }

            return output_queue;
        }

        public static double PostFix_Evaluate(Queue<string> expr)
        {
            Stack<double> eval_stack = new Stack<double>();
            string tmp_tok;
            double tmp_stack_item;
            bool isNumber;
            while (expr.Count > 0)
            {
                tmp_tok = expr.Dequeue();
                isNumber = Double.TryParse(tmp_tok, out tmp_stack_item);

                //Operand - push to stack
                if (isNumber) {
                    eval_stack.Push(tmp_stack_item);
                }
                //Operator - pop top 2 numbers on stack, apply operator, push result back onto stack => evaluate op2 <operator> op1
                //Due to nature of postfix, guarenteed at least 2 operands on stack
                else {
                    try {
                        double op1 = eval_stack.Pop();
                        double op2 = eval_stack.Pop();

                        if (tmp_tok == "+")
                            eval_stack.Push(op2 + op1);
                        else if (tmp_tok == "-")
                            eval_stack.Push(op2 - op1);
                        else if (tmp_tok == "/") {
                            if (op1 == 0)
                                throw new DivideByZeroException();
                            eval_stack.Push(op2 / op1);
                        }
                        else if (tmp_tok == "*")
                            eval_stack.Push(op2 * op1);
                    }
                    catch (InvalidOperationException e) {
                        throw new InvalidOperationException("Invalid Numerical Expression");
                    }

                }
            }

            return Math.Round(eval_stack.Pop(), 4);
        }

        public static double WrongAnswer_Out(double ans)
        {
            double wrong_res = ans;


            //If real answer if an integer generate a random integer
            //if real answer is double then can generate random double

            double stdDev = 10;
            //make sure random answer is not real answer
            while (wrong_res == ans)
            {
                if ((ans == Math.Floor(ans))) //integer answer
                    wrong_res = (int)GuassianRNG(ans,stdDev);
                else //double answer
                    wrong_res = Math.Round(GuassianRNG(ans,stdDev),4);
            }

            return wrong_res;
        }
        
        private static double GuassianRNG(double mean,double stdDev)
        {
            Random rand = new Random();

            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mean + stdDev * randStdNormal;

            return randNormal;
        }

    }
}

