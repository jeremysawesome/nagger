using System;
using System.Collections.Generic;
using Nagger.Interfaces;
using Nagger.Models;

namespace Nagger.Data.Fake
{
    public class FakeRemoteTaskRepository : IRemoteTaskRepository
    {
        public IEnumerable<Task> GetTasks()
        {
            var tasks = new List<Task>();

            var task1 = GetTask("FakeTask1", "A Simple task with no subtasks");
            var task2 = GetTask("FakeTask2", "Create a Fake task repo for Nagger");
            task2.Tasks = new List<Task>
            {
                GetTask("FakeTask2_SubTask1", "Add subtasks", task2),
                GetTask("FakeTask2_SubTask2", "Create a method to build tasks easily", task2),
                GetTask("FakeTask2_SubTask3", "Create the third subtask", task2)
            };

            var task3 = GetTask("FakeTask3", "Create at least three tasks in the task repo");
            task3.Tasks = new List<Task>
            {
                GetTask("FakeTask3_SubTask1", "Create only one subtask for this task", task3)
            };

            tasks.Add(task1);
            tasks.Add(task2);
            tasks.Add(task3);

            return tasks;
        }

        public IEnumerable<Task> GetTasks(Project project)
        {
            throw new NotImplementedException();
        }

        private static Task GetTask(string id, string name, Task parent = null)
        {
            return new Task
            {
                Id = id,
                Name = name,
                Parent = parent
            };
        }
    }
}