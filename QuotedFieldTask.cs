using System.Linq;
using NUnit.Framework;

namespace TableParser
{
    [TestFixture]
    public class QuotedFieldTaskTests
    {
        [TestCase("''", 0, "", 2)]
        [TestCase("'a'", 0, "a", 3)]
        [TestCase(@"a ""bcd ef"" 'x y'", 2, "bcd ef", 8)]
        [TestCase(@"a ""bcd\"" ef"" 'x y'", 2, @"bcd"" ef", 10)]
        [TestCase(@"""a", 0, "a", 2)]
        public void Test(string line, int startIndex, string expectedValue, int expectedLength)
        {
            var actualToken = QuotedFieldTask.ReadQuotedField(line, startIndex);
            Assert.AreEqual(new Token(expectedValue, startIndex, expectedLength), actualToken);
        }
    }

    class QuotedFieldTask
    {
        public static Token ReadQuotedField(string line, int startIndex)
        {
            if (!line.Contains('"') && !line.Contains('\''))
                return new Token(new string(line.Skip(startIndex + 1).ToArray()), startIndex, line.Length - startIndex);

            char fieldSign;

            if (startIndex == 0)
            {
                fieldSign = line.First(c => c == '"' || c == '\'');
                startIndex = line.IndexOf(fieldSign);
            }
            else
                fieldSign = line[startIndex];

            string result = "";
            int length = 1;

            for (int i = startIndex + 1; i < line.Length; i++)
            {
                char c = line[i];
                length++;

                if (c == fieldSign)
                    break;

                if (c == '\\' && line.Length > i + 1)
                    if (line[i + 1] == '"' || line[i + 1] == '\'' || line[i + 1] == '\\')
                    {
                        c = line[i + 1];
                        length++;
                        i++;
                    }
                result += c;
            }

            return new Token(result, startIndex, length);
        }
    }
}