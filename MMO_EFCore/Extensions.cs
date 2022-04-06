using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO_EFCore
{
    public static class Extensions
    {
        //IEnumerable (LINQ to Object / LINQ to XML 쿼리)
        //IQueryable (LINQ to SQL 쿼리)
        public static IQueryable<GuildDTO> MapGuildToDto(this IQueryable<Guild> guild)
        {
            return guild.Select(g => new GuildDTO()
            {
                Name = g.GuildName,
                MemberCount = g.Members.Count
            });
        }
    }
}
