using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            
			List<UPA> ups = GetData();

			var usergrps = ups.GroupBy(x => x.permission).Select(group => new UGP
			{ permission = group.Key, 
				userslist = group.Select(x => x.user).ToList(),
				users = String.Join(",", group.Select(x => x.user)) }).ToList();


            Dictionary<string, List<string>> grpperms = new Dictionary<string, List<string>>();

			var pgs = usergrps.GroupBy(x => x.users).Select(group => new UPGP
			{
				users = group.Key,
				permissionlist = group.Select(x => x.permission).ToList()
				
			}).ToList();

			foreach(var pg in pgs)
            {
				grpperms.Add(pg.users, pg.permissionlist);
			}
			
            foreach (var grp in usergrps)
			{
				Console.WriteLine(grp.permission + " - " + grp.users);
				
				Console.WriteLine("------------------------");
			}
			List<string> splits = new List<string>();

			var tempgrps = usergrps;
			
			HashSet<string> grpusers = new HashSet<string>();
			List<string> usglist = new List<string>();
			foreach (var grp in usergrps)
            {
				usglist.Add(grp.users);
            }

			grpusers = usglist.OrderByDescending(x => x.Length).ToHashSet();

			HashSet<string> coveredusers = new HashSet<string>();
			List<string> matchedusers = new List<string>();

			foreach (var grpi in grpusers)
            {
				List<string> perms;
				HashSet<string> currentusers = new HashSet<string>();
				matchedusers = new List<string>();
				foreach (var str in grpi.Split(',').ToList())
				{
					currentusers.Add(str);
				}
				grpperms.TryGetValue(grpi, out perms);
				foreach (var grpj in grpusers)
                {
					if (grpi == grpj || grpj.Length > grpi.Length)
                    {
                        continue;
                    }
                    bool issub = checksubsets(grpi, grpj);
                    if (issub)
                    {
						matchedusers.Add(grpj);
						//foreach (var perm in perms)
      //                  {
						//	tempgrps.Add(new UGP {permission = perm,  users = grpj });
						//}
						foreach(var str in grpj.Split(',').ToList())
                        {							
							currentusers.Remove(str);							
						}

						if(currentusers.Count()==0)
                        {
							coveredusers.Add(grpi);
							var musers = matchedusers.OrderBy(x => x).ToList();
							foreach (var g in musers)
                            {
								foreach (var perm in perms)
								{
									tempgrps.Add(new UGP { permission = perm, users = g });
								}
							}
							break;
						}
					}

                }
            }
			Console.WriteLine("\n");
			Console.WriteLine("-----------------------");
			Console.WriteLine("Final Roles");

			string prevusers = string.Empty;
			var finalroles = tempgrps.OrderBy(x => x.users).ToList();
			foreach(var grp in finalroles)
			{			
				if(coveredusers.Contains(grp.users))
                {
					continue;
                }
				
				if (prevusers!=grp.users)
                {
					Console.WriteLine("------------------------");
					
					prevusers = grp.users;
				}
				Console.WriteLine(grp.permission + " - " + grp.users);
			}
			Console.ReadLine();
		}

		static List<UPA> GetData()
        {
			List<UPA> ups = new List<UPA>();
            ups.Add(new UPA { permission = "coffee", user = "u1" });
            ups.Add(new UPA { permission = "coffee", user = "u2" });
            ups.Add(new UPA { permission = "coffee", user = "u3" });
            ups.Add(new UPA { permission = "coffee", user = "u4" });
            ups.Add(new UPA { permission = "coffee", user = "u5" });
			ups.Add(new UPA { permission = "coffee", user = "u6" });

			ups.Add(new UPA { permission = "webpage", user = "u1" });
            ups.Add(new UPA { permission = "webpage", user = "u6" });

            ups.Add(new UPA { permission = "spend", user = "u3" });

            ups.Add(new UPA { permission = "teach", user = "u2" });
            ups.Add(new UPA { permission = "teach", user = "u3" });
            ups.Add(new UPA { permission = "teach", user = "u4" });
            ups.Add(new UPA { permission = "teach", user = "u5" });
			ups.Add(new UPA { permission = "teach", user = "u6" });

			ups.Add(new UPA { permission = "supervise", user = "u2" });
            ups.Add(new UPA { permission = "supervise", user = "u3" });
            ups.Add(new UPA { permission = "supervise", user = "u4" });
            ups.Add(new UPA { permission = "supervise", user = "u5" });
			ups.Add(new UPA { permission = "supervise", user = "u6" });

			ups.Add(new UPA { permission = "canteen", user = "u5" });
			ups.Add(new UPA { permission = "canteen", user = "u6" });

			ups.Add(new UPA { permission = "ground", user = "u2" });
			ups.Add(new UPA { permission = "ground", user = "u3" });
			ups.Add(new UPA { permission = "ground", user = "u4" });



			//ups.Add(new UPA { permission = "p1", user = "u1" });
			//ups.Add(new UPA { permission = "p1", user = "u3" });
			//ups.Add(new UPA { permission = "p1", user = "u4" });

			//ups.Add(new UPA { permission = "p2", user = "u1" });
			//ups.Add(new UPA { permission = "p2", user = "u3" });
			//ups.Add(new UPA { permission = "p2", user = "u4" });

			//ups.Add(new UPA { permission = "p4", user = "u2" });
			//ups.Add(new UPA { permission = "p4", user = "u3" });

			//ups.Add(new UPA { permission = "p5", user = "u1" });
			//ups.Add(new UPA { permission = "p5", user = "u2" });
			//ups.Add(new UPA { permission = "p5", user = "u3" });

			//ups.Add(new UPA { permission = "p6", user = "u1" });
			//ups.Add(new UPA { permission = "p6", user = "u2" });

			//ups.Add(new UPA { permission = "p7", user = "u1" });
			//ups.Add(new UPA { permission = "p7", user = "u2" });

			return ups;
		}
		static bool checksubsets(string grp1, string grp2)
		{
			List<string> g1 = grp1.Split(',').ToList();
			List<string> g2 = grp2.Split(',').ToList();
			if (g2.All(i => g1.Contains(i)))
			{
				return true;
			}
			return false;
		}
		public class UPA
		{
			public string permission { get; set; }
			public string user { get; set; }
		}

		public class UGP
		{
			public string permission { get; set; }
			public List<string> userslist { get; set; }

			public string users { get; set; }
		}

		public class UPGP
		{
			public List<string> permissionlist { get; set; }
			
			public string users { get; set; }
		}
	}
}
