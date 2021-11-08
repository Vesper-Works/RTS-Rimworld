internal class PickupItemStackJob : BaseJob
{
    private PlayerEntity playerEntity;
    private ItemEntity itemToPickup;
    public PickupItemStackJob(PlayerEntity playerEntity, Cell commandCell)
    {
        this.playerEntity = playerEntity;
        this.commandCell = commandCell;
        itemToPickup = commandCell.ItemContained;
    }

    public override bool IsFinished => playerEntity.Inventory.ItemHasBeenAdded(itemToPickup);

    public override void Cancel(BaseJob nextJob)
    {
        base.Cancel(nextJob);
    }

    public override bool Execute()
    {
        if (commandCell.ContainsItem)
        {
            playerEntity.Inventory.AddItem(commandCell.ItemContained);
            commandCell.RemoveEntity();
            return true;
        }
        return false;
    }
}