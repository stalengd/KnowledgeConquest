using KnowledgeConquest.UtilityCli;
using KnowledgeConquest.UtilityCli.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.CommandLine;
using System.Diagnostics;

var inputOption = new Option<FileInfo?>(
    name: "-i",
    description: "Input file path");

var outputOption = new Option<FileInfo?>(
    name: "-o",
    description: "Output file path");

var connectionStringArgument = new Argument<string>(
    name: "connection string",
    description: "Connection string for PostgreSQL database. Example: 'Host=localhost; Port=5432; Database=database; User Id=user; Password=password;'");

var waitForDebuggerOption = new Option<bool>(
    name: "--debugger");

var rootCommand = new RootCommand("");
rootCommand.AddGlobalOption(waitForDebuggerOption);

var questionsCommand = new Command("questions", "Questions management");
rootCommand.AddCommand(questionsCommand);

var parseLatexTestCommand = new Command("parse-latex", "Extract questions latex test file")
{
    inputOption,
    outputOption,
};
questionsCommand.AddCommand(parseLatexTestCommand);
parseLatexTestCommand.SetHandler((inputFile, outputFile, waitForDebugger) =>
{
    if (waitForDebugger)
    {
        Debugger.Launch();
    }
    inputFile ??= new FileInfo("input.tex");
    outputFile ??= new FileInfo("questions.json");
    using var inputStream = inputFile.OpenRead();
    using var reader = new StreamReader(inputStream);
    var questions = new LatexQuestionsParser().Parse(reader);
    using var outputStream = outputFile.Open(FileMode.Create);
    using var textWriter = new StreamWriter(outputStream);
    using var jsonWriter = new JsonTextWriter(textWriter);
    var serializer = new JsonSerializer()
    {
        Formatting = Formatting.Indented,
    };
    serializer.Serialize(jsonWriter, questions);
}, inputOption, outputOption, waitForDebuggerOption);

var pushCommand = new Command("push", "Push questions to database")
{
    inputOption,
    connectionStringArgument,
};
questionsCommand.Add(pushCommand);
pushCommand.SetHandler((inputFile, connectionString, waitForDebugger) =>
{
    if (waitForDebugger)
    {
        Debugger.Launch();
    }
    inputFile ??= new FileInfo("questions.json");
    using var inputStream = inputFile.OpenRead();
    using var textReader = new StreamReader(inputStream);
    using var jsonReader = new JsonTextReader(textReader);
    var serializer = new JsonSerializer();
    var questions = serializer.Deserialize<List<Question>>(jsonReader);
    if (questions == null)
    {
        Console.WriteLine("No questions found");
        return;
    }

    using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(connectionString)
        .Options);
    foreach (var question in questions)
    {
        if (question.Answers.Count == 0) 
        {
            continue;
        }
        db.Questions.Add(question);
    }
    db.SaveChanges();
}, inputOption, connectionStringArgument, waitForDebuggerOption);

return rootCommand.InvokeAsync(args).Result;
