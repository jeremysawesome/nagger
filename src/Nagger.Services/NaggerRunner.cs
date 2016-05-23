namespace Nagger.Services
{
    using System;
    using System.Linq;
    using Interfaces;
    using Models;

    //TODO: this really needs to be refactored...

    public class NaggerRunner : IRunnerService
    {
        readonly IInputService _inputService;
        readonly IOutputService _outputService;
        readonly ITimeService _timeService;
        readonly IRemoteRunner _remoteRunner;
        readonly ISettingsService _settingsService;

        public NaggerRunner(ITimeService timeService,
            IInputService inputService, IOutputService outputService, IRemoteRunner remoteRunner, ISettingsService settingsService)
        {
            _timeService = timeService;
            _inputService = inputService;
            _outputService = outputService;
            _remoteRunner = remoteRunner;
            _settingsService = settingsService;
        }

        public void Run()
        {
            _timeService.DailyTimeOperations();

            var askTime = DateTime.Now;

            _outputService.ShowInterface();
            _outputService.OutputSound();

            var runMiss = 0;
            var lastTimeEntry = _timeService.GetLastTimeEntry();
            var currentTask = lastTimeEntry?.Task;
            var comment = "";

            if (currentTask != null)
            {
                var stillWorking = false;

                if (lastTimeEntry.HasComment)
                {
                    stillWorking =
                        _inputService.AskForBoolean($"Are you still working on {lastTimeEntry.Comment} ({currentTask.Name})?");

                    if (stillWorking) comment = lastTimeEntry.Comment;
                }
                
                if(!stillWorking)
                {
                    // attempt to log all time up to this point because tasks were switched
                    if(_settingsService.GetSetting<bool>("SyncOnTaskSwitch")) _timeService.DailyTimeOperations(true);

                    stillWorking = _inputService.AskForBoolean($"Are you still working on {currentTask}?");
                }

                if (!stillWorking) currentTask = null;
            }

            if (currentTask == null)
            {
                if (!_inputService.AskForBoolean("Are you working?"))
                {
                    runMiss = _timeService.IntervalsSinceLastRecord();
                    if (runMiss <= 1) _timeService.RecordMarker(askTime);
                    else
                    {
                        var lastTime = _timeService.GetLastTimeEntry().TimeRecorded;
                        var minutes = _timeService.GetIntervalMinutes(1).First();
                        lastTime = lastTime.AddMinutes(minutes);

                        // insert an internal time marker for ask time
                        _timeService.RecordMarker(lastTime);
                    }
                }
                else
                {
                    currentTask = _remoteRunner.AskForTask();
                }
            }

            if (currentTask == null)
            {
                _outputService.HideInterface();
                return;
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                var recentComments = _timeService.GetRecentlyRecordedCommentsForTask(5, currentTask).ToList();
                comment = _inputService.AskForSelectionOrInput("Choose from options or insert a new comment. (Leave Blank for no comment)", recentComments);
            }

            //todo: refactor the way runMiss is done
            runMiss = _timeService.IntervalsSinceLastRecord();
            // there will usually be 1 interval between the last time this ran and this time (it only makes sense)
            if (runMiss <= 1) _timeService.RecordTime(currentTask, askTime, comment);
            else
            {
                AskAboutBreak(currentTask, askTime, runMiss, comment);
            }
            _outputService.HideInterface();
        }

        void AskAboutBreak(Task currentTask, DateTime askTime, int missedInterval, string comment)
        {
            if (
                !_inputService.AskForBoolean("Looks like we missed " + missedInterval +
                                             " check in(s). Were you on break?"))
            {
                _timeService.RecordTime(currentTask, askTime, comment);

                // record an entry for now in the case where they forgot about nagger and are just now answering the questions
                if(_timeService.IntervalsSinceTime(askTime) > 1) _timeService.RecordTime(currentTask, DateTime.Now, comment);

                //TODO: Create a method to ask about abscences (what if they have worked on multiple things in the amount of time they were gone?)
            }
            else
            {
                var lastTime = _timeService.GetLastTimeEntry().TimeRecorded;
                var minutes = _timeService.GetIntervalMinutes(1).First();
                lastTime = lastTime.AddMinutes(minutes);

                // insert an internal time marker for ask time
                _timeService.RecordMarker(lastTime);

                if (_inputService.AskForBoolean("Have you worked on anything since you've been back?"))
                {
                    var intervalsMissed = _timeService.GetIntervalMinutes(missedInterval).ToList();

                    var minutesWorked =
                        _inputService.AskForSelection(
                            "Which of these options represents about how long you have been working?", intervalsMissed);

                    // insert an entry for when they started working
                    _timeService.RecordTime(currentTask, missedInterval, minutesWorked, lastTime, comment);
                }

                // also insert an entry for the current time (since they are working and are no longer on break)
                _timeService.RecordTime(currentTask, DateTime.Now, comment);
            }
        }
    }
}