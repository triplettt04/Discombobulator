using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Discombobulator.DiscombobulatorCode.Character;
using Discombobulator.DiscombobulatorCode.Extensions;

namespace Discombobulator.DiscombobulatorCode.Potions;

[Pool(typeof(DiscombobulatorPotionPool))]
public abstract class DiscombobulatorPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath();
}