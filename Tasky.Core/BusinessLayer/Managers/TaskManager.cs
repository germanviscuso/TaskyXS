using System;
using System.Collections.Generic;
using Tasky.BL;

namespace Tasky.BL.Managers
{
	public static class TaskManager
	{
		static TaskManager ()
		{
		}
		
		public static Task GetTask(int id)
		{
			return DAL.TaskRepositoryKii.GetTask(id);
		}
		
		public static IList<Task> GetTasks ()
		{
			return new List<Task>(DAL.TaskRepositoryKii.GetTasks());
		}
		
		public static int SaveTask (Task item)
		{
			return DAL.TaskRepositoryKii.SaveTask(item);
		}
		
		public static int DeleteTask(int id)
		{
			return DAL.TaskRepositoryKii.DeleteTask(id);
		}
		
	}
}