using CounterStrikeSharp.API.Modules.Commands;

namespace CounterStrikeSharp.Helper.Extensions;

/// <summary>
/// Provides a set of extension methods for the <see cref="CommandInfo"/> class.
/// </summary>
public static class CommandInfoExtensions
{
    /// <summary>
    /// Gets command arguments from the specified <see cref="CommandInfo"/>.
    /// </summary>
    /// <param name="info">The <see cref="CommandInfo"/> instance.</param>
    /// <param name="startIndex">
    /// The starting index of the argument range. Defaults to 0.
    /// If less than 0, it will be clamped to 0.
    /// </param>
    /// <param name="endIndex">
    /// The ending index of the argument range. Defaults to -1, which means the last argument.
    /// If less than 0, it will be set to the last index.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of strings representing the selected arguments.
    /// Arguments enclosed in double quotes (<c>"</c>) will have the quotes removed.
    /// Returns an empty sequence if <see cref="CommandInfo.ArgString"/> is null or empty.
    /// </returns>
    public static IEnumerable<string> GetArgs(this CommandInfo info, int startIndex = 0, int endIndex = -1)
    {
        if (string.IsNullOrEmpty(info.ArgString))
            return [];

        string[] args = info.ArgString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int lastIndex = args.Length - 1;

        startIndex = Math.Clamp(startIndex, 0, lastIndex);
        endIndex = Math.Clamp(endIndex < 0 ? lastIndex : endIndex, startIndex, lastIndex);

        string[] selectedArgs = startIndex == endIndex
            ? [args[startIndex]]
            : args[startIndex..(endIndex + 1)];

        return selectedArgs.Select(arg =>
            arg.Length >= 2 && arg[0] == '"' && arg[^1] == '"'
                ? arg[1..^1]
                : arg
        );
    }
}
