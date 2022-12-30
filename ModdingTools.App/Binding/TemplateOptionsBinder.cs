using System.CommandLine;
using System.CommandLine.Binding;
using ModdingTools.Core.Options;

namespace ModdingTools.App.Binding;

public class TemplateOptionsBinder : BinderBase<TemplateOptions>
{
    private readonly Argument<DirectoryInfo?> _location;
    private readonly IValueDescriptor<string?> _projectName;
    private readonly Option<string?> _assemblyName;
    private readonly Option<string?> _solutionName;
    private readonly Option<string?> _rootNamespace;
    private readonly Option<string?> _templateShortName;

    public TemplateOptionsBinder(
        Argument<DirectoryInfo?> location,
        IValueDescriptor<string?> projectName,
        Option<string?> assemblyName,
        Option<string?> solutionName,
        Option<string?> rootNamespace,
        Option<string?> templateShortName)
    {
        _location = location;
        _projectName = projectName;
        _assemblyName = assemblyName;
        _solutionName = solutionName;
        _rootNamespace = rootNamespace;
        _templateShortName = templateShortName;
    }

    protected override TemplateOptions GetBoundValue(BindingContext bindingContext)
    {
        var location = bindingContext.ParseResult.GetValueForArgument(_location) ?? new DirectoryInfo(".");
        var sanitizedLocationName = location.Name.Replace(' ', '.');
        var projectName = "";
        
        if (_projectName is Option<string?> pNameOption)
            projectName = bindingContext.ParseResult.GetValueForOption(pNameOption) ?? sanitizedLocationName;
        else if (_projectName is Argument<string?> pNameArgument)
            projectName = bindingContext.ParseResult.GetValueForArgument(pNameArgument) ?? sanitizedLocationName;
        
        var assemblyName = bindingContext.ParseResult.GetValueForOption(_assemblyName) ?? projectName;
        var solutionName = bindingContext.ParseResult.GetValueForOption(_solutionName) ?? sanitizedLocationName;
        var rootNamespace = bindingContext.ParseResult.GetValueForOption(_rootNamespace) ?? projectName;
        var templateShortName = bindingContext.ParseResult.GetValueForOption(_templateShortName) ?? "";

        return new TemplateOptions
        {
            Location = location,
            ProjectName = projectName,
            AssemblyName = assemblyName,
            SolutionName = solutionName,
            RootNamespace = rootNamespace,
            TemplateShortName = templateShortName
        };
    }
}
