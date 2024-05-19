using System.Text.RegularExpressions;
using KnowledgeConquest.UtilityCli.Models;

namespace KnowledgeConquest.UtilityCli;

public sealed partial class LatexQuestionsParser
{
    public List<Question> Parse(TextReader reader)
    {
        var questions = new List<Question>();

        var line = reader.ReadLine();
        while (line != null)
        {
            if (line.StartsWith("\\VOPRN."))
            {
                questions.Add(ParseQuestion(line, reader));
            }
            line = reader.ReadLine();
        }

        return questions;
    }

    private Question ParseQuestion(string firstLine, TextReader reader)
    {
        var question = new Question
        {
            Title = ParseText(firstLine.AsSpan()[("\\VOPRN. ".Length - 1)..].Trim().ToString())
        };
        var rightAnswers = new HashSet<int>();
        var line = reader.ReadLine();
        while (line != null)
        {
            if (TryMatch(QuestionAnswerDescriptionRegex(), line, out var answerMatch))
            {
                question.Answers.Add(new QuestionAnswer()
                {
                    Index = int.Parse(answerMatch.Groups[1].ValueSpan) - 1,
                    Title = ParseText(answerMatch.Groups[2].ValueSpan.Trim().ToString()),
                });
            }
            else if (TryMatch(QuestionRightAnswersRegex(), line, out var rightAnswersMatch))
            {
                foreach (var answerNumber in ParseCsv(',', rightAnswersMatch.Groups[1].Value.Replace("]", ""), int.Parse))
                {
                    rightAnswers.Add(answerNumber - 1);
                }
            }
            else
            {
                break;
            }
            line = reader.ReadLine();
        }
        foreach (var rightAnswerIndex in rightAnswers)
        {
            question.Answers[rightAnswerIndex].IsRight = true;
        }
        return question;
    }

    private static bool TryMatch(Regex regex, string input, out Match match)
    {
        match = regex.Match(input);
        return match.Success;
    }

    private static List<T> ParseCsv<T>(char separator, string str, Func<string, T> elementParser)
    {
        var list = new List<T>();
        ParseCsv(list, separator, str, elementParser);
        return list;
    }

    private static void ParseCsv<T>(List<T> list, char separator, string str, Func<string, T> elementParser)
    {
        var i = 0;
        var prev = 0;
        while (true)
        {
            i = str.IndexOf(separator, i + 1);
            if (i == -1) break;
            list.Add(elementParser(str[prev..i]));
            prev = i + 1;
        }
        if (prev < str.Length)
        {
            list.Add(elementParser(str[prev..]));
        }
    }

    private static string ParseText(string latex)
    {
        var regex = LatexTagRegex();
        return regex.Replace(latex, match => match.Groups[1].Value);
    }

    [GeneratedRegex(@"\\par.*\{(\d*)\}(.*)")]
    private static partial Regex QuestionAnswerDescriptionRegex();

    [GeneratedRegex(@"\\appto\\answer\{(.*)\}.*")]
    private static partial Regex QuestionRightAnswersRegex();

    [GeneratedRegex(@"\\.*?\{(.*?)\}")]
    private static partial Regex LatexTagRegex();
}
