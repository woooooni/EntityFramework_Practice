using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_EFCore
{
    //클래스 이름 = 테이블 이름
    [Table("Item")] //Attribute로 테이블 이름 지정.
    public class Item
    {
        //이름ID => PK
        public int ItemId { get; set; }
        public int TemplateId { get; set; } // ex) 101 = 포션, ...
        public DateTime CreatedDate { get; set; }

        //FK(Navigational Property) => 다른 클래스를 참조하고 있음.
        public int OwnerId { get; set; }
        public Player Owner { get; set; }
    }
    
    public class Player
    {
        //ID를 붙여주는게 기본 컨벤션. 이름 ID => PK
        public int PlayerId { get; set; }
        public string Name { get; set; }
    }
}
