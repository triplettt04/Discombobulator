using BaseLib.Abstracts;
using Discombobulator.DiscombobulatorCode.Extensions;
using Godot;

namespace Discombobulator.DiscombobulatorCode.Character;

public class DiscombobulatorPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Discombobulator.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}