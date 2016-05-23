namespace Nagger.Services
{
    using System.Linq;
    using Interfaces;

    public class CommandService :ICommandService
    {
        readonly ITimeService _timeService;
        readonly IOutputService _outputService;

        public CommandService(ITimeService timeService, IOutputService outputService)
        {
            _timeService = timeService;
            _outputService = outputService;
        }

        public void ExecuteCommands(string[] args)
        {
            switch (args.FirstOrDefault())
            {
                case "-push":
                    _timeService.DailyTimeOperations(true);
                    _outputService.ShowInformation("Push Has been run.");
                    break;
            }
        }
    }
}
