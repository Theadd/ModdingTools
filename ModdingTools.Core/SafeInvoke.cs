namespace ModdingTools.Core;

public static class SafeInvoke
{
    public static void InvokeOnAll(params Action[] actions) => InvokeOnAll(true, actions);

    public static void InvokeOnAll(bool quiet, params Action[] actions)
    {
        foreach (var action in actions) action.TryInvoke(quiet);
    }

    public static bool All(params Action[] actions) => All(true, actions);
    public static bool All(bool quiet, params Action[] actions) => actions.All(action => TryInvoke(action, quiet));
    public static bool All(params Func<bool>[] actions) => All(true, actions);
    public static bool All(bool quiet, params Func<bool>[] actions) => actions.All(action => TryInvoke(action, quiet));

    public static bool TryInvoke(this Action action, bool quiet = false)
    {
        bool success = true;

        if (action != null)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                if (!quiet)
                    Console.WriteLine(e);
                success = false;
            }
        }

        return success;
    }

    public static bool TryInvoke(this Func<bool> action, bool quiet = false)
    {
        bool success = true;

        if (action != null)
        {
            try
            {
                success = action.Invoke();
            }
            catch (Exception e)
            {
                if (!quiet)
                    Console.WriteLine(e);
                success = false;
            }
        }

        return success;
    }

    public static T? Invoke<T>(Func<T> action, bool quiet = false)
    {
        try
        {
            return action.Invoke();
        }
        catch (Exception e)
        {
            if (!quiet) Console.WriteLine(e);
            return default;
        }
    }
}
