using DotNet.Globbing;

namespace ModdingTools.Core.PatternMatching;

public class GlobMatcher
{
    public IList<Glob> GlobPatterns { get; set; } = new List<Glob>();

    public GlobMatcher(IList<string> patterns)
    {
        // Overide the default options globally for all matche:
        GlobOptions.Default.Evaluation.CaseInsensitive = true;

        GlobPatterns = patterns.Select(Glob.Parse).ToList();

        // There's also a simple, already built in, glob parser we could use.
        //  FileSystemName.MatchesSimpleExpression(expression.AsSpan(), name, ignoreCase)
    }

    public bool IsMatch(FileInfo file) => IsMatch(file.Name);
    
    public bool IsMatch(string text) => GlobPatterns.Any(gl => gl.IsMatch(text));
}
