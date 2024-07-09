namespace BotTom;

partial class Program
{
#region Validation
	/// <summary>
	/// handles cli arg validation confirmation
	/// </summary>
	/// <returns>false if any of our arguments failed to validate</returns>
	static bool ConfirmCliArgumentValidation()
	{
		if (F_FailedCliArgs.Count != 0)
		{
			foreach(string failedArgReply in F_FailedCliArgs)
			{
					Console.WriteLine($"{failedArgReply}");
			}
			return false;
		}

		return true;
	}

	/// <summary>
	/// attempts to validate an argument
	/// </summary>
	/// <param name="argVal">value of argument to validate</param>
	/// <param name="argName">name of the argument we're attempting to validate</param>
	/// <param name="rawArgs">the unfiltered arg cmd to check</param>
	/// <param name="isOptional">if the arg is optional or not</param>
	static void ValidateArgument(out List<string> argVal, string argName, Dictionary<string, string> rawArgs, bool isOptional = false)
	{
		argVal = [];
		argName = argName.ToLowerInvariant();

		if (!rawArgs.TryGetValue(argName, out string? value))
		{
			if (!isOptional)
				F_FailedCliArgs.Add($"Failed Validation: {argName}");
			return;
		}

		if (string.IsNullOrWhiteSpace(value))
		{
			F_FailedCliArgs.Add($"Failed Validation: {argName}");
			return;
		}

		argVal = [.. value.Split(',')];
		Console.WriteLine($"{argName} --- {string.Join(",", argVal)}");
	}

	/// <summary>
	/// attempts to validate an argument
	/// </summary>
	/// <param name="argVal">value of argument to validate</param>
	/// <param name="argName">name of the argument we're attempting to validate</param>
	/// <param name="rawArgs">the unfiltered arg cmd to check</param>
	/// <param name="isOptional">if the arg is optional or not</param>
	static void ValidateArgument(out string argVal, string argName, Dictionary<string, string> rawArgs, bool isOptional = false)
	{
		argVal = string.Empty;
		argName = argName.ToLowerInvariant();

		if (!rawArgs.TryGetValue(argName, out string? value))
		{
			if (!isOptional)
				F_FailedCliArgs.Add($"Failed Validation: {argName}");
			return;
		}

		if (string.IsNullOrWhiteSpace(value))
		{
			F_FailedCliArgs.Add($"Failed Validation: {argName}");
			return;
		}

		argVal = value;
		Console.WriteLine($"{argName} --- {(argName.Equals(F_PassWordArg, StringComparison.InvariantCultureIgnoreCase) ? "************" : $"{argVal}")}");
	}

	/// <summary>
	/// attempts to validate an argument
	/// </summary>
	/// <param name="argVal">value of argument to validate</param>
	/// <param name="argName">name of the argument we're attempting to validate</param>
	/// <param name="rawArgs">the unfiltered arg cmd to check</param>
	/// <param name="isOptional">if the arg is optional or not</param>
	static void ValidateArgument(out int argVal, string argName, Dictionary<string, string> rawArgs, bool isOptional = false)
	{
		argVal = -1;
		argName = argName.ToLowerInvariant();

		if (!rawArgs.ContainsKey(argName) && !isOptional)
		{
			F_FailedCliArgs.Add($"Failed Validation: {argName}");
			return;
		}

		if (string.IsNullOrWhiteSpace(rawArgs[argName]))
		{
			F_FailedCliArgs.Add($"Failed Validation: {argName}");
			return;
		}

		if (!int.TryParse(rawArgs[argName], out argVal))
		{
			F_FailedCliArgs.Add($"Failed Validation: {argName}");
			return;
		}

		Console.WriteLine($"{argName} --- {argVal}");
	}
#endregion
}