using System;

public static class ContainerOps
{
    public static void TransferAll(IItemContainer source, IItemContainer destination)
    {
        if (source == null || destination == null || source == destination)
            return;

        for (var i = 0; i < source.SlotCount; i++)
        {
            var slot = source.GetSlotData(i);
            if (!slot.IsValid)
                continue;

            var inserted = destination.AddStack(slot.Item, slot.Amount);
            if (inserted <= 0)
                continue;

            var remaining = slot.Amount - inserted;
            if (remaining <= 0)
                source.SetSlot(i, null, 0);
            else
                source.SetSlot(i, slot.Item, remaining);
        }
    }

    public static void MoveStack(IItemContainer source, int sourceIndex, SlotData sourceData,
                                 IItemContainer destination, int destinationIndex, SlotData destinationData)
    {
        if (source == null || destination == null)
            return;
        if (!sourceData.IsValid)
            return;

        if (!destinationData.IsValid)
        {
            destination.SetSlot(destinationIndex, sourceData.Item, sourceData.Amount);
            source.SetSlot(sourceIndex, null, 0);
            return;
        }

        if (destinationData.Item == sourceData.Item)
        {
            var max = sourceData.Item.MaxStack;
            var space = max - destinationData.Amount;
            var move = Math.Min(sourceData.Amount, space);
            if (move <= 0)
                return;

            destination.SetSlot(destinationIndex, destinationData.Item, destinationData.Amount + move);
            var newSrc = sourceData.Amount - move;
            if (newSrc <= 0)
                source.SetSlot(sourceIndex, null, 0);
            else
                source.SetSlot(sourceIndex, sourceData.Item, newSrc);
            return;
        }

        destination.SetSlot(destinationIndex, sourceData.Item, sourceData.Amount);
        source.SetSlot(sourceIndex, destinationData.Item, destinationData.Amount);
    }
}
