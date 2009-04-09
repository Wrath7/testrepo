using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Illuminate.Tools
{
	/// <summary>
	/// The PluginInvoker class retrieves and instantiates Agent Plugins for a Node
	/// </summary>
	public class Invoker
	{
		/// <summary>
		/// Invokes a plug-in from a reference path
		/// </summary>
		/// <param name="PluginPath">The physical path to the plug-</param>
		/// <returns>The plug-in object</returns>
		public object Invoke(string PluginPath, Type TypeToGet)
		{
			//Load the pluging into a generic assembly
			Assembly ass = Assembly.LoadFrom(PluginPath);

			//Retrieve all the types from the assembly
			Type[] assTypes = ass.GetTypes();

			//Loop though the assembly types
			for (int i = 0; i < assTypes.Length; i++)
			{
				//Get all the interefaces associated with the assembly types
				Type[] interfaces = assTypes[i].GetInterfaces();

				//Loop through the assemblies
				for (int j = 0; j < interfaces.Length; j++)
				{
					//If the interface is a Agent interface, invoke the object
					if (interfaces[j].UnderlyingSystemType == TypeToGet)
					{
						//Set the obbject
						return Activator.CreateInstance(assTypes[i]);
					}
				}
			}

			return null;
		}
	}
}
