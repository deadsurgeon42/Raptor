//  Raptor - a client API for Terraria
//  Copyright (C) 2013-2015 MarioE
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;

namespace Raptor.Api
{
	/// <summary>
	///   The Raptor API.
	/// </summary>
	public static class ClientApi
	{
		/// <summary>
		///   The Raptor version.
		/// </summary>
		public static readonly Version ApiVersion = new Version(1, 0);

		internal static List<TerrariaPlugin> plugins = new List<TerrariaPlugin>();

		/// <summary>
		///   Gets the Terraria.Main instance.
		/// </summary>
		public static Main Main { get; internal set; }

		/// <summary>
		///   Gets the list of loaded plugins.
		/// </summary>
		public static ReadOnlyCollection<TerrariaPlugin> Plugins => new ReadOnlyCollection<TerrariaPlugin>(plugins);

		internal static void DeInitialize()
		{
			foreach (var plugin in plugins)
				try
				{
					plugin.Dispose();
				}
				catch (Exception ex)
				{
					Log.LogError("Plugin \"{0}\" failed to dispose:", plugin.Name, ex);
					Log.LogError(ex.ToString());
				}
		}

		internal static void Initialize()
		{
			#region Load plugins

			var loadedAssemblies = new Dictionary<string, Assembly>();
			foreach (string path in Directory.EnumerateFiles("Plugins", "*.dll"))
			{
				string fileName = Path.GetFileNameWithoutExtension(path);

				try
				{
					Assembly assembly;
					if (!loadedAssemblies.TryGetValue(fileName, out assembly))
					{
						assembly = Assembly.Load(File.ReadAllBytes(path));
						loadedAssemblies.Add(fileName, assembly);
					}

					foreach (var type in assembly.GetExportedTypes())
					{
						if (!type.IsSubclassOf(typeof(TerrariaPlugin)) || !type.IsPublic || type.IsAbstract)
							continue;

						var customAttributes = type.GetCustomAttributes(typeof(ApiVersionAttribute), false);
						if (customAttributes.Length == 0)
						{
							Log.LogError("Plugin \"{0}\" has no API version and was ignored.", type.FullName);
							continue;
						}

						var apiVersion = ((ApiVersionAttribute) customAttributes[0]).ApiVersion;
						if (apiVersion.Major != ApiVersion.Major || apiVersion.Minor != ApiVersion.Minor)
						{
							Log.LogError("Plugin \"{0}\" is designed for a different API version ({1}) and was ignored.",
								type.FullName, apiVersion.ToString(2));
							continue;
						}

						try
						{
							plugins.Add((TerrariaPlugin) Activator.CreateInstance(type));
						}
						catch (Exception ex)
						{
							Log.LogError("Could not create an instance of plugin \"{0}\":", type.FullName);
							Log.LogError(ex.ToString());
						}
					}
				}
				catch (Exception ex)
				{
					Log.LogError("Failed to load assembly \"{0}\":", fileName);
					Log.LogError(ex.ToString());
				}
			}

			#endregion

			#region Initialize plugins

			var orderPlugins = from p in Plugins
				orderby p.Order, p.Name
				select p;

			foreach (var plugin in orderPlugins)
				try
				{
					plugin.Initialize();
					Log.LogInfo("Plugin \"{0}\" v{1} (by {2}) initialized.", plugin.Name, plugin.Version, plugin.Author);
				}
				catch (Exception ex)
				{
					Log.LogError("Plugin \"{0}\" failed to initialize:", plugin.Name);
					Log.LogError(ex.ToString());
				}

			#endregion
		}
	}
}