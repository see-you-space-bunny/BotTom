using System.Text;
using System.Text.RegularExpressions;

namespace Engine.DiceRoll;

/// <summary>
/// Dice String Parser utilizing mXparser library.
/// </summary>
public class DiceParser
{
	#region static random stuff

	/// <value>Object from which to draw pseudo-random numbers.</value>
	private static Random _random;
	/// <value>Object to lock out the use of <c>_random</c>.</value>
	private static object _randLock;

	/// <summary>
	/// This constructor creates an object containing an initialized <c>Random</c> object
	/// and a <c>_randLock</c> object.
	/// </summary>
	/// <remarks>
	/// The <c>_randLock</c> object (I assume) locks the die roller out of being used
	/// simultaneously in a way that would create duplicate random numbers.
	/// </remarks>
	static DiceParser()
	{
		_random = new Random();
		_randLock = new object();
		Envy.Load(Path.Combine(Environment.CurrentDirectory, ".env"));
	}

	/// <summary>
	/// Method <c>RollDice</c> which generates a number of dice results from an amount of
	/// dice, and the sides of the dice. The sizes are the same for every roll in each
	/// calling of this method.
	/// </summary>
	/// <param name="number">An <see cref="System.Integer"/> representing the number of
	/// dice to be rolled.</param>
	/// <param name="size">An <see cref="System.Integer"/> representing sides of the dice
	/// to be rolled.</param>
	/// <returns>
	/// An ? array ? of integers of a length equal to the <c>number</c> parameter.
	/// </returns>
	/// <remarks>
	/// Puts a lot on <c>_randLock</c> to pull from <c>_random</c> a <c>number</c> of
	/// times equal to the length of the <c>result</c> array.
	/// </remarks>
	public static int[] RollDice(int number, int size)
	{
		int[] result = new int[number];
		lock (_randLock)
		{
			for (int ii = 0; ii < number; ii++)
			{
				result[ii] = _random.Next(size) + 1;
			}
		}
		return result;
	}

	/// <summary>
	/// Method <c>RollDie</c> which generates single die result.
	/// </summary>
	/// <param name="size">An <see cref="System.Integer"/> representing sides of the die
	/// to be rolled.</param>
	/// <returns>
	/// An <see cref="System.Integer"/> representing the die result.
	/// </returns>
	/// <remarks>
	/// Puts a lot on <c>_randLock</c> to pull a <c>result</c> from <c>_random</c>.
	/// </remarks>
	public static int RollDie(int size)
	{
		int result = 0;
		lock (_randLock)
		{
			result = _random.Next(size) + 1;
		}
		return result;
	}

	#endregion
	
	/// <summary>
	/// Replicates the functionality of BasicRoll to include brackets 
	/// functionality, while still returning a consistent output.
	/// </summary>
	/// <param name="string_">Dice string to be evaluated. It is returned as <c>Item3</c>.</param>
	/// <returns>Tuple&lt;string,int&gt;<br/>
	/// &emsp;&emsp;result of evaluated string in markdown and as the total sum</returns>
	public static Tuple<string,int> BasicRoll(string string_)
	{
		/// <value><c>List&lt;Tuple&lt;int,int&gt;&gt; match_positions</c><br/>
		/// &emsp;&emsp;<c>List</c> containing the positions and size of dice strings as found by 
		/// the regular expression filter <c>dice_matches</c>.<value>
		List<Tuple<int,int>> match_positions = new();
		/// <value><c>List&lt;Tuple&lt;string,int&gt;&gt; roll_results</c><br/>
		/// &emsp;&emsp;<c>List</c> containing markdown-formatted <c>string</c> and the 
		/// sum of the dice rolls as <c>int</c>.<value>
		List<Tuple<string,int>> roll_results = new();
		/// <value><c>MatchCollection dice_matches</c><br/>
		/// &emsp;&emsp;Contains the all matches the regex filter "[\d]+d[\d]*[d\d]*" finds in 
		/// <c>string_</c>. The filter targets instances of dice-strings.<value>
		MatchCollection dice_matches = Regex.Matches(string_,@"[\d]+d[\d]*[d\d]*[klhxe\d]*");
		foreach (Match match in dice_matches.Cast<Match>())
		{
			/// <value><c>var dice</c><br/>
			/// &emsp;&emsp;<c>string[]?</c> that contains the segments of a split-up dice-roll 
			/// <c>string</c>. The position zero (0) value is the <c>dice_count</c>.<value>
			var dice = Regex.Match(match.Value,@"[\d]+d[\d]*[d\d]*").Value.Split('d');

			Match symatch = Regex.Match(match.Value,@"[klhxe]+[\d]*[klhxe\d]*");
			(SpecialDiceSymbolTypes,int[])?[]? symbols = symatch.Success ? CheckSpecialRollSymbols(symatch.Value) : CheckSpecialRollSymbols(null);

			int? keep_highest = null;
			int? keep_lowest = null;
			int? explode = null;
			if (symbols is not null)
				foreach ((SpecialDiceSymbolTypes,int[])? symbol in symbols){
					if (symbol is not null )
						if (symbol.HasValue && symbol.Value.Item1 == SpecialDiceSymbolTypes.KeepHigh)
						{
							keep_highest = symbol.Value.Item2[0];
						}
						else if (symbol.HasValue && symbol.Value.Item1 == SpecialDiceSymbolTypes.KeepLow)
						{
							keep_lowest = symbol.Value.Item2[0];
						}
						else if (symbol.HasValue && symbol.Value.Item1 == SpecialDiceSymbolTypes.Explode)
						{
							explode = symbol.Value.Item2[0];
						}
						// TODO: Explode
						// TODO: ExplodeRange
				}

			/// <value><c>int dice_count</c><br/>
			/// &emsp;&emsp;An <c>int</c> that represents the number of dice to be rolled. 
			/// Of <c>XdY</c>, this value is <c>X</c>.<value>
			// First entry of dice string is the number of dice.
			// If it's parseable as an integer, place it in dice_count.
			// If it's not parseable, then set dice_count to one.
			if (!int.TryParse(dice[0].Trim(), out int dice_count))
				dice_count = 1;

			StringBuilder sb = new(match.Value);
			// Multiple definitions ("2d6d10") iterate through left-to-right.
			for (int i = 1; i < dice.Length; i++)
			{
				

				sb.Append("`[");
				/// <value><c>int faces</c><br/>
				/// &emsp;&emsp;An <c>int</c> that represents the size of dice to be rolled. 
				/// Of <c>XdY</c>, this value is <c>Y</c>.<value>
				// If no indicator for faces is found, assume 10.
				if (!int.TryParse(dice[i].Trim(), out int faces))
					faces = 10;

				// The result of the roll becomes the new dice_count.
				int[] dice_rolls = RollDice(dice_count, faces);
				bool[] dice_kept = new bool[dice_rolls.Length];                    

				if (keep_highest is not null || keep_lowest is not null || explode  is not null )
				{
					if (keep_highest is not null || keep_lowest is not null)
					{
						if (keep_highest is not null){
							int[] kh = MaxNOf((int) keep_highest, dice_rolls);
							for (int k = 0; k < kh.Length; k++)
									dice_kept[kh[k]] = true;
						}
						if (keep_lowest is not null){
							int[] kl = MinNOf((int) keep_lowest, dice_rolls);
							for (int k = 0; k < kl.Length; k++)
									dice_kept[kl[k]] = true;
						}
					}
					else
					{
						for (int k = 0; k < dice_rolls.Length; k++)
							dice_kept[k] = true;
					}
				}
				else
				{
					for (int k = 0; k < dice_rolls.Length; k++)
						dice_kept[k] = true;
				}

				for (int j = 0; j < dice_rolls.Length; j++)
				{
					if (dice_kept[j] is not true)
						sb.Append("`~~`");
					sb.Append($"{dice_rolls[j]}");
					if (dice_kept[j] is not true)
					{
						sb.Append("`~~`");
						dice_rolls[j] = 0;
					}
					if (j + 1 < dice_rolls.Length)
						sb.Append(", ");
				}
				sb.Append("]`");

				dice_count = dice_rolls.Sum();
			}
			// The final dice_count becomes added to the total result.
			roll_results.Add(new Tuple<string, int> (sb.ToString(),dice_count));
			match_positions.Add(new Tuple<int, int> (match.Index,match.Length));
		}
		var expression_to_evaluate = string_;
		string markdown_format_output = string_;
		
		for (int i = match_positions.Count - 1; i >= 0; i--)
		{
			markdown_format_output = markdown_format_output.Remove(match_positions[i].Item1, match_positions[i].Item2)
														   .Insert(match_positions[i].Item1, roll_results[i].Item1);
			expression_to_evaluate = expression_to_evaluate.Remove(match_positions[i].Item1, match_positions[i].Item2)
														   .Insert(match_positions[i].Item1, roll_results[i].Item2.ToString());
		}
		return new Tuple<string,int> (markdown_format_output,(int) Math.Round(new org.mariuszgromada.math.mxparser.Expression($"{expression_to_evaluate}").calculate() + 0.001));
	}

	/// <summary>
	/// 
	/// </summary>
	internal enum SpecialDiceSymbolTypes
	{
		KeepHigh = 0x00, // symbol: "k","kh" syntax "k","k00","kh","kh00" default: 1
		KeepLow = 0x01, // symbol: "kl" syntax: "kl","kl00" default 1
		Explode = 0x02, // symbol: "x","e" syntax: "x","x00","e","e00" default: (2147483647) faces
						// exploding behaviors:
						// x: explode all dice! (negative int value) (-2147483648)
						// e: only explode kept dice! (positive int value) (2147483647)
		ExplodeRange = 0x03, // symbol: "x","e" syntax: "x00-00","e00-00"
							 // exploding behaviors:
							 // x: explode all dice! (negative int values) (-2147483648)
							 // e: only explode kept dice! (positive int value) (2147483647)
	}

	/// <summary>
	/// A method with the purpose of finding and evaluating the dice symbols present in 
	/// the dice <c>string</c> being evaluated and returning their types and values.
	/// </summary>
	/// <param name="string_">The dice <c>string</c> to be evaluated for special 
	/// symbols.</param>
	/// <returns>Tuple&lt;SpecialDiceSymbolTypes?,int&gt;<br/>
	/// &emsp;&emsp;An <c>Array</c> containing the types and values of the present 
	/// symbols.</returns>
	internal static (SpecialDiceSymbolTypes,int[])?[] CheckSpecialRollSymbols(string? string_)
	{
		List<(SpecialDiceSymbolTypes,int[])?> symbols = new List<(SpecialDiceSymbolTypes, int[])?>();

		if (string_ is null)
			return symbols.ToArray();

		Match kh_matches = Regex.Match(string_,@"k(?!l|h)[\d]*|kh[\d]*");
		if (kh_matches.Success)
		{
			string[] split_match = Array.Empty<string>();
			if (Regex.Match(kh_matches.Value,@"kh").Success)
				split_match = kh_matches.Value.Split(new string[] { "kh" }, StringSplitOptions.None);
			else if (Regex.Match(kh_matches.Value,@"k").Success)
				split_match = kh_matches.Value.Split('k');
			
			if (!int.TryParse(split_match[1].Trim(), out int value))
				value = 1;
			
			symbols.Add((SpecialDiceSymbolTypes.KeepHigh,new int[1] {value}));
		}

		Match kl_match = Regex.Match(string_,@"kl[\d]*");
		if (kl_match.Success)
		{
			string[] split_match = Array.Empty<string>();
			if (Regex.Match(kl_match.Value,@"kl").Success)
				split_match = kl_match.Value.Split(new string[] { "kl" }, StringSplitOptions.None);
			
			if (!int.TryParse(split_match[1].Trim(), out int value))
				value = 1;
			
			symbols.Add((SpecialDiceSymbolTypes.KeepLow,new int[1] {value}));
		}

		Match xe_match = Regex.Match(string_,@"[xe][\d]*");
		if (xe_match.Success)
		{
			string[] split_match = Array.Empty<string>();
			if (Regex.Match(xe_match.Value,@"e").Success)
				split_match = xe_match.Value.Split('e');
			else if (Regex.Match(xe_match.Value,@"x").Success)
				split_match = xe_match.Value.Split('x');
			
			if (!int.TryParse(split_match[1].Trim(), out int value))
				value = 2147483647;
			
			symbols.Add((SpecialDiceSymbolTypes.Explode,new int[1] {value}));
		}

		return symbols.ToArray();
	}

	/// <summary>
	/// Find the <c>capacity</c> maximum values of an integer <c>n</c>umber array.
	/// </summary>
	/// <param name="capacity">number of numbers you want to retrieve</param>
	/// <param name=n">your list of numbers</param>
	/// <returns>An array of the <c>capacity</c> highest values within <c>n</c>.</returns>
	public static int[] MaxNOf(int capacity, int[] n)
	{
		int [] max_so_far = new int[capacity]; // max so far

		// say that the first 'capacity' elements are the biggest, for now
		for (int k = 0; k < capacity; k++)
			max_so_far[k] = k;

		// for each number not processed
		for (int k = capacity; k < n.Length; k++)
		{
			// find out the smallest 'max so far' number
			int m = 0;
			for (int j = 0; j < capacity; j++)
				if (n[max_so_far[j]] < n[max_so_far[m]])
					m = j;

			// if our current number is bigger than the smallest stored, replace it
			if (n[k] > n[max_so_far[m]])
				max_so_far[m] = k;
		}
		return max_so_far;
	}

	/// <summary>
	/// Find the <c>capacity</c> maximum values of an integer <c>n</c>umber array.
	/// </summary>
	/// <param name="capacity">number of numbers you want to retrieve</param>
	/// <param name=n">your list of numbers</param>
	/// <returns>An array of the <c>capacity</c> highest values within <c>n</c>.</returns>
	public static int[] MinNOf(int capacity, int[] n)
	{
		int [] min_so_far = new int[capacity]; // min so far

		// say that the first 'capacity' elements are the smallest, for now
		for (int k = 0; k < capacity; k++)
			min_so_far[k] = k;

		// for each number not processed
		for (int k = capacity; k < n.Length; k++)
		{
			// find out the biggest 'min so far' number
			int m = 0;
			for (int j = 0; j < capacity; j++)
				if (n[min_so_far[j]] > n[min_so_far[m]])
					m = j;

			// if our current number is smaller than the smallest stored, replace it
			if (n[k] < n[min_so_far[m]])
				min_so_far[m] = k;
		}
		return min_so_far;
	}

	/// <summary>BasicRoll function that evaluates a dicestring including basic math.</summary>
	/// <param name="string_">Dice string to be evaluated. It is returned as <c>Item3</c>.</param>
	/// <returns>Tuple&lt;string,int,string&gt;<br/>
	/// &emsp;&emsp;result of evaluated string</returns>
	public static Tuple<string,int> BasicRollCowboy(string string_)
	{
		/// <value><c>var rollcontainer</c><br/>
		/// &emsp;&emsp;Temporary storage for recursive BasicRoll results.</value>
		Tuple<string,int> rollcontainer;
		/// <value><c>int total_result</c><br/>
		/// &emsp;&emsp;The total numerical result that will eventually be returned.</value>
		int total_result = 0;
		/// <value><c>string string_out</c><br/>
		/// &emsp;&emsp;The <see cref="System.string"/> passed in the return in markdown formatting.</value>
		string string_out = string_;
		/// <value><c>var add</c><br/>
		/// &emsp;&emsp;Addition is the lowest priority of resolution.</value>
		var add = string_.Split('+');
		
		if (add.Length > 1)
		{
			rollcontainer = BasicRoll(add[0]);
			total_result = rollcontainer.Item2;
			string_out = rollcontainer.Item1;
			foreach (var addString in add.Skip(1))
			{
				rollcontainer = BasicRoll(addString);
				total_result += rollcontainer.Item2;
				string_out += "+" + rollcontainer.Item1;
			}
		}
		else
		{
			/// <value><c>var sub</c><br/>
			/// &emsp;&emsp;Subtraction is the second lowest priority of resolution.</value>
			var sub = add[0].Split('-');
			
			if (sub.Length > 1)
			{
				rollcontainer = BasicRoll(sub[0]);
				total_result = rollcontainer.Item2;
				string_out = rollcontainer.Item1;
				foreach (var subString in sub.Skip(1))
				{
					rollcontainer = BasicRoll(subString);
					total_result -= rollcontainer.Item2;
					string_out += "-" + rollcontainer.Item1;
				}
			}
			else
			{
				/// <value><c>var mul</c><br/>
				/// &emsp;&emsp;Multiplication is the second highest priority of resolution.</value>
				var mul = sub[0].Split('*');

				if (mul.Length > 1)
				{
					rollcontainer = BasicRoll(mul[0]);
					total_result = rollcontainer.Item2;
					string_out = rollcontainer.Item1;
					foreach (var mulString in mul.Skip(1))
					{
						rollcontainer = BasicRoll(mulString);
						total_result *= rollcontainer.Item2;
						string_out += "*" + rollcontainer.Item1;
					}
				}   
				else
				{
					/// <value><c>var div</c><br/>
					/// &emsp;&emsp;Division is the first highest priority of resolution.</value>
					var div = mul[0].Split('/');

					if (div.Length > 1)
					{
						rollcontainer = BasicRoll(div[0]);
						total_result = rollcontainer.Item2;
						string_out = rollcontainer.Item1;
						foreach (var divString in div.Skip(1))
						{
							rollcontainer = BasicRoll(divString);
							total_result = (int) Math.Round((float) total_result / rollcontainer.Item2 + 0.0000001, 0);
							string_out += "/" + rollcontainer.Item1;
						}
					}
					else
					{
						/// <value><c>var dice</c><br/>
						/// &emsp;&emsp;Dice are absolute highest priority of resolution.<value>
						var dice = mul[0].Split('d');

						// First entry of dice string is the number of dice.
						// If it's parseable as an integer, place it in dice_count.
						// If it's not parseable, then set dice_count to one.
						if (!int.TryParse(dice[0].Trim(), out int dice_count))
							dice_count = 1;

						StringBuilder sb = new(string_out);
						// Multiple definitions ("2d6d10") iterate through left-to-right.
						for (int i = 1; i < dice.Length; i++)
						{
							sb.Append("`[");
							// If no indicator for faces is found, assume 10.
							if (!int.TryParse(dice[i].Trim(), out int faces))
								faces = 10;

							// The result of the roll becomes the new dice_count.
							int[] dice_rolls = RollDice(dice_count, faces);

							for (int j = 0; j < dice_rolls.Length; j++)
							{
								sb.Append($"{dice_rolls[j]}");
								if (j + 1 < dice_rolls.Length)
									sb.Append(", ");
							}
							sb.Append("]`");

							dice_count = dice_rolls.Sum();
						}
						// The final dice_count becomes added to the total_result.
						total_result += dice_count;
						
						// The modified string_out is built and replaces string_out.
						string_out = sb.ToString();
					}
				}
			}
		}
		return Tuple.Create(string_out, total_result);
	}
}