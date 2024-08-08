using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using Plugins.Tokenizer;
using RoleplayingGame.Objects;
using RoleplayingGame.Systems;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
	public override void Initialize()
	{
		CharacterClasses.InitializeFrom(Path.Combine(_csvDirectory,CharacterClasses.SourceFilePath));

		if (!File.Exists(RoleplayingGameURI))
			return;

		using (var reader = new BinaryReader(File.OpenRead(RoleplayingGameURI)))
		{
			Characters = CharacterTracker.Deserialize(reader,CharacterClasses);
		}
		Characters.Initialize();

		base.Initialize();
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Shutdown()
	{
		if (!Directory.Exists(CacheURL))
			Directory.CreateDirectory(CacheURL);

		if (Characters.Count == 0)
		{
			File.Delete(RoleplayingGameURI);
			return;
		}

		using (var writer = new BinaryWriter(File.OpenWrite(RoleplayingGameURI)))
		{
			Characters.Serialize(writer);
		}

		base.Shutdown();
	}
}