using Mono.Cecil;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ModdingTools.Core.Utils;

public class GameUtility
{
    private const string MainDLLFileName = "Assembly-CSharp.dll";

    public static Version GetCurrentGameVersion(string baseDllPath)
    {
        DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();
        MetadataResolver metadataResolver = new MetadataResolver((IAssemblyResolver) assemblyResolver);
        assemblyResolver.AddSearchDirectory(baseDllPath);
        ReaderParameters parameters = new ReaderParameters()
        {
            MetadataResolver = (IMetadataResolver) metadataResolver
        };
        using (AssemblyDefinition assemblyDefinition =
               AssemblyDefinition.ReadAssembly(Path.Combine(baseDllPath, "Assembly-CSharp.dll"),
                   parameters))
        {
            CustomAttribute customAttribute =
                assemblyDefinition.CustomAttributes.FirstOrDefault<CustomAttribute>(
                    (Func<CustomAttribute, bool>) (x => x.AttributeType.Name == "VersionAttribute"));
            CustomAttributeArgument attributeArgument = customAttribute.Fields
                .FirstOrDefault<CustomAttributeNamedArgument>(
                    (Func<CustomAttributeNamedArgument, bool>) (x => x.Name == "Major")).Argument;
            short major = (short) attributeArgument.Value;
            attributeArgument = customAttribute.Fields
                .FirstOrDefault<CustomAttributeNamedArgument>(
                    (Func<CustomAttributeNamedArgument, bool>) (x => x.Name == "Minor")).Argument;
            short minor = (short) attributeArgument.Value;
            attributeArgument = customAttribute.Fields
                .FirstOrDefault<CustomAttributeNamedArgument>(
                    (Func<CustomAttributeNamedArgument, bool>) (x => x.Name == "Revision")).Argument;
            short revision = (short) attributeArgument.Value;
            attributeArgument = customAttribute.Fields
                .FirstOrDefault<CustomAttributeNamedArgument>(
                    (Func<CustomAttributeNamedArgument, bool>) (x => x.Name == "Build")).Argument;
            int build = (int) attributeArgument.Value;
            attributeArgument = customAttribute.Fields
                .FirstOrDefault<CustomAttributeNamedArgument>(
                    (Func<CustomAttributeNamedArgument, bool>) (x => x.Name == "Label")).Argument;
            string str = (string) attributeArgument.Value;
            return new Version((int) major, (int) minor, build, (int) revision);
        }
    }

    public static string GetDefaultPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "/Library/Application Support/Steam/steamapps/common/Humankind");
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Humankind"
            : string.Empty;
    }
}
