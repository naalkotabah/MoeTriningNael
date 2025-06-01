using Microsoft.EntityFrameworkCore;

namespace Moe.Core.Models.Entities
{
    public class WarehouseItem : BaseEntity
    {

        #region One-To-N
        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Warehouse Warehouse { get; set; }
        public Guid WarehouseId { get; set; }


        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Item Item { get; set; }
        public Guid ItemId { get; set; }
       
        #endregion

        #region Functional
        public int Qty { get; set; }
        #endregion

        #region Non-Functional
        #endregion

        #region Many-To-N
        #endregion

    }

}
