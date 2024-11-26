namespace HINOSystem.Models.KB3.Master
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_MS_OldPart")]
    [PrimaryKey(nameof(F_Plant), nameof(F_Parent_Part),
        nameof(F_Ruibetsu), nameof(F_Store_Cd), nameof(F_Start_Date))]
    public class TB_MS_OldPart
    {

        public string F_Plant { get; set; }
        public string F_Parent_Part { get; set; }
        public string F_Ruibetsu { get; set; }
        public string F_Part_Name { get; set; }
        public string F_Store_Cd { get; set; }
        public string F_Start_Date { get; set; }
        public string F_End_Date { get; set; }
        public string F_Update_By { get; set; }
        public DateTime F_Update_Date { get; set; }

    }
}


