
using Hangfire;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.GamePlayerOnline.Core.JobApplicationServices;

internal class JobInitCommandService(
	ILogger<JobExecuteCommandService> logger)
	: ICommandService<JobInitCommand>
{
	public ValueTask ExecuteAsync(JobInitCommand command, CancellationToken cancellationToken = default)
	{
		logger.LogDebug(nameof(ExecuteAsync));

		var recurringJobId = $"GamePlayerOnline|Clean";
		var jobExecuteCommand = new JobExecuteCommand();
		var timeZone = TimeZoneInfo.CreateCustomTimeZone("Asia/Taipei", TimeSpan.FromHours(8), "Asia/Taipei", "Asia/Taipei");
		RecurringJob.AddOrUpdate<IHangfireUtensil>(
			recurringJobId: recurringJobId,
			methodCall: service => service.JobExecuteAsync(jobExecuteCommand),
			cronExpression: "*/10 * * * *",
			options: new() { TimeZone = timeZone });

		//初始化強制第一次處理
		BackgroundJob.Enqueue<IHangfireUtensil>(service => service.JobExecuteAsync(jobExecuteCommand));

		return ValueTask.CompletedTask;
	}
}
