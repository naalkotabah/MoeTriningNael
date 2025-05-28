namespace Moe.Core.Models.Entities
{
    public class WarehouseItem : BaseEntity
    {

        #region One-To-N
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
        #endregion
        #region Functional
        public int Qtu { get; set; }
        #endregion

        #region Non-Functional
        #endregion

        #region Many-To-N
        #endregion

    }

}
