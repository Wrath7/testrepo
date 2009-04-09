using System;
using System.Collections.Generic;

namespace Illuminate.ActiveJob
{
	public interface IQueueManager
	{
		IQueueEntity GetActiveJobQueueEntry(int jobId, string agentId);
		IQueueEntity GetActiveJobQueueEntryWithNoAgentId(int jobId);
		IQueueEntity GetActiveJobQueueEntryRuntimeNow();
		bool UpdateAgentId(int jobId, string oldStatus, string newStatus, DateTime nextRunTime, string newAgentId);
		List<IQueueEntity> GetExpiredActiveJobQueueEntries();
	}
}
