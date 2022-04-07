using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_EFCore
{
    // DB 관계 모델링
    // 1:1
    // 1:N          //Player : Item은 1:N 관계.
    // N:N

    //Entity 클래스 이름 = 테이블 이름
    [Table("Item")] //Attribute로 테이블 이름 지정.
    public class Item
    {
        //이름ID => PK
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // ex) 101 = 포션, ...
        public DateTime CreatedDate { get; set; }

        //FK(Navigational Property) => 다른 클래스를 참조하고 있음.
        [ForeignKey("OwnerId")]
        public int PlayerId { get; set; }
        public Player Owner { get; set; }
    }
    
    [Table("Player")]
    public class Player
    {
        //ID를 붙여주는게 기본 컨벤션. 이름 ID => PK
        public int PlayerId { get; set; }
        public string Name { get; set; }

        public ICollection<Item> Items { get; set; }
        public Guild Guild { get; set; }
    }

    [Table("Guild")]
    public class Guild
    {
        public int GuildId { get; set; }
        public string GuildName { get; set; }
        public ICollection<Player> Members { get; set; }
    }


    // DTO (Data Transfer Object)
    public class GuildDTO
    {
        public int GuildId { get; set; }
        public string Name { get; set; }
        public int MemberCount { get; set; }
    }
}
