using System;
using System.Collections;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Illuminate.Tools;
using System.Threading;
using Illuminate.Interfaces;

namespace Illuminate.Core.Node
{
	/// <summary>
	/// This class contains the references for a Agent collection
	/// </summary>
	[Serializable]
	public class AgentCollection : CollectionBase
	{
		#region Delegates/Events

		/// <summary>
		/// Delegate from OnNewAgent delegate
		/// </summary>
		/// <param name="w">The Agent which has completed execution</param>
		public delegate void OnNewAgentDelegate(Agent w);

		#endregion

		#region Class Members

		/// <summary>
		/// The Id of the Node.
		/// </summary>
		public string _nodeId = string.Empty;
		protected Illuminate.Communication.Communicator _com;

		#endregion

		#region Public Properties

		/// <summary>
		/// Default Property to retrieve Agent
		/// </summary>
		/// <param name="Index">The index of the Agent</param>
		/// <returns>Agent object</returns>
		public Agent this[int Index]
		{
			get
			{
				return (Agent)InnerList[Index];
			}
		}


		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="LockEvent">Shared lock object to manage lock synchronization</param>
		/// <param name="NodeId">The ID of the Node</param>
		public AgentCollection(string NodeId)
		{
			_nodeId = NodeId;
			_com = new Illuminate.Communication.Communicator();
			_com.OnDataOut += new Illuminate.Communication.Communicator.OnDataOutDelegate(RunCommand);
		}

		#endregion

		public void RunCommand(Illuminate.Communication.Command Command)
		{
			for (int i = 0; i < Count; i++)
			{
				if (Command.Destination == this[i].AgentId)
				{
					this[i].Communicator.RaiseDataIn(Command);
				}
			}
		}

		/// <summary>
		/// Adds a Agent to the collection
		/// </summary>
		/// <param name="w">The Agent to add to the collection</param>
		public void Add(Agent w)
		{
			bool exists = false;

			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].AgentId == w.AgentId)
				{
					exists = true;
					break;
				}
			}

			if (!exists) InnerList.Add(w);
		}

		public void StartAgents()
		{
			if (!Illuminate.Node.Configuration.ConfigurationRead)
				throw new Illuminate.Exceptions.CriticalException("Cannot start agents.  Configuration was not loaded properly");

			int agentId = 0;
			foreach (string agent in Illuminate.Node.Configuration.Agents)
			{
				string thisAgentPath = Illuminate.Node.Configuration.GetSettings(agent, "path");

				StartAgent(thisAgentPath, agent);
				agentId++;
			}
		}

		/// <summary>
		/// Discover all the Agents which are inside the path (recursive)
		/// </summary>
		/// <param name="StartupPath"></param>
		public void StartAgent(string path, string agentId)
		{
			Logger.WriteLine("Starting Agent: " + path, Logger.Severity.Debug, _nodeId);

			if (File.Exists(path))
			{
				Agent a = new Agent(_nodeId, agentId, path, _com);
				Add(a);
				Logger.WriteLine("Agent added to collection: " + path, Logger.Severity.Debug, _nodeId);

				a.Start();

				Logger.WriteLine("Agent Started: " + path, Logger.Severity.Debug, _nodeId);
			}
			else
			{
				Logger.WriteLine("Agent does not exists: " + path, Logger.Severity.Error, _nodeId);
			}
		}

		public void RemoveAgent(string agentId)
		{
			//find the agent, then stop it
			bool found = false;
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].AgentId == agentId)
				{
					Agent agent = this[i];
					found = true;

					int stopCount = 0;
					while (agent.Status != Agent.AgentStatus.Stopped)
					{
						agent.Stop();
						stopCount++;
						if (stopCount >= 20)
						{
							Logger.WriteLine("Agent " + agentId + " has not stopped after another 10 seconds.", Logger.Severity.Error, _nodeId);
							stopCount = 0; //reset to count for another 10 seconds
						}

						Thread.Sleep(500);
					}

					this.RemoveAt(i);
					break;
				}
			}

			if (!found)
			{
				Logger.WriteLine("Agent " + agentId + " was not found on node " + _nodeId, Logger.Severity.Error, _nodeId);
			}
		}
	}
}
