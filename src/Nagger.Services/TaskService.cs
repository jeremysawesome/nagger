namespace Nagger.Services
{
    using System.Collections.Generic;
    using Interfaces;
    using Models;

    public class TaskService : ITaskService
    {
        readonly ILocalTaskRepository _localTaskRepository;
        readonly IRemoteTaskRepository _remoteTaskRepository;

        public TaskService(ILocalTaskRepository localTaskRepository, IRemoteTaskRepository remoteTaskRepository)
        {
            _localTaskRepository = localTaskRepository;
            _remoteTaskRepository = remoteTaskRepository;
        }

        public Task GetLastTask()
        {
            return _localTaskRepository.GetLastTask();
        }

        public void StoreTask(Task task)
        {
            _localTaskRepository.StoreTask(task);
        }

        void StoreTasks(IEnumerable<Task> tasks)
        {
            foreach (var task in tasks)
            {
                _localTaskRepository.StoreTask(task);
            }
        }

        public void SyncTasksWithRemote()
        {
            /*
             * So, I feel like SyncTasksWithRemote should only get the most recent tasks that we don't have.
             * If there are a TON of tasks we don't have then it might take a long time.
             * The question is, do we want to differentiate this functionality from an "importAllTasks" functionality?
             */


            //todo: figure out if there is a way to retrieve only the unsynced tasks... 
            // maybe pass in a last task id? assuming the remote task repo creates tasks in order?
            // maybe only get tasks that are greater than a certain date?
            var remoteTasks = _remoteTaskRepository.GetTasks();
            StoreTasks(remoteTasks);
        }

        /**
         * Maybe we should only sync tasks once a day? Perhaps we can maintain a "TasksSynced" boolean?
         * Or maybe we just sync the tasks before popping up any nagger messages?
        **/

        public IEnumerable<Task> GetTasks()
        {
            SyncTasksWithRemote();
            return _localTaskRepository.GetTasks();
        }

        public IEnumerable<Task> GetTasksByProject(Project project)
        {
            return GetTasksByProjectId(project.Id);
        }

        public IEnumerable<Task> GetTasksByProjectId(string projectId)
        {
            // we should grab the most recent task for this project
            // then when we call the remote task repository we get all tasks since the most recent one
            var mostRecentTask = GetLastTask();
            var mostRecentTaskId = mostRecentTask == null ? "" : mostRecentTask.Id;
            var remoteTasks = _remoteTaskRepository.GetTasksByProjectId(projectId, mostRecentTaskId);
            StoreTasks(remoteTasks);
            return _localTaskRepository.GetTasks(projectId);
        }

        // todo: remove
        public Task GetTestTask()
        {
            return _localTaskRepository.GetTestTask();
        }
    }
}
