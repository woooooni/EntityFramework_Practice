using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MMO_EFCore
{
    // AsNoTracking : ReadOnly << Tracking Snapshot
    // 데이터만 긁어오려고 할때 사용.
    // Include : Eager Loading(즉시 로딩) :: TODO
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
        }


        //ex)
        //1 + 2) 특정 길드에 있는 길드원들이 소지한 모든 아이템들을 보고 싶어요!

        // 장점 : DB 접근 한 번으로 다 로딩(Join)
        // 단점 : 필요 없는 데이터도 전부 가져옴..
        public static void EagerLoading()
        {
            // Include() vs ThenInclude() :: TODO
            // First() :: TODO
            Console.WriteLine("길드 이름을 입력하세요.");
            Console.Write(" > ");
            string name = Console.ReadLine();

            using(var db = new AppDbContext())
            {
                Guild guild = db.Guilds.AsNoTracking()
                                        .Where(g => g.GuildName == name)
                                        .Include(g => g.Members)
                                        .ThenInclude(p => p.Items)
                                        .First();

                foreach(Player player in guild.Members)
                {
                    foreach(Item item in player.Items)
                    {
                        Console.WriteLine($"TemplateId({item.TemplateId}, Owner({player.Name})");
                    }
                }
            }
        }

        // 장점 : 필요한 시점에 필요한 데이터만 로딩 가능.
        // 단점 : DB 접근 비용이 크다.
        
        public static void ExplictLoading()
        {
            Console.WriteLine("길드 이름을 입력하세요.");
            Console.Write(" > ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                Guild guild = db.Guilds
                                    .Where(g => g.GuildName == name)
                                    .First();

                // 명시적
                db.Entry(guild).Collection(g => g.Members).Load();

                foreach (Player player in guild.Members)
                {
                    db.Entry(player).Collection(p => p.Items).Load();
                }

                foreach (Player player in guild.Members)
                {
                    foreach (Item item in player.Items)
                    {
                        Console.WriteLine($"TemplateId({item.TemplateId}, Owner({player.Name})");
                    }
                }
            }
        }


        // 3) 특정 길드에 있는 길드원의 수는?
        // SELECT COUNT(*)
        // 장점 : 필요한 정보만 쏙 빼서 로딩.
        // 단점 : 일일이 Select문을 안에 만들어줘야 함.
        public static void SelectLoading()
        {
            Console.WriteLine("길드 이름을 입력하세요.");
            Console.Write(" > ");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var info = db.Guilds
                                .Where(g => g.GuildName == name)
                                .MapGuildToDto()
                                .First();

                Console.WriteLine($"GuildName({info.Name}), MemberCount({info.MemberCount})");
            }
        }

    }
}

