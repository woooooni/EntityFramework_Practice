using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace MMO_EFCore
{
    // AsNoTracking : ReadOnly << Tracking Snapshot
    // 데이터만 긁어오려고 할때 사용.

    // Include : Eager Loading(즉시 로딩) :: TODO
    // State(상태)

    public class DbCommands
    {
        public static void InitializeDB(bool forceReset = false)
        {
            using (AppDbContext db = new AppDbContext())
            {
                if (!forceReset && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                    return;

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                CreateTestData(db);
                Console.WriteLine("DB Initialized");
            }
        }

        public static void CreateTestData(AppDbContext db)
        {
            var tinyPrince = new Player(){ Name = "Tiny-Prince" };
            var faker = new Player(){ Name = "Faker" };
            var taewonKim = new Player(){ Name = "KimTaeWon" };

            var items = new List<Item>()
            {
                new Item()
                {
                    TemplateId = 101,
                    CreatedDate = DateTime.Now,
                    Owner = tinyPrince
                },
                new Item()
                {
                   TemplateId = 102,
                   CreatedDate = DateTime.Now,
                   Owner = faker
                },
                new Item()
                {
                   TemplateId = 103,
                   CreatedDate = DateTime.Now,
                   Owner = taewonKim
                }

            };

            Guild guild = new Guild()
            {
                GuildName = "T1",
                Members = new List<Player>() { tinyPrince, taewonKim, faker }
            };

            db.Items.AddRange(items);
            db.Guilds.Add(guild);

            db.SaveChanges();

            {
                var owner = db.Players.Where(p => p.Name == "Tiny-Prince").First();
                Item item = new Item()
                {
                    TemplateId = 300,
                    CreatedDate = DateTime.Now,
                    Owner = owner
                };
                db.Items.Add(item);
                db.SaveChanges();
            }
        }



        #region RelationShip Update
        public static void ShowItems()
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in db.Items.Include(i => i.Owner).ToList())
                {
                    if (item.Owner == null)
                        Console.WriteLine($"ItemID({item.ItemId}) TemplateID({item.TemplateId}) Owner(null)");
                    else
                        Console.WriteLine($"ItemID({item.ItemId}) TemplateID({item.TemplateId}) OwnerId({item.Owner.PlayerId}) OwnerName({item.Owner.Name})");
                }
            }
        }

        // 1vs1
        public static void Update_1v1()
        {
            ShowItems();

            Console.WriteLine("Input Item Switch PlayerId");
            Console.Write(" > ");
            int id = int.Parse(Console.ReadLine());

            using (var db = new AppDbContext())
            {
                Player player = db.Players
                    .Include(p => p.Items)
                    .Single(p => p.PlayerId == id);

                player.Items.Add(new Item()
                {
                    TemplateId = 777,
                    CreatedDate = DateTime.Now
                });
                db.SaveChanges();
            }

            Console.WriteLine("--- Test Complete ---");
            ShowItems();
        }

        // 1 : N
        // Player : Guild
        public static void ShowGuilds()
        {
            using (var db = new AppDbContext())
            {
                foreach (var guild in db.Guilds.MapGuildToDto())
                {
                    Console.WriteLine
                        ($"GuildID({guild.GuildId}) " +
                        $"Guild Name({guild.Name}) " +
                        $"MemberCount({guild.MemberCount})");
                }
            }
        }

        // N : N

        public static void Update_1vN()
        {
            ShowGuilds();

            Console.WriteLine("Input Item Switch PlayerId");
            Console.Write(" > ");
            int id = int.Parse(Console.ReadLine());

            using (var db = new AppDbContext())
            {
                Guild guild = db.Guilds
                    .Include(g => g.Members)
                    .Single(g => g.GuildId == id);

                guild.Members.Add(new Player()
                {
                    Name = "Dopa"
                });
                db.SaveChanges();
            }

            Console.WriteLine("--- Test Complete ---");
            ShowGuilds();
        }
        #endregion

        #region Nullable
        //
        // Q) Dependent 데이터가 Principal 데이터 없이 존재할 수 있을까?
        //public static void ShowItems()
        //{
        //    using(var db = new AppDbContext())
        //    {
        //        foreach(var item in db.Items.Include(i => i.Owner).ToList())
        //        {
        //            if(item.Owner == null)
        //                Console.WriteLine($"ItemID({item.ItemId}) TemplateID({item.TemplateId}) Owner(null)");
        //            else
        //                Console.WriteLine($"ItemID({item.ItemId}) TemplateID({item.TemplateId}) OwnerId({item.Owner.PlayerId}) OwnerName({item.Owner.Name})");
        //        }
        //    }
        //}

        //public static void Test()
        //{
        //    ShowItems();

        //    Console.WriteLine("Input Delete PlayerId");
        //    Console.Write(" > ");
        //    int id = int.Parse(Console.ReadLine());

        //    using(var db = new AppDbContext())
        //    {
        //        Player player = db.Players.Include(p => p.Items)
        //                                    .Single(p => p.PlayerId == id);

        //        db.Players.Remove(player);
        //        db.SaveChanges();
        //    }

        //    Console.WriteLine("--- Test Complete ---");
        //    ShowItems();
        //}

        #endregion
        #region Update
        //Update 3단계
        // 1) Tracked Entity를 얻어온다.
        // 2) Entity 클래스의 Property를 변경.
        // 3) SaveChanges()를 호출.
        //public static void UpdateTest()
        //{
        //    using(AppDbContext db = new AppDbContext())
        //    {
        //        var guild = db.Guilds.Single(g => g.GuildName == "T1"); // 1
        //        guild.GuildName = "DWG"; // 2
        //        db.SaveChanges(); // 3
        //    }
        //    /* 내부 SQL구성
        //     * 
        //     * SELECT TOP(2) GuildId, GuildName
        //     * FROM [Guilds]
        //     * WHERE GuildName = N'T1';
        //     * 
        //     * SET NOCOUNT ON;
        //     * UPDATE [Guilds]
        //     * SET GuildName = @p0
        //     * WHERE GuildId = @p1;
        //     * SELECT @@ROWCOUNT;
        //     */
        //}


        //public static void ShowGuilds()
        //{
        //    using(var db = new AppDbContext())
        //    {
        //        foreach(var guild in db.Guilds.MapGuildToDto())
        //        {
        //            Console.WriteLine($"GuildID({guild.GuildId}) Guild Name({guild.Name}) MemberCount({guild.MemberCount})");
        //        }
        //    }
        //}
        //public static void UpdateByReload()
        //{
        //    ShowGuilds();
        //    Console.WriteLine("Input Guild Id");
        //    Console.Write(" > ");
        //    int id = int.Parse(Console.ReadLine());
        //    Console.WriteLine("Input Guild Name");
        //    Console.Write(" > ");
        //    string name = Console.ReadLine();

        //    using (AppDbContext db = new AppDbContext())
        //    {
        //        //Reload
        //        Guild guild = db.Find<Guild>(id);
        //        guild.GuildName = name;
        //        db.SaveChanges();
        //    }

        //    Console.WriteLine("------ Update Complete ------");
        //    ShowGuilds();
        //}

        //public static string MakeUpdateJsonStr()
        //{
        //    var jsonStr = "{\"GuildId\" : 1, \"GuildName\" : \"Hello\", \"Members\":null}";
        //    return jsonStr;
        //}

        //public static void UpdateByFull()
        //{
        //    ShowGuilds();

        //    //string jsonStr = MakeUpdateJsonStr();
        //    //Guild guild = JsonConvert.DeserializeObject<Guild>(jsonStr);

        //    Guild guild = new Guild()
        //    {
        //        GuildId = 1,
        //        GuildName = "TestGuild",
        //    };
        //    using (AppDbContext db = new AppDbContext())
        //    {
        //        db.Guilds.Update(guild);
        //        db.SaveChanges();
        //    }

        //    Console.WriteLine("------ Update Complete ------");
        //    ShowGuilds();
        //}
        #endregion
        #region Loading 방법에 대하여..
        //ex)
        //1 + 2) 특정 길드에 있는 길드원들이 소지한 모든 아이템들을 보고 싶어요!

        // 장점 : DB 접근 한 번으로 다 로딩(Join)
        // 단점 : 필요 없는 데이터도 전부 가져옴..
        //public static void EagerLoading()
        //{
        //    // Include() vs ThenInclude() :: TODO
        //    // First() :: TODO
        //    Console.WriteLine("길드 이름을 입력하세요.");
        //    Console.Write(" > ");
        //    string name = Console.ReadLine();

        //    using(var db = new AppDbContext())
        //    {
        //        Guild guild = db.Guilds.AsNoTracking()
        //                                .Where(g => g.GuildName == name)
        //                                .Include(g => g.Members)
        //                                .ThenInclude(p => p.Items)
        //                                .First();

        //        foreach(Player player in guild.Members)
        //        {
        //            foreach(Item item in player.Items)
        //            {
        //                Console.WriteLine($"TemplateId({item.TemplateId}, Owner({player.Name})");
        //            }
        //        }
        //    }
        //}

        // 장점 : 필요한 시점에 필요한 데이터만 로딩 가능.
        // 단점 : DB 접근 비용이 크다.

        //public static void ExplictLoading()
        //{
        //    Console.WriteLine("길드 이름을 입력하세요.");
        //    Console.Write(" > ");
        //    string name = Console.ReadLine();

        //    using (var db = new AppDbContext())
        //    {
        //        Guild guild = db.Guilds
        //                            .Where(g => g.GuildName == name)
        //                            .First();

        //        // 명시적
        //        db.Entry(guild).Collection(g => g.Members).Load();

        //        foreach (Player player in guild.Members)
        //        {
        //            db.Entry(player).Collection(p => p.Items).Load();
        //        }

        //        foreach (Player player in guild.Members)
        //        {
        //            foreach (Item item in player.Items)
        //            {
        //                Console.WriteLine($"TemplateId({item.TemplateId}, Owner({player.Name})");
        //            }
        //        }
        //    }
        //}


        //// 3) 특정 길드에 있는 길드원의 수는?
        //// SELECT COUNT(*)
        //// 장점 : 필요한 정보만 쏙 빼서 로딩.
        //// 단점 : 일일이 Select문을 안에 만들어줘야 함.
        //public static void SelectLoading()
        //{
        //    Console.WriteLine("길드 이름을 입력하세요.");
        //    Console.Write(" > ");
        //    string name = Console.ReadLine();

        //    using (var db = new AppDbContext())
        //    {
        //        var info = db.Guilds
        //                        .Where(g => g.GuildName == name)
        //                        .MapGuildToDto()
        //                        .First();

        //        Console.WriteLine($"GuildName({info.Name}), MemberCount({info.MemberCount})");
        //    }
        //}
        #endregion

    }
}

