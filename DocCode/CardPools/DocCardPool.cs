using BaseLib.Abstracts;
using Doc.DocCode.Cards;
using Doc.DocCode.Cards.Doctor;
using Doc.DocCode.Cards.Doctor.Basic;
using Doc.DocCode.Characters;
using Doc.DocCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Doc.DocCode.CardPools;

public class DocCardPool : CustomCardPoolModel
{
    public override string Title => DoctorCharacter.CharacterId;

    public override string BigEnergyIconPath => "Charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "Charui/text_energy.png".ImagePath();

    public override float H => 0.12f;
    public override float S => 0.85f;
    public override float V => 0.9f;

    public override Color DeckEntryCardColor => new("D48A30");
    public override Color EnergyOutlineColor => new("B5651D");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return new CardModel[]
        {
            ModelDb.Card<DocAttack>(),
            ModelDb.Card<DocDefence>(),
            ModelDb.Card<Orchestrate>(),
            ModelDb.Card<Plan>(),
            ModelDb.Card<MacroStrategy>(),
            ModelDb.Card<LessonLearned>(),

            ModelDb.Card<Gravel>(),
            ModelDb.Card<Nearl>(),
            ModelDb.Card<Mlynar>(),
            ModelDb.Card<Ashlock>(),
            ModelDb.Card<Flametail>(),
            ModelDb.Card<WildMane>(),
            ModelDb.Card<Platinum>(),
            ModelDb.Card<Whislash>(),
            ModelDb.Card<Meteor>(),
            ModelDb.Card<Proviso>(),
            ModelDb.Card<JusticeKnight>(),
            ModelDb.Card<Blemishine>(),
            ModelDb.Card<NearlTheRadianKnight>(),

            ModelDb.Card<Beeswax>(),
            ModelDb.Card<Estelle>(),
            ModelDb.Card<Bubble>(),
            ModelDb.Card<Papyrus>(),
            ModelDb.Card<Philae>(),
            ModelDb.Card<Minimalist>(),
            ModelDb.Card<Gavial>(),
            ModelDb.Card<Titi>(),
            ModelDb.Card<Eunectes>(),
            ModelDb.Card<GavialTheInvincible>(),
            ModelDb.Card<Carnelian>(),
            ModelDb.Card<Lancet_2>(),
            ModelDb.Card<SandReckoner>(),
            ModelDb.Card<Scene>(),
            ModelDb.Card<Wulfenite>(),
            ModelDb.Card<Kestrel>(),
            ModelDb.Card<Hadiya>(),
            ModelDb.Card<Manticore>(),
            ModelDb.Card<Pepe>(),
            ModelDb.Card<Sesa>(),
            ModelDb.Card<Narantuya>(),
            ModelDb.Card<Passenger>(),
            ModelDb.Card<Tuye>(),

            ModelDb.Card<Vulcan>(),
            ModelDb.Card<Sideroca>(),
            ModelDb.Card<Varkaris>(),


    };
    }
}