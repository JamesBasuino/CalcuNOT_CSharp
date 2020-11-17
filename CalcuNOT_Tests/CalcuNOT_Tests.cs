using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalcuNOT_BL;
using System.Collections.Generic;
using System.Globalization;
using System;

namespace CalcuNOT_Tests
{
    [TestClass]
    public class CalcuNOT_Tests
    {
        [TestMethod]
        public void TokenizeTests()
        {
            string expr1 = "2+2";
            List<string> expr1_expected = new List<string>() { "2", "+", "2" };
            List<string> expr1_actual = CalcuNOT.Tokenize(expr1);
            CollectionAssert.AreEqual(expr1_expected, expr1_actual);

            string expr2 = "(2)+2/6*(8-4)";
            List<string> expr2_expected = new List<string>() {"(","2",")","+","2","/","6","*","(","8","-","4",")"};
            List<string> expr2_actual = CalcuNOT.Tokenize(expr2);
            CollectionAssert.AreEqual(expr2_expected, expr2_actual);

            string expr3 = "\t/89&a\n**";
            List<string> expr3_expected = new List<string>() {"\t","/","89&a\n","*","*"};
            List<string> expr3_actual = CalcuNOT.Tokenize(expr3);
            CollectionAssert.AreEqual(expr3_expected, expr3_actual);

            string expr4 = "abcdef*66..2~~//((((4h_<#";
            List<string> expr4_expected = new List<string>() {"abcdef","*","66..2~~","/","/","(","(","(","(","4h_<#"};
            List<string> expr4_actual = CalcuNOT.Tokenize(expr4);
            CollectionAssert.AreEqual(expr4_expected, expr4_actual);

            string expr5 = "";
            List<string> expr5_expected = new List<string>();
            List<string> expr5_actual = CalcuNOT.Tokenize(expr5);
            CollectionAssert.AreEqual(expr5_expected, expr5_actual);
        }

        [TestMethod]
        public void SY_ConvertTest()
        {
            List<string> expr1_tokens = new List<string>() { "2", "+", "2" };
            Queue<string> expr1_expected = new Queue<string>(new[] { "2", "2", "+" });
            Queue<string> expr1_actual = CalcuNOT.SY_convert(expr1_tokens);
            CollectionAssert.AreEqual(expr1_expected, expr1_actual);

            List<string> expr2_tokens = new List<string>() {"(","2",")","+","2","/","6","*","(","8","-","4",")"};
            Queue<string> expr2_expected = new Queue<string>(new[] { "2","2","6","/","8","4","-","*","+"});
            Queue<string> expr2_actual = CalcuNOT.SY_convert(expr2_tokens);
            CollectionAssert.AreEqual(expr2_expected, expr2_actual);

            List<string> expr3_tokens = new List<string>() { "(", "(", "153", "+", "32", ")", "*", "(", "1", "-", "89", ")", "+", "1324", ")", "/", "(", "88", "+", "499", ")" };
            Queue<string> expr3_expected = new Queue<string>(new[] { "153", "32", "+", "1", "89", "-", "*", "1324", "+", "88", "499", "+", "/" });
            Queue<string> expr3_actual = CalcuNOT.SY_convert(expr3_tokens);
            CollectionAssert.AreEqual(expr3_expected, expr3_actual);

            //Exception, extra left parenthesis
            List<string> expr4_tokens = new List<string>() { "(", "3", "+", "(", "26", "*", "12", ")" };
            Queue<string> expr4_res;
            Assert.ThrowsException<InvalidOperationException>(() => expr4_res = CalcuNOT.SY_convert(expr4_tokens));

            //Exception, extra right parenthesis
            List<string> expr5_tokens = new List<string>() { "3", "+", "26", "*", "12", ")" };
            Queue<string> expr5_res;
            Assert.ThrowsException<InvalidOperationException>(() => expr5_res = CalcuNOT.SY_convert(expr5_tokens));

            //Exception, invalid token
            List<string> expr6_tokens = new List<string>() { "3", "+", "26", "*", "ab`" };
            Queue<string> expr6_res;
            Assert.ThrowsException<ArgumentException>(() => expr6_res = CalcuNOT.SY_convert(expr6_tokens));

            //Exception, invalid token and extra left parenthesis.
            //Should throw argument exception first
            List<string> expr7_tokens = new List<string>() { "(","3", "+", "26", "*", "ab`" };
            Queue<string> expr7_res;
            Assert.ThrowsException<ArgumentException>(() => expr7_res = CalcuNOT.SY_convert(expr7_tokens));
        }

        [TestMethod]
        public void PostFix_EvaluateTests()
        {

            Queue<string> expr1_pf = new Queue<string>(new[] { "2", "2", "+" });
            double res1_expected = 4;
            double res1_actual = CalcuNOT.PostFix_Evaluate(expr1_pf);
            Assert.AreEqual(res1_expected, res1_actual);

            Queue<string> expr2_pf = new Queue<string>(new[] { "153", "32", "+", "1", "89", "-", "*", "1324", "+", "88", "499", "+", "/" });
            double res2_expected = -25.4787;
            double res2_actual = CalcuNOT.PostFix_Evaluate(expr2_pf);
            Assert.AreEqual(res2_expected, res2_actual);

            //Invalid expression
            Queue<string> expr3_pf = new Queue<string>(new[] { "3", "+", "26", "12", "*" });
            double expr3_res;
            Assert.ThrowsException<InvalidOperationException>(() => expr3_res = CalcuNOT.PostFix_Evaluate(expr3_pf));

            //Invalid Token, shouldnt happen in real calculator app but should still be tested
            Queue<string> expr4_pf = new Queue<string>(new[] { "3", "a", "+", "12", "*" });
            double expr4_res;
            Assert.ThrowsException<InvalidOperationException>(() => expr4_res = CalcuNOT.PostFix_Evaluate(expr4_pf));

            Queue<string> expr5_pf = new Queue<string>(new[] { "1", "0", "/" });
            double expr5_res;
            Assert.ThrowsException<DivideByZeroException>(() => expr5_res = CalcuNOT.PostFix_Evaluate(expr5_pf));
        }

        [TestMethod]
        public void WrongAnswer_OutTests()
        {
            //How many times to check for wrong answer
            int wc_amt = 100;
            int cntr = 0;
            double tmp_ans;

            //Wrong answer checkers
            double ans1 = 153;
            for(cntr = 0; cntr < wc_amt; cntr++) {
                tmp_ans = CalcuNOT.WrongAnswer_Out(ans1);
                Assert.AreNotEqual(ans1, tmp_ans);
            }

            double ans2 = 3798.88891;
            for(int i = 0; i < wc_amt; i++) {
                tmp_ans = CalcuNOT.WrongAnswer_Out(ans2);
                Assert.AreNotEqual(ans2, tmp_ans);
            }

            //Boundary checks
            double ans3 = Double.MaxValue;
            CalcuNOT.WrongAnswer_Out(ans3);

            double ans4 = Double.MinValue;
            CalcuNOT.WrongAnswer_Out(ans4);

            double ans5 = 0;
            CalcuNOT.WrongAnswer_Out(ans5);
        }
    }
}
