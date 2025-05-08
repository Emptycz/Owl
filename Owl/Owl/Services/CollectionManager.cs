using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;

namespace Owl.Services;

public static class CollectionManager
{
	public static IEnumerable<string> CollectionFiles = [];
	public static void LoadCollections(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		// TODO: Implement recursive search for collections
		try
		{
			var files = Directory.GetFiles(path, "*.owl");
			CollectionFiles = files.Where(f => !f.Contains("-log.owl")).Select(f => f.Split("/").Last());
		}
		catch (Exception err)
		{
			Log.Error("Failed to load collections: {Error}", err);
		}
	}
}
