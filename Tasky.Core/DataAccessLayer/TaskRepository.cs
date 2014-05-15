using System;
using System.Collections.Generic;
using System.IO;
using Tasky.BL;
using KiiCorp.Cloud.Storage;

namespace Tasky.DAL {
	public class TaskRepositoryKii {

		protected static TaskRepositoryKii me;
		protected static string appId = null;
		protected static string appKey = null;
		protected static KiiBucket bucket;
		protected static string BUCKET_NAME = "tasks";
		protected static Kii.Site site = Kii.Site.US;
		
		static TaskRepositoryKii ()
		{
			me = new TaskRepositoryKii();
		}

		protected TaskRepositoryKii ()
		{
			if (appId == null || appKey == null) {
				appId = "bdf4d82f";
				appKey = "0baf6630d5748e72dd48a3b18bfc7f20";
				try {
					Kii.Initialize (appId, appKey, site);
					bucket = Kii.Bucket(BUCKET_NAME);
					System.Console.WriteLine ("Kii is initialized");
					System.Console.WriteLine ("Signing in");
					//grab pin and see if it exists
					string pin = "2332";
					string user = pin + "kiiuser";
					string pass = "kiipass" + pin;
					try {
						KiiUser.LogIn(user, pass);
						System.Console.WriteLine ("User successfully signed in");
					}
					catch (Exception nfe) {
						// first time, we need to register the user
						System.Console.WriteLine ("Signing up");
						KiiUser.Builder builder;
						builder = KiiUser.BuilderWithName(user);
						//builder.WithEmail("user_123456@example.com");
						//builder.WithPhone("+819012345678");
						KiiUser userb = builder.Build();
						userb.Register(pass);
						System.Console.WriteLine ("User successfully signed up");
					}
				} catch(Exception e) {
					System.Console.WriteLine ("Error intializing Kii database: " + e.ToString());
				}
			}	
		}

		public static Task GetTask(int id)
		{
			KiiObject obj = GetObjectById (id);
			if (obj != null) {
				System.Console.WriteLine ("Found task!");
				Task task = new Task();
				task.ID = id;
				task.Name = (string) obj["name"];
				task.Notes = (string) obj["notes"];
				task.Done = (bool) obj["done"];
				return task;
			} else {
				System.Console.WriteLine ("Error retrieving task with id " + id.ToString() + " from Kii database");
			}
			return null;
		}
		
		public static IEnumerable<Task> GetTasks ()
		{
			List<Task> list = new List<Task> ();
			KiiQuery query = new KiiQuery(); // all query
			try 
			{
				KiiQueryResult<KiiObject> result = bucket.Query(query);
				if(result != null)
					System.Console.WriteLine ("Found " + result.Count.ToString() + " tasks");
				foreach (KiiObject obj in result) 
				{
					Task task = new Task();
					task.ID = (int) obj["id"];
					task.Name = (string) obj["name"];
					task.Notes = (string) obj["notes"];
					task.Done = (bool) obj["done"];
					list.Add(task);
				}
				return list;
			}
			catch (Exception e) 
			{
				System.Console.WriteLine ("Error retrieving objects from Kii database: " + e.ToString());
			}
			return null;
		}
		
		public static int SaveTask (Task item)
		{
			if (item.ID > 0) {
				// update
				KiiObject obj = GetObjectById (item.ID);
				if (obj != null) {
					obj ["id"] = item.ID;
					obj ["name"] = item.Name;
					obj ["notes"] = item.Notes;
					obj["done"] = item.Done;
					obj.Save ();
					System.Console.WriteLine ("Task updated");
					return item.ID;
				} else {
					System.Console.WriteLine ("Error retrieving object with id " + item.ID.ToString() + " from Kii database");
				}
				return 0;
			}
			// insert
			int nextId = NextId ();
			if (nextId > 0) {
				item.ID = nextId;
				KiiObject obj = bucket.NewKiiObject ();
				obj["id"] = nextId;
				obj ["name"] = item.Name;
				obj ["notes"] = item.Notes;
				obj["done"] = item.Done;
				obj.Save ();
				System.Console.WriteLine ("Task saved");
				return nextId;
			}
			return 0;
		}

		public static int DeleteTask(int id)
		{
			KiiObject obj = GetObjectById (id);
			if (obj != null) {
				obj.Delete();
				System.Console.WriteLine ("Task deleted");
				return (int)obj["id"];
			} else {
				System.Console.WriteLine ("Error retrieving object with id " + id.ToString() + " from Kii database");
			}
			return 0;
		}

		public static KiiObject GetObjectById(int id) {
			KiiQuery query = new KiiQuery(KiiClause.Equals("id", id));
			query.Limit = 1;
			try 
			{
				KiiQueryResult<KiiObject> result = bucket.Query(query);
				foreach (KiiObject obj in result) 
				{
					return obj;
				}
			}
			catch (Exception e) 
			{
				System.Console.WriteLine ("Error retrieving object with id " + id.ToString() + " from Kii database: " + e.ToString());
			}
			return null;
		}

		public static int NextId () {
			KiiQuery query = new KiiQuery(); // all query
			try 
			{
				KiiQueryResult<KiiObject> result = bucket.Query(query);
				System.Console.WriteLine ("Returning next id");
				return result.Count + 1;
			}
			catch (Exception e) 
			{
				System.Console.WriteLine ("Error retrieving objects from Kii database: " + e.ToString());
			}
			return 0; // id 0 is an invalid id, they start from 1
		}
	}
}

