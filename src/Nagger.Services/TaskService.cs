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

        public Task GetTaskByName(string name)
        {
            var task = _localTaskRepository.GetTaskByName(name);
            if (task != null) return task;

            task = _remoteTaskRepository.GetTaskByName(name);
            if(task != null) StoreTask(task);
            return task;
        }

        public Task GetTaskById(string taskId)
        {
            return _localTaskRepository.GetTaskById(taskId);
        }

        Task GetLastSyncedTask()
        {
            return _localTaskRepository.GetLastSyncedTask();
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

        public IEnumerable<Task> GetTasksByTaskIds(IEnumerable<string> taskIds)
        {
            return _localTaskRepository.GetTasksByTaskIds(taskIds);
        }

        public IEnumerable<Task> GetTasksByProject(Project project)
        {
            return GetTasksByProjectId(project.Id);
        }

        public IEnumerable<Task> GetTasksByProjectId(string projectId)
        {
            // we should grab the most recent task for this project
            // then when we call the remote task repository we get all tasks since the most recent one
            var mostRecentTask = GetLastSyncedTask();
            string mostRecentTaskId = null;
            if (mostRecentTask != null && mostRecentTask.Project != null && mostRecentTask.Project.Id == projectId)
            {
                mostRecentTaskId =  mostRecentTask.Id;
            }

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
