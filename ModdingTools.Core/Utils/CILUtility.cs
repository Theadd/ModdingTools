using Mono.Cecil;

namespace ModdingTools.Core.Utils;

public class CILUtility
{
    public void MakePublic(AssemblyDefinition original)
    {
        foreach (TypeDefinition typeDefinition in original.MainModule.Types.Recursive())
        {
            if (typeDefinition.IsNested)
                typeDefinition.IsNestedPublic = true;
            else
                typeDefinition.IsPublic = true;
        }
    }
}

public static class CILExtensions
{
    public static IEnumerable<TypeDefinition> Recursive(
        this IEnumerable<TypeDefinition> self)
    {
        foreach (TypeDefinition t in self)
        {
            yield return t;
            if (t.HasNestedTypes)
            {
                foreach (TypeDefinition typeDefinition in t.NestedTypes.AsEnumerable<TypeDefinition>()
                             .Recursive())
                    yield return typeDefinition;
            }
        }
    }
}
