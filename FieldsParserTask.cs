using NUnit.Framework;
using System.Collections.Generic;

namespace TableParser
{
    [TestFixture]
    public class FieldParserTaskTests
    {
        public static void Test(string input, string[] expectedResult)
        {
            var actualResult = FieldsParserTask.ParseLine(input);
            Assert.AreEqual(expectedResult.Length, actualResult.Count);
            for (int i = 0; i < expectedResult.Length; ++i)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i].Value);
            }
        }

        [TestCase("text", new[] { "text" })]
        [TestCase("hello world", new[] { "hello", "world" })]
        [TestCase("", new string[0])]
        [TestCase(" ", new string[0])]
        [TestCase("''", new[] { "" })]
        [TestCase(@"''""""", new[] { "", "" })]
        [TestCase(@"' a ", new[] { " a " })]
        [TestCase("'a'", new[] { "a" })]
        [TestCase(@"""'""", new[] { "'" })]
        [TestCase(@"'\\'", new[] { @"\" })]
        [TestCase(@"'\""'", new[] { @"""" })]
        [TestCase(@"'\''", new[] { @"'" })]
        [TestCase(@"""a", new[] { "a" })]
        [TestCase(@"a  ""bcd ef""", new[] { "a", "bcd ef" })]
        [TestCase(@"'ab'  ba", new[] { "ab", "ba" })]
        [TestCase(@"""bcd\""", new[] { @"bcd""" })]
        public static void RunTests(string input, string[] expectedOutput)
        {
            Test(input, expectedOutput);
        }
    }

    public class FieldsParserTask
    {
        public static List<Token> ParseLine(string line)
        {
            List<Token> result = new List<Token>();
            Token curToken;

            while (true)
            {
                if (result.Count == 0)
                    curToken = GetToken(line);
                else
                    curToken = GetToken(line, result[result.Count - 1].GetIndexNextToToken());

                if (curToken == null)
                    break;

                result.Add(curToken);
            }

            return result;
        }

        static Token GetToken(string line, int startIndex = 0)
        {
            if (startIndex >= line.Length)
                return null;

            string value = line.Remove(0, startIndex);
            int delta = value.Length;
            value = value.TrimStart(' ');
            delta -= value.Length;

            if (string.IsNullOrEmpty(value))
                return null;
            
            if (value[0] == '"' || value[0] == '\'')
                return ReadQuotedField(line, startIndex + delta);

            int endIndex = value.IndexOfAny(new[] { ' ', '"', '\'', });
            if (endIndex != -1)
                value = value.Substring(0, endIndex);

            return new Token(value, startIndex + delta, value.Length);
        }

        private static Token ReadField(string line, int startIndex)
        {
            return new Token(line, 0, line.Length);
        }

        public static Token ReadQuotedField(string line, int startIndex)
        {
            return QuotedFieldTask.ReadQuotedField(line, startIndex);
        }
    }
}