using Kitchen;
using Unity.Entities;

namespace KitchenGiveThatBack
{
    [UpdateInGroup(typeof(InteractionGroup), OrderFirst = true)]
    public class InteractReturnTeleport : ItemInteractionSystem
    {
        protected override InteractionType RequiredType => InteractionType.Act;

        Entity Receiver;
        CConveyTeleport ReceiverTeleport;

        Entity Sender;
        CConveyTeleport SenderTeleport;
        CItemHolder SenderHolder;

        Entity Item;

        protected override bool IsPossible(ref InteractionData data)
        {
            Receiver = data.Target;
            if (!Require(data.Target, out ReceiverTeleport) || ReceiverTeleport.Target == null)
                return false;
            if (!Require(data.Target, out CItemHolder holder) || holder.HeldItem != default)
                return false;
            Sender = ReceiverTeleport.Target;
            if (!Require(Sender, out SenderTeleport) || SenderTeleport.CurrentCooldown > 0f || SenderTeleport.Target != data.Target)
                return false;
            if (!Require(Sender, out SenderHolder) || SenderHolder.HeldItem == default)
                return false;
            Item = SenderHolder.HeldItem;
            if (Has<CItemUndergoingProcess>(Item) || !Has<CHeldBy>(Item))
                return false;
            return true;

        }

        protected override void Perform(ref InteractionData data)
        {
            Set(Item, (CHeldBy)Receiver);
            SenderHolder.HeldItem = default;
            SenderTeleport.HasReceivedTeleport = false;
            SenderTeleport.CurrentCooldown = SenderTeleport.SendCooldown;
            Set(Sender, SenderTeleport);
            
            Set(Receiver, (CItemHolder)Item);
            ReceiverTeleport.HasReceivedTeleport = true;
            Set(Receiver, ReceiverTeleport);
        }
    }
}
