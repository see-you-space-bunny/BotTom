using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Widget.CardGame.PersistentEntities;

namespace Widget.CardGame.Interfaces
{
	public interface ICommandIO<TKey>
	{
		internal TKey AlternateKey { get; }
	}
}