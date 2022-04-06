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
            var player = new Player()
            {
                Name = "Tiny-Prince"
            };

            var items = new List<Item>()
            {
                new Item()
                {
                    TemplateId = 101,
                    CreatedDate = DateTime.Now,
                    Owner = player
                },
                new Item()
                {
                   TemplateId = 102,
                   CreatedDate = DateTime.Now,
                   Owner = new Player(){ Name = "Faker" }
                },
                new Item()
                {
                   TemplateId = 103,
                   CreatedDate = DateTime.Now,
                   Owner = new Player{ Name = "Woooo_ni" }
                }

            };

            db.Items.AddRange(items);
            db.SaveChanges();
        }

        public static void ReadAll()
        {
            using(var db = new AppDbContext())
            {
                // AsNoTracking : ReadOnly << Tracking Snapshot
                // 데이터만 긁어오려고 할때 사용.
                // Include : Eager Loading(즉시 로딩) :: TODO
                foreach(Item item in db.Items.AsNoTracking().Include(i => i.Owner))
                {
                    Console.WriteLine($"TemplateID({item.TemplateId}) Owner({item.Owner.Name}) CreatedDate({item.CreatedDate})");
                }
            }
        }

        //특정 플레이어가 소지한 아이템들의 생성날짜를 수정
        public static void UpdateDate()
        {
            Console.WriteLine("Input Player Name");
            Console.WriteLine(">");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var items = db.Items.Include(i => i.Owner)
                    .Where(i => i.Owner.Name == name);

                foreach(Item item in items)
                {
                    item.CreatedDate = DateTime.Now;
                }
                db.SaveChanges();
            }

            ReadAll();
        }

        public static void DeleteItem()
        {
            Console.WriteLine("Input Player Name");
            Console.WriteLine(">");
            string name = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var items = db.Items.Include(i => i.Owner)
                    .Where(i => i.Owner.Name == name);

                db.Items.RemoveRange(items);
                db.SaveChanges();
            }

            ReadAll();
        }
    }
}
